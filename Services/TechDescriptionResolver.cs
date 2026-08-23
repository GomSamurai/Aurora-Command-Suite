using System;

namespace AuroraDesignSuite.Services
{
    public static class TechDescriptionResolver
    {
        public static string? ResolveDescription(string techName, string categoryName = "")
        {
            if (string.IsNullOrWhiteSpace(techName)) return categoryName;

            string t = techName.ToLowerInvariant();
            string category = (categoryName ?? string.Empty).ToLowerInvariant();

            // 1. Jump Drive / Motor de Salto
            if (category.Contains("jump") || t.Contains("jump") || t.Contains("salto") || t.Contains("puerta estelar"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es un motor de salto gravitacional que permite abrir brechas espacio-temporales en Puntos de Salto (Jump Points).\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Capacidad de Salto: Permite a la nave (y a su escuadra de salto) viajar entre sistemas estelares sin requerir puertas fijas.\n• Límite de Masa: El tonelaje máximo de la nave no puede superar la capacidad máxima de masa del motor de salto.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nUna nave guía equipada con este motor de salto militar abrirá el paso para toda una flotilla de destructores en territorio enemigo.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nAsegúrate de que el motor de salto cubra el tonelaje total del buque más pesado del grupo de combate.";
            }

            // 2. Engines / Propulsion (Propulsores y Motores)
            if (category.Contains("engine") || category.Contains("drive") || category.Contains("motor") || category.Contains("propulsor") ||
                t.Contains("engine") || t.Contains("drive") || t.Contains("motor") || t.Contains("propulsor") || t.Contains("rds-") || t.Contains("ep-") || t.Contains("thruster"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es un impulsor de reacción espacial encargado de convertir combustible hidrocarburo Sorium LPH en empuje (EP).\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Empuje y Velocidad: Determina la velocidad máxima en km/s de la nave (Velocidad = Total Empuje / Total HS * 1000).\n• Eficiencia de Combustible: Los motores comerciales reducen el consumo; los militares maximizan la velocidad táctica.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nInstalar estos motores en un buque proporcionará la aceleración requerida para maniobrar y esquivar ataques enemigos.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nEquilibra los multiplicadores de potencia en el Diseñador para optimizar la velocidad sin agotar las reservas de Sorium.";
            }

            // 3. Fuel Tanks / Combustible
            if (category.Contains("fuel") || category.Contains("tank") || category.Contains("combustible") || category.Contains("tanque") ||
                t.Contains("fuel") || t.Contains("tank") || t.Contains("combustible") || t.Contains("tanque") || t.Contains("sorium"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es un depósito de almacenamiento de combustible hidrocarburo Sorium LPH para la autonomía naval.\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Almacenamiento de Litros: Acumula miles de litros de Sorium refinado para alimentar los propulsores espaciales.\n• Rango Operativo: La capacidad dividida por el consumo del motor determina el alcance máximo en kilómetros y años-luz.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nEquipar tanques de combustible de gran capacidad en un crucero le permitirá operar durante más de 3 años en el frente.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nProtege los tanques tras varias capas de blindaje para evitar detonaciones secundarias tras un impacto penetrante.";
            }

            // 4. Sensors & Radars
            if (category.Contains("sensor") || category.Contains("radar") || category.Contains("augur") ||
                t.Contains("sensor") || t.Contains("augur") || t.Contains("radar") || t.Contains("scanner") || t.Contains("search sensor"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es una matriz de escaneo pasivo o activo electromagnético/térmico de exploración espacial.\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Detección y Cobertura: Los sensores activos detectan naves a millones de km y guían los controles de tiro; los pasivos detectan firmas térmicas/EM en sigilo.\n• Resolución Táctica: La resolución determina la masa mínima del objetivo detectable (Res 1 para misiles, Res 20 para cazas, Res 100 para naves capitales).\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nActivar este sensor en tu nave de vanguardia revelará la aproximación de la flota enemiga a más de 50 millones de km de distancia.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nMantén los radares activos apagados durante la aproximación y usa sensores térmicos pasivos para ejecutar ataques sorpresa.";
            }

            // 5. Missile Launchers & Ordnance
            if (category.Contains("missile") || category.Contains("launcher") || category.Contains("ordnance") ||
                t.Contains("launcher") || t.Contains("missile") || t.Contains("lanzador") || t.Contains("misil") || t.Contains("box launcher") || t.Contains("torpedo"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es un tubo o silo de lanzamiento de misiles cinéticos o nucleares para combate táctico a larga distancia.\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Salvas de Misiles: Lanza proyectiles auto-guiados capaces de impactar objetivos a cientos de millones de km.\n• Tasa de Recarga: Los silos estándar recargan desde el pañol de munición (Magazine); los Box Launchers son de un solo uso por batalla pero muy ligeros.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nLanzar una salva concentrada de 20 misiles saturará las defensas punto cercano (PDC) enemigas antes de que puedan responder.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nAsegúrate de equipar pañoles de munición (Magazines) con suficiente capacidad para reabastecer los lanzadores en combates prolongados.";
            }

            // 6. Beam & Kinetic Energy Weapons
            if (category.Contains("weapon") || category.Contains("beam") || category.Contains("cannon") || category.Contains("turret") ||
                t.Contains("laser") || t.Contains("carronade") || t.Contains("railgun") || t.Contains("meson") || t.Contains("microwave") || t.Contains("gauss") || t.Contains("beam") || t.Contains("cañón"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es un sistema de armamento naval de energía directa o proyectiles cinéticos de alta cadencia.\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Perforación de Blindaje: Inflige daño calórico o cinético directo sobre el casco. Cañones Gauss interceptan misiles; lásers destruyen naves.\n• Tasa de Recarga: Requiere potencia continua producida por reactores energéticos en cada turno de combate.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nUna salva de 4 de estos cañones atravesará el blindaje de un crucero enemigo y destruirá sus sistemas internos.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nMonta armas energéticas en torretas orientables para maximizar la velocidad de seguimiento contra misiles hiper-veloces.";
            }

            // 7. Shields / Escudos
            if (category.Contains("shield") || t.Contains("shield") || t.Contains("escudo"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es un generador de campo de fuerza defensivo que envuelve el casco de la nave.\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Absorción Energética: Absorbe el impacto de láseres, misiles y proyectiles cinéticos antes de que dañen la armadura física.\n• Recarga Continua: Los escudos regeneran sus puntos defensivos turno a turno mientras el generador reciba energía.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nTener escudos activos permite a una nave soportar el primer golpe de una salva enemiga sin sufrir desperfectos en el casco.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nLos escudos requieren tiempo para cargarse tras salir del punto de salto; mantén la distancia durante la fase de recarga inicial.";
            }

            // 8. Armor / Armadura
            if (category.Contains("armor") || category.Contains("armour") || t.Contains("armour") || t.Contains("armor") || t.Contains("armadura") || t.Contains("composite"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es la estructura de blindaje físico defensivo de capas metálicas y cerámicas del casco.\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Protección de Casco: Protege la maquinaria, reactores, pañoles y tripulación frente a la penetración de impactos enemigos.\n• Matriz de Armadura: Dispone capas de grosor que deben ser perforadas por el fuego enemigo antes de dañar componentes internos.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nAñadir 3 capas de blindaje evitará que un rayo láser de corta distancia alcance el reactor de fusión principal.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nAumenta la armadura en buques de línea de frente y acorazados diseñados para resistir combates de desgaste.";
            }

            // 9. Maintenance & Engineering
            if (category.Contains("maint") || category.Contains("engineering") || t.Contains("maintenance") || t.Contains("maint") || t.Contains("engineering") || t.Contains("repuesto") || t.Contains("msp"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es un módulo de repuestos de mantenimiento (MSP) y espacios de ingeniería de la nave.\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Repuestos MSP: Acumula materiales para reparar averías en combate y prevenir colapsos por fatiga mecánica.\n• Control de Daños: Permite a las cuadrillas arreglar motores, cañones o sensores destruidos por fuego enemigo.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nSi un rayo láser destruye el motor principal, el equipo de ingeniería usará los repuestos de este almacén para restaurar la operatividad en combate.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nInstala suficientes espacios para que la Vida de Mantenimiento supere el tiempo de despliegue militar proyectado.";
            }

            // 10. Habitation & Crew Quarters
            if (category.Contains("habitation") || category.Contains("crew") || t.Contains("habitation") || t.Contains("crew") || t.Contains("quarters") || t.Contains("dormitorio") || t.Contains("alojamiento"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es el módulo de habitabilidad y alojamiento militar para oficiales y marineros a bordo.\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Capacidad de Tripulación: Proporciona dormitorios y soporte vital para la tripulación requerida por sistemas y armas.\n• Moral y Eficiencia: Mantener los alojamientos al 100% de la tripulación requerida evita la degradación de la moral.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nUn acorazado con 200 tripulantes requerirá módulos de habitabilidad para garantizar la salud y operatividad en travesías largas.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nUtiliza el balanceador automático de habitabilidad en el Diseñador para asegurar el soporte vital de toda la dotación.";
            }

            // 11. Hangar & Carrier Facilities
            if (category.Contains("hangar") || category.Contains("carrier") || t.Contains("hangar") || t.Contains("parasite") || t.Contains("pod bay") || t.Contains("nodriza"))
            {
                return $"📌 CONCEPTO & DEFINICIÓN:\n{techName} es una cubierta de hangar y bahía de lanzamiento para cazas, lanzaderas y naves parásitas.\n\n⚙️ FUNCIÓN Y MECÁNICA EN JUEGO:\n• Transporte y Mantenimiento: Alberga cazas y embarcaciones pequeñas, suministrando combustible y munición mientras están atracadas.\n• Lanzamiento y Recuperación: Permite desplegar escuadrones de cazas rápidamente en el campo de batalla.\n\n💡 EJEMPLO PRÁCTICO EN PARTIDA:\nUn portaaviones de flota equipado con 5,000 toneladas de hangar podrá transportar y desplegar 20 cazas de interceptación.\n\n🛡️ CONSEJO TÁCTICO IMPERIAL:\nAsigna buques nodriza con hangares para proteger cazas de corto alcance en saltos estelares.";
            }

            return null;
        }
    }
}
