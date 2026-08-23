using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using Newtonsoft.Json;
using AuroraDesignSuite.Models;

namespace AuroraDesignSuite.Services
{
    public static class TutorTooltipService
    {
        private static Dictionary<string, string> _dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static bool _isLoaded = false;

        // Global Tutor Mode Toggle (ON / OFF)
        public static bool IsTutorEnabled { get; set; } = true;

        static TutorTooltipService()
        {
            // Register global WPF class handler to suppress ALL tooltips when TUTOR is OFF
            EventManager.RegisterClassHandler(
                typeof(FrameworkElement),
                ToolTipService.ToolTipOpeningEvent,
                new ToolTipEventHandler(OnGlobalToolTipOpening));
        }

        private static void OnGlobalToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (!IsTutorEnabled)
            {
                e.Handled = true; // Completely cancels and suppresses tooltips when TUTOR: OFF is active
            }
        }

        // Bilingual and Synonym Mappings for 100% Accurate Lookups
        private static readonly Dictionary<string, string> Synonyms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Academia Militar", "Academias Militares de Oficiales" },
            { "Military Academy", "Academias Militares de Oficiales" },
            { "Fábrica de Construcción", "Fábricas de Construcción Industrial" },
            { "Construction Factory", "Fábricas de Construcción Industrial" },
            { "Refinería de Combustible", "Refinerías de Sorium (Combustible)" },
            { "Fuel Refinery", "Refinerías de Sorium (Combustible)" },
            { "Centro Financiero", "Centros Financieros y Comerciales" },
            { "Financial Centre", "Centros Financieros y Comerciales" },
            { "Laboratorio de Investigación", "Laboratorios de I+D e Investigación" },
            { "Research Facility", "Laboratorios de I+D e Investigación" },
            { "Mina Convencional", "Minas Convencionales" },
            { "Conventional Mine", "Minas Convencionales" },
            { "Mina Automatizada", "Minas Automatizadas" },
            { "Automated Mine", "Minas Automatizadas" },
            { "Fábrica de Misiles/Munición", "Fábricas de Ordenanza y Municiones" },
            { "Ordnance Factory", "Fábricas de Ordenanza y Municiones" },
            { "Fábrica de Cazas", "Fábricas de Cazas Navales" },
            { "Fighter Factory", "Fábricas de Cazas Navales" },
            { "Instalación de Mantenimiento", "Instalaciones de Mantenimiento Naval" },
            { "Maintenance Facility", "Instalaciones de Mantenimiento Naval" },
            { "Cuartel General Naval", "Cuartel General Naval de Sector" },
            { "Naval HQ", "Cuartel General Naval de Sector" },
            { "Puerto Espacial", "Puertos Espaciales de Carga" },
            { "Spaceport", "Puertos Espaciales de Carga" },
            { "Estación de Espacio Profundo", "Estaciones de Tracking Espacial Profundo" },
            { "Deep Space Tracking Station", "Estaciones de Tracking Espacial Profundo" },
            { "Complejo de Tropas Terrestres", "Complejo de Entrenamiento Terrestre" },
            { "Ground Force Training Complex", "Complejo de Entrenamiento Terrestre" },
            { "Catapulta de Masa", "Catapulta de Masa Orbital" },
            { "Mass Driver", "Catapulta de Masa Orbital" },
            { "Instalación de Terraformación", "Estación de Terraformación Atmosférica" },
            { "Terraforming Station", "Estación de Terraformación Atmosférica" },
            { "Infraestructura Poblacional", "Infraestructura de Hábitat Urbano" },
            { "Infrastructure", "Infraestructura de Hábitat Urbano" },
            { "Battle Fleet", "Battle Fleet" },
            { "Cargo Fleet", "Cargo Fleet" },
            { "Colony Fleet", "Colony Fleet" },
            { "Shipyard Fleet", "Shipyard Fleet" },
            { "Survey Fleet", "Survey Fleet" }
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

                if (element is ComboBox cb)
                {
                    cb.SelectionChanged -= ComboBox_SelectionChanged;
                    cb.SelectionChanged += ComboBox_SelectionChanged;
                }
            }
        }

        private static void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsTutorEnabled) return;
            if (sender is ComboBox cb)
            {
                UpdateControlToolTip(cb);
            }
        }

        private static void Element_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsTutorEnabled) return;

            if (sender is FrameworkElement fe)
            {
                UpdateControlToolTip(fe);
            }
        }

        public static void UpdateControlToolTip(FrameworkElement fe)
        {
            if (!IsTutorEnabled || fe == null) return;

            string? textToLookup = null;

            if (fe is ComboBox cb)
            {
                if (cb.SelectedItem is ComboBoxItem cbi && cbi.Content != null)
                {
                    textToLookup = cbi.Content.ToString();
                }
                else if (cb.SelectedItem != null)
                {
                    var prop = cb.SelectedItem.GetType().GetProperty("Name") ?? 
                               cb.SelectedItem.GetType().GetProperty("TechName") ?? 
                               cb.SelectedItem.GetType().GetProperty("Description");
                    if (prop != null)
                    {
                        textToLookup = prop.GetValue(cb.SelectedItem)?.ToString();
                    }
                    else
                    {
                        textToLookup = cb.SelectedItem.ToString();
                    }
                }
                else if (!string.IsNullOrEmpty(cb.Text))
                {
                    textToLookup = cb.Text;
                }
            }
            else if (fe.DataContext != null)
            {
                var dc = fe.DataContext;
                var type = dc.GetType();

                if (IsCommanderOrLeader(dc))
                {
                    fe.ToolTip = CreateCommanderToolTip(dc);
                    return;
                }

                string? categoryHint = null;
                if (dc is SelectedComponentItem sci)
                {
                    textToLookup = sci.ComponentName;
                    categoryHint = sci.TypeName;
                }
                else if (dc is Component comp)
                {
                    textToLookup = comp.ComponentName;
                    categoryHint = comp.TypeName;
                }
                else
                {
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

                if (!string.IsNullOrEmpty(textToLookup) && textToLookup.Length > 1 && !textToLookup.StartsWith("System.") && !textToLookup.StartsWith("AuroraDesignSuite."))
                {
                    AttachToolTip(fe, textToLookup, categoryHint);
                    return;
                }
            }

            if (string.IsNullOrEmpty(textToLookup) && fe is ComboBoxItem item)
            {
                textToLookup = item.Content?.ToString();
            }

            if (!string.IsNullOrEmpty(textToLookup))
            {
                if (textToLookup.Length > 1 && !textToLookup.StartsWith("System.") && !textToLookup.StartsWith("AuroraDesignSuite."))
                {
                    AttachToolTip(fe, textToLookup);
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

        public static string? GetTutorText(string? keyOrTerm, string? categoryHint = null)
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

            // 4. Case-insensitive exact dictionary search
            foreach (var kvp in _dictionary)
            {
                if (string.Equals(kvp.Key, cleaned, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(kvp.Key, raw, StringComparison.OrdinalIgnoreCase))
                {
                    if (IsRichContent(kvp.Value)) return kvp.Value;
                }
            }

            // 5. Prioritize TechDescriptionResolver Category / Keyword Matching (for Engines, Fuel, Jump Drive, Weapon, Sensor, etc.)
            string resolved = TechDescriptionResolver.ResolveDescription(cleaned, categoryHint ?? "");
            if (!string.IsNullOrWhiteSpace(resolved) && !resolved.Contains("es una especificación y componente técnico fundamental"))
            {
                return resolved;
            }

            // 6. Keyword & Component Category Fallback Matching (SPANISH + ENGLISH)
            if (ContainsWord(raw, "Puerto") || ContainsWord(raw, "Spaceport"))
            {
                if (_dictionary.TryGetValue("Puertos Espaciales de Carga", out val)) return val;
            }
            if (ContainsWord(raw, "Cuartel") || ContainsWord(raw, "Naval HQ") || ContainsWord(raw, "Sector HQ"))
            {
                if (_dictionary.TryGetValue("Cuartel General Naval de Sector", out val)) return val;
            }
            if (ContainsWord(raw, "Terrestre") || ContainsWord(raw, "Entrenamiento") || ContainsWord(raw, "Ground Force"))
            {
                if (_dictionary.TryGetValue("Complejo de Entrenamiento Terrestre", out val)) return val;
            }
            if (ContainsWord(raw, "Tracking") || ContainsWord(raw, "Espacio Profundo") || ContainsWord(raw, "Deep Space"))
            {
                if (_dictionary.TryGetValue("Estaciones de Tracking Espacial Profundo", out val)) return val;
            }
            if (ContainsWord(raw, "Catapulta") || ContainsWord(raw, "Mass Driver"))
            {
                if (_dictionary.TryGetValue("Catapulta de Masa Orbital", out val)) return val;
            }
            if (ContainsWord(raw, "Terraformación") || ContainsWord(raw, "Terraforming"))
            {
                if (_dictionary.TryGetValue("Estación de Terraformación Atmosférica", out val)) return val;
            }
            if (ContainsWord(raw, "Hábitat") || ContainsWord(raw, "Infraestructura") || ContainsWord(raw, "Infrastructure"))
            {
                if (_dictionary.TryGetValue("Infraestructura de Hábitat Urbano", out val)) return val;
            }
            if (ContainsWord(raw, "Construcción") || ContainsWord(raw, "Construction"))
            {
                if (_dictionary.TryGetValue("Fábricas de Construcción Industrial", out val)) return val;
            }
            if (ContainsWord(raw, "Mina") || ContainsWord(raw, "Minas") || ContainsWord(raw, "Mine"))
            {
                if (_dictionary.TryGetValue("Minas Convencionales", out val)) return val;
            }
            if (ContainsWord(raw, "Financiero") || ContainsWord(raw, "Financieros") || ContainsWord(raw, "Financial"))
            {
                if (_dictionary.TryGetValue("Centros Financieros y Comerciales", out val)) return val;
            }
            if (ContainsWord(raw, "Refinería") || ContainsWord(raw, "Refinerías") || ContainsWord(raw, "Refinery"))
            {
                if (_dictionary.TryGetValue("Refinerías de Sorium (Combustible)", out val)) return val;
            }
            if (ContainsWord(raw, "Ordenanza") || ContainsWord(raw, "Municiones") || ContainsWord(raw, "Ordnance"))
            {
                if (_dictionary.TryGetValue("Fábricas de Ordenanza y Municiones", out val)) return val;
            }
            if (ContainsWord(raw, "Cazas") || ContainsWord(raw, "Fighter Factory"))
            {
                if (_dictionary.TryGetValue("Fábricas de Cazas Navales", out val)) return val;
            }
            if (ContainsWord(raw, "Laboratorio") || ContainsWord(raw, "Laboratorios") || ContainsWord(raw, "Research Facility"))
            {
                if (_dictionary.TryGetValue("Laboratorios de I+D e Investigación", out val)) return val;
            }
            if (ContainsWord(raw, "Mantenimiento") || ContainsWord(raw, "Maintenance Facility"))
            {
                if (_dictionary.TryGetValue("Instalaciones de Mantenimiento Naval", out val)) return val;
            }
            if (ContainsWord(raw, "Academia") || ContainsWord(raw, "Academias") || ContainsWord(raw, "Military Academy"))
            {
                if (_dictionary.TryGetValue("Academias Militares de Oficiales", out val)) return val;
            }
            if (ContainsWord(raw, "CIWS"))
            {
                if (_dictionary.TryGetValue("CIWS", out val)) return val;
            }
            if (ContainsWord(raw, "Fighter") || ContainsWord(raw, "Vástago") || ContainsWord(raw, "Pod Bay"))
            {
                if (_dictionary.TryGetValue("Fighter Pod Bay", out val)) return val;
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

            // 7. Substring match for compound DB technology names (Strict: prefix match with long key)
            foreach (var kvp in _dictionary)
            {
                if (kvp.Key.Length >= 8)
                {
                    if (cleaned.StartsWith(kvp.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        if (IsRichContent(kvp.Value)) return kvp.Value;
                    }
                }
            }

            // 8. Final Fallback to TechDescriptionResolver
            return resolved;
        }

        private static bool IsRichContent(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            return text.Contains("CONCEPTO") || text.Length > 120;
        }

        private static bool ContainsWord(string source, string target)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target)) return false;
            return source.IndexOf(target, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static ToolTip? CreateTutorToolTip(string? keyOrTerm, string? customTitle = null, string? categoryHint = null)
        {
            if (!IsTutorEnabled) return null;

            string? bodyText = GetTutorText(keyOrTerm, categoryHint);
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
                MaxWidth = 520
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

        public static bool IsCommanderOrLeader(object? dc)
        {
            if (dc == null) return false;
            if (dc is CommanderInfo || dc is ScientistInfo || dc is FleetCommanderInfo) return true;
            string typeName = dc.GetType().Name;
            if (typeName.Equals("EmpireOfficerSummary", StringComparison.OrdinalIgnoreCase)) return false;
            return typeName.Contains("Commander") || typeName.Contains("Scientist") || typeName.Contains("Officer") || typeName.Contains("Leader");
        }

        public static ToolTip? CreateCommanderToolTip(object leaderObj, string? customTitle = null)
        {
            if (!IsTutorEnabled || leaderObj == null) return null;

            string name = "";
            string title = "Oficial Imperial";
            string roleType = "Alto Mando";
            string icon = "🎖️";
            string location = "Sin Asignar";
            string seniorityStr = "Rating 0";
            string loyaltyStr = "100%";
            string promotionStr = "50/100";
            string bonusesStr = "Sin bonificaciones destacadas";
            string impactStr = "Proporciona bonificaciones tácticas y estratégicas al ser asignado.";

            if (leaderObj is CommanderInfo c)
            {
                name = c.Name;
                title = c.Title;
                roleType = c.TypeDisplay;
                icon = c.RoleIcon;
                location = c.AssignmentLocation;
                seniorityStr = $"{c.Seniority:N0} Rating";
                loyaltyStr = $"{c.LoyaltyRating:F0}%";
                promotionStr = $"{c.PromotionScore:F0} / 100";
                if (c.DetailedBonuses != null && c.DetailedBonuses.Count > 0)
                {
                    bonusesStr = string.Join("\n• ", c.DetailedBonuses.Select(b => $"{b.Description}: {b.ValueDisplay}"));
                }
                else
                {
                    bonusesStr = c.PrimaryBonusDisplay;
                }

                impactStr = c.CommanderType switch
                {
                    1 => $"Científico especializado en I+D. Al asignarlo a un proyecto de laboratorio, incrementa los RP/año generados en su área.",
                    2 => $"Comandante naval de flota. Mejora la maniobrabilidad, precisión de disparo y coordinación defensiva de su escuadra.",
                    3 => $"Gobernador colonial. Incrementa la producción de minas, rendimiento industrial y crecimiento poblacional de la colonia.",
                    _ => $"Comandante militar terrestre. Potencia el poder de combate, moral y capacidad de penetración de las unidades del ejército."
                };
            }
            else if (leaderObj is ScientistInfo s)
            {
                name = s.Name;
                title = "Investigador / Científico I+D";
                roleType = $"Científico ({s.FieldName})";
                icon = "👨‍🔬";
                location = $"{s.MaxLabs} Labs Máximos";
                seniorityStr = $"{s.Seniority:N0} Rating";
                loyaltyStr = $"{s.Loyalty:F0}%";
                promotionStr = "N/A";
                bonusesStr = $"+{s.BonusPercent:F0}% Eficiencia en {s.FieldName}";
                impactStr = $"Acelera los proyectos de investigación en el área de {s.FieldName} en un +{s.BonusPercent:F0}%.";
            }
            else if (leaderObj is FleetCommanderInfo fc)
            {
                if (!fc.HasCommander) return null;
                name = fc.Name;
                title = fc.RankName;
                roleType = "Comandante de Flota";
                icon = "⚓";
                location = "Flota Activa";
                seniorityStr = $"{fc.Seniority:N0} Rating";
                loyaltyStr = $"{fc.Loyalty:F0}%";
                promotionStr = "Activo";
                bonusesStr = fc.AllBonusesDisplay;
                impactStr = "Manda la escuadra naval proporcionando bonificaciones tácticas a todos los buques asignados.";
            }
            else
            {
                var t = leaderObj.GetType();
                name = t.GetProperty("Name")?.GetValue(leaderObj)?.ToString() ?? 
                       t.GetProperty("OfficerName")?.GetValue(leaderObj)?.ToString() ?? "Comandante";
                title = t.GetProperty("Title")?.GetValue(leaderObj)?.ToString() ?? 
                        t.GetProperty("RankTitle")?.GetValue(leaderObj)?.ToString() ?? "Oficial";
                roleType = t.GetProperty("TypeDisplay")?.GetValue(leaderObj)?.ToString() ?? 
                           t.GetProperty("RoleType")?.GetValue(leaderObj)?.ToString() ?? "Alto Mando";
                location = t.GetProperty("AssignmentLocation")?.GetValue(leaderObj)?.ToString() ?? 
                           t.GetProperty("Assignment")?.GetValue(leaderObj)?.ToString() ?? "Asignado";
                bonusesStr = t.GetProperty("PrimaryBonusDisplay")?.GetValue(leaderObj)?.ToString() ?? 
                             t.GetProperty("PrimaryBonus")?.GetValue(leaderObj)?.ToString() ?? "+15% Eficiencia General";
            }

            string headerTitle = !string.IsNullOrEmpty(customTitle) ? customTitle : $"{icon} EXPEDIENTE DE MANDO: {name}";

            string bodyText =
                $"👤 FICHA PERSONAL & EXPEDIENTE DE MANDO:\n" +
                $"{name} es un {roleType} del Imperio.\n" +
                $"• Rango / Título: {title}\n" +
                $"• Estado / Destino: {location}\n" +
                $"• Antigüedad & Lealtad: {seniorityStr} | Lealtad: {loyaltyStr}\n" +
                $"• Mérito de Ascenso: {promotionStr}\n\n" +
                $"⚙️ HABILIDADES & BONIFICACIONES:\n" +
                $"• {bonusesStr}\n\n" +
                $"💡 IMPACTO EN EL IMPERIO:\n" +
                $"{impactStr}\n\n" +
                $"🛡️ CONSEJO TÁCTICO IMPERIAL:\n" +
                $"Inspecciona los méritos de {name} desde el Cuartel General de Líderes para asignarle misiones estratégicas o promover su rango.";

            ToolTip toolTip = new ToolTip
            {
                Background = new SolidColorBrush(Color.FromArgb(245, 11, 16, 26)),
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 176, 0)), // Amber Gold for Leader Dossiers
                BorderThickness = new Thickness(1.5),
                Padding = new Thickness(12),
                Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint,
                HasDropShadow = true
            };

            Border cardBorder = new Border
            {
                CornerRadius = new CornerRadius(6),
                MaxWidth = 520
            };

            StackPanel panel = new StackPanel();

            TextBlock lblTitle = new TextBlock
            {
                Text = headerTitle,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 176, 0)),
                FontWeight = FontWeights.Bold,
                FontSize = 13,
                Margin = new Thickness(0, 0, 0, 8),
                TextWrapping = TextWrapping.Wrap
            };
            panel.Children.Add(lblTitle);

            Border line = new Border
            {
                Height = 1,
                Background = new SolidColorBrush(Color.FromArgb(120, 255, 176, 0)),
                Margin = new Thickness(0, 0, 0, 8)
            };
            panel.Children.Add(line);

            TextBlock lblBody = new TextBlock
            {
                Text = bodyText,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 230, 237, 243)),
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
            if (element == null) return;

            if (!IsTutorEnabled || string.IsNullOrWhiteSpace(keyOrTerm))
            {
                element.ToolTip = null;
                return;
            }

            try
            {
                if (element.DataContext != null && IsCommanderOrLeader(element.DataContext))
                {
                    element.ToolTip = CreateCommanderToolTip(element.DataContext, customTitle);
                    return;
                }

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
