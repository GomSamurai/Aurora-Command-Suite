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
            _apiKey = string.IsNullOrWhiteSpace(apiKey) ? ApiKeyManager.GetApiKey() : apiKey;
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
                    sb.AppendLine($"• Colonias Registradas: {colonies.Count} hábitats planetarios.");
                    sb.AppendLine($"• Reserva de Combustible Planetaria: {capital.FuelStockpile:N0} L");
                    sb.AppendLine($"• Reservas en Capital: Duranium: {capital.MineralStockpiles.Duranium:N0} T | Sorium: {capital.MineralStockpiles.Sorium:N0} T | Gallicite: {capital.MineralStockpiles.Gallicite:N0} T | Corundium: {capital.MineralStockpiles.Corundium:N0} T");
                }

                var fleets = dbService.GetActiveFleets(raceId);
                int activeFleetsCount = fleets.Count(f => f.ShipCount > 0);
                int totalShips = fleets.Sum(f => f.ShipCount);
                sb.AppendLine($"• Flotas Registradas: {fleets.Count} flotas ({activeFleetsCount} activas con naves, {totalShips} naves en total).");

                foreach (var f in fleets.Where(fl => fl.ShipCount > 0))
                {
                    sb.AppendLine($"  - Flota '{f.FleetName}': {f.ShipCount} Nave(s) activa(s). Combustible: {f.TotalFuelLiters:N0} L.");
                    if (f.Ships != null && f.Ships.Count > 0)
                    {
                        foreach (var s in f.Ships)
                        {
                            sb.AppendLine($"     • Nave: '{s.ShipName}' (Clase: {s.ClassName}, Desplazamiento: {s.Tonnage:N0} T)");
                        }
                    }
                }

                var infra = dbService.GetEmpireInfrastructure(raceId);
                double constFactories = infra.Where(i => i.Name.Contains("Construcción") || i.Name.Contains("Convencional")).Sum(i => i.Amount);
                sb.AppendLine($"• Capacidad Industrial: {constFactories:N0} Fábricas activas.");

                var research = dbService.GetActiveResearchProjects(raceId);
                sb.AppendLine($"• Proyectos I+D Activos: {research.Count} proyectos en desarrollo.");
                foreach (var r in research.Take(5))
                {
                    sb.AppendLine($"  - Proyecto: '{r.TechName}' (Progreso: {r.ProgressPercent:F1}%)");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Error recopilando contexto: {ex.Message}");
            }

            return sb.ToString();
        }

        public async Task<string> AskAIAsync(string userQuery, string imperialContext = "", bool useOnlineGemini = true)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _apiKey = ApiKeyManager.GetApiKey();
            }
            _apiKey = ApiKeyManager.CleanApiKey(_apiKey);

            string lastError = "";

            if (useOnlineGemini && !string.IsNullOrWhiteSpace(_apiKey))
            {
                try
                {
                    string systemInstruction = @"Eres la Matriz Computacional de Inteligencia Imperial para Aurora 4X (v2.7.1).
Analiza los datos reales del juego proporcionados en el contexto teleférico.
Tus respuestas deben ser precisas, solemnes, en español y basadas estrictamente en la telemetría del contexto imperial en tiempo real.
NO inventes datos si el contexto teleférico contiene la información exacta.";

                    string fullPrompt = $"{systemInstruction}\n\n[TELEMETRÍA IMPERIAL EN TIEMPO REAL]\n{imperialContext}\n\n[CONSULTA DEL COMANDANTE IMPERIAL]\n{userQuery}";

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

                    string[] models = new[]
                    {
                        "gemini-pro-latest",
                        "gemini-flash-latest",
                        "gemini-3.5-flash",
                        "gemini-3.7-flash",
                        "gemini-flash-lite-latest",
                        "gemini-1.5-flash",
                        "gemini-1.5-pro"
                    };

                    foreach (var model in models)
                    {
                        try
                        {
                            string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_apiKey}";
                            using var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                            var response = await _httpClient.PostAsync(endpoint, content);
                            string respBody = await response.Content.ReadAsStringAsync();

                            if (response.IsSuccessStatusCode)
                            {
                                using var doc = JsonDocument.Parse(respBody);
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
                            else
                            {
                                int code = (int)response.StatusCode;
                                lastError = $"[Modelo {model}] HTTP {code}: {respBody}";

                                // If API key is invalid or quota exceeded or permission denied, stop loop and report immediately!
                                if (code == 400 || code == 403)
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception mEx)
                        {
                            lastError = $"[Modelo {model}] Excepción: {mEx.Message}";
                        }
                    }
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;
                }
            }

            string localDiag = GenerateLocalDiagnostic(userQuery, imperialContext);

            if (useOnlineGemini)
            {
                if (string.IsNullOrWhiteSpace(_apiKey))
                {
                    return $"⚠️ MODO LOCAL ACTIVO: No hay una clave Gemini API Key configurada.\nUtiliza el botón '⚙️ Configurar API Key' en la cabecera para vincular tu clave Gemini.\n\n---\n{localDiag}";
                }
                else if (!string.IsNullOrEmpty(lastError))
                {
                    return $"⚠️ ERROR AL COMUNICAR CON GEMINI API:\n{lastError}\n\n💡 Sugerencia: Revisa o renueva tu API Key en aistudio.google.com mediante el botón '⚙️ Configurar API Key'.\n\n---\n{localDiag}";
                }
            }

            return localDiag;
        }

        public string GenerateLocalDiagnostic(string userQuery, string imperialContext)
        {
            var sb = new StringBuilder();

            sb.AppendLine("🖥️ [ANÁLISIS DE TELEMETRÍA IMPERIAL EN VIVO]");
            if (!string.IsNullOrWhiteSpace(imperialContext))
            {
                sb.AppendLine(imperialContext.Trim());
            }
            else
            {
                sb.AppendLine("• Telemetría sincronizada desde base de datos activa AuroraDB.db.");
            }

            sb.AppendLine();
            sb.AppendLine("[DIAGNÓSTICO TÁCTICO IMPERIAL]");
            sb.AppendLine("1. Evaluación Naval: Revisa la composición de tus flotas y disponibilidad de combustible Sorium en depósitos.");
            sb.AppendLine("2. Logística Industrial: Mantén el balance entre extracción de Duranium y consumo en astilleros.");
            sb.AppendLine("3. Seguridad Espacial: Asegura repuestos MSP antes de emprender expediciones de exploración profunda.");

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
