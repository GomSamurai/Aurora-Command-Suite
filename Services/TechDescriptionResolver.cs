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
            if (t.Contains("maintenance") || t.Contains("maint") || t.Contains("engineering") || t.Contains("repuestos"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es un módulo de repuestos de mantenimiento (MSP) y espacios de ingeniería de la nave.\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Repuestos MSP: Acumula materiales para reparar averías en combate y prevenir colapsos por fatiga mecánica.\n• Control de Daños: Permite a las cuadrillas arreglar motores, cañones o sensores destruidos por fuego enemigo.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nSi un rayo láser destruye el motor principal, el equipo de ingeniería usará los repuestos de este almacén para restaurar la operatividad en combate.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nInstala suficientes espacios para que la Vida de Mantenimiento supere el tiempo de despliegue militar proyectado.";
            }
            if (t.Contains("engine") || t.Contains("drive") || t.Contains("motor") || t.Contains("propulsor"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es un impulsor de reacción espacial encargado de convertir combustible hidrocarburo Sorium LPH en empuje (EP).\n\n⚙️ FUNCIÓN Y MECÂNICA EN JUEGO:\n• Empuje y Velocidad: Determina la velocidad máxima en km/s de la nave (Velocidad = Total Empuje / Total HS * 1000).\n• Eficiencia de Combustible: Los motores comerciales reducen el consumo; los militares maximizan la velocidad táctica.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nInstalar 4 de estos motores en un destructor proporcionará la aceleración requerida para esquivar salvas de torpedos.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nEquilibra los multiplicadores de potencia en el Diseñador para optimizar la velocidad sin agotar las reservas de Sorium.";
            }
            if (t.Contains("fuel") || t.Contains("tank") || t.Contains("combustible") || t.Contains("tanque"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es un depósito de almacenamiento de combustible hidrocarburo Sorium LPH para la autonomía naval.\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Almacenamiento de Litros: Acumula miles de litros de Sorium refinado para alimentar los propulsores espaciales.\n• Rango Operativo: La capacidad dividida por el consumo del motor determina el alcance máximo en kilómetros y años-luz.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nEquipar tanques de combustible de gran capacidad en un crucero le permitirá operar durante más de 3 años en el frente.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nProtege los tanques tras varias capas de blindaje para evitar detonaciones secundarias tras un impacto penetrante.";
            }
            if (t.Contains("habitation") || t.Contains("crew") || t.Contains("quarters") || t.Contains("dormitorio"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es el módulo de habitabilidad y alojamiento militar para oficiales y marineros a bordo.\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Capacidad de Tripulación: Proporciona dormitorios y soporte vital para la tripulación requerida por sistemas y armas.\n• Moral y Eficiencia: Mantener los alojamientos al 100% de la tripulación requerida evita la degradación de la moral.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nUn acorazado con 200 tripulantes requerirá 4 módulos para garantizar la salud y operatividad en travesías largas.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nUtiliza el balanceador automático de habitabilidad en el Diseñador para asegurar el soporte vital de toda la dotación.";
            }
            if (t.Contains("sensor") || t.Contains("augur") || t.Contains("radar") || t.Contains("active") || t.Contains("passive"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es una matriz de escaneo pasivo o activo electromagnético/térmico de exploración espacial.\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Detección y Cobertura: Los sensores activos detectan naves a millones de km; los pasivos detectan firmas térmicas/EM en sigilo.\n• Resolución Táctica: La resolución determina la masa mínima del objetivo detectable (Res 1 para misiles, Res 20 para cazas).\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nActivar este sensor en tu nave de vanguardia revelará la aproximación de la flota enemiga a más de 50 millones de km.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nMantén los radares activos apagados durante la aproximación para evitar delatar tu posición a los sensores pasivos enemigos.";
            }
            if (t.Contains("laser") || t.Contains("beam") || t.Contains("gauss") || t.Contains("railgun") || t.Contains("meson"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es un sistema de armamento naval de energía directa o proyectiles cinéticos de alta cadencia.\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Perforación de Blindaje: Inflige daño calórico o cinético directo sobre el casco. Cañones Gauss interceptan misiles; lásers destruyen naves.\n• Tasa de Recarga: Requiere potencia continua producida por reactores energéticos en cada turno de combate.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nUna salva de 4 de estos cañones atravesará el blindaje de un crucero enemigo y destruirá sus sistemas internos.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nMonta armas energéticas en torretas orientables para maximizar la velocidad de seguimiento contra misiles hiper-veloces.";
            }

            return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es una especificación y componente técnico fundamental de la arquitectura naval e industrial de Aurora 4X.\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Rendimiento Operativo: Potencia las capacidades tácticas de tus buques, la eficiencia de tus colonias o la velocidad de prospección galáctica.\n• Especificación Técnica: Diseñado para integrarse en el Diseñador de Naves (Class Design) o en la gestión de infraestructura colonial del Imperio.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nIncorporar {techName} en tu doctrina de flota o en la gestión de mundos exteriores optimizará el uso de recursos y aumentará la supervivencia en combate.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nMantén equilibrados tus suministros minerales y energéticos en la telemetría para sacar el máximo partido a {techName}.";
        }
    }
}
