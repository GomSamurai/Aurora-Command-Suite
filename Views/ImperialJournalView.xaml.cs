using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class ImperialJournalView : UserControl
    {
        private List<ImperialJournalEntry> _entries = new List<ImperialJournalEntry>();
        private ImperialJournalEntry? _selectedEntry;

        public ImperialJournalView()
        {
            InitializeComponent();
            RefreshList();
        }

        private void RefreshList()
        {
            _entries = ImperialJournalService.LoadEntries();
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (LstJournalEntries == null) return;

            string search = TxtSearchJournal?.Text.Trim().ToLower() ?? "";
            string category = (CboCategoryFilter?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "🌟 Todas las Categorías";

            var query = _entries.AsEnumerable();

            if (!string.IsNullOrEmpty(category) && !category.Contains("Todas"))
            {
                query = query.Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Title.ToLower().Contains(search) || x.Content.ToLower().Contains(search));
            }

            var list = query.ToList();
            LstJournalEntries.ItemsSource = list;

            if (list.Count > 0)
            {
                LstJournalEntries.SelectedIndex = 0;
            }
            else
            {
                ClearEditor();
            }
        }

        private void LstJournalEntries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstJournalEntries.SelectedItem is ImperialJournalEntry entry)
            {
                _selectedEntry = entry;
                TxtEditTitle.Text = entry.Title;
                TxtEditContent.Text = entry.Content;
                ChkEditCompleted.IsChecked = entry.IsCompleted;

                SetComboValue(CboEditCategory, entry.Category);
                SetComboValue(CboEditPriority, entry.Priority);
            }
        }

        private void SetComboValue(ComboBox combo, string value)
        {
            if (combo == null) return;
            foreach (ComboBoxItem item in combo.Items)
            {
                if (item.Content?.ToString()?.Equals(value, StringComparison.OrdinalIgnoreCase) == true)
                {
                    combo.SelectedItem = item;
                    break;
                }
            }
        }

        private void ClearEditor()
        {
            _selectedEntry = null;
            if (TxtEditTitle != null) TxtEditTitle.Text = "";
            if (TxtEditContent != null) TxtEditContent.Text = "";
            if (ChkEditCompleted != null) ChkEditCompleted.IsChecked = false;
        }

        private void BtnNewEntry_Click(object sender, RoutedEventArgs e)
        {
            var newEntry = new ImperialJournalEntry
            {
                Title = "📜 Nueva Directiva Imperial N° " + (_entries.Count + 1),
                Category = "⚔️ Misión Militar",
                Priority = "Media",
                Content = "Escribe aquí las instrucciones tácticas o notas de campaña..."
            };

            _entries.Insert(0, newEntry);
            ImperialJournalService.SaveEntries(_entries);
            RefreshList();

            LstJournalEntries.SelectedItem = newEntry;
        }

        private void BtnSaveEntry_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry == null) return;

            _selectedEntry.Title = TxtEditTitle.Text.Trim();
            _selectedEntry.Category = (CboEditCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "⚔️ Misión Militar";
            _selectedEntry.Priority = (CboEditPriority.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Media";
            _selectedEntry.Content = TxtEditContent.Text;
            _selectedEntry.IsCompleted = ChkEditCompleted.IsChecked == true;

            ImperialJournalService.SaveEntries(_entries);
            RefreshList();

            MessageBox.Show("✅ Nota de bitácora guardada con éxito.", "Bitácora Imperial", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry == null) return;

            if (MessageBox.Show($"¿Eliminar la nota '{_selectedEntry.Title}'?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _entries.Remove(_selectedEntry);
                ImperialJournalService.SaveEntries(_entries);
                RefreshList();
            }
        }

        private void BtnExportMarkdown_Click(object sender, RoutedEventArgs e)
        {
            string md = ImperialJournalService.ExportToMarkdown(_entries);

            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Exportar Bitácora Imperial en Formato Markdown",
                Filter = "Markdown Document (*.md)|*.md|All Files (*.*)|*.*",
                FileName = $"Imperial_Journal_Export_{DateTime.Now:yyyyMMdd}.md"
            };

            if (dlg.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(dlg.FileName, md);
                MessageBox.Show($"✅ Bitácora imperial exportada con éxito en:\n{dlg.FileName}", "Exportación Completada", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void TxtSearchJournal_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void CboCategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }
    }
}
