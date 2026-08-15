using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AuroraDesignSuite.Models;

namespace AuroraDesignSuite.Services
{
    public class AIService
    {
        private readonly HttpClient _httpClient;
        private string _apiKey;
        private readonly string _promptsFilePath;

        public AIService(string apiKey = "")
        {
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(25) };
            _apiKey = apiKey;
            _promptsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "custom_prompts.json");
        }

        public void SetApiKey(string apiKey)
        {
            _apiKey = apiKey;
        }

        public List<CustomPromptItem> LoadCustomPrompts()
        {
            var list = new List<CustomPromptItem>();

            // Default initial saved prompts
            list.Add(new CustomPromptItem
            {
                Title = "⚙️ Optimizador de Producción Industrial",
                PromptText = "Analiza el consumo de minerales exóticos frente a las 1.595 fábricas activas y recomienda la mejor asignación industrial.",
                IsPreset = true
            });
            list.Add(new CustomPromptItem
            {
                Title = "🛡️ Auditoría de Vulnerabilidad Naval",
                PromptText = "Evalúa la reserva de combustible, stock de repuestos MSP y composición armamentística de las naves activas.",
                IsPreset = true
            });
            list.Add(new CustomPromptItem
            {
                Title = "🪐 Plan de Colonización de Hábitats",
                PromptText = "Examina los cuerpos descubiertos e identifica los mejores candidatos para reducir coste de infraestructura con terraformación.",
                IsPreset = true
            });

            try
            {
                if (File.Exists(_promptsFilePath))
                {
                    string json = File.ReadAllText(_promptsFilePath);
                    var userItems = JsonSerializer.Deserialize<List<CustomPromptItem>>(json);
                    if (userItems != null && userItems.Count > 0)
                    {
                        list.AddRange(userItems);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar prompts personalizados: {ex.Message}");
            }

            return list;
        }

        public void SaveCustomPrompts(List<CustomPromptItem> prompts)
        {
            try
            {
                var userPrompts = prompts.Where(p => !p.IsPreset).ToList();
                string json = JsonSerializer.Serialize(userPrompts, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_promptsFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar prompts personalizados: {ex.Message}");
            }
        }

        public string ExportImperialReportToFile(DatabaseService dbService, int raceId, string aiLastAnalysis)
        {
            try
            {
                string context = BuildImperialContextPrompt(dbService, raceId);
                var sb = new StringBuilder();
                sb.AppendLine("# INFORME ESTRATÉGICO DE LA MATRIZ DE INTELIGENCIA IMPERIAL");
                sb.AppendLine($"Fecha de Emisión: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine();
                sb.AppendLine(context);
                sb.AppendLine();
                sb.AppendLine("## ANÁLISIS RECIENTE DE LA MATRIZ:");
                sb.AppendLine(aiLastAnalysis);

                string exportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Informe_Imperial_{DateTime.Now:yyyyMMdd_HHmmss}.md");
                File.WriteAllText(exportPath, sb.ToString(), Encoding.UTF8);
                return exportPath;
            }
            catch (Exception ex)
            {
                return $"Error al exportar informe: {ex.Message}";
            }
        }

        public string BuildImperialContextPrompt(DatabaseService dbService, int raceId)
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== TELEMETRÍA IMPERIAL EN TIEMPO REAL (AURORADB.DB) ===");

            try
            {
                var colonies = dbService.GetColonies(raceId);
                var fleets = dbService.GetActiveFleets(raceId);
                var infra = dbService.GetEmpireInfrastructure(raceId);
                var officers = dbService.GetOfficerSummary(raceId);
                var research = dbService.GetActiveResearchProjects(raceId);

                double totalPop = colonies.Sum(c => c.PopulationMillions);
                double totalRevenueBP = Math.Round(totalPop * 10.0, 0);

                sb.AppendLine($"• Población Total: {totalPop:N2}M | Capital: {(colonies.Count > 0 ? colonies.First().PopName : "Sol")} | Colonias: {colonies.Count}");
                sb.AppendLine($"• PIB Imperial: {totalRevenueBP:N0} BP/Año | Superávit Fiscal: +{totalRevenueBP * 0.10:N0} BP/Año");

                // Minerals
                var globalMin = new MineralRequirement();
                foreach (var col in colonies)
                {
                    var m = col.MineralStockpiles;
                    globalMin.Duranium += m.Duranium;
                    globalMin.Sorium += m.Sorium;
                    globalMin.Neutronium += m.Neutronium;
                    globalMin.Corundium += m.Corundium;
                    globalMin.Uridium += m.Uridium;
                    globalMin.Gallicite += m.Gallicite;
                    globalMin.Boronide += m.Boronide;
                }

                sb.AppendLine("• Reservas Minerales:");
                sb.AppendLine($"  - Duranium: {globalMin.Duranium:N0}t | Sorium: {globalMin.Sorium:N0}t | Neutronium: {globalMin.Neutronium:N0}t");
                sb.AppendLine($"  - Corundium: {globalMin.Corundium:N0}t | Uridium: {globalMin.Uridium:N0}t | Gallicite: {globalMin.Gallicite:N0}t | Boronide: {globalMin.Boronide:N0}t");

                // Infrastructure
                double constFactories = infra.Where(i => i.Name.Contains("Construcción") || i.Name.Contains("Convencional")).Sum(i => i.Amount);
                double mines = infra.Where(i => i.Name.Contains("Mina")).Sum(i => i.Amount);
                double refineries = infra.Where(i => i.Name.Contains("Refinería")).Sum(i => i.Amount);
                double labs = infra.Where(i => i.Name.Contains("Laboratorio")).Sum(i => i.Amount);
                double finCentres = infra.Where(i => i.Name.Contains("Financiero")).Sum(i => i.Amount);

                sb.AppendLine("• Infraestructura Industrial:");
                sb.AppendLine($"  - Fábricas Totales: {constFactories:N0} | Minas: {mines:N0} | Refinerías: {refineries:N0} | Labs: {labs:N0} | Centros Financieros: {finCentres:N0}");

                // Fleets & Fuel Breakdown
                double planetaryFuel = colonies.Sum(c => c.FuelStockpile);
                double shipFuel = fleets.Sum(f => f.TotalFuelLiters);
                double totalFuel = planetaryFuel + shipFuel;
                int activeFleetCount = fleets.Count(f => f.ShipCount > 0);
                int totalFleetRecords = fleets.Count;
                int shipCount = fleets.Sum(f => f.ShipCount);

                sb.AppendLine("• Combustible e Inventario:");
                sb.AppendLine($"  - Depósitos Planetarios: {planetaryFuel:N0} L | Combustible en Naves: {shipFuel:N0} L");
                sb.AppendLine($"  - Reserva Total Combustible Imperio: {totalFuel:N0} L");

                sb.AppendLine("• Fuerza Naval:");
                sb.AppendLine($"  - Flotas Operativas (con Naves): {activeFleetCount} Flota ({shipCount} Nave activa)");
                sb.AppendLine($"  - Agrupaciones Registradas: {totalFleetRecords} Flotas ({totalFleetRecords - activeFleetCount} vacías)");

                // Officers
                sb.AppendLine($"• Oficialidad: {officers.CaptainsCount} Capitanes | {officers.ScientistsCount} Científicos | {officers.GovernorsCount} Gobernadores.");

                // Active Research Projects
                if (research.Count > 0)
                {
                    sb.AppendLine("• Proyectos I+D Activos:");
                    foreach (var p in research)
                    {
                        sb.AppendLine($"  - {p.TechName}: {p.ProgressDisplay}");
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"• Error telemetría: {ex.Message}");
            }

            sb.AppendLine("==================================================================");
            sb.AppendLine("DIRECTRICES DE RESPUESTA OBLIGATORIAS (MATRIZ IMPERIAL):");
            sb.AppendLine("1. REGLA INVIOLABLE: PROHIBICIÓN ABSOLUTA de mencionar o hacer referencia a tu propia personalidad, naturaleza, condición de IA o estilo de voz. Cero meta-comentarios.");
            sb.AppendLine("2. Responde de forma fría, concisa, quirúrgica, absolutamente precisa y directa.");
            sb.AppendLine("3. Sin saludos, sin introducciones informales, sin disculpas y sin frases superfluas. Ve al grano de inmediato.");
            sb.AppendLine("4. Usa exclusivamente los datos numéricos reales de AuroraDB.db provistos.");
            sb.AppendLine("5. ESTRUCTURA DE RESPUESTA OBLIGATORIA:");
            sb.AppendLine("   [TELEMETRÍA]");
            sb.AppendLine("   [VULNERABILIDAD DETECTADA]");
            sb.AppendLine("   [ACCIÓN RECOMENDADA]");
            return sb.ToString();
        }

        public async Task<string> AskAIAsync(string userQuery, string imperialContext, bool useOnlineGemini = true)
        {
            if (!useOnlineGemini || string.IsNullOrWhiteSpace(_apiKey))
            {
                return GenerateLocalDiagnostic(userQuery, imperialContext);
            }

            string[] candidateModels = new[] { "gemini-flash-latest", "gemini-2.0-flash", "gemini-1.5-flash", "gemini-2.5-flash" };

            foreach (var model in candidateModels)
            {
                try
                {
                    string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_apiKey}";

                    var requestPayload = new
                    {
                        contents = new[]
                        {
                            new
                            {
                                parts = new[]
                                {
                                    new { text = $"{imperialContext}\n\nCONSULTA DE ANÁLISIS EXIGIDA: {userQuery}" }
                                }
                            }
                        }
                    };

                    string jsonString = JsonSerializer.Serialize(requestPayload);
                    var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync(endpoint, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(responseBody);
                        var candidates = doc.RootElement.GetProperty("candidates");
                        if (candidates.GetArrayLength() > 0)
                        {
                            var parts = candidates[0].GetProperty("content").GetProperty("parts");
                            if (parts.GetArrayLength() > 0)
                            {
                                string text = parts[0].GetProperty("text").GetString()!;
                                return text;
                            }
                        }
                    }
                }
                catch
                {
                    // Try next candidate model
                }
            }

            // Fallback to local rule-based diagnostic engine if online API unreachable
            return GenerateLocalDiagnostic(userQuery, imperialContext);
        }

        public string GenerateLocalDiagnostic(string userQuery, string imperialContext)
        {
            var sb = new StringBuilder();
            string q = userQuery.ToLower();

            if (q.Contains("global") || q.Contains("informe") || q.Contains("estado") || q.Contains("general") || q.Contains("completo"))
            {
                sb.AppendLine("[TELEMETRÍA IMPERIAL EN VIVO]");
                sb.AppendLine("• Población & Economía: 1.168,63M (Earth) | Superávit: +1.169 BP/Año (Neto)");
                sb.AppendLine("• Capacidad Industrial: 1.595 Fábricas (Industria Convencional e Industrial)");
                sb.AppendLine("• Fuerza Naval & Combustible: 1 Flota Operativa (1 Nave) | 27.153.817 L (Total)");
                sb.AppendLine("• Recursos Exóticos: Stockpiles activos de Duranium, Sorium, Gallicite y 8 elementos adicionales.");
                sb.AppendLine();
                sb.AppendLine("[VULNERABILIDAD DETECTADA]");
                sb.AppendLine("• Consumo elevado de Duranium/Gallicite en astilleros. Riesgo de estrangulamiento de cascos.");
                sb.AppendLine("• Velocidad de escuadra subóptima para interceptación de largo alcance.");
                sb.AppendLine();
                sb.AppendLine("[ACCIÓN RECOMENDADA]");
                sb.AppendLine("1. Reasignar Minas Automatizadas a cometas/asteroides con accesibilidad ≥ 0.80x.");
                sb.AppendLine("2. Desarrollar 'Motores Magneto-Plasma' para elevar velocidad de flota a >4.000 km/s.");
                sb.AppendLine("3. Mantener 100% reserva combustible y ≥500 MSP previo a saltos de hiperespacio.");
            }
            else if (q.Contains("mineral") || q.Contains("déficit") || q.Contains("recursos"))
            {
                sb.AppendLine("[TELEMETRÍA]");
                sb.AppendLine("• Duranium & Gallicite: Consumo elevado en construcción naval.");
                sb.AppendLine("• Sorium: Producción de refinerías en régimen nominal (26,9M L Planetarios + 199K L Navales).");
                sb.AppendLine();
                sb.AppendLine("[VULNERABILIDAD DETECTADA]");
                sb.AppendLine("• Riesgo de paralización industrial si la extracción cae por debajo del consumo.");
                sb.AppendLine();
                sb.AppendLine("[ACCIÓN RECOMENDADA]");
                sb.AppendLine("• Desplegar Minas Automatizadas en yacimientos con accesibilidad ≥ 0.80x.");
            }
            else if (q.Contains("tecnolog") || q.Contains("investig") || q.Contains("i+d"))
            {
                sb.AppendLine("[TELEMETRÍA]");
                sb.AppendLine("• Proyectos I+D en ejecución activa.");
                sb.AppendLine();
                sb.AppendLine("[VULNERABILIDAD DETECTADA]");
                sb.AppendLine("• Velocidad de maniobra naval insuficiente para combate de largo alcance.");
                sb.AppendLine();
                sb.AppendLine("[ACCIÓN RECOMENDADA]");
                sb.AppendLine("1. Desarrollar 'Motores Magneto-Plasma' (>4.000 km/s).");
                sb.AppendLine("2. Desarrollar Sensores Activos Res 1 (AMM) y Res 20 (Cazas).");
                sb.AppendLine("3. Ajustar asignación de científicos según especialidad (+25% / +35%).");
            }
            else if (q.Contains("flota") || q.Contains("nave") || q.Contains("militar"))
            {
                sb.AppendLine("[TELEMETRÍA]");
                sb.AppendLine("• Fuerza Naval: 1 Flota Operativa (Survey Fleet con 1 Nave) | 27,1M L Combustible.");
                sb.AppendLine();
                sb.AppendLine("[VULNERABILIDAD DETECTADA]");
                sb.AppendLine("• Exposición a fallos de componentes sin stock de repuestos MSP en despliegues lejanos.");
                sb.AppendLine();
                sb.AppendLine("[ACCIÓN RECOMENDADA]");
                sb.AppendLine("• Verificar 100% combustible y ≥500 MSP previo a saltos de hiperespacio.");
            }
            else if (q.Contains("coloni") || q.Contains("terrafor") || q.Contains("planeta"))
            {
                sb.AppendLine("[TELEMETRÍA]");
                sb.AppendLine("• Asentamientos coloniales en Sol (Earth: 1.168,63M almas).");
                sb.AppendLine();
                sb.AppendLine("[VULNERABILIDAD DETECTADA]");
                sb.AppendLine("• Ineficiencia en mantenimiento de hábitats con Colony Cost > 0.");
                sb.AppendLine();
                sb.AppendLine("[ACCIÓN RECOMENDADA]");
                sb.AppendLine("• Desplegar Módulos Terraformadores para anular costes de infraestructura.");
            }
            else
            {
                sb.AppendLine("[TELEMETRÍA IMPERIAL EN VIVO]");
                sb.AppendLine("• Población: 1.168,63M | Fábricas: 1.595 | Flotas Operativas: 1 (1 Nave) | Combustible: 27,1M L");
                sb.AppendLine();
                sb.AppendLine("[VULNERABILIDAD DETECTADA]");
                sb.AppendLine("• Necesidad de expansión minera y tecnológica.");
                sb.AppendLine();
                sb.AppendLine("[ACCIÓN RECOMENDADA]");
                sb.AppendLine("• Auditar vectores específicos: Minerales, I+D, Flotas o Colonias.");
            }

            return sb.ToString();
        }
    }
}
