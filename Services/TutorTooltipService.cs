using System;
using System.Collections.Generic;
using System.IO;
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

                if (!string.IsNullOrEmpty(textToLookup))
                {
                    // Filter out generic placeholders
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

        public static string? GetTutorText(string? keyOrTerm)
        {
            EnsureLoaded();
            if (string.IsNullOrWhiteSpace(keyOrTerm)) return null;

            string trimmed = keyOrTerm.Trim();

            // 1. Direct match
            if (_dictionary.TryGetValue(trimmed, out string? val) && !string.IsNullOrEmpty(val))
            {
                return val;
            }

            // 2. Case insensitive match
            foreach (var kvp in _dictionary)
            {
                if (string.Equals(kvp.Key, trimmed, StringComparison.OrdinalIgnoreCase))
                {
                    return kvp.Value;
                }
            }

            // 3. Substring match for compound names
            foreach (var kvp in _dictionary)
            {
                if (kvp.Key.Length > 3)
                {
                    if (trimmed.StartsWith(kvp.Key, StringComparison.OrdinalIgnoreCase) ||
                        trimmed.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        return kvp.Value;
                    }
                }
            }

            // 4. Fallback to TechDescriptionResolver
            return TechDescriptionResolver.ResolveDescription(trimmed, "Tecnología");
        }

        public static ToolTip? CreateTutorToolTip(string? keyOrTerm, string? customTitle = null)
        {
            string? bodyText = GetTutorText(keyOrTerm);
            if (string.IsNullOrEmpty(bodyText)) return null;

            string cleanKey = (keyOrTerm ?? "").Trim();
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
