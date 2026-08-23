using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace AuroraDesignSuite.Services
{
    public static class ChronicleMessageFormatter
    {
        public static void FormatMessageToTextBlock(TextBlock textBlock, string message)
        {
            if (textBlock == null) return;
            textBlock.Inlines.Clear();
            if (string.IsNullOrWhiteSpace(message)) return;

            string cleanMsg = Regex.Replace(message, @"\s+", " ");
            var tokens = Tokenize(cleanMsg);

            foreach (var token in tokens)
            {
                var run = new Run(token.Text);
                if (token.IsBold) run.FontWeight = FontWeights.Bold;

                if (!string.IsNullOrEmpty(token.HexColor))
                {
                    try
                    {
                        var color = (Color)ColorConverter.ConvertFromString(token.HexColor);
                        run.Foreground = new SolidColorBrush(color);
                    }
                    catch
                    {
                        run.Foreground = System.Windows.Media.Brushes.White;
                    }
                }

                textBlock.Inlines.Add(run);
            }
        }

        private class TokenSpan
        {
            public string Text { get; set; } = string.Empty;
            public string HexColor { get; set; } = "#E6EDF3";
            public bool IsBold { get; set; } = false;
        }

        private static List<TokenSpan> Tokenize(string text)
        {
            var list = new List<TokenSpan>();

            string rankRegex = @"\b(?:Capitán de (?:Corbeta|Navío|Fragata)|Almirante|Comandante|Adept|Seeker|Syntagmatarchis|Antisyntagmatarchis|CIV|R\d+|Dr\.|Científico)\b\s+[A-ZÁÉÍÓÚÑa-záéíóúñ]+(?:\s+[A-ZÁÉÍÓÚÑa-záéíóúñ]+)*";
            string ledByRegex = @"(?<=led by\s+)[A-ZÁÉÍÓÚÑa-záéíóúñ]+(?:\s+[A-ZÁÉÍÓÚÑa-záéíóúñ]+)+";
            string techRegex = @"(?<=research into\s+)[A-Za-z0-9\-\.\s\/]+?(?=\s+(?:working|completed|at|$))";
            string locRegex = @"(?<=\b(?:working on|completed at|trained on)\s+)[A-ZÁÉÍÓÚÑa-záéíóúñ0-9\s]+?(?=\s+(?:has|completed|is|$|\.))";
            string alertRegex = @"\b(retired from the service|killed in an accident|killed in action|run out of fuel|destroyed|Low Fuel|Critical Fuel)\b";
            string percentRegex = @"\b\d+%\b";

            string masterPattern = $"({rankRegex})|({ledByRegex})|({techRegex})|({locRegex})|({alertRegex})|({percentRegex})";

            MatchCollection matches;
            try
            {
                matches = Regex.Matches(text, masterPattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                list.Add(new TokenSpan { Text = text });
                return list;
            }

            int lastIdx = 0;
            foreach (Match m in matches)
            {
                if (m.Index > lastIdx)
                {
                    list.Add(new TokenSpan { Text = text.Substring(lastIdx, m.Index - lastIdx) });
                }

                string val = m.Value;
                if (m.Groups[1].Success || m.Groups[2].Success) // Officer Name
                {
                    list.Add(new TokenSpan { Text = val, HexColor = "#FFD700", IsBold = true }); // Amber Gold
                }
                else if (m.Groups[3].Success) // Tech
                {
                    list.Add(new TokenSpan { Text = val, HexColor = "#00F0FF", IsBold = true }); // Cyan
                }
                else if (m.Groups[4].Success) // Location
                {
                    list.Add(new TokenSpan { Text = val, HexColor = "#00FF88", IsBold = true }); // Emerald
                }
                else if (m.Groups[5].Success) // Alert
                {
                    list.Add(new TokenSpan { Text = val, HexColor = "#FF5252", IsBold = true }); // Crimson
                }
                else if (m.Groups[6].Success) // Percent
                {
                    list.Add(new TokenSpan { Text = val, HexColor = "#FFE066", IsBold = true }); // Yellow
                }
                else
                {
                    list.Add(new TokenSpan { Text = val });
                }

                lastIdx = m.Index + m.Length;
            }

            if (lastIdx < text.Length)
            {
                list.Add(new TokenSpan { Text = text.Substring(lastIdx) });
            }

            return list;
        }
    }
}
