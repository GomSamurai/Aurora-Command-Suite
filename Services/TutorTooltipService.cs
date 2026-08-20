using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace AuroraDesignSuite.Services
{
    public static class TutorTooltipService
    {
        private static Dictionary<string, string> _dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static bool _isLoaded = false;

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
                        trimmed.Contains(kvp.Key))
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

            string title = !string.IsNullOrEmpty(customTitle) ? customTitle : ("💡 MODO TUTOR: " + (keyOrTerm ?? "").Trim());

            ToolTip toolTip = new ToolTip
            {
                Background = new SolidColorBrush(Color.FromArgb(240, 13, 26, 38)), // Dark Cyber Blue
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 0, 240, 255)), // Cyan Accent Glow
                BorderThickness = new Thickness(1),
                Padding = new Thickness(12),
                Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint,
                HasDropShadow = true
            };

            Border cardBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                MaxWidth = 450
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
                Background = new SolidColorBrush(Color.FromArgb(100, 0, 240, 255)),
                Margin = new Thickness(0, 0, 0, 8)
            };
            panel.Children.Add(line);

            // Body Content
            TextBlock lblBody = new TextBlock
            {
                Text = bodyText,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 224, 230, 237)), // Soft Primary Text
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
