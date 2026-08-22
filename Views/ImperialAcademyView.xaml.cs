using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace AuroraDesignSuite.Views
{
    public partial class ImperialAcademyView : UserControl
    {
        public class CodexItem
        {
            public string Key { get; set; } = string.Empty;
            public string DisplayTitle { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
        }

        private List<CodexItem> _allItems = new List<CodexItem>();

        public ImperialAcademyView()
        {
            InitializeComponent();
            try
            {
                LoadCodexDatabase();
                FilterList("", "🌟 Todas las Categorías");
            }
            catch (Exception ex)
            {
                if (LblArticleTitle != null) LblArticleTitle.Text = "Academia Imperial Aurora 4X";
                if (LblArticleCategory != null) LblArticleCategory.Text = "🎓 Enciclopedia Imperial";
                if (TxtArticleBody != null) TxtArticleBody.Text = "Error al cargar la base de datos de lecciones: " + ex.Message;
            }
        }

        private void LoadCodexDatabase()
        {
            _allItems = new List<CodexItem>();

            // --------------------------------------------------------------------
            // 🎓 MASTER TUTORIAL CURRICULUM: 15 DETAILED COMPREHENSIVE LESSONS
            // --------------------------------------------------------------------

            _allItems.Add(new CodexItem
            {
                Key = "Lección 1: Primeros Pasos en Sol & Conversión TN",
                DisplayTitle = "🎓 Lección 1: Primeros Pasos en Sol & Conversión TN",
                Category = "🎓 Tutoriales & Lecciones",
                Body = "========================================================================================\n" +
                       "🎓 LECCIÓN 1: INICIO EN EL SISTEMA SOLAR, INTERFAZ Y CONVERSIÓN TRANS-NEWTONIANA\n" +
                       "========================================================================================\n\n" +
                       "Al iniciar una partida en el planeta Tierra (Sistema Sol), tu imperio comienza en la era convencional o en la aurora de la tecnología Trans-Newtoniana (TN). La Tierra cuenta con una población masiva, industrias convencionales y laboratorios de investigación iniciales.\n\n" +
                       "1. COMPRENSIÓN DEL TIEMPO E INCREMENTOS TÁCTICOS:\n" +
                       "   • El tiempo en Aurora 4X avanza mediante incrementos manuales (5 Segundos a 1 Año).\n" +
                       "   • Para tiempo de paz y desarrollo industrial, utiliza incrementos de 5 Días o 30 Días.\n" +
                       "   • En situaciones de combate espacial o aproximación de misiles, reduce el incremento a 5 Segundos o Sub-Pulso para no perder el control táctico.\n\n" +
                       "2. PLAN DE CONVERSIÓN INDUSTRIAL (VENTANA DE ECONOMÍA):\n" +
                       "   • Las Fábricas Convencionales producen a un rendimiento muy bajo. Tu prioridad absoluta es convertirlas a Fábricas de Construcción (Construction Factories).\n" +
                       "   • Ve a la pestaña 'Industria' en FormEconomics, selecciona 'Convert Conventional Factory' y asigna el 100% de la capacidad de construcción inicial.\n" +
                       "   • Costo de conversión: 120 Puntos de Construcción (BP) por fábrica. Una vez convertidas, la eficiencia industrial de tu planeta se multiplicará por 10.\n\n" +
                       "3. ASIGNACIÓN INICIAL DE INVESTIGACIÓN (I+D):\n" +
                       "   • Dirígete a la pestaña 'Investigación'. Dispones de Laboratorios de Investigación (Research Facilities) y científicos especializados.\n" +
                       "   • Asigna laboratorios a los siguientes proyectos clave inmediatos:\n" +
                       "     a) Trans-Newtonian Technology (Desbloquea minerales TN y construcciones avanzadas).\n" +
                       "     b) Conventional Engine / Nuclear Thermal Engine (Desbloquea motores espaciales).\n" +
                       "     c) Geological Survey Sensors (Permite explorar minerales en otros planetas y asteroides).\n" +
                       "     d) Mass Driver Efficiency (Mejora la logística de transferencia mineral).\n\n" +
                       "💡 CONSEJO TÁCTICO IMPERIAL:\n" +
                       "No construyas naves militares de inmediato. Dedica los primeros 2 a 3 años del juego exclusivamente a estabilizar la economía, acumular minerales y convertir tus industrias convencionales."
            });

            _allItems.Add(new CodexItem
            {
                Key = "Lección 2: Minería Trans-Newtoniana & Catapultas de Masa",
                DisplayTitle = "⛏️ Lección 2: Minería Trans-Newtoniana & Catapultas de Masa",
                Category = "🎓 Tutoriales & Lecciones",
                Body = "========================================================================================\n" +
                       "⛏️ LECCIÓN 2: LOS 11 MINERALES TRANS-NEWTONIANOS Y LOGÍSTICA DE CATAPULTAS DE MASA\n" +
                       "========================================================================================\n\n" +
                       "Los 11 minerales exóticos trans-newtonianos son el pilar de toda la civilización espacial en Aurora 4X. Sin un flujo constante de minerales, las fábricas se detendrán y los astilleros no podrán construir naves.\n\n" +
                       "DESGLOSE DETALLADO DE LOS 11 MINERALES Y SUS USOS ESTRATÉGICOS:\n" +
                       "1. Duranium: El mineral estructural primario. Se consume en CADA edificio, nave, misil y fortificación.\n" +
                       "2. Sorium: Refinado exclusivo para producir combustible de hidrocarburo LPH para motores espaciales.\n" +
                       "3. Neutronium: Material ultradenso para blindaje pesado de naves, fortificaciones y placas protectoras.\n" +
                       "4. Gallicite: Utilizado en motores navales de alta velocidad, propulsores de misiles y componentes mecánicos.\n" +
                       "5. Uridium: Utilizado en sensores pasivos térmicos/EM, radares activos, ópticas y sistemas de puntería.\n" +
                       "6. Corundium: Componente crítico de armas láser, cañones de partículas y lentes energéticas.\n" +
                       "7. Boronide: Utilizado en escudos de fuerza energéticos y sistemas de habitabilidad colonial.\n" +
                       "8. Mercassium: Se consume en laboratorios de I+D, reactores de potencia y sistemas de investigación.\n" +
                       "9. Vendarite: Elemento esencial para la construcción de refinerías, astilleros e industria pesada.\n" +
                       "10. Tritium: Componente primordial de cabezas de guerra de misiles y cargas explosivas.\n" +
                       "11. Tritanium: Aleación para estructuras avanzadas de misiles, tubos lanzadores y torretas.\n\n" +
                       "PROPIEDADES DE UN YACIMIENTO MINERAL:\n" +
                       "• Cantidad (Toneladas): Reserva total de mineral presente en el cuerpo celeste.\n" +
                       "• Accesibilidad (0.1 a 1.0): Mide la facilidad de extracción. Una accesibilidad de 1.0 produce el 100% por mina/año. Una accesibilidad de 0.2 produce solo el 20%.\n\n" +
                       "LOGÍSTICA DE CATAPULTAS DE MASA (MASS DRIVERS):\n" +
                       "• Las Catapultas de Masa permiten enviar minerales extraídos en asteroides o lunas directamente hacia la Tierra sin gastar naves de carga.\n" +
                       "• CÓMO CONFIGURARLAS:\n" +
                       "  1. Construye una Catapulta de Masa en la Tierra y despliega otra en el asteroide minero (ej. Luna o Ceres).\n" +
                       "  2. En la ventana de Economía del asteroide, selecciona 'Mass Driver Destination' y marca 'Tierra'.\n" +
                       "  3. Asegúrate de que el planeta receptor (Tierra) tenga AL MENOS 1 Catapulta de Masa para capturar los paquetes de mineral.\n" +
                       "⚠️ ADVERTENCIA DE SEGURIDAD:\n" +
                       "Si lanzas minerales con una Catapulta de Masa hacia un planeta que NO tiene una Catapulta receptora, los paquetes bombardearán la superficie provocando destrucción industrial y muertes civiles."
            });

            _allItems.Add(new CodexItem
            {
                Key = "Lección 3: Ingeniería y Diseño de Naves Espaciales",
                DisplayTitle = "🛠️ Lección 3: Ingeniería y Diseño de Naves Espaciales",
                Category = "🎓 Tutoriales & Lecciones",
                Body = "========================================================================================\n" +
                       "🛠️ LECCIÓN 3: REGLAS FUNDAMENTALES DEL DISEÑADOR DE NAVES (FormClassDesign)\n" +
                       "========================================================================================\n\n" +
                       "El diseñador de naves (Class Design) es el corazón táctico de Aurora 4X. Aquí configuras cada sistema, motor, blindaje y sensor de tus buques espaciales.\n\n" +
                       "CONCEPTOS Y UNIDADES DE MEDIDA CLAVE:\n" +
                       "• Tamaño de Casco (HS - Hull Size): 1 HS equivale exactamente a 50 toneladas métricas de desplazamiento.\n" +
                       "• Velocidad Naval: Calculada como (Potencia Total de Motores / Desplazamiento Total en HS) * 1,000 km/s.\n" +
                       "• Firma Térmica (TCS - Thermal Cross Section): Visibilidad de la nave ante radares enemigos. A mayor tamaño y potencia de motor, mayor es la firma TCS.\n\n" +
                       "DISTINCIÓN FUNDAMENTAL: MOTORES COMERCIALES VS MILITARES:\n" +
                       "1. MOTORES COMERCIALES:\n" +
                       "   • Requisitos: Tamaño de motor >= 25 HS y multiplicador de potencia <= 50%.\n" +
                       "   • Ventajas: NUNCA sufren averías mecánicas por mantenimiento. No requieren pañoles MSP.\n" +
                       "   • Uso: Cargueros, Colonizadores, Naves Tanque, Mineros Orbitales, Estaciones.\n\n" +
                       "2. MOTORES MILITARES:\n" +
                       "   • Permiten multiplicadores de potencia del 100% al 300% para lograr velocidades extremas.\n" +
                       "   • Sufren desgaste mecánico y fallos en travesías largas. Requieren Pañoles de Mantenimiento (MSP) y Equipos de Control de Daños (DCR).\n\n" +
                       "PASOS PARA CREAR UNA CLASE DE NAVE:\n" +
                       "1. Abre FormClassDesign y haz clic en 'New Class'. Asigna un nombre y rol (ej. 'Clase Escolta Sol - Frigata').\n" +
                       "2. Añade Motores en la pestaña de componentes.\n" +
                       "3. Añade Sensores de puente, Puente de Mando (Bridge) y Pañoles de Mantenimiento.\n" +
                       "4. Asigna el grosor del blindaje (Armor Layers). El blindaje se dispone en filas y columnas protegiendo los sistemas internos.\n" +
                       "5. Revisa que el indicador de 'Commercial' o 'Military' coincida con el propósito proyectado de la nave."
            });

            _allItems.Add(new CodexItem
            {
                Key = "Lección 4: Autonomía, Logística de Combustible y Cadena de Suministro",
                DisplayTitle = "⛽ Lección 4: Autonomía, Logística de Combustible y Cadena de Suministro",
                Category = "🎓 Tutoriales & Lecciones",
                Body = "========================================================================================\n" +
                       "⛽ LECCIÓN 4: RESERVAS DE COMBUSTIBLE, AUTONOMÍA Y RED DE SUMINISTRO\n" +
                       "========================================================================================\n\n" +
                       "Sin combustible, tus flotas quedan a la deriva e indefensas en el vacío del espacio. La gestión del combustible determina el radio operativo de tus escuadrones.\n\n" +
                       "1. REFINADO DE SORIUM:\n" +
                       "   • El mineral Sorium extraído en las colonias debe ser procesado en Refinerías de Fuel (Fuel Refineries).\n" +
                       "   • Rendimiento Base: Cada refinería produce 200,000 litros de combustible hidrocarburo LPH por año.\n" +
                       "   • Mantén refinerías activas en tu colonia principal y en yacimientos de Sorium ricos.\n\n" +
                       "2. CÁLCULO DE AUTONOMÍA Y CONSUMO:\n" +
                       "   • La autonomía en kilómetros se calcula dividiendo la capacidad total de tanques por el consumo de litros por hora a máxima velocidad.\n" +
                       "   • Diseña naves militares con una autonomía de al menos 20,000 a 50,000 millones de kilómetros para operar entre sistemas vecinos.\n\n" +
                       "3. LOGÍSTICA DE NAVES TANQUE (TANKERS):\n" +
                       "   • Añade el componente 'Refuelling System' o 'Refuelling Hub' a buques comerciales grandes para convertirlos en Naves Tanque (Tankers).\n" +
                       "   • Las Naves Tanque pueden acompañar a la flota de combate o permanecer desplegadas en Puntos de Salto clave.\n" +
                       "   • En la Ventana de Flotas, usa la orden 'Refuel From Target' o 'Refuel Selected Fleet' para transferir combustible en espacio profundo."
            });

            _allItems.Add(new CodexItem
            {
                Key = "Lección 5: Doctrina de Combate I - Misiles de Largo Alcance",
                DisplayTitle = "🚀 Lección 5: Doctrina de Combate I - Misiles de Largo Alcance",
                Category = "🎓 Tutoriales & Lecciones",
                Body = "========================================================================================\n" +
                       "🚀 LECCIÓN 5: DOCTRINA ESPACIAL DE MISILES Y DISPARO A LARGA DISTANCIA\n" +
                       "========================================================================================\n\n" +
                       "Los misiles permiten atacar al enemigo a distancias de decenas de millones de kilómetros, destruyendo blancos antes de que puedan responder.\n\n" +
                       "COMPONENTES DE UN SISTEMA DE MISILES:\n" +
                       "1. El Misil (Diseñado en la ventana de Missile Design):\n" +
                       "   • Cabeza de Guerra (Warhead): Determina la profundidad de penetración en el blindaje enemigo (Penetración = Raíz de Potencia).\n" +
                       "   • Motor del Misil: Determina la velocidad en km/s y el alcance máximo.\n" +
                       "   • Sensores integrados (Opcional): Permiten que el misil busque un objetivo secundario si el blanco principal es destruido.\n\n" +
                       "2. El Control de Tiro de Misiles (MFC - Missile Fire Control):\n" +
                       "   • Sistema de puntería en la nave lanzadora. Determina el alcance máximo de guiado y el número de salvas simultáneas.\n\n" +
                       "3. Los Lanzadores (Launchers) y Pañoles (Magazines):\n" +
                       "   • Tubos Lanzadores (Standard Launchers) vs Lanzadores en Caja (Box Launchers - de 1 solo uso, ultraligeros para cazas).\n" +
                       "   • Los Pañoles (Magazines) almacenan la reserva de misiles a bordo y deben estar protegidos con blindaje contra explosiones secundarias.\n\n" +
                       "FÓRMULA DE PROBABILIDAD DE IMPACTO DE MISIL:\n" +
                       "   Hit% = Min(100%, Velocidad Misil / Velocidad Blanco)\n" +
                       "Si tu misil viaja a 30,000 km/s y el crucero enemigo a 10,000 km/s, la probabilidad de impacto es del 100%."
            });

            _allItems.Add(new CodexItem
            {
                Key = "Lección 6: Doctrina de Combate II - Armas de Energía y Defensa de Punto",
                DisplayTitle = "⚡ Lección 6: Doctrina de Combate II - Armas de Energía y Defensa de Punto",
                Category = "🎓 Tutoriales & Lecciones",
                Body = "========================================================================================\n" +
                       "⚡ LECCIÓN 6: ARMAS DE ENERGÍA DIRECTA, REACTORES Y DEFENSA ANTIMISIL (POINT DEFENSE)\n" +
                       "========================================================================================\n\n" +
                       "Las armas de energía directa (Láseres, Cañones Gauss, Cañones de Partículas) no consumen munición, lo que permite travesías de combate prolongadas sin depender de líneas de reabastecimiento.\n\n" +
                       "1. TIPOS DE ARMAS DE ENERGÍA Y SUS ROLES:\n" +
                       "   • Láseres (Lasers): Gran penetración de blindaje a corta y media distancia. Disminuyen su daño con la distancia.\n" +
                       "   • Cañones Gauss (Gauss Cannons): Disparan múltiples proyectiles por turno. Son la mejor arma para Defensa de Punto (Point Defense) contra misiles entrantes.\n" +
                       "   • Cañones de Partículas (Particle Beams): Daño constante e ignoran la atenuación por distancia. Ideales para francotiradores navales.\n" +
                       "   • Carronadas de Plasma (Plasma Carronades): Daño masivo a bocajarro pero se disipan rápidamente.\n" +
                       "   • Cañones de Microondas (HPM): Destruyen componentes electrónicos y sensores enemigos sin dañar el casco.\n\n" +
                       "2. REQUERIMIENTOS ENERGÉTICOS:\n" +
                       "   • Las armas de energía requieren Reactores de Potencia (Reactors) y Recarga de Capacitores (Capacitor Recharge rate).\n" +
                       "   • Asegúrate de que la producción total de EU (Energy Units) de tus reactores sea igual o mayor que el consumo de disparo por turno de tus armas.\n\n" +
                       "3. SISTEMA DE DEFENSA DE PUNTO (POINT DEFENSE / PD):\n" +
                       "   • Configura tus cañones Gauss o torretas láser en modo 'Point Defense (Area)' o 'Point Defense (Self)' en el control de tiro (BFC).\n" +
                       "   • Cuando una salva de misiles enemigos se aproxime, tus torretas dispararán automáticamente durante la fase de intercepción destruyendo los misiles antes del impacto."
            });

            _allItems.Add(new CodexItem
            {
                Key = "Lección 7: Terraformación Colonial y Modificación Atmosférica",
                DisplayTitle = "🌍 Lección 7: Terraformación Colonial y Modificación Atmosférica",
                Category = "🎓 Tutoriales & Lecciones",
                Body = "========================================================================================\n" +
                       "🌍 LECCIÓN 7: TERRAFORMACIÓN, MODIFICACIÓN ATMOSFÉRICA Y HÁBITAT HABITABLE\n" +
                       "========================================================================================\n\n" +
                       "La terraformación transforma planetas estériles y helados en mundos verdes autosostenibles. Al reducir el Costo Colonial (Colony Cost) a 0.00, eliminas la necesidad de enviar Infraestructura Poblacional.\n\n" +
                       "FACTORES DEL COSTO COLONIAL (COLONY COST):\n" +
                       "1. Presión de Oxígeno (O2): Debe estar entre 0.10 atm y 0.30 atm. Menos de 0.10 provoca asfixia; más de 0.30 es tóxico.\n" +
                       "2. Temperatura Planetaria: Debe estar dentro del rango de tolerancia biológica de la especie (ej. -10°C a 35°C para humanos).\n" +
                       "3. Presión Atmosférica Total: La presión combinada no debe superar los límites respirables de la especie (ej. max 4.0 atm).\n" +
                       "4. Gases Tóxicos: Gases como el Cloro, Amoníaco, Metano o Dióxido de Azufre deben ser completamente eliminados de la atmósfera.\n\n" +
                       "ESTRATEGIA PASO A PASO PARA TERRAFORMAR UN PLANETA:\n" +
                       "1. Despliega Instalaciones de Terraformación (Terraforming Stations) o Barcos Terraformadores en la órbita del planeta objetivo (ej. Marte).\n" +
                       "2. En la pestaña 'Environment' de FormEconomics, selecciona el gas a inyectar:\n" +
                       "   • Para subir la temperatura: Inyecta Gas de Invernadero Seguro (A-GHG - Safe Greenhouse Gas).\n" +
                       "   • Para bajar la temperatura: Inyecta Gas Anti-Invernadero (Anti-GHG).\n" +
                       "   • Para habilitar respiración: Inyecta Oxígeno (O2) hasta alcanzar 0.15 atm.\n" +
                       "3. Monitoriza anualmente el avance en la barra de presión parcial."
            });

            _allItems.Add(new CodexItem
            {
                Key = "Lección 8: Astilleros Navales y Re-equipamiento Industrial",
                DisplayTitle = "🏗️ Lección 8: Astilleros Navales y Re-equipamiento Industrial",
                Category = "🎓 Tutoriales & Lecciones",
                Body = "========================================================================================\n" +
                       "🏗️ LECCIÓN 8: ASTILLEROS NAVALES, GRADAS Y RE-EQUIPAMIENTO INDUSTRIAL (RETOOLING)\n" +
                       "========================================================================================\n\n" +
                       "Los astilleros son las únicas instalaciones industriales capaces de construir naves espaciales. Gestionar su expansión y re-equipamiento es vital para mantener la supremacía naval.\n\n" +
                       "TIPOS DE ASTILLEROS:\n" +
                       "1. Astilleros Militares (Naval Shipyards): Construyen buques de guerra militares de cualquier tamaño. Su expansión requiere trabajadores y minerales pesados.\n" +
                       "2. Astilleros Comerciales (Commercial Shipyards): Construyen únicamente naves comerciales. Tienen un costo de expansión mucho menor y crecen en bloques de 10,000 toneladas.\n\n" +
                       "OPERACIONES INDUSTRIALES DE ASTILLERO:\n" +
                       "• Expand Shipyard Capacity: Aumenta el tonelaje máximo que puede construir el astillero (ej. de 5,000 a 10,000 toneladas).\n" +
                       "• Add Slipway: Añade una nueva grada de construcción al astillero, permitiendo fabricar múltiples naves simultáneamente.\n" +
                       "• Retool Shipyard: Reconfigura el astillero para fabricar una clase de nave diferente.\n\n" +
                       "💡 ESTRATEGIA DE RETOOLING DE CLASES DERIVADAS:\n" +
                       "Si reequipas un astillero para fabricar una clase variante de una nave previa (ej. 'Frigata Mk2' derivada de 'Frigata Mk1'), el tiempo y costo de retooling se reduce hasta un 80%. Mantén diseños estandarizados."
            });

            _allItems.Add(new CodexItem
            {
                Key = "Lección 9: Mando y Control: Comandantes, Oficiales y Asignación",
                DisplayTitle = "🎖️ Lección 9: Mando y Control: Comandantes, Oficiales y Asignación",
                Category = "🎓 Tutoriales & Lecciones",
                Body = "========================================================================================\n" +
                       "🎖️ LECCIÓN 9: OFICIALES IMPERIALES, GOBERNADORES, CIENTÍFICOS Y BONIFICACIONES DE MANDO\n" +
                       "========================================================================================\n\n" +
                       "Los oficiales y comandantes aportan bonificaciones críticas que potencian la velocidad industrial, la efectividad en combate y el rendimiento de la investigación.\n\n" +
                       "CATEGORÍAS DE OFICIALES Y SUS ATRIBUTOS CLAVE:\n" +
                       "1. Gobernadores Planetarios (Planetary Governors):\n" +
                       "   • Aportan bonificaciones en Producción Industrial, Minería, Riqueza, Terraformación y Reducción de Malestar.\n" +
                       "   • Asigna tus mejores gobernadores industriales a la Tierra y mundos mineros clave.\n\n" +
                       "2. Comandantes de Nave y Flota (Ship Captains & Fleet Commanders):\n" +
                       "   • Bonificaciones en Velocidad de Maniobra, Puntería de Armas, Eficiencia de Combustible y Moral de Tripulación.\n" +
                       "   • Los Comandantes de Flota aplican un porcentaje de su bonificación a TODAS las naves del grupo táctico.\n\n" +
                       "3. Científicos (Scientists):\n" +
                       "   • Cada científico pertenece a una rama del conocimiento (Propulsión, Energía, Sensores, Biología, etc.).\n" +
                       "   • Si la especialidad del científico coincide con el proyecto investigado, su bonificación (10% a 50%) acelera drásticamente el descubrimiento."
            });

            _allItems.Add(new CodexItem
            {
                Key = "Lección 10: Sensores, Detección Pasiva, Radares y Sigilo",
                DisplayTitle = "🛰️ Lección 10: Sensores, Detección Pasiva, Radares y Sigilo",
                Category = "🎓 Tutoriales & Lecciones",
                Body = "========================================================================================\n" +
                       "🛰️ LECCIÓN 10: SENSORES ESPACIALES, DETECCIÓN TÁCTICA Y TÉCNICAS DE SIGILO (STEALTH)\n" +
                       "========================================================================================\n\n" +
                       "En el espacio, la información es la mayor arma. Quien detecta primero al enemigo dicta las condiciones de la batalla.\n\n" +
                       "1. SENSORES PASIVOS (TÉRMICOS Y ELECTROMAGNÉTICOS):\n" +
                       "   • Sensores Térmicos (TH): Detectan la radiación infrarroja de motores e industrias sin revelar tu propia posición.\n" +
                       "   • Sensores Electromagnéticos (EM): Detectan escudos de fuerza activos, reactores y emisiones de radar enemigas.\n\n" +
                       "2. SENSORES ACTIVOS (RADARES):\n" +
                       "   • Emiten pulsos de radar para localizar naves y medir sus coordenadas exactas.\n" +
                       "   • Resolución de Radar: Define el tamaño de blanco optimizado.\n" +
                       "     - Resolución 1: Detecta cazas y misiles pequeños.\n" +
                       "     - Resolución 100: Detecta naves capitales y cruceros a distancias gigantescas.\n" +
                       "   ⚠️ REGLA DE ORO DE RADAR: Al encender tu radar activo, tu posición queda delatada a todos los sensores pasivos enemigos en el sistema.\n\n" +
                       "3. TECNOLOGÍA DE SIGILO (STEALTH):\n" +
                       "   • Recubrimientos Térmicos (Thermal Reduction): Reducen las emisiones de motor.\n" +
                       "   • Recubrimientos Anti-Radar (Stealth Coatings): Reducen el perfil TCS de la nave facilitando emboscadas."
            });

            _allItems.Add(new CodexItem
            {
                Key = "Lección 11: Fuerzas Terrestres, Formaciones y Combate Planetario",
                DisplayTitle = "🪖 Lección 11: Fuerzas Terrestres, Formaciones y Combate Planetario",
                Category = "🎓 Tutoriales & Lecciones",
                Body = "========================================================================================\n" +
                       "🪖 LECCIÓN 11: INVASIONES PLANETARIAS, FORMACIONES TERRESTRES Y BOMBARDEO ORBITAL\n" +
                       "========================================================================================\n\n" +
                       "Conquistar o defender un planeta requiere el despliegue de Fuerzas Terrestres organizadas en formaciones tácticas.\n\n" +
                       "1. DISEÑO DE FORMACIONES TERRESTRES (FormGroundForce):\n" +
                       "   • Combina Infantería de Marina, Blindados Pesados (TANKS), Artillería de Apoyo y Cañones Antiaéreos (AA).\n" +
                       "   • Añade Elementos de Cuartel General (HQ) para otorgar bonificaciones de mando a la formación.\n\n" +
                       "2. LOGÍSTICA DE TRANSPORTE Y DESEMBARCO:\n" +
                       "   • Transportes de Tropas (Troop Transports): Llevan tropas entre sistemas.\n" +
                       "   • Módulos de Desembarco Orbital (Drop Modules): Permiten lanzar unidades blindadas directamente sobre mundos enemigos bajo fuego hostil.\n\n" +
                       "3. FASES DEL COMBATE TERRESTRE:\n" +
                       "   • Las tropas defienden fortificaciones o avanzan en asalto frontal.\n" +
                       "   • Apoyo Orbital: Las naves espaciales equipadas con láseres o carronadas en órbita pueden realizar Bombardeo Orbital sobre las posiciones enemigas."
            });

            _allItems.Add(new CodexItem
            {
                Key = "Lección 12: Gestión Financiera, Riqueza y Sector Civil",
                DisplayTitle = "💵 Lección 12: Gestión Financiera, Riqueza y Sector Civil",
                Category = "🎓 Tutoriales & Lecciones",
                Body = "========================================================================================\n" +
                       "💵 LECCIÓN 12: ECONOMÍA IMPERIAL, RIQUEZA, IMPUESTOS Y SECTOR CIVIL AUTÓNOMO\n" +
                       "========================================================================================\n\n" +
                       "La riqueza (Wealth) financia el pago de salarios, el mantenimiento naval y la investigación. Mantener un superávit financiero evita la bancarrota industrial.\n\n" +
                       "FUENTES DE INGRESOS Y RIQUEZA:\n" +
                       "1. Impuestos a Trabajadores Civiles: Proporcional a la población residente en mundos habitables.\n" +
                       "2. Centros Financieros (Financial Centres): Edificios industriales dedicados exclusivamente a generar riqueza comercial.\n" +
                       "3. Licencias y Tarifas Comerciales del Sector Civil.\n\n" +
                       "EL SECTOR CIVIL AUTÓNOMO:\n" +
                       "• Las Empresas Navieras Civiles (Civilian Shipping Lines) se crean de forma autónoma con el capital privado de tu imperio.\n" +
                       "• Construyen sus propios cargueros civiles y barcos colonizadores, transportando colonos e infraestructura a tus nuevas colonias sin costo industrial para el estado."
            });

            _allItems.Add(new CodexItem
            {
                Key = "Lección 13: Exploración Galáctica, Motores de Salto y Puntos de Salto",
                DisplayTitle = "🪐 Lección 13: Exploración Galáctica, Motores de Salto y Puntos de Salto",
                Category = "🎓 Tutoriales & Lecciones",
                Body = "========================================================================================\n" +
                       "🪐 LECCIÓN 13: EXPLORACIÓN ESTELAR, PUNTOS DE SALTO Y PUERTAS GRAVITACIONALES\n" +
                       "========================================================================================\n\n" +
                       "El universo de Aurora 4X está interconectado mediante Puntos de Salto (Jump Points) gravitacionales que conducen a nuevos sistemas estelares.\n\n" +
                       "1. EXPLORACIÓN GRAVITACIONAL Y GEOLÓGICA:\n" +
                       "   • Equipa naves de exploración con Sensores Gravitacionales (Gravitational Survey Sensors) para cartografiar los Puntos de Salto de un sistema.\n" +
                       "   • Equipa naves con Sensores Geológicos (Geological Survey Sensors) para analizar depósitos minerales en planetas desconocidos.\n\n" +
                       "2. TRANSICIÓN DE SALTO ENTRE SISTEMAS:\n" +
                       "   • Motores de Salto (Jump Drives): Permiten transitar Puntos de Salto. Un 'Jump Ship' puede guiar a una escuadra entera a través del salto.\n" +
                       "   • Puertas de Salto (Jump Gates): Las naves 'Jump Gate Constructor' pueden construir una estructura permanente en el punto de salto, permitiendo que CUALQUIER nave transite sin necesidad de llevar motor de salto."
            });

            _allItems.Add(new CodexItem
            {
                Key = "Lección 14: Tácticas Avanzadas de Flota y Órdenes de Combate",
                DisplayTitle = "⚔️ Lección 14: Tácticas Avanzadas de Flota y Órdenes de Combate",
                Category = "🎓 Tutoriales & Lecciones",
                Body = "========================================================================================\n" +
                       "⚔️ LECCIÓN 14: ORGANIZACIÓN TÁCTICA DE FLOTAS, ORDENES Y CADENA DE COMANDO\n" +
                       "========================================================================================\n\n" +
                       "Una victoria naval depende de la coordinación precisa de múltiples naves especializadas operando en formación conjunta.\n\n" +
                       "ORGANIZACIÓN EN LA VENTANA DE FLOTAS (FormNavalAdmin):\n" +
                       "• Crea Fuerzas Tácticas (Task Forces) separando escuadrones de asalto, piquetes de radar y cargueros logísticos.\n" +
                       "• Asigna la orden 'Follow Fleet' o 'Shadow' para mantener naves de apoyo detrás de la línea de frente.\n\n" +
                       "TÁCTICA DE ATAQUE ALPHA STRIKE:\n" +
                       "• Coordina la velocidad de movimiento y el alcance de disparo de tus destructores de misiles.\n" +
                       "• Lanza salvas masivas concentradas sobre la nave capital enemiga para saturar su defensa de punto (Point Defense)."
            });

            _allItems.Add(new CodexItem
            {
                Key = "Lección 15: Razas No Jugadoras, Ruinas Antiguas y Modo Maestro",
                DisplayTitle = "🌌 Lección 15: Razas No Jugadoras, Ruinas Antiguas y Modo Maestro",
                Category = "🎓 Tutoriales & Lecciones",
                Body = "========================================================================================\n" +
                       "🌌 LECCIÓN 15: ENCUENTROS ALIENÍGENAS (NPR), RUINAS ANTI GUAS Y MODO SPACE MASTER\n" +
                       "========================================================================================\n\n" +
                       "En la frontera galáctica te encontrarás con civilizaciones alienígenas no jugadoras (NPR), vestigios de imperios extintos y amenazas celestes.\n\n" +
                       "1. PRIMER CONTACTO Y PROTOCOLOS DIPLOMÁTICOS:\n" +
                       "   • Al detectar naves desconocidas, se inicia el protocolo de Primer Contacto.\n" +
                       "   • Puedes enviar barcos diplomáticos para establecer relaciones pacíficas o iniciar hostilidades.\n\n" +
                       "2. RUINAS ANTI GUAS Y ARTEFACTOS PRECURSORES:\n" +
                       "   • Al explorar planetas puedes descubrir Ruinas Antiguas.\n" +
                       "   • Despliega Xenólogos y Tropas de Asalto Terrestre para investigar las ruinas y recuperar tecnologías avanzadas e instalaciones intactas.\n\n" +
                       "3. MODO MAESTRO DEL ESPACIO (SPACE MASTER MODE / SM):\n" +
                       "   • El modo SM permite modificar parámetros de la partida, crear imperios de prueba y depurar situaciones tácticas complejas."
            });

            // --------------------------------------------------------------------
            // 2. LOAD JSON MASTER DICTIONARY FILE DYNAMICALLY (1,935+ GLOSSARY TERMS)
            // --------------------------------------------------------------------
            try
            {
                string[] searchPaths = new string[]
                {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "AuroraTooltipDictionary.json"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AuroraTooltipDictionary.json"),
                    "c:/VSCODE/AuroraDesignSuite/config/AuroraTooltipDictionary.json",
                    "c:/VSCODE/Aurora271Full/Patches/AuroraSpanish/AuroraTooltipDictionary.json"
                };

                string jsonContent = null;
                foreach (string p in searchPaths)
                {
                    try
                    {
                        if (File.Exists(p))
                        {
                            jsonContent = File.ReadAllText(p);
                            break;
                        }
                    }
                    catch { }
                }

                if (!string.IsNullOrEmpty(jsonContent))
                {
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
                    if (dict != null)
                    {
                        foreach (var kvp in dict)
                        {
                            string key = kvp.Key.Trim();
                            string val = kvp.Value.Trim();

                            string cat = ClassifyCategory(key, val);
                            string icon = ClassifyIcon(key, cat);

                            _allItems.Add(new CodexItem
                            {
                                Key = key,
                                DisplayTitle = icon + " " + key,
                                Category = cat,
                                Body = val
                            });
                        }
                    }
                }
            }
            catch { }

            if (LblTotalCount != null)
            {
                LblTotalCount.Text = string.Format("({0} Artículos)", _allItems.Count);
            }
        }

        private string ClassifyCategory(string key, string body)
        {
            string k = key.ToLower();
            string b = body.ToLower();

            if (k.Contains("duranium") || k.Contains("sorium") || k.Contains("neutronium") || k.Contains("gallicite") ||
                k.Contains("corundium") || k.Contains("uridium") || k.Contains("boronide") || k.Contains("mercassium") ||
                k.Contains("vendarite") || k.Contains("tritium") || k.Contains("tritanium") || k.Contains("mineral"))
            {
                return "⛏️ Minerales & Recursos";
            }

            if (k.Contains("admin command") || k.Contains("commander") || k.Contains("rank") || k.Contains("governor") ||
                k.Contains("bonus") || k.Contains("academy") || k.Contains("officer"))
            {
                return "🎖️ Comandantes & Oficiales";
            }

            if (k.Contains("fleet") || k.Contains("salvo") || k.Contains("missile") || k.Contains("combat") ||
                k.Contains("target") || k.Contains("fire") || k.Contains("ordnance") || k.Contains("ammunition") ||
                k.Contains("oob") || k.Contains("ship kills") || k.Contains("missile kills"))
            {
                return "⚓ Flotas, Combate & Misiles";
            }

            if (k.Contains("population") || k.Contains("colony") || k.Contains("worker") || k.Contains("industry") ||
                k.Contains("factory") || k.Contains("mine") || k.Contains("refinery") || k.Contains("infrastructure") ||
                k.Contains("wealth") || k.Contains("tax") || k.Contains("spaceport") || k.Contains("construction"))
            {
                return "📊 Economía, Industria & Colonias";
            }

            if (k.Contains("engine") || k.Contains("laser") || k.Contains("sensor") || k.Contains("reactor") ||
                k.Contains("armor") || k.Contains("shield") || k.Contains("warhead") || k.Contains("turret") ||
                k.Contains("speed") || k.Contains("thermal") || k.Contains("eccm") || b.Contains("fct_techsystem"))
            {
                return "🔬 Tecnologías e Investigaciones";
            }

            if (k.Contains("ship") || k.Contains("class") || k.Contains("hs") || k.Contains("tcs") ||
                k.Contains("dcr") || k.Contains("msp") || k.Contains("retool") || k.Contains("overhaul") ||
                k.Contains("hull") || k.Contains("shipyard"))
            {
                return "🚀 Naves, Cascos & Componentes";
            }

            return "📖 Glosario Táctico Imperial";
        }

        private string ClassifyIcon(string key, string category)
        {
            if (category.Contains("Tutoriales")) return "🎓";
            if (category.Contains("Minerales")) return "⛏️";
            if (category.Contains("Comandantes")) return "🎖️";
            if (category.Contains("Flotas")) return "⚓";
            if (category.Contains("Economía")) return "📊";
            if (category.Contains("Tecnologías")) return "🔬";
            if (category.Contains("Naves")) return "🚀";

            return "💡";
        }

        private void FilterList(string query, string category)
        {
            if (_allItems == null || LstCodexItems == null || LblTotalCount == null) return;

            query = (query ?? "").Trim().ToLower();
            category = (category ?? "").Trim();

            var filtered = _allItems.AsEnumerable();

            if (!string.IsNullOrEmpty(category) && !category.Contains("Todas"))
            {
                filtered = filtered.Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(query))
            {
                filtered = filtered.Where(x => x.Key.ToLower().Contains(query) ||
                                               x.DisplayTitle.ToLower().Contains(query) ||
                                               x.Category.ToLower().Contains(query) ||
                                               x.Body.ToLower().Contains(query));
            }

            var list = filtered.ToList();
            LstCodexItems.ItemsSource = list;
            LblTotalCount.Text = string.Format("({0} Artículos)", list.Count);

            if (list.Count > 0)
            {
                LstCodexItems.SelectedIndex = 0;
            }
            else
            {
                if (LblArticleTitle != null) LblArticleTitle.Text = "Sin Resultados";
                if (LblArticleCategory != null) LblArticleCategory.Text = "🔍 Búsqueda";
                if (TxtArticleBody != null) TxtArticleBody.Text = "No se encontraron lecciones o conceptos que coincidan con los criterios de búsqueda seleccionados.";
            }
        }

        private void TxtSearchCodex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtSearchCodex == null || CboCategoryFilter == null || _allItems == null) return;
            string category = (CboCategoryFilter.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "🌟 Todas las Categorías";
            FilterList(TxtSearchCodex.Text, category);
        }

        private void CboCategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TxtSearchCodex == null || CboCategoryFilter == null || _allItems == null) return;
            string category = (CboCategoryFilter.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "🌟 Todas las Categorías";
            FilterList(TxtSearchCodex.Text, category);
        }

        private void LstCodexItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstCodexItems == null || LblArticleTitle == null || LblArticleCategory == null || TxtArticleBody == null) return;
            if (LstCodexItems.SelectedItem is CodexItem selected)
            {
                LblArticleTitle.Text = selected.DisplayTitle;
                LblArticleCategory.Text = selected.Category;
                TxtArticleBody.Text = selected.Body;
            }
        }

        private void BtnToggleCalculators_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PnlCalculatorsContainer == null || BtnToggleCalculators == null) return;
            if (PnlCalculatorsContainer.Visibility == System.Windows.Visibility.Visible)
            {
                PnlCalculatorsContainer.Visibility = System.Windows.Visibility.Collapsed;
                BtnToggleCalculators.Content = "👁️ Mostrar Calculadoras";
            }
            else
            {
                PnlCalculatorsContainer.Visibility = System.Windows.Visibility.Visible;
                BtnToggleCalculators.Content = "👁️ Ocultar Calculadoras";
            }
        }

        private void BtnSelectCalculators_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PnlCalculatorSelector == null) return;
            PnlCalculatorSelector.Visibility = PnlCalculatorSelector.Visibility == System.Windows.Visibility.Visible
                ? System.Windows.Visibility.Collapsed
                : System.Windows.Visibility.Visible;
        }

        private void OnCalcVisibilityChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (CardCalc1 == null || CardCalc2 == null || CardCalc3 == null || CardCalc4 == null || CardCalc5 == null ||
                CardCalc6 == null || CardCalc7 == null || CardCalc8 == null || CardCalc9 == null || CardCalc10 == null ||
                CardCalc11 == null || CardCalc12 == null || CardCalc13 == null || CardCalc14 == null || CardCalc15 == null ||
                CardCalc16 == null || CardCalc17 == null || CardCalc18 == null || CardCalc19 == null || CardCalc20 == null) return;

            int activeCount = 0;

            if (ChkCalc1 != null) { CardCalc1.Visibility = ChkCalc1.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc1.IsChecked == true) activeCount++; }
            if (ChkCalc2 != null) { CardCalc2.Visibility = ChkCalc2.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc2.IsChecked == true) activeCount++; }
            if (ChkCalc3 != null) { CardCalc3.Visibility = ChkCalc3.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc3.IsChecked == true) activeCount++; }
            if (ChkCalc4 != null) { CardCalc4.Visibility = ChkCalc4.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc4.IsChecked == true) activeCount++; }
            if (ChkCalc5 != null) { CardCalc5.Visibility = ChkCalc5.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc5.IsChecked == true) activeCount++; }
            if (ChkCalc6 != null) { CardCalc6.Visibility = ChkCalc6.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc6.IsChecked == true) activeCount++; }
            if (ChkCalc7 != null) { CardCalc7.Visibility = ChkCalc7.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc7.IsChecked == true) activeCount++; }
            if (ChkCalc8 != null) { CardCalc8.Visibility = ChkCalc8.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc8.IsChecked == true) activeCount++; }
            if (ChkCalc9 != null) { CardCalc9.Visibility = ChkCalc9.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc9.IsChecked == true) activeCount++; }
            if (ChkCalc10 != null) { CardCalc10.Visibility = ChkCalc10.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc10.IsChecked == true) activeCount++; }
            if (ChkCalc11 != null) { CardCalc11.Visibility = ChkCalc11.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc11.IsChecked == true) activeCount++; }
            if (ChkCalc12 != null) { CardCalc12.Visibility = ChkCalc12.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc12.IsChecked == true) activeCount++; }
            if (ChkCalc13 != null) { CardCalc13.Visibility = ChkCalc13.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc13.IsChecked == true) activeCount++; }
            if (ChkCalc14 != null) { CardCalc14.Visibility = ChkCalc14.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc14.IsChecked == true) activeCount++; }
            if (ChkCalc15 != null) { CardCalc15.Visibility = ChkCalc15.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc15.IsChecked == true) activeCount++; }
            if (ChkCalc16 != null) { CardCalc16.Visibility = ChkCalc16.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc16.IsChecked == true) activeCount++; }
            if (ChkCalc17 != null) { CardCalc17.Visibility = ChkCalc17.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc17.IsChecked == true) activeCount++; }
            if (ChkCalc18 != null) { CardCalc18.Visibility = ChkCalc18.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc18.IsChecked == true) activeCount++; }
            if (ChkCalc19 != null) { CardCalc19.Visibility = ChkCalc19.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc19.IsChecked == true) activeCount++; }
            if (ChkCalc20 != null) { CardCalc20.Visibility = ChkCalc20.IsChecked == true ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed; if (ChkCalc20.IsChecked == true) activeCount++; }

            if (BtnSelectCalculators != null)
            {
                BtnSelectCalculators.Content = $"🎛️ Selector de Calculadoras ({activeCount}/20) 🔻";
            }
        }

        private void BtnCheckAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetAllCheckState(true);
        }

        private void BtnUncheckAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetAllCheckState(false);
        }

        private void BtnResetDefault_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SetAllCheckState(true);
        }

        private void SetAllCheckState(bool isChecked)
        {
            if (ChkCalc1 != null) ChkCalc1.IsChecked = isChecked;
            if (ChkCalc2 != null) ChkCalc2.IsChecked = isChecked;
            if (ChkCalc3 != null) ChkCalc3.IsChecked = isChecked;
            if (ChkCalc4 != null) ChkCalc4.IsChecked = isChecked;
            if (ChkCalc5 != null) ChkCalc5.IsChecked = isChecked;
            if (ChkCalc6 != null) ChkCalc6.IsChecked = isChecked;
            if (ChkCalc7 != null) ChkCalc7.IsChecked = isChecked;
            if (ChkCalc8 != null) ChkCalc8.IsChecked = isChecked;
            if (ChkCalc9 != null) ChkCalc9.IsChecked = isChecked;
            if (ChkCalc10 != null) ChkCalc10.IsChecked = isChecked;
            if (ChkCalc11 != null) ChkCalc11.IsChecked = isChecked;
            if (ChkCalc12 != null) ChkCalc12.IsChecked = isChecked;
            if (ChkCalc13 != null) ChkCalc13.IsChecked = isChecked;
            if (ChkCalc14 != null) ChkCalc14.IsChecked = isChecked;
            if (ChkCalc15 != null) ChkCalc15.IsChecked = isChecked;
            if (ChkCalc16 != null) ChkCalc16.IsChecked = isChecked;
            if (ChkCalc17 != null) ChkCalc17.IsChecked = isChecked;
            if (ChkCalc18 != null) ChkCalc18.IsChecked = isChecked;
            if (ChkCalc19 != null) ChkCalc19.IsChecked = isChecked;
            if (ChkCalc20 != null) ChkCalc20.IsChecked = isChecked;

            OnCalcVisibilityChanged(this, new System.Windows.RoutedEventArgs());
        }

        private void OnFormulaInputChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // 1. Warhead Yield Formula
                if (TxtFormulaWarheadYield != null && LblFormulaWarheadResult != null && double.TryParse(TxtFormulaWarheadYield.Text, out double yieldVal) && yieldVal > 0)
                {
                    int penetration = (int)Math.Sqrt(yieldVal);
                    if (penetration < 1) penetration = 1;
                    LblFormulaWarheadResult.Text = $"{penetration} Capas de Blindaje (Crater Depth: {penetration} x Width: {penetration})";
                }

                // 2. Active Radar Formula
                if (TxtFormulaRadarStrength != null && TxtFormulaRadarRes != null && LblFormulaRadarResult != null &&
                    double.TryParse(TxtFormulaRadarStrength.Text, out double strength) &&
                    double.TryParse(TxtFormulaRadarRes.Text, out double res) && strength > 0 && res > 0)
                {
                    double rangeKm = strength * Math.Sqrt(res) * 40000.0 * 10.0;
                    double rangeMkm = rangeKm / 1000000.0;
                    LblFormulaRadarResult.Text = $"{rangeMkm:N2} Mkm ({rangeKm:N0} km)";
                }

                // 3. Engine Burn Formula
                if (TxtFormulaEngineEP != null && TxtFormulaEngineFuelRate != null && LblFormulaEngineResult != null &&
                    double.TryParse(TxtFormulaEngineEP.Text, out double ep) &&
                    double.TryParse(TxtFormulaEngineFuelRate.Text, out double rate) && ep > 0 && rate > 0)
                {
                    double fuelPerHour = ep * rate;
                    double fuelPerDay = fuelPerHour * 24.0;
                    LblFormulaEngineResult.Text = $"{fuelPerDay:N0} Litros / Día ({fuelPerHour:N1} L/hora)";
                }

                // 4. Retooling Cost Formula
                if (TxtFormulaRetoolCapacity != null && TxtFormulaRetoolDiff != null && LblFormulaRetoolResult != null &&
                    double.TryParse(TxtFormulaRetoolCapacity.Text, out double capTons) &&
                    double.TryParse(TxtFormulaRetoolDiff.Text, out double diffPct))
                {
                    double normDiff = Math.Min(100.0, Math.Max(0.0, diffPct)) / 100.0;
                    double retoolCostBP = capTons * 0.05 * (0.20 + 0.80 * normDiff);
                    double retoolDays = Math.Round(retoolCostBP / (500.0 / 365.0), 0);
                    LblFormulaRetoolResult.Text = $"{retoolCostBP:N0} BP ({retoolDays:N0} Días de re-equipamiento a 500 BP/año)";
                }

                // 5. Naval Speed Formula
                if (TxtFormulaSpeedEP != null && TxtFormulaSpeedHS != null && LblFormulaSpeedResult != null &&
                    double.TryParse(TxtFormulaSpeedEP.Text, out double speedEP) &&
                    double.TryParse(TxtFormulaSpeedHS.Text, out double speedHS) && speedHS > 0)
                {
                    double speedKmS = (speedEP / speedHS) * 1000.0;
                    double speedKmH = speedKmS * 3600.0;
                    LblFormulaSpeedResult.Text = $"{speedKmS:N0} km/s ({speedKmH:N0} km/h)";
                }

                // 6. Max Range & Endurance Formula
                if (TxtFormulaRangeFuel != null && TxtFormulaRangeSpeed != null && LblFormulaRangeResult != null &&
                    double.TryParse(TxtFormulaRangeFuel.Text, out double totalFuel) &&
                    double.TryParse(TxtFormulaRangeSpeed.Text, out double cruiseSpeedKmS) && totalFuel > 0 && cruiseSpeedKmS > 0)
                {
                    double burnLPH = 500.0 * 0.8;
                    double flightHours = totalFuel / burnLPH;
                    double flightDays = flightHours / 24.0;
                    double totalDistBkm = (flightHours * cruiseSpeedKmS * 3600.0) / 1_000_000_000.0;
                    double totalDistAU = totalDistBkm / 0.14959787;
                    LblFormulaRangeResult.Text = $"{totalDistBkm:N2} Mil Millones km ({totalDistAU:N2} AU) | {flightDays:F1} Días de vuelo continuo";
                }

                // 7. Maintenance AFR & MSP Formula
                if (TxtFormulaMaintBP != null && TxtFormulaMaintDCR != null && LblFormulaMaintResult != null &&
                    double.TryParse(TxtFormulaMaintBP.Text, out double shipCostBP) &&
                    double.TryParse(TxtFormulaMaintDCR.Text, out double dcrRating) && shipCostBP > 0)
                {
                    double effDCR = Math.Max(1.0, dcrRating);
                    double afrPercent = Math.Min(100.0, Math.Max(1.0, (shipCostBP / (effDCR * 5.0))));
                    double mtbfDays = Math.Round(365.0 / (afrPercent / 100.0), 0);
                    LblFormulaMaintResult.Text = $"AFR: {afrPercent:F1}% | Tiempo Promedio entre Fallos Mecánicos: ~{mtbfDays:N0} Días";
                }

                // 8. Planet Colony Cost Formula
                if (TxtFormulaColonyTemp != null && TxtFormulaColonyO2 != null && LblFormulaColonyResult != null &&
                    double.TryParse(TxtFormulaColonyTemp.Text, out double colTemp) &&
                    double.TryParse(TxtFormulaColonyO2.Text, out double colO2))
                {
                    double tempDiff = Math.Abs(colTemp - 15.0);
                    double tempCost = tempDiff > 30.0 ? (tempDiff - 30.0) / 20.0 : 0.0;
                    double o2Cost = colO2 < 0.10 ? 2.0 : (colO2 > 0.30 ? 3.0 : 0.0);
                    double netColonyCost = Math.Round(tempCost + o2Cost, 2);
                    double infraReq = netColonyCost * 100.0;
                    LblFormulaColonyResult.Text = $"Colony Cost: {netColonyCost:F2} (Requiere {infraReq:N0} Infraestructuras por millón de habs)";
                }

                // 9. Mining Yield Formula
                if (TxtFormulaMiningCount != null && TxtFormulaMiningAcc != null && LblFormulaMiningResult != null &&
                    double.TryParse(TxtFormulaMiningCount.Text, out double mineCount) &&
                    double.TryParse(TxtFormulaMiningAcc.Text, out double mineAcc) && mineCount > 0 && mineAcc > 0)
                {
                    double ratePerMine = 12.0;
                    double outputPerMin = mineCount * ratePerMine * Math.Min(1.0, mineAcc);
                    double totalAllMins = outputPerMin * 11.0;
                    LblFormulaMiningResult.Text = $"{outputPerMin:N0} Tons / Año por mineral ({totalAllMins:N0} Tons combinadas de los 11 minerales)";
                }

                // 10. Passive Thermal Signature & Stealth Detection Formula
                if (TxtFormulaStealthTH != null && TxtFormulaStealthSens != null && LblFormulaStealthResult != null &&
                    double.TryParse(TxtFormulaStealthTH.Text, out double thermalSig) &&
                    double.TryParse(TxtFormulaStealthSens.Text, out double sensPower) && thermalSig > 0 && sensPower > 0)
                {
                    double detRangeKm = thermalSig * sensPower * 10000.0;
                    double detRangeMkm = detRangeKm / 1000000.0;
                    LblFormulaStealthResult.Text = $"{detRangeMkm:N2} Mkm ({detRangeKm:N0} km de delación por radiación térmica)";
                }

                // 11. Shield Recharge Formula
                if (TxtFormulaShieldCount != null && TxtFormulaShieldTech != null && LblFormulaShieldResult != null &&
                    double.TryParse(TxtFormulaShieldCount.Text, out double shCount) &&
                    double.TryParse(TxtFormulaShieldTech.Text, out double shPower) && shCount > 0 && shPower > 0)
                {
                    double totalShields = shCount * shPower;
                    double rechargeSecs = totalShields * 10.0;
                    double rechargeMins = rechargeSecs / 60.0;
                    LblFormulaShieldResult.Text = $"{totalShields:N0} Puntos de Escudo | Tiempo de Recarga 100%: {rechargeSecs:N0} Segundos ({rechargeMins:F1} min)";
                }

                // 12. Beam Capacitor Recharge Formula
                if (TxtFormulaBeamPower != null && TxtFormulaCapacitorRate != null && LblFormulaBeamResult != null &&
                    double.TryParse(TxtFormulaBeamPower.Text, out double beamEU) &&
                    double.TryParse(TxtFormulaCapacitorRate.Text, out double capEU) && beamEU > 0 && capEU > 0)
                {
                    double turnsNeeded = Math.Ceiling(beamEU / capEU);
                    double secsNeeded = turnsNeeded * 5.0;
                    LblFormulaBeamResult.Text = $"1 Disparo cada {secsNeeded:N0} Segundos ({turnsNeeded:N0} Turnos de 5s)";
                }

                // 13. Missile Hit Probability Formula
                if (TxtFormulaMissileSpeed != null && TxtFormulaTargetSpeed != null && LblFormulaMissileHitResult != null &&
                    double.TryParse(TxtFormulaMissileSpeed.Text, out double mSpeed) &&
                    double.TryParse(TxtFormulaTargetSpeed.Text, out double tSpeed) && mSpeed > 0 && tSpeed > 0)
                {
                    double speedRatio = mSpeed / tSpeed;
                    double hitPct = Math.Min(100.0, Math.Max(1.0, speedRatio * 37.5));
                    LblFormulaMissileHitResult.Text = $"{hitPct:F1}% de Impacto (Relación de Velocidad {speedRatio:F2}x)";
                }

                // 14. Construction Capacity Formula
                if (TxtFormulaFactoriesCount != null && TxtFormulaGovMod != null && LblFormulaConstResult != null &&
                    double.TryParse(TxtFormulaFactoriesCount.Text, out double facCount) &&
                    double.TryParse(TxtFormulaGovMod.Text, out double govMod) && facCount > 0)
                {
                    double totalBP = facCount * 10.0 * (1.0 + (govMod / 100.0));
                    double daysFor1Fac = Math.Round(120.0 / (totalBP / 365.0), 0);
                    LblFormulaConstResult.Text = $"{totalBP:N0} BP / Año (Tiempo para edificar 1 fábrica de 120 BP: ~{daysFor1Fac:N0} Días)";
                }

                // 15. Passive EM Sensor Detection Formula
                if (TxtFormulaEMSensingSig != null && TxtFormulaEMSensRating != null && LblFormulaEMResult != null &&
                    double.TryParse(TxtFormulaEMSensingSig.Text, out double emSig) &&
                    double.TryParse(TxtFormulaEMSensRating.Text, out double emSens) && emSig > 0 && emSens > 0)
                {
                    double emRangeKm = emSig * emSens * 10000.0;
                    double emRangeMkm = emRangeKm / 1000000.0;
                    LblFormulaEMResult.Text = $"{emRangeMkm:N2} Mkm ({emRangeKm:N0} km de delación pasiva de escudos/radares)";
                }

                // 16. Mass Driver Transport Logistics Formula
                if (TxtFormulaMassDriverCount != null && TxtFormulaDriverDist != null && LblFormulaMassDriverResult != null &&
                    double.TryParse(TxtFormulaMassDriverCount.Text, out double driverCount) &&
                    double.TryParse(TxtFormulaDriverDist.Text, out double driverDistMkm) && driverCount > 0)
                {
                    double tonsPerYear = driverCount * 5000.0;
                    double flightDays = Math.Round(driverDistMkm / 43.2, 1);
                    LblFormulaMassDriverResult.Text = $"{tonsPerYear:N0} Tons / Año catapultadas (Tránsito: ~{flightDays:F1} Días por paquete)";
                }

                // 17. Research RP Output Formula
                if (TxtFormulaLabsCount != null && TxtFormulaSciBonus != null && LblFormulaResearchResult != null &&
                    double.TryParse(TxtFormulaLabsCount.Text, out double labCount) &&
                    double.TryParse(TxtFormulaSciBonus.Text, out double sciBonus) && labCount > 0)
                {
                    double totalRP = labCount * 200.0 * (1.0 + (sciBonus / 100.0));
                    double yearsFor5k = Math.Round(5000.0 / totalRP, 1);
                    LblFormulaResearchResult.Text = $"{totalRP:N0} RP / Año (Tiempo para investigar tecnología de 5,000 RP: ~{yearsFor5k:F1} Años)";
                }

                // 18. Point Defense & CIWS Interception Formula
                if (TxtFormulaGaussCount != null && TxtFormulaTrackingSpeed != null && LblFormulaPointDefResult != null &&
                    double.TryParse(TxtFormulaGaussCount.Text, out double gaussShots) &&
                    double.TryParse(TxtFormulaTrackingSpeed.Text, out double trackSpeed) && gaussShots > 0 && trackSpeed > 0)
                {
                    double trackRatio = Math.Min(1.0, trackSpeed / 10000.0);
                    double intercepted = Math.Round(gaussShots * 0.5 * trackRatio, 1);
                    LblFormulaPointDefResult.Text = $"~{intercepted:F1} Misiles enemigos interceptados por turno de 5 segundos";
                }

                // 19. Naval Maintenance Drydock Capacity Formula
                if (TxtFormulaMaintFacCount != null && TxtFormulaColonyPop != null && LblFormulaNavalMaintResult != null &&
                    double.TryParse(TxtFormulaMaintFacCount.Text, out double maintFacs) &&
                    double.TryParse(TxtFormulaColonyPop.Text, out double colPop) && maintFacs > 0)
                {
                    double maxTonnage = maintFacs * 2000.0;
                    LblFormulaNavalMaintResult.Text = $"{maxTonnage:N0} Toneladas Navales soportadas sin desgaste en dique seco";
                }

                // 20. Empire Wealth Generation Formula
                if (TxtFormulaPopMillions != null && TxtFormulaFinancialCount != null && LblFormulaWealthResult != null &&
                    double.TryParse(TxtFormulaPopMillions.Text, out double popM) &&
                    double.TryParse(TxtFormulaFinancialCount.Text, out double finCount))
                {
                    double popIncome = popM * 1000.0;
                    double finIncome = finCount * 25000.0;
                    double totalWealth = popIncome + finIncome;
                    LblFormulaWealthResult.Text = $"{totalWealth:N0} Riqueza / Año (Población: {popIncome:N0} + Centros Financieros: {finIncome:N0})";
                }
            }
            catch { }
        }
    }
}
