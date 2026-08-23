using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AuroraDesignSuite.Services;
using Microsoft.Win32;

namespace AuroraDesignSuite.Views
{
    public class ColorRoleViewModel : INotifyPropertyChanged
    {
        private string _hexValue = "#000000";
        private SolidColorBrush _swatchBrush = Brushes.Black;

        public string CategoryBadge { get; set; } = "🎨 Rol";
        public string RoleTitle { get; set; } = string.Empty;
        public string RoleDescription { get; set; } = string.Empty;
        public string PropertyKey { get; set; } = string.Empty;

        public Action? OnColorChanged { get; set; }

        public string HexValue
        {
            get => _hexValue;
            set
            {
                if (value != _hexValue)
                {
                    _hexValue = FormatHex(value);
                    UpdateSwatch();
                    OnPropertyChanged();
                    OnColorChanged?.Invoke();
                }
            }
        }

        public SolidColorBrush SwatchBrush
        {
            get => _swatchBrush;
            private set
            {
                _swatchBrush = value;
                OnPropertyChanged();
            }
        }

        private string FormatHex(string val)
        {
            if (string.IsNullOrWhiteSpace(val)) return "#000000";
            val = val.Trim();
            if (!val.StartsWith("#")) val = "#" + val;
            if (val.Length > 7) val = val.Substring(0, 7);
            return val;
        }

        private void UpdateSwatch()
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(_hexValue);
                SwatchBrush = new SolidColorBrush(color);
            }
            catch
            {
                SwatchBrush = Brushes.Transparent;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public partial class CustomThemeEditorWindow : Window
    {
        private readonly ObservableCollection<ColorRoleViewModel> _colorRoles = new ObservableCollection<ColorRoleViewModel>();
        private ThemeOption _currentDraft = new ThemeOption();
        private ThemeOption? _initialOriginalTheme;

        public CustomThemeEditorWindow(ThemeOption? startingTheme = null)
        {
            InitializeComponent();
            _initialOriginalTheme = startingTheme;

            InitializeBaseTemplatesDropdown();
            InitializeColorRoleViewModels();

            LoadStartingTheme(startingTheme);
        }

        private void InitializeBaseTemplatesDropdown()
        {
            var templates = ThemeManager.AvailableThemes.Where(t => !t.IsHeader && !t.IsEditorAction).ToList();
            CmbBaseTemplate.ItemsSource = templates;
            if (templates.Count > 0)
            {
                CmbBaseTemplate.SelectedIndex = 0;
            }
        }

        private void InitializeColorRoleViewModels()
        {
            _colorRoles.Clear();

            // 1. Fondos y Estructura
            _colorRoles.Add(new ColorRoleViewModel
            {
                CategoryBadge = "🌌 Estructura",
                RoleTitle = "Fondo Principal de la Aplicación",
                RoleDescription = "Superficie base de las ventanas principales y el fondo del lienzo.",
                PropertyKey = nameof(ThemeOption.BgDark)
            });

            _colorRoles.Add(new ColorRoleViewModel
            {
                CategoryBadge = "🌌 Estructura",
                RoleTitle = "Fondo de Tarjetas e Inspectores",
                RoleDescription = "Superficie contenedora para módulos, tarjetas de datos e inspectores tácticos.",
                PropertyKey = nameof(ThemeOption.CardBg)
            });

            _colorRoles.Add(new ColorRoleViewModel
            {
                CategoryBadge = "🌌 Estructura",
                RoleTitle = "Fondo de Encabezados y Filas Alternas",
                RoleDescription = "Superficie para barras de título, tablas, filas alternas y menús flotantes.",
                PropertyKey = nameof(ThemeOption.CardHeader)
            });

            _colorRoles.Add(new ColorRoleViewModel
            {
                CategoryBadge = "🌌 Estructura",
                RoleTitle = "Bordes y Divisores Tácticos",
                RoleDescription = "Líneas de contorno, divisores de sección y marcos de la interfaz.",
                PropertyKey = nameof(ThemeOption.BorderColor)
            });

            // 2. Tipografía y Textos
            _colorRoles.Add(new ColorRoleViewModel
            {
                CategoryBadge = "📝 Tipografía",
                RoleTitle = "Texto Principal",
                RoleDescription = "Tipografía primaria de alta visibilidad para títulos, nombres y datos clave.",
                PropertyKey = nameof(ThemeOption.TextPrimary)
            });

            _colorRoles.Add(new ColorRoleViewModel
            {
                CategoryBadge = "📝 Tipografía",
                RoleTitle = "Texto Secundario",
                RoleDescription = "Tipografía secundaria para etiquetas de campo, subtítulos e información de apoyo.",
                PropertyKey = nameof(ThemeOption.TextSecondary)
            });

            // 3. Acentos y Marcadores Funcionales
            _colorRoles.Add(new ColorRoleViewModel
            {
                CategoryBadge = "⚡ Acento / Rol",
                RoleTitle = "Acento Primario de Navegación",
                RoleDescription = "Color para botones de acción principal, pestañas activas e indicadores de enfoque.",
                PropertyKey = nameof(ThemeOption.AccentCyan)
            });

            _colorRoles.Add(new ColorRoleViewModel
            {
                CategoryBadge = "⚡ Acento / Rol",
                RoleTitle = "Acento Secundario de Títulos",
                RoleDescription = "Color destacado para titulares de sección, distintivos y cifras de resumen.",
                PropertyKey = nameof(ThemeOption.AccentAmber)
            });

            _colorRoles.Add(new ColorRoleViewModel
            {
                CategoryBadge = "⚡ Acento / Rol",
                RoleTitle = "Indicador de Estado Óptimo / Éxito",
                RoleDescription = "Color para métricas de rendimiento positivas, estado seguro y confirmaciones.",
                PropertyKey = nameof(ThemeOption.AccentGreen)
            });

            _colorRoles.Add(new ColorRoleViewModel
            {
                CategoryBadge = "⚡ Acento / Rol",
                RoleTitle = "Indicador de Alertas / Advertencias",
                RoleDescription = "Color de advertencia táctica, alerta de consumo, masa excesiva o peligro.",
                PropertyKey = nameof(ThemeOption.AccentRed)
            });

            _colorRoles.Add(new ColorRoleViewModel
            {
                CategoryBadge = "⚡ Acento / Rol",
                RoleTitle = "Indicador de Investigación / Especial",
                RoleDescription = "Color asignado a proyectos I+D, tecnología avanzada y elementos especiales.",
                PropertyKey = nameof(ThemeOption.AccentPurple)
            });

            foreach (var vm in _colorRoles)
            {
                vm.OnColorChanged = OnColorRoleChanged;
            }

            IcColorRoles.ItemsSource = _colorRoles;
        }

        private void LoadStartingTheme(ThemeOption? theme)
        {
            if (theme == null || theme.IsHeader || theme.IsEditorAction)
            {
                theme = ThemeManager.AvailableThemes.FirstOrDefault(t => !t.IsHeader && !t.IsEditorAction) ?? new ThemeOption();
            }

            _currentDraft = new ThemeOption
            {
                Name = theme.IsCustom ? theme.Name : $"💾 Mi {theme.Name.Replace("───", "").Replace("👑", "").Replace("🌌", "").Replace("☀️", "").Replace("☯️", "").Replace("💎", "").Trim()}",
                Icon = "💾",
                Category = "💾 Mis Temas Personalizados",
                IsCustom = true,
                BgDark = theme.BgDark,
                CardBg = theme.CardBg,
                CardHeader = theme.CardHeader,
                TextPrimary = theme.TextPrimary,
                TextSecondary = theme.TextSecondary,
                AccentCyan = theme.AccentCyan,
                AccentAmber = theme.AccentAmber,
                AccentGreen = theme.AccentGreen,
                AccentRed = theme.AccentRed,
                AccentPurple = theme.AccentPurple,
                BorderColor = theme.BorderColor
            };

            TxtCustomThemeName.Text = _currentDraft.Name;

            // Populate viewmodels
            PopulateRoleHexFromDraft();
            ApplyLiveHotSwap();

            BtnDeleteTheme.IsEnabled = theme.IsCustom;
        }

        private void PopulateRoleHexFromDraft()
        {
            foreach (var vm in _colorRoles)
            {
                string hex = vm.PropertyKey switch
                {
                    nameof(ThemeOption.BgDark) => _currentDraft.BgDark,
                    nameof(ThemeOption.CardBg) => _currentDraft.CardBg,
                    nameof(ThemeOption.CardHeader) => _currentDraft.CardHeader,
                    nameof(ThemeOption.TextPrimary) => _currentDraft.TextPrimary,
                    nameof(ThemeOption.TextSecondary) => _currentDraft.TextSecondary,
                    nameof(ThemeOption.AccentCyan) => _currentDraft.AccentCyan,
                    nameof(ThemeOption.AccentAmber) => _currentDraft.AccentAmber,
                    nameof(ThemeOption.AccentGreen) => _currentDraft.AccentGreen,
                    nameof(ThemeOption.AccentRed) => _currentDraft.AccentRed,
                    nameof(ThemeOption.AccentPurple) => _currentDraft.AccentPurple,
                    nameof(ThemeOption.BorderColor) => _currentDraft.BorderColor,
                    _ => "#000000"
                };

                vm.HexValue = hex;
            }
        }

        private void OnColorRoleChanged()
        {
            // Sync values from viewmodels back to draft
            foreach (var vm in _colorRoles)
            {
                switch (vm.PropertyKey)
                {
                    case nameof(ThemeOption.BgDark): _currentDraft.BgDark = vm.HexValue; break;
                    case nameof(ThemeOption.CardBg): _currentDraft.CardBg = vm.HexValue; break;
                    case nameof(ThemeOption.CardHeader): _currentDraft.CardHeader = vm.HexValue; break;
                    case nameof(ThemeOption.TextPrimary): _currentDraft.TextPrimary = vm.HexValue; break;
                    case nameof(ThemeOption.TextSecondary): _currentDraft.TextSecondary = vm.HexValue; break;
                    case nameof(ThemeOption.AccentCyan): _currentDraft.AccentCyan = vm.HexValue; break;
                    case nameof(ThemeOption.AccentAmber): _currentDraft.AccentAmber = vm.HexValue; break;
                    case nameof(ThemeOption.AccentGreen): _currentDraft.AccentGreen = vm.HexValue; break;
                    case nameof(ThemeOption.AccentRed): _currentDraft.AccentRed = vm.HexValue; break;
                    case nameof(ThemeOption.AccentPurple): _currentDraft.AccentPurple = vm.HexValue; break;
                    case nameof(ThemeOption.BorderColor): _currentDraft.BorderColor = vm.HexValue; break;
                }
            }

            ApplyLiveHotSwap();
        }

        private void ApplyLiveHotSwap()
        {
            ThemeManager.ApplyTheme(_currentDraft);
        }

        private void CmbBaseTemplate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbBaseTemplate.SelectedItem is ThemeOption baseTheme)
            {
                LoadStartingTheme(baseTheme);
            }
        }

        private void BtnPickColor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ColorRoleViewModel model)
            {
                using var dialog = new System.Windows.Forms.ColorDialog();
                if (!string.IsNullOrEmpty(model.HexValue))
                {
                    try
                    {
                        var mediaColor = (Color)ColorConverter.ConvertFromString(model.HexValue);
                        dialog.Color = System.Drawing.Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
                    }
                    catch { }
                }

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string hex = $"#{dialog.Color.R:X2}{dialog.Color.G:X2}{dialog.Color.B:X2}";
                    model.HexValue = hex;
                }
            }
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (CmbBaseTemplate.SelectedItem is ThemeOption baseTheme)
            {
                LoadStartingTheme(baseTheme);
            }
        }

        private void BtnSaveTheme_Click(object sender, RoutedEventArgs e)
        {
            string themeName = TxtCustomThemeName.Text.Trim();
            if (string.IsNullOrWhiteSpace(themeName))
            {
                MessageBox.Show("Por favor ingresa un nombre para tu tema personalizado.", "Nombre Requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _currentDraft.Name = themeName;
            var themeData = CustomThemeData.FromThemeOption(_currentDraft, themeName);
            CustomThemeService.SaveCustomTheme(themeData);

            // Save in user preferences so it stays selected
            var prefs = UserPreferencesService.LoadPreferences();
            prefs.SelectedTheme = themeData.ToThemeOption().Name;
            UserPreferencesService.SavePreferences(prefs);

            MessageBox.Show($"🎨 ¡El tema personalizado '{themeData.ThemeName}' ha sido guardado exitosamente y aplicado a la suite!\nPodrás seleccionarlo en cualquier momento desde la categoría '💾 MIS TEMAS PERSONALIZADOS'.", "Tema Personalizado Guardado", MessageBoxButton.OK, MessageBoxImage.Information);

            // Refresh parent UI if instance available
            if (MainWindow.Instance != null)
            {
                MainWindow.Instance.RefreshThemeSelectorDropdown(themeData.ToThemeOption().Name);
            }

            DialogResult = true;
            Close();
        }

        private void BtnExportTheme_Click(object sender, RoutedEventArgs e)
        {
            var saveDlg = new SaveFileDialog
            {
                Filter = "Archivo de Tema JSON (*.json)|*.json",
                FileName = $"{TxtCustomThemeName.Text.Replace("💾", "").Trim().Replace(" ", "_")}.json",
                Title = "Exportar Tema Personalizado"
            };

            if (saveDlg.ShowDialog() == true)
            {
                var themeData = CustomThemeData.FromThemeOption(_currentDraft, TxtCustomThemeName.Text);
                if (CustomThemeService.ExportThemeToJson(themeData, saveDlg.FileName, out string msg))
                {
                    MessageBox.Show(msg, "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(msg, "Error de Exportación", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnImportTheme_Click(object sender, RoutedEventArgs e)
        {
            var openDlg = new OpenFileDialog
            {
                Filter = "Archivo de Tema JSON (*.json)|*.json",
                Title = "Importar Tema Personalizado"
            };

            if (openDlg.ShowDialog() == true)
            {
                var themeData = CustomThemeService.ImportThemeFromJson(openDlg.FileName, out string msg);
                if (themeData != null)
                {
                    CustomThemeService.SaveCustomTheme(themeData);
                    LoadStartingTheme(themeData.ToThemeOption());
                    MessageBox.Show($"{msg}\nSe ha cargado e incorporado a tus temas guardados.", "Importación Completada", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(msg, "Error al Importar", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnDeleteTheme_Click(object sender, RoutedEventArgs e)
        {
            string cleanName = TxtCustomThemeName.Text.Replace("💾", "").Trim();
            var res = MessageBox.Show($"¿Estás seguro de que deseas eliminar el tema personalizado '{cleanName}'?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                if (CustomThemeService.DeleteCustomTheme(cleanName))
                {
                    MessageBox.Show($"El tema '{cleanName}' ha sido eliminado.", "Tema Eliminado", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (MainWindow.Instance != null)
                    {
                        MainWindow.Instance.RefreshThemeSelectorDropdown();
                    }
                    Close();
                }
            }
        }
    }
}
