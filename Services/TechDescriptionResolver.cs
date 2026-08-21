using System;

namespace AuroraDesignSuite.Services
{
    public static class TechDescriptionResolver
    {
        public static string ResolveDescription(string techName, string categoryName)
        {
            if (string.IsNullOrWhiteSpace(techName)) return categoryName;

            string t = techName.ToLower();

            if (t.Contains("carronade"))
            {
                return "Arma energética de fuego directo de corto alcance. Inflige un daño destructivo devastador a corta distancia, ideal para cazas interceptores y corbetas de embestida sin requerir polvorín de munición.";
            }
            if (t.Contains("railgun"))
            {
                return "Cañón cinético magnético de disparo múltiple (4 impactos por turno). Excelente para Defensa Punto Cercano (PDC) contra salvas de misiles enemigas sin consumir reservas de combustible.";
            }
            if (t.Contains("laser") || t.Contains("focal"))
            {
                return "Arma de energía focalizada de alta precisión. Perfora el blindaje de buques enemigos a larga distancia manteniendo gran concentración de daño de penetración profunda.";
            }
            if (t.Contains("meson"))
            {
                return "Emisor de partículas mesónicas que atraviesa escudos energéticos y blindaje metálico sin atenuación, destruyendo directamente componentes e instalaciones internas del objetivo.";
            }
            if (t.Contains("microwave"))
            {
                return "Arma de pulso electromagnético de alta frecuencia. Inutiliza la electrónica, sensores de control de tiro y sistemas de guiado del objetivo sin destruir la estructura física del casco.";
            }
            if (t.Contains("terraforming"))
            {
                return "Módulo de modificación atmosférica planetaria. Permite procesar gases (Oxígeno, CO2, Nitrógeno) para reducir el Coste Colonial (Colony Cost) a 0.00 en mundos inhóspitos.";
            }
            if (t.Contains("mining") || t.Contains("mina"))
            {
                return "Instalación de extracción exótica automatizada. Extrae los 11 minerales trans-newtonianos sin requerir población civil ni infraestructura de habitabilidad en asteroides.";
            }
            if (t.Contains("refuelling") || t.Contains("fuel") || t.Contains("refinería"))
            {
                return "Sistema de refino y transferencia logística de combustible Sorium (LPH). Indispensable para tanqueros de flota, nodrizas y operaciones de prospección profunda.";
            }
            if (t.Contains("ecm") || t.Contains("eccm"))
            {
                return "Dispositivos de guerra electrónica avanzada. ECM reduce la precisión de misiles y control de tiro enemigo; ECCM neutraliza los inhibidores de frecuencia del objetivo.";
            }
            if (t.Contains("armour") || t.Contains("composite") || t.Contains("armadura"))
            {
                return "Blindaje defensivo de casco. Añade capas estructurales de absorción de impactos que protegen la maquinaria, laboratorios y motores frente a penetración perforante.";
            }
            if (t.Contains("sensor") || t.Contains("control"))
            {
                return "Sistemas de detección térmica (TH), electromagnética (EM) y radar activo (Res 1 para misiles, Res 20 para cazas, Res 100 para naves capitales). Clave para adquirir objetivos.";
            }
            if (t.Contains("troop") || t.Contains("drop") || t.Contains("transport"))
            {
                return "Módulos y bahías de transporte de tropas terrestres. Permite el desembarco táctico de batallones y brigadas mecanizadas en asaltos planetarios u abordajes.";
            }
            if (t.Contains("shield"))
            {
                return "Barrera de energía deflectora. Absorbe impactos de energía y proyectiles cinéticos antes de dañar la armadura metálica exterior. Se recarga automáticamente con energía del reactor.";
            }
            if (t.Contains("fleet") || t.Contains("flota") || t.Contains("task force") || t.Contains("escuadra"))
            {
                return "⚓ Flota y Escuadra Naval. Formación militar o comercial de buques espaciales coordinados bajo la cadena de mando para misiones de combate, logística o exploración.";
            }
            if (t.Contains("ship") || t.Contains("nave") || t.Contains("buque") || t.Contains("caza"))
            {
                return "🚀 Buque Espacial Trans-Newtoniano. Unidad móvil dotada de casco, motores, sistemas de mantenimiento y armamento para operar en espacio profundo.";
            }
            if (t.Contains("system") || t.Contains("sistema"))
            {
                return "🌌 Sistema Estelar. Sector compuesto por cuerpos celestes, asteroides, yacimientos minerales y nodos de salto interestelares.";
            }

            return categoryName switch
            {
                "⚡ Potencia y Propulsión" => "Tecnología de ingeniería de propulsión espacial y reactores. Aumenta la velocidad máxima en km/s, la eficiencia de consumo LPH y el alcance operativo.",
                "💥 Energía y Láseres" => "Armamento energético de fuego directo. Incrementa la penetración de blindaje, la focalización de lentes y la tasa de recarga de condensadores.",
                "🚀 Misiles y Cinéticas" => "Sistemas de proyectiles balísticos y cinéticos. Mejora el rendimiento de motores de misiles, cabezas de guerra y cadencia de fuego cinético.",
                "📡 Sensores y Control" => "Sistemas de vigilancia de sector y telemetría de tiro. Aumenta el alcance de fijación de blancos y la resolución de firmas térmicas.",
                "🧬 Biología y Ciencias" => "Biotecnología y ciencias aplicadas. Permite mejorar el soporte vital, la tolerancia ambiental de colonos y la eficiencia médica.",
                "🏗️ Construcción y Logística" => "Infraestructura industrial y logística espacial. Incrementa el rendimiento de fábricas, astilleros y capacidades extractivas.",
                _ => "Tecnología avanzada para el desarrollo y expansión de las capacidades industriales, militares y científicas del Imperio."
            };
        }
    }
}
