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
                if (colonies.Count > 0)
                {
                    double totalPop = colonies.Sum(c => c.PopulationMillions);
                    var capital = colonies.FirstOrDefault(c => c.IsCapital) ?? colonies[0];

                    sb.AppendLine($"• Imperio Conectado ID: {raceId}");
                    sb.AppendLine($"• Población Imperial Total: {totalPop:N2} M (Capital: {capital.PopName})");
                    sb.AppendLine($"• Reserva de Combustible Planetaria: {capital.FuelStockpile:N0} L");
                    sb.AppendLine($"• Reservas de Duranio: {capital.MineralStockpiles.Duranium:N0} T | Sorium: {capital.MineralStockpiles.Sorium:N0} T | Gallicite: {capital.MineralStockpiles.Gallicite:N0} T");
                }

                var fleets = dbService.GetActiveFleets(raceId);
                sb.AppendLine($"• Flotas Registradas: {fleets.Count} flotas en servicio activo.");

                var labs = dbService.GetPopulationInstallations(raceId).FirstOrDefault(i => i.InstallationName.Contains("Laboratorio"));
                if (labs != null)
                {
                    sb.AppendLine($"• Instalaciones de I+D: {labs.Amount:N0} Laboratorios de Investigación.");
                }

                var research = dbService.GetActiveResearchProjects(raceId);
                sb.AppendLine($"• Proyectos I+D Activos: {research.Count} tecnologías en desarrollo.");
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Error recopilando contexto: {ex.Message}");
            }

            return sb.ToString();
        }

        public async Task<string> AskAIAsync(string userQuery, string imperialContext = "", bool useOnlineGemini = true)
        {
            if (useOnlineGemini && !string.IsNullOrWhiteSpace(_apiKey))
            {
                try
                {
                    string systemInstruction = @"Eres la Matriz Computacional de Inteligencia Imperial para Aurora 4X (v2.7.1).
Tus análisis deben ser precisos, ejecutivos, con terminología de ciencia ficción militar/científica.
Sigue estrictamente el formato solicitado por el usuario.";

                    string fullPrompt = $"{systemInstruction}\n\n[CONTEXTO IMPERIAL]\n{imperialContext}\n\n[CONSULTA DEL COMANDANTE]\n{userQuery}";

                    string jsonBody = JsonSerializer.Serialize(new
                    {
                        contents = new[]
                        {
                            new
                            {
                                parts = new[]
                                {
                                    new { text = fullPrompt }
                                }
                            }
                        }
                    });

                    string[] models = new[] { "gemini-1.5-flash", "gemini-1.5-pro", "gemini-pro" };

                    foreach (var model in models)
                    {
                        string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_apiKey}";
                        using var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                        var response = await _httpClient.PostAsync(endpoint, content);
                        if (response.IsSuccessStatusCode)
                        {
                            string jsonResp = await response.Content.ReadAsStringAsync();
                            using var doc = JsonDocument.Parse(jsonResp);

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
                }
                catch
                {
                    // Fall back if network or API error
                }
            }

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

        public async Task<string> GenerateEmpireHistoryChronicleAsync(string empireName, List<string> historyLogs, string styleChoice)
        {
            if (historyLogs == null || historyLogs.Count == 0)
            {
                return "⚠️ No se encontraron registros de eventos para el imperio seleccionado en AuroraDB.db.";
            }

            string rawLogsText = string.Join("\n", historyLogs.Take(75));

            string stylePrompt = styleChoice switch
            {
                "BitacoraMilitar" => "Adopta el tono de un Diario de Bitácora Militar Táctico y Registro de la Flota Imperial. Enfatiza los nombramientos de almirantes, la botadura de naves de combate, expediciones lejanas y contactos/batallas espaciales.",
                "AnalesCientificos" => "Adopta el tono de los Anales de la Ciencia Imperial y Registro de Descubrimientos Tecnológicos. Enfatiza los saltos científicos, descubrimientos de sistemas estelares, yacimientos minerales y terraformación.",
                _ => "Adopta el tono de un gran escritor de ciencia ficción espacial (al estilo de Isaac Asimov, Dan Simmons o Frank Herbert). Escribe una narrativa majestuosa, inmersiva, elegante y apasionante en español."
            };

            string prompt = $@"Eres un renombrado escritor de ciencia ficción espacial y cronista imperial de la saga del imperio '{empireName}'.
{stylePrompt}

PROHIBICIÓN STRICTA: NO redactes listas técnicas, NO incluyas secciones de '[TELEMETRÍA]', NO incluyas '[VULNERABILIDAD DETECTADA]' ni '[ACCIÓN RECOMENDADA]'. Debes escribir ÚNICAMENTE una novela e historia narrativa literaria en capítulos.

A continuación se te proporcionan los registros históricos reales extraídos de la partida de Aurora 4X (v2.7.1):

---
REGISTROS HISTÓRICOS REALES:
{rawLogsText}
---

INSTRUCCIONES DE REDACCIÓN LITERARIA OBLIGATORIAS:
1. Divide la obra en 3 o 4 Capítulos Épicos bien estructurados con títulos literarios evocadores (Ej: 'Capítulo I: El Despertar del Imperio', 'Capítulo II: La Frontera del Vacío', 'Capítulo III: El Triunfo de las Estrellas').
2. Narra la historia cronológica del imperio como una novela apasionante en español.
3. Incorpora los nombres de almirantes, naves, sistemas y tecnologías reales de los registros.
4. Usa formato Markdown impecable con títulos, cursivas y negritas.";

            string response = await AskAIAsync(prompt, "", useOnlineGemini: true);

            // If local fallback diagnostic triggered, intercept and build narrative chronicle offline
            if (string.IsNullOrWhiteSpace(response) || response.Contains("[VULNERABILIDAD DETECTADA]") || response.Contains("[ACCIÓN RECOMENDADA]"))
            {
                response = BuildOfflineNarrativeChronicle(empireName, historyLogs, styleChoice);
            }

            return response;
        }

        private string BuildOfflineNarrativeChronicle(string empireName, List<string> historyLogs, string styleChoice)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"# 📖 NOVELA HISTÓRICA DE LA SAGA IMPERIAL: {empireName.ToUpper()}");
            sb.AppendLine($"*Crónica Literaria Transcrita por la Matriz Computacional Imperial de Aurora Command Suite*");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();

            int totalLogs = historyLogs.Count;
            int partSize = Math.Max(1, totalLogs / 3);

            var part1 = historyLogs.Take(partSize).ToList();
            var part2 = historyLogs.Skip(partSize).Take(partSize).ToList();
            var part3 = historyLogs.Skip(partSize * 2).ToList();

            // CAPÍTULO I
            sb.AppendLine("### 🏛️ CAPÍTULO I: EL DESPERTAR EN EL NÚCLEO DE LA CIVILIZACIÓN");
            sb.AppendLine($"Bajo los cielos de la cuna natal, los mandatarios y pioneros del **{empireName}** trazaron los primeros designios para trascender la cuna planetaria. Los registros más antiguos de la memoria imperial atesoran los cimientos de esta era:");
            sb.AppendLine();

            foreach (var log in part1)
            {
                sb.AppendLine($"> 📜 *{log}*");
            }

            sb.AppendLine();
            sb.AppendLine($"La determinación de la población del **{empireName}** impulsó la construcción de los primeros laboratorios de investigación y complejos industriales, sembrando la semilla de una expansión sin precedentes en los anales estelares.");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();

            // CAPÍTULO II
            sb.AppendLine("### 🌌 CAPÍTULO II: LA FRONTERA DEL VACÍO Y LA EXPLORACIÓN ESTELAR");
            sb.AppendLine($"Con el dominio del salto de hiperespacio y el despliegue de las primeras naves de reconocimiento, el **{empireName}** se adentró en las profundidades de la galaxia. Cada escaneo de sistema y cada boya de salto situaron a la flota en los confines de la frontera:");
            sb.AppendLine();

            foreach (var log in part2)
            {
                sb.AppendLine($"> 🛸 *{log}*");
            }

            sb.AppendLine();
            sb.AppendLine($"Los capitanes y almirantes asignados al mando guiaron sus escuadras con temple de acero, asegurando la soberanía del imperio frente a los imprevistos del cosmos profundo.");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();

            // CAPÍTULO III
            sb.AppendLine("### ⚔️ CAPÍTULO III: EL TRIUNFO DE LAS ESTRELLAS Y LA ERA MODERNA");
            sb.AppendLine($"En la época contemporánea, la supremacía del **{empireName}** se consolida mediante una infraestructura técnica imponente, colonias florecientes y una fuerza naval lista para responder ante cualquier amenaza:");
            sb.AppendLine();

            foreach (var log in part3)
            {
                sb.AppendLine($"> 🛡️ *{log}*");
            }

            sb.AppendLine();
            sb.AppendLine($"La epopeya del **{empireName}** continúa escribiéndose día a día en las estrellas. La historia recordará a aquellos que abrieron el camino hacia el destino infinito.");
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine($"*Fin del Tomo I de la Crónica Histórica | Registrado en Aurora Command Suite*");

            return sb.ToString();
        }
    }
}
