using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AuroraDesignSuite.Services
{
    public static class ImperialChronicleTranslator
    {
        private static readonly List<(Regex Pattern, string Replacement)> TranslationRules = new();

        static ImperialChronicleTranslator()
        {
            try
            {
                // 1. Research & Science Events
                AddRule(@"^A science team led by (.+?) working on (.+?) has completed research into (.+?)$",
                    "🔬 HITO CIENTÍFICO: El equipo de I+D liderado por $1 en $2 ha culminado exitosamente la investigación de $3.");

                AddRule(@"^Research into (.+?) completed$",
                    "🔬 HITO CIENTÍFICO: La investigación de $1 ha sido completada con éxito por el Departamento de Ciencia Imperial.");

                AddRule(@"^The Research Admin bonus of (.+?) has increased to (.+?)$",
                    "🧠 DESARROLLO CIENTÍFICO: La capacidad de administración de I+D del oficial $1 se ha incrementado al $2.");

                // 2. Officers, Promotions, Retirements & Deaths
                AddRule(@"^([^\.\,\n]+?) has retired from the service at the age of (\d+)\.\s*Current Assignment:\s*(.+?)$",
                    "🎖️ RETIRO CON HONORES: El oficial $1 ha pasado a retiro del servicio imperial a la edad de $2 años. Asignación previa: $3.");

                AddRule(@"^([^\.\,\n]+?) promoted to (.+?)$",
                    "🎖️ DECRETO DE ASCENSO: El oficial $1 ha sido promovido al rango de $2 por sus distinguidos servicios a la Corona Imperial.");

                AddRule(@"^([^\.\,\n]+?) has been killed in an accident\.\s*Assignment prior to death:\s*(.+?)$",
                    "✝️ IN MEMORIAM: El oficial $1 ha fallecido trágicamente en acto de servicio debido a un accidente. Puesto previo: $2.");

                AddRule(@"^The (.+?) bonus of (.+?) has increased to (.+?)$",
                    "🎖️ PERFECCIONAMIENTO TÁCTICO: La bonificación de $1 del oficial $2 se ha incrementado al $3.");

                // 3. Stellar Exploration & Mineral Discoveries
                AddRule(@"^([^\.\,\n]+?) under the command of (.+?) has discovered the new system of (.+?)$",
                    "🧭 EXPLORACIÓN ESTELAR: La nave $1 al mando del oficial $2 ha descubierto el nuevo sistema estelar $3.");

                AddRule(@"^([^\.\,\n]+?) under the command of (.+?) discovered minerals on (.+?):\s*(.+?)$",
                    "💎 PROSPECCIÓN GEOLÓGICA: La nave $1 comandada por $2 ha hallado yacimientos de minerales exóticos en $3: $4.");

                AddRule(@"^([^\.\,\n]+?) under the command of (.+?) discovered a new jump point in (.+?)$",
                    "🌌 PUNTO DE SALTO: La nave $1 al mando de $2 ha localizado un nuevo punto de salto astrofísico en $3.");

                AddRule(@"^Gravitational survey completed in (.+?)$",
                    "🪐 PROSPECCIÓN GRAVITACIONAL: Estudio de resonancia gravitatoria completado con éxito en el sistema $1.");

                AddRule(@"^Ground survey completed on (.+?)$",
                    "🌍 PROSPECCIÓN TERRESTRE: Levantamiento geológico de superficie completado en el cuerpo celeste $1.");

                AddRule(@"^([^\.\,\n]+?) conducted an orbital survey of (.+?) that revealed the potential for a ground survey is (.+?)$",
                    "🛰️ ESCÁNER ORBITAL: La exploración orbital realizada por $1 en $2 reveló un potencial de prospección terrestre $3.");

                // 4. Industry, Ship Construction & Ground Troops
                AddRule(@"^Production of (.+?) completed at (.+?)$",
                    "🏭 PRODUCCIÓN INDUSTRIAL: Concluida la fabricación e instalación de $1 en la colonia de $2.");

                AddRule(@"^Production of (.+?) has begun at (.+?)$",
                    "🏭 INICIO DE FABRICACIÓN: Comenzados los trabajos industriales de $1 en la colonia de $2.");

                AddRule(@"^([^\.\,\n]+?) trained on (.+?)$",
                    "🪖 ADIESTRAMIENTO DE TROPAS: La unidad terrestre $1 ha completado su entrenamiento combate en $2.");

                AddRule(@"^([^\.\,\n]+?) built at (.+?) and assigned to (.+?)$",
                    "⚓ BOTADURA NAVAL: La nave $1 ha finalizado su construcción en los astilleros de $2 y ha sido asignada a $3.");

                AddRule(@"^A civilian mining operation,\s*(.+?),\s*has been established on (.+?)$",
                    "🏭 EXPANSIÓN COMERCIAL: La empresa minera civil $1 ha establecido un complejo de extracción en $2.");

                AddRule(@"^A new shipping line has been established:\s*(.+?)$",
                    "🚀 NUEVA RUTA COMERCIAL: Se ha fundado la compañía de transporte de carga civil $1.");

                AddRule(@"^([^\.\,\n]+?) has launched a new (.+?) class (.+?)$",
                    "🛸 BOTADURA CIVIL: La corporación $1 ha lanzado una nueva unidad de clase $2 ($3).");

                // 5. Combat, Explosions & Logistics Alerts
                AddRule(@"^([^\.\,\n]+?) has suffered a catastrophic failure and exploded!$",
                    "💥 ALERTA DE CATÁSTROFE: ¡La nave $1 ha sufrido un fallo crítico e impulsión destructiva, explotando en el espacio!");

                AddRule(@"^([^\.\,\n]+?) was struck by enemy fire but continued functioning due to the efforts of damage control teams\.\s*(.+?)$",
                    "⚔️ COMBATE NAVAL: La nave $1 ha sido alcanzada por fuego enemigo. El equipo de control de daños contuvo la avería. $2");

                AddRule(@"^([^\.\,\n]+?) has only ([0-9\.]+) percent of its specified minimum fuel(?:\s*\((.+?)\))?$",
                    "⛽ ALERTA LOGÍSTICA: La nave $1 cuenta con solo el $2% de sus reservas mínimas de combustible ($3).");

                AddRule(@"^([^\.\,\n]+?) has run out of fuel$",
                    "⛽ EMERGENCIA LOGÍSTICA: La nave $1 ha agotado todo su combustible y ha quedado a la deriva en el espacio.");

                AddRule(@"^Fuel storage for harvester (.+?) is more than 90% full$",
                    "⛽ TANQUES LLENOS: Los depósitos de combustible de la unidad cosechadora $1 se encuentran al 90% de su capacidad.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing ImperialChronicleTranslator: {ex.Message}");
            }
        }

        private static void AddRule(string regexPattern, string replacement)
        {
            try
            {
                TranslationRules.Add((new Regex(regexPattern, RegexOptions.IgnoreCase), replacement));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to add translation rule '{regexPattern}': {ex.Message}");
            }
        }

        public static string TranslateToEpicSpanish(string rawMessage)
        {
            if (string.IsNullOrWhiteSpace(rawMessage)) return string.Empty;

            string cleanMsg = rawMessage.Trim();
            string translated = cleanMsg;

            try
            {
                foreach (var (pattern, replacement) in TranslationRules)
                {
                    if (pattern.IsMatch(cleanMsg))
                    {
                        translated = pattern.Replace(cleanMsg, replacement);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error translating message: {ex.Message}");
            }

            // Post-processing translations for residual English terms
            try
            {
                translated = Regex.Replace(translated, @"\bUnassigned\b", "Sin Asignar", RegexOptions.IgnoreCase);
                translated = Regex.Replace(translated, @"\bcompleted at\b", "completado en", RegexOptions.IgnoreCase);
                translated = Regex.Replace(translated, @"\bcompleted\b", "completado", RegexOptions.IgnoreCase);
                translated = Regex.Replace(translated, @"\btrained on\b", "adiestrado en", RegexOptions.IgnoreCase);
                translated = Regex.Replace(translated, @"\bdiscovered\b", "descubierto en", RegexOptions.IgnoreCase);
                translated = Regex.Replace(translated, @"\bunder the command of\b", "al mando de", RegexOptions.IgnoreCase);
                translated = Regex.Replace(translated, @"\bhas retired from the service\b", "se ha retirado del servicio imperial", RegexOptions.IgnoreCase);
                translated = Regex.Replace(translated, @"\bpromoted to\b", "promovido a", RegexOptions.IgnoreCase);
                translated = Regex.Replace(translated, @"\bhas increased to\b", "se ha incrementado a", RegexOptions.IgnoreCase);
            }
            catch
            {
                // Fallback
            }

            return translated;
        }
    }
}
