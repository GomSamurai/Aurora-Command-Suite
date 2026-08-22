using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class ImperialJournalView : UserControl
    {
        private List<ImperialJournalEntry> _entries = new List<ImperialJournalEntry>();
        private ImperialJournalEntry? _selectedEntry;
        private ObservableCollection<ImperialSubTask> _activeSubTasks = new ObservableCollection<ImperialSubTask>();

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
            string status = (CboStatusFilter?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "🌟 Todos los Estados";

            var query = _entries.AsEnumerable();

            if (!string.IsNullOrEmpty(category) && !category.Contains("Todas"))
            {
                query = query.Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(status) && !status.Contains("Todos"))
            {
                if (status.Contains("Completadas"))
                {
                    query = query.Where(x => x.IsCompleted);
                }
                else if (status.Contains("Pendientes"))
                {
                    query = query.Where(x => !x.IsCompleted);
                }
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Title.ToLower().Contains(search) || 
                                         x.Content.ToLower().Contains(search) || 
                                         x.ProjectFolder.ToLower().Contains(search));
            }

            var list = query.ToList();
            LstJournalEntries.ItemsSource = list;

            if (_selectedEntry != null && list.Any(x => x.Id == _selectedEntry.Id))
            {
                LstJournalEntries.SelectedItem = list.First(x => x.Id == _selectedEntry.Id);
            }
            else if (list.Count > 0)
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
                TxtEditFolder.Text = entry.ProjectFolder;
                TxtEditContent.Text = entry.Content;
                ChkEditCompleted.IsChecked = entry.IsCompleted;
                DpTargetDate.SelectedDate = entry.TargetDate;
                TxtEstimatedDays.Text = entry.EstimatedDays.ToString("F0");
                TxtRequiredBP.Text = entry.RequiredBP.ToString("N0");
                TxtRequiredMinerals.Text = entry.RequiredMinerals;

                SetComboValue(CboEditCategory, entry.Category);
                SetComboValue(CboEditPriority, entry.Priority);

                _activeSubTasks = new ObservableCollection<ImperialSubTask>(entry.SubTasks ?? new List<ImperialSubTask>());
                DgSubTasks.ItemsSource = _activeSubTasks;

                RecalculateProgressAndStyle();
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
            if (TxtEditFolder != null) TxtEditFolder.Text = "📁 General";
            if (TxtEditContent != null) TxtEditContent.Text = "";
            if (ChkEditCompleted != null) ChkEditCompleted.IsChecked = false;
            if (DpTargetDate != null) DpTargetDate.SelectedDate = null;
            if (TxtEstimatedDays != null) TxtEstimatedDays.Text = "30";
            if (TxtRequiredBP != null) TxtRequiredBP.Text = "0";
            if (TxtRequiredMinerals != null) TxtRequiredMinerals.Text = "";
            
            _activeSubTasks = new ObservableCollection<ImperialSubTask>();
            if (DgSubTasks != null) DgSubTasks.ItemsSource = _activeSubTasks;

            RecalculateProgressAndStyle();
        }

        private void RecalculateProgressAndStyle()
        {
            bool isDone = ChkEditCompleted?.IsChecked == true;
            double progress = 0.0;

            if (isDone)
            {
                progress = 100.0;
            }
            else if (_activeSubTasks != null && _activeSubTasks.Count > 0)
            {
                progress = Math.Round(_activeSubTasks.Average(t => t.IsDone ? 100.0 : Math.Clamp(t.ProgressPercent, 0, 100)), 1);
            }

            if (PbarProgress != null) PbarProgress.Value = progress;
            if (TxtProgressPercentLabel != null)
            {
                TxtProgressPercentLabel.Text = isDone ? "100% COMPLETADO (DIRECTIVA FINALIZADA)" : $"{progress:F0}% EN CURSO";
                TxtProgressPercentLabel.Foreground = isDone ? (Brush)Application.Current.Resources["AccentGreenBrush"] : (Brush)Application.Current.Resources["AccentAmberBrush"];
            }

            // Update Header Banner Styling for Green Completed Directive
            if (BdrStatusBanner != null && PnlEditorBorder != null)
            {
                if (isDone)
                {
                    BdrStatusBanner.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0D3728"));
                    BdrStatusBanner.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
                    PnlEditorBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
                }
                else
                {
                    BdrStatusBanner.Background = (Brush)Application.Current.Resources["CardHeaderBrush"];
                    BdrStatusBanner.BorderBrush = (Brush)Application.Current.Resources["BorderBrush"];
                    PnlEditorBorder.BorderBrush = (Brush)Application.Current.Resources["BorderBrush"];
                }
            }
        }

        private void ChkEditCompleted_Changed(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry != null)
            {
                _selectedEntry.IsCompleted = ChkEditCompleted.IsChecked == true;
                RecalculateProgressAndStyle();
            }
        }

        private void BtnAddSubTask_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry == null) return;

            var sub = new ImperialSubTask
            {
                Title = $"Sub-tarea N° {(_activeSubTasks.Count + 1)}",
                IsDone = false,
                ProgressPercent = 0,
                ResourceAssignment = "Recurso Imperial",
                Notes = "Añadir detalles..."
            };

            _activeSubTasks.Add(sub);
            RecalculateProgressAndStyle();
        }

        private void BtnDeleteSubTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ImperialSubTask sub)
            {
                _activeSubTasks.Remove(sub);
                RecalculateProgressAndStyle();
            }
        }

        private void BtnNewEntry_Click(object sender, RoutedEventArgs e)
        {
            var newEntry = new ImperialJournalEntry
            {
                Title = "📜 Nueva Directiva Imperial N° " + (_entries.Count + 1),
                Category = "⚔️ Misión Militar",
                Priority = "Media",
                ProjectFolder = "📁 NUEVO-PROYECTO",
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
            _selectedEntry.ProjectFolder = TxtEditFolder.Text.Trim();
            _selectedEntry.Category = (CboEditCategory.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "⚔️ Misión Militar";
            _selectedEntry.Priority = (CboEditPriority.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Media";
            _selectedEntry.Content = TxtEditContent.Text;
            _selectedEntry.IsCompleted = ChkEditCompleted.IsChecked == true;
            _selectedEntry.TargetDate = DpTargetDate.SelectedDate;

            double.TryParse(TxtEstimatedDays.Text, out double days);
            _selectedEntry.EstimatedDays = days;

            double.TryParse(TxtRequiredBP.Text.Replace(",", "").Replace(".", ""), out double bp);
            _selectedEntry.RequiredBP = bp;

            _selectedEntry.RequiredMinerals = TxtRequiredMinerals.Text;
            _selectedEntry.SubTasks = _activeSubTasks.ToList();

            ImperialJournalService.SaveEntries(_entries);
            RefreshList();

            MessageBox.Show("✅ Directiva Imperial y Plan de Sub-tareas guardados con éxito.", "Bitácora Imperial", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEntry == null) return;

            if (MessageBox.Show($"¿Eliminar la directiva '{_selectedEntry.Title}'?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
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

        private void CboStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }
    }
}
