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
                else
                {
                    run.Foreground = (Brush)Application.Current.Resources["TextPrimaryBrush"] ?? System.Windows.Media.Brushes.White;
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

            // Regex 1: Event Type Header Banners (e.g. 🔬 HITO CIENTÍFICO:, 🎖️ RETIRO CON HONORES:)
            string headerRegex = @"^(?:🔬 HITO CIENTÍFICO|🧠 DESARROLLO CIENTÍFICO|🎖️ DECRETO DE ASCENSO|🎖️ DECRETO DE HONOR|🎖️ RETIRO CON HONORES|✝️ IN MEMORIAM|🎖️ PERFECCIONAMIENTO TÁCTICO|🧭 EXPLORACIÓN ESTELAR|💎 PROSPECCIÓN GEOLÓGICA|🌌 PUNTO DE SALTO|🪐 PROSPECCIÓN GRAVITACIONAL|🌍 PROSPECCIÓN TERRESTRE|🛰️ ESCÁNER ORBITAL|🏭 PRODUCCIÓN INDUSTRIAL|🏭 INICIO DE FABRICACIÓN|🪖 ADIESTRAMIENTO DE TROPAS|⚓ BOTADURA NAVAL|🏭 EXPANSIÓN COMERCIAL|🚀 NUEVA RUTA COMERCIAL|🛸 BOTADURA CIVIL|💥 ALERTA DE CATÁSTROFE|⚔️ COMBATE NAVAL|⛽ ALERTA LOGÍSTICA|⛽ EMERGENCIA LOGÍSTICA|⛽ TANQUES LLENOS):";

            // Regex 2: Officer & Leader Names with ranks
            string rankRegex = @"(?:\b(?:El oficial|oficial|Capitán de (?:Corbeta|Navío|Fragata)|Almirante|Comandante|Adept|Seeker|Syntagmatarchis|Antisyntagmatarchis|CIV|R\d+|Dr\.|Científico)\b\s+[A-ZÁÉÍÓÚÑa-záéíóúñ]+(?:\s+[A-ZÁÉÍÓÚÑa-záéíóúñ]+)*)|(?<=liderado por\s+)[A-ZÁÉÍÓÚÑa-záéíóúñ]+(?:\s+[A-ZÁÉÍÓÚÑa-záéíóúñ]+)+|(?<=al mando del oficial\s+)[A-ZÁÉÍÓÚÑa-záéíóúñ]+(?:\s+[A-ZÁÉÍÓÚÑa-záéíóúñ]+)+";

            // Regex 3: Tech Research Names
            string techRegex = @"(?<=investigación de\s+)[A-Za-z0-9\-\.\s\/]+?(?=\s+(?:ha sido|en|$))";

            // Regex 4: Locations & Systems
            string locRegex = @"(?<=\b(?:en la colonia de|en el sistema|en el cuerpo celeste|en|sobre)\s+)[A-ZÁÉÍÓÚÑa-záéíóúñ0-9\s]+?(?=\s+(?:ha|reveló|completado|es|$|\.))";

            // Regex 5: Alert / Status Triggers
            string alertRegex = @"\b(Sin Asignar|retirado con honores|fallecido trágicamente|fallo crítico|explotando|fuego enemigo|sin combustible|a la deriva|emergencia|combate)\b";

            // Regex 6: Numbers & Percentages (e.g., "32 años", "90%")
            string numberRegex = @"\b\d+(?:\.\d+)?(?:\s*años|\s*%|\s*litros)?\b";

            string masterPattern = $"({headerRegex})|({rankRegex})|({techRegex})|({locRegex})|({alertRegex})|({numberRegex})";

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
                    list.Add(new TokenSpan { Text = text.Substring(lastIdx, m.Index - lastIdx), HexColor = "#E6EDF3" });
                }

                string val = m.Value;
                if (m.Groups[1].Success) // Header Banner
                {
                    list.Add(new TokenSpan { Text = val, HexColor = "#FFD700", IsBold = true }); // Imperial Gold
                }
                else if (m.Groups[2].Success) // Officer Name
                {
                    list.Add(new TokenSpan { Text = val, HexColor = "#FFE066", IsBold = true }); // Warm Amber Gold
                }
                else if (m.Groups[3].Success) // Tech
                {
                    list.Add(new TokenSpan { Text = val, HexColor = "#00F0FF", IsBold = true }); // Cyan
                }
                else if (m.Groups[4].Success) // Location
                {
                    list.Add(new TokenSpan { Text = val, HexColor = "#00FF88", IsBold = true }); // Emerald
                }
                else if (m.Groups[5].Success) // Alert / Sin Asignar
                {
                    list.Add(new TokenSpan { Text = val, HexColor = "#FF6B6B", IsBold = true }); // Rose
                }
                else if (m.Groups[6].Success) // Numbers / Stats
                {
                    list.Add(new TokenSpan { Text = val, HexColor = "#64F4FF", IsBold = true }); // Light Cyan
                }
                else
                {
                    list.Add(new TokenSpan { Text = val, HexColor = "#E6EDF3" });
                }

                lastIdx = m.Index + m.Length;
            }

            if (lastIdx < text.Length)
            {
                list.Add(new TokenSpan { Text = text.Substring(lastIdx), HexColor = "#E6EDF3" });
            }

            return list;
        }
    }
}
