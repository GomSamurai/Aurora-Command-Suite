using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;

namespace AuroraDesignSuite.Services
{
    public static class TutorTooltipService
    {
        private static Dictionary<string, string> _dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static bool _isLoaded = false;

        // Bilingual and Synonym Mappings for 100% Accurate Lookups
        private static readonly Dictionary<string, string> Synonyms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Academia Militar", "Military Academy" },
            { "Fábrica de Construcción", "Construction Factory" },
            { "Refinería de Combustible", "Fuel Refinery" },
            { "Centro Financiero", "Financial Centre" },
            { "Laboratorio de Investigación", "Research Facility" },
            { "Mina Convencional", "Conventional Mine" },
            { "Mina Automatizada", "Automated Mine" },
            { "Fábrica de Misiles/Munición", "Ordnance Factory" },
            { "Fábrica de Cazas", "Fighter Factory" },
            { "Instalación de Mantenimiento", "Maintenance Facility" },
            { "Cuartel General Naval", "Naval HQ" },
            { "Puerto Espacial", "Spaceport" },
            { "Estación de Espacio Profundo", "Deep Space Tracking Station" },
            { "Complejo de Tropas Terrestres", "Ground Force Training Complex" },
            { "Catapulta de Masa", "Mass Driver" },
            { "Instalación de Terraformación", "Terraforming Station" },
            { "Infraestructura Poblacional", "Infrastructure" },
            { "Infraestructura de Hábitat Urbano", "Infrastructure" }
        };

        // --------------------------------------------------------------------
        // ATTACHED PROPERTY FOR AUTOMATIC WPF TOOLTIPS
        // --------------------------------------------------------------------
        public static readonly DependencyProperty AutoTutorProperty =
            DependencyProperty.RegisterAttached(
                "AutoTutor",
                typeof(bool),
                typeof(TutorTooltipService),
                new PropertyMetadata(false, OnAutoTutorChanged));

        public static bool GetAutoTutor(DependencyObject obj) => (bool)obj.GetValue(AutoTutorProperty);
        public static void SetAutoTutor(DependencyObject obj, bool value) => obj.SetValue(AutoTutorProperty, value);

        private static void OnAutoTutorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element && (bool)e.NewValue)
            {
                element.MouseEnter -= Element_MouseEnter;
                element.MouseEnter += Element_MouseEnter;
            }
        }

        private static void Element_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement fe)
            {
                string? textToLookup = null;

                if (fe.DataContext != null)
                {
                    var dc = fe.DataContext;
                    var type = dc.GetType();

                    // Reflection lookup for common property names across models
                    var prop = type.GetProperty("TechName") ?? 
                               type.GetProperty("InstallationName") ?? 
                               type.GetProperty("ComponentName") ?? 
                               type.GetProperty("Name") ?? 
                               type.GetProperty("Description") ??
                               type.GetProperty("FleetName") ??
                               type.GetProperty("Key");

                    if (prop != null)
                    {
                        textToLookup = prop.GetValue(dc)?.ToString();
                    }
                    else if (dc is string str)
                    {
                        textToLookup = str;
                    }
                    else
                    {
                        textToLookup = dc.ToString();
                    }
                }

                if (string.IsNullOrEmpty(textToLookup) && fe is ComboBoxItem cbi)
                {
                    textToLookup = cbi.Content?.ToString();
                }
                else if (string.IsNullOrEmpty(textToLookup) && fe is ComboBox cb)
                {
                    if (cb.SelectedItem != null)
                    {
                        textToLookup = cb.SelectedItem.ToString();
                    }
                    else if (cb.Text != null)
                    {
                        textToLookup = cb.Text;
                    }
                }

                if (!string.IsNullOrEmpty(textToLookup))
                {
                    if (textToLookup.Length > 2 && !textToLookup.StartsWith("System."))
                    {
                        AttachToolTip(fe, textToLookup);
                    }
                }
            }
        }

        public static void EnsureLoaded()
        {
            if (_isLoaded) return;
            _isLoaded = true;

            try
            {
                string[] searchPaths = new string[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "AuroraTooltipDictionary.json"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AuroraTooltipDictionary.json"),
                    "c:/VSCODE/AuroraDesignSuite/config/AuroraTooltipDictionary.json",
                    "c:/VSCODE/Aurora_Command_Suite_v2.7.1_Portable/App/config/AuroraTooltipDictionary.json",
                    "c:/VSCODE/Aurora271Full/Patches/AuroraSpanish/AuroraTooltipDictionary.json"
                };

                foreach (string path in searchPaths)
                {
                    if (File.Exists(path))
                    {
                        string json = File.ReadAllText(path);
                        var loaded = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                        if (loaded != null && loaded.Count > 0)
                        {
                            foreach (var kvp in loaded)
                            {
                                _dictionary[kvp.Key] = kvp.Value;
                            }
                            break;
                        }
                    }
                }
            }
            catch { }
        }

        private static string CleanKey(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            
            // Remove Emojis and Special Characters
            string cleaned = Regex.Replace(input, @"[^\w\s\-\(\)\/\.]", "").Trim();
            
            // Remove common UI status prefixes
            cleaned = Regex.Replace(cleaned, @"^(ACTIVO|JUEGO|ESTÁNDAR|BÁSICO|MK\-I|MK\-II|MK\-III)\s+", "", RegexOptions.IgnoreCase);
            
            return cleaned.Trim();
        }

        public static string? GetTutorText(string? keyOrTerm)
        {
            EnsureLoaded();
            if (string.IsNullOrWhiteSpace(keyOrTerm)) return null;

            string raw = keyOrTerm.Trim();
            string cleaned = CleanKey(raw);

            // 1. Direct match on Raw
            if (_dictionary.TryGetValue(raw, out string? val) && IsRichContent(val)) return val;

            // 2. Direct match on Cleaned
            if (_dictionary.TryGetValue(cleaned, out val) && IsRichContent(val)) return val;

            // 3. Synonym / Bilingual match
            if (Synonyms.TryGetValue(cleaned, out string? synonymKey) || Synonyms.TryGetValue(raw, out synonymKey))
            {
                if (_dictionary.TryGetValue(synonymKey, out val) && IsRichContent(val)) return val;
            }

            // 4. Case-insensitive dictionary search
            foreach (var kvp in _dictionary)
            {
                if (string.Equals(kvp.Key, cleaned, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(kvp.Key, raw, StringComparison.OrdinalIgnoreCase))
                {
                    if (IsRichContent(kvp.Value)) return kvp.Value;
                }
            }

            // 5. Keyword & Component Category Fallback Matching
            if (ContainsWord(raw, "Fighter") || ContainsWord(raw, "Vástago") || ContainsWord(raw, "Pod Bay"))
            {
                if (_dictionary.TryGetValue("Fighter Pod Bay", out val)) return val;
            }
            if (ContainsWord(raw, "Academia") || ContainsWord(raw, "Academy"))
            {
                if (_dictionary.TryGetValue("Academia Militar", out val)) return val;
            }
            if (ContainsWord(raw, "Refinería") || ContainsWord(raw, "Refinery"))
            {
                if (_dictionary.TryGetValue("Refinería de Combustible", out val)) return val;
            }
            if (ContainsWord(raw, "Construcción") || ContainsWord(raw, "Construction"))
            {
                if (_dictionary.TryGetValue("Fábrica de Construcción", out val)) return val;
            }
            if (ContainsWord(raw, "Financiero") || ContainsWord(raw, "Financial"))
            {
                if (_dictionary.TryGetValue("Centro Financiero", out val)) return val;
            }
            if (ContainsWord(raw, "Laboratorio") || ContainsWord(raw, "Research Facility"))
            {
                if (_dictionary.TryGetValue("Laboratorio de Investigación", out val)) return val;
            }
            if (ContainsWord(raw, "Infraestructura") || ContainsWord(raw, "Infrastructure"))
            {
                if (_dictionary.TryGetValue("Infraestructura Poblacional", out val)) return val;
            }
            if (ContainsWord(raw, "Hardening") || ContainsWord(raw, "Endurecimiento"))
            {
                if (_dictionary.TryGetValue("Electronic Hardening", out val)) return val;
            }
            if (ContainsWord(raw, "ECCM") || ContainsWord(raw, "Contra-contramedidas"))
            {
                if (_dictionary.TryGetValue("Electronic Counter-countermeasures", out val)) return val;
            }
            if (ContainsWord(raw, "Laser") || ContainsWord(raw, "Láser"))
            {
                if (_dictionary.TryGetValue("Laser", out val)) return val;
            }
            if (ContainsWord(raw, "Shield") || ContainsWord(raw, "Escudo"))
            {
                if (_dictionary.TryGetValue("Shield Generator", out val)) return val;
            }
            if (ContainsWord(raw, "Active") && (ContainsWord(raw, "Sensor") || ContainsWord(raw, "Radar")))
            {
                if (_dictionary.TryGetValue("Active Search Sensor", out val)) return val;
            }
            if (ContainsWord(raw, "Thermal") || ContainsWord(raw, "Térmico"))
            {
                if (_dictionary.TryGetValue("Thermal Sensor", out val)) return val;
            }
            if (ContainsWord(raw, "EM Sensor") || ContainsWord(raw, "Electromagnético"))
            {
                if (_dictionary.TryGetValue("EM Sensor", out val)) return val;
            }

            // 6. Substring match for compound DB technology names
            foreach (var kvp in _dictionary)
            {
                if (kvp.Key.Length > 4)
                {
                    if (cleaned.StartsWith(kvp.Key, StringComparison.OrdinalIgnoreCase) ||
                        cleaned.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        if (IsRichContent(kvp.Value)) return kvp.Value;
                    }
                }
            }

            // 7. Fallback to TechDescriptionResolver
            return TechDescriptionResolver.ResolveDescription(cleaned, "Tecnología / Elemento Imperial");
        }

        private static bool IsRichContent(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            // Rich content contains multi-line sections with headers like 📌 CONCEPTO
            return text.Contains("CONCEPTO") || text.Length > 120;
        }

        private static bool ContainsWord(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target)) return false;
            return source.IndexOf(target, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static ToolTip? CreateTutorToolTip(string? keyOrTerm, string? customTitle = null)
        {
            string? bodyText = GetTutorText(keyOrTerm);
            if (string.IsNullOrEmpty(bodyText)) return null;

            string cleanKey = CleanKey(keyOrTerm ?? "");
            if (string.IsNullOrEmpty(cleanKey)) cleanKey = keyOrTerm ?? "";

            string title = !string.IsNullOrEmpty(customTitle) ? customTitle : ("💡 TUTOR IMPERIAL: " + cleanKey);

            ToolTip toolTip = new ToolTip
            {
                Background = new SolidColorBrush(Color.FromArgb(245, 11, 16, 26)), // Dark Cyber Blue
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 240, 255)), // Cyan Accent Glow
                BorderThickness = new Thickness(1.5),
                Padding = new Thickness(12),
                Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint,
                HasDropShadow = true
            };

            Border cardBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                MaxWidth = 480
            };

            StackPanel panel = new StackPanel();

            // Title
            TextBlock lblTitle = new TextBlock
            {
                Text = title,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 176, 0)), // Amber Accent
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                Margin = new Thickness(0, 0, 0, 8),
                TextWrapping = TextWrapping.Wrap
            };
            panel.Children.Add(lblTitle);

            // Separator Line
            Border line = new Border
            {
                Height = 1,
                Background = new SolidColorBrush(Color.FromArgb(120, 0, 240, 255)),
                Margin = new Thickness(0, 0, 0, 8)
            };
            panel.Children.Add(line);

            // Body Content
            TextBlock lblBody = new TextBlock
            {
                Text = bodyText,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 230, 237, 243)), // Primary White Text
                FontSize = 11.5,
                LineHeight = 18,
                TextWrapping = TextWrapping.Wrap
            };
            panel.Children.Add(lblBody);

            cardBorder.Child = panel;
            toolTip.Content = cardBorder;

            return toolTip;
        }

        public static void AttachToolTip(FrameworkElement? element, string? keyOrTerm, string? customTitle = null)
        {
            if (element == null || string.IsNullOrWhiteSpace(keyOrTerm)) return;
            try
            {
                ToolTip? tip = CreateTutorToolTip(keyOrTerm, customTitle);
                if (tip != null)
                {
                    element.ToolTip = tip;
                }
            }
            catch { }
        }
    }
}
