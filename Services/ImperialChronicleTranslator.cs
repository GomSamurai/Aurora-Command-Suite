using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AuroraDesignSuite.Services
{
    public static class ImperialChronicleTranslator
    {
        private static readonly List<(Regex Pattern, string Replacement)> TranslationRules = new()
        {
            // 1. Research & Science Events
            (
                new Regex(@"^A science team led by ([^]+?) working on ([^]+?) has completed research into ([^]+?)$", RegexOptions.IgnoreCase),
                "🔬 HITO CIENTÍFICO: El equipo de I+D liderado por $1 en $2 ha culminado exitosamente la investigación de $3."
            ),
            (
                new Regex(@"^Research into ([^]+?) completed$", RegexOptions.IgnoreCase),
                "🔬 HITO CIENTÍFICO: La investigación de $1 ha sido completada con éxito por el Departamento de Ciencia Imperial."
            ),
            (
                new Regex(@"^The Research Admin bonus of ([^]+?) has increased to ([^]+?)$", RegexOptions.IgnoreCase),
                "🧠 DESARROLLO CIENTÍFICO: La capacidad de administración de I+D del oficial $1 se ha incrementado al $2."
            ),

            // 2. Officers, Promotions, Retirements & Deaths
            (
                new Regex(@"^([^\.\,\n]+?) has retired from the service at the age of (\d+)\.\s*Current Assignment:\s*([^]+?)$", RegexOptions.IgnoreCase),
                "🎖️ RETIRO CON HONORES: El oficial $1 ha pasado a retiro del servicio imperial a la edad de $2 años. Asignación previa: $3."
            ),
            (
                new Regex(@"^([^\.\,\n]+?) promoted to ([^]+?)$", RegexOptions.IgnoreCase),
                "🎖️ DECRETO DE ASCENSO: El oficial $1 ha sido promovido al rango de $2 por sus distinguidos servicios a la Corona Imperial."
            ),
            (
                new Regex(@"^([^\.\,\n]+?) has been killed in an accident\.\s*Assignment prior to death:\s*([^]+?)$", RegexOptions.IgnoreCase),
                "✝️ IN MEMORIAM: El oficial $1 ha fallecido trágicamente en acto de servicio debido a un accidente. Puesto previo: $2."
            ),
            (
                new Regex(@"^The ([^]+?) bonus of ([^]+?) has increased to ([^]+?)$", RegexOptions.IgnoreCase),
                "🎖️ PERFECCIONAMIENTO TÁCTICO: La bonificación de $1 del oficial $2 se ha incrementado al $3."
            ),

            // 3. Stellar Exploration & Mineral Discoveries
            (
                new Regex(@"^([^\.\,\n]+?) under the command of ([^]+?) has discovered the new system of ([^]+?)$", RegexOptions.IgnoreCase),
                "🧭 EXPLORACIÓN ESTELAR: La nave $1 al mando del oficial $2 ha descubierto el nuevo sistema estelar $3."
            ),
            (
                new Regex(@"^([^\.\,\n]+?) under the command of ([^]+?) discovered minerals on ([^]+?):\s*([^]+?)$", RegexOptions.IgnoreCase),
                "💎 PROSPECCIÓN GEOLÓGICA: La nave $1 comandada por $2 ha hallado yacimientos de minerales exóticos en $3: $4."
            ),
            (
                new Regex(@"^([^\.\,\n]+?) under the command of ([^]+?) discovered a new jump point in ([^]+?)$", RegexOptions.IgnoreCase),
                "🌌 PUNTO DE SALTO: La nave $1 al mando de $2 ha localizado un nuevo punto de salto astrofísico en $3."
            ),
            (
                new Regex(@"^Gravitational survey completed in ([^]+?)$", RegexOptions.IgnoreCase),
                "🪐 PROSPECCIÓN GRAVITACIONAL: Estudio de resonancia gravitatoria completado con éxito en el sistema $1."
            ),
            (
                new Regex(@"^Ground survey completed on ([^]+?)$", RegexOptions.IgnoreCase),
                "🌍 PROSPECCIÓN TERRESTRE: Levantamiento geológico de superficie completado en el cuerpo celeste $1."
            ),
            (
                new Regex(@"^([^\.\,\n]+?) conducted an orbital survey of ([^]+?) that revealed the potential for a ground survey is ([^]+?)$", RegexOptions.IgnoreCase),
                "🛰️ ESCÁNER ORBITAL: La exploración orbital realizada por $1 en $2 reveló un potencial de prospección terrestre $3."
            ),

            // 4. Industry, Ship Construction & Ground Troops
            (
                new Regex(@"^Production of ([^]+?) completed at ([^]+?)$", RegexOptions.IgnoreCase),
                "🏭 PRODUCCIÓN INDUSTRIAL: Concluida la fabricación e instalación de $1 en la colonia de $2."
            ),
            (
                new Regex(@"^Production of ([^]+?) has begun at ([^]+?)$", RegexOptions.IgnoreCase),
                "🏭 INICIO DE FABRICACIÓN: Comenzados los trabajos industriales de $1 en la colonia de $2."
            ),
            (
                new Regex(@"^([^\.\,\n]+?) trained on ([^]+?)$", RegexOptions.IgnoreCase),
                "🪖 ADIESTRAMIENTO DE TROPAS: La unidad terrestre $1 ha completado su entrenamiento combate en $2."
            ),
            (
                new Regex(@"^([^\.\,\n]+?) built at ([^]+?) and assigned to ([^]+?)$", RegexOptions.IgnoreCase),
                "⚓ BOTADURA NAVAL: La nave $1 ha finalizado su construcción en los astilleros de $2 y ha sido asignada a $3."
            ),
            (
                new Regex(@"^A civilian mining operation,\s*([^]+?),\s*has been established on ([^]+?)$", RegexOptions.IgnoreCase),
                "🏭 EXPANSIÓN COMERCIAL: La empresa minera civil $1 ha establecido un complejo de extracción en $2."
            ),
            (
                new Regex(@"^A new shipping line has been established:\s*([^]+?)$", RegexOptions.IgnoreCase),
                "🚀 NUEVA RUTA COMERCIAL: Se ha fundado la compañía de transporte de carga civil $1."
            ),
            (
                new Regex(@"^([^\.\,\n]+?) has launched a new ([^]+?) class ([^]+?)$", RegexOptions.IgnoreCase),
                "🛸 BOTADURA CIVIL: La corporación $1 ha lanzado una nueva unidad de clase $2 ($3)."
            ),

            // 5. Combat, Explosions & Logistics Alerts
            (
                new Regex(@"^([^\.\,\n]+?) has suffered a catastrophic failure and exploded!$", RegexOptions.IgnoreCase),
                "💥 ALERTA DE CATÁSTROFE: ¡La nave $1 ha sufrido un fallo crítico e impulsión destructiva, explotando en el espacio!"
            ),
            (
                new Regex(@"^([^\.\,\n]+?) was struck by enemy fire but continued functioning due to the efforts of damage control teams\.\s*([^]+?)$", RegexOptions.IgnoreCase),
                "⚔️ COMBATE NAVAL: La nave $1 ha sido alcanzada por fuego enemigo. El equipo de control de daños contuvo la avería. $2"
            ),
            (
                new Regex(@"^([^\.\,\n]+?) has only ([0-9\.]+) percent of its specified minimum fuel(?:\s*\(([^]+?)\))?$", RegexOptions.IgnoreCase),
                "⛽ ALERTA LOGÍSTICA: La nave $1 cuenta con solo el $2% de sus reservas mínimas de combustible ($3)."
            ),
            (
                new Regex(@"^([^\.\,\n]+?) has run out of fuel$", RegexOptions.IgnoreCase),
                "⛽ EMERGENCIA LOGÍSTICA: La nave $1 ha agotado todo su combustible y ha quedado a la deriva en el espacio."
            ),
            (
                new Regex(@"^Fuel storage for harvester ([^]+?) is more than 90% full$", RegexOptions.IgnoreCase),
                "⛽ TANQUES LLENOS: Los depósitos de combustible de la unidad cosechadora $1 se encuentran al 90% de su capacidad."
            )
        };

        public static string TranslateToEpicSpanish(string rawMessage)
        {
            if (string.IsNullOrWhiteSpace(rawMessage)) return string.Empty;

            string cleanMsg = rawMessage.Trim();

            foreach (var (pattern, replacement) in TranslationRules)
            {
                if (pattern.IsMatch(cleanMsg))
                {
                    return pattern.Replace(cleanMsg, replacement);
                }
            }

            // Fallback word replacement for non-templated messages
            string text = cleanMsg;
            text = Regex.Replace(text, @"\bcompleted at\b", "completado en", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\bcompleted\b", "completado", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\btrained on\b", "adiestrado en", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\bdiscovered\b", "descubierto en", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\bunder the command of\b", "al mando de", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\bhas retired from the service\b", "se ha retirado del servicio imperial", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\bpromoted to\b", "promovido a", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\bhas increased to\b", "se ha incrementado a", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\bUnassigned\b", "Sin Asignar", RegexOptions.IgnoreCase);

            return text;
        }
    }
}
