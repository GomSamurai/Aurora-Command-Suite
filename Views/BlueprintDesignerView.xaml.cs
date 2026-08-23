using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;
using Component = AuroraDesignSuite.Models.Component;

namespace AuroraDesignSuite.Views
{
    public class PresetItem
    {
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int Index { get; set; }
        public bool IsUserPreset { get; set; } = false;
        public string TacticalDescription { get; set; } = string.Empty;
        public UserPresetData? UserData { get; set; }

        public override string ToString() => Title;
    }

    public class ValidationDisplayItem
    {
        public string Message { get; set; } = string.Empty;
        public string ColorHex { get; set; } = "#FF8888";
    }

    public partial class BlueprintDesignerView : UserControl
    {
        private DatabaseService? _dbService;
        private readonly ShipCalculationEngine _calcEngine = new ShipCalculationEngine();

        private readonly ObservableCollection<Component> _allComponents = new ObservableCollection<Component>();
        private readonly ObservableCollection<Component> _filteredComponents = new ObservableCollection<Component>();
        private readonly ObservableCollection<SelectedComponentItem> _selectedComponents = new ObservableCollection<SelectedComponentItem>();
        private readonly List<PresetItem> _allPresetsList = new List<PresetItem>();

        public ShipDesign CurrentDesign { get; private set; } = new ShipDesign();
        public int SelectedRaceID => (CmbEmpire?.SelectedItem as Empire)?.RaceID ?? 0;
        public DatabaseService? DbService => _dbService;

        public BlueprintDesignerView()
        {
            InitializeComponent();
            DgComponentPalette.ItemsSource = _filteredComponents;
            DgSelectedComponents.ItemsSource = _selectedComponents;

            InitializeCategories();
            InitializePresets();
            InitializeRealWorldObjectsCatalog();
        }

        private void InitializeCategories()
        {
            var categories = new List<string>
            {
                "📂 Todas las Categorías",
                "🚀 Motores / Propulsión",
                "⛽ Tanques de Combustible",
                "🏠 Habitabilidad y Tripulación",
                "🛠️ Mantenimiento e Ingeniería",
                "📡 Sensores Activos / Pasivos",
                "💥 Armas de Energía / Láseres",
                "🚀 Lanzadores y Misiles",
                "🛡️ Escudos y Armadura",
                "🌌 Motores de Salto"
            };
            CmbCategoryFilter.ItemsSource = categories;
            CmbCategoryFilter.SelectedIndex = 0;

            var presetCatFilters = new List<string>
            {
                "📂 Todas las Categorías",
                "1. Destructores (DD / DDG / PD)",
                "2. Cruceros (CA / CC / CG / BC)",
                "3. Portaaviones (CV / CVL / CVE)",
                "4. Cazas y Naves Parásitas",
                "5. Exploración y Ciencia",
                "6. Petroleros y Reabastecimiento",
                "7. Naves de Misiles y Asedio",
                "8. Combate Compacto y Corbetas",
                "9. Formaciones Terrestres",
                "10. Logística Modular",
                "💾 Diseños del Usuario"
            };
            CmbPresetCategoryFilter.ItemsSource = presetCatFilters;
            CmbPresetCategoryFilter.SelectedIndex = 0;
        }

        private void InitializePresets()
        {
            _allPresetsList.Clear();
            int idx = 0;

            // 1. Destructores
            AddPreset(ref idx, "🚀 DDG Artemis - Destructor Lanzamisiles de Flota (9,000 t)", "1. Destructores (DD / DDG / PD)", 
                "🎯 PROPÓSITO: Destructor de escolta y combate de misiles antibuque de medio y largo alcance.\n⚓ DOCTRINA: Opera en la vanguardia de la flota lanzando salvas concentradas de misiles pesados mientras mantiene distancia de seguridad del fuego beam enemigo.\n📊 EXPECTATIVAS: Alta cadencia VLS, radar activo de búsqueda de 96Mkm y 4 capas de blindaje composite.\n📏 MAGNITUD EN PUERTO: Con sus 185 metros de longitud y 9,000 toneladas, esta nave es tan grande como el Coliseo Romano de extremo a extremo. Su imponente casco domina las bahías de atracamiento del dique orbital.");

            AddPreset(ref idx, "🛡️ DD-PD Aegis-G - Destructor Escolta Defensa de Punto (8,500 t)", "1. Destructores (DD / DDG / PD)",
                "🎯 PROPÓSITO: Destructor especializado en defensa de punto (AMM/PD) para protección de portaaviones y cruceros.\n⚓ DOCTRINA: Se posiciona en el centro del grupo de batalla entrelazando barreras Gauss y cañones de energía para interceptar salvas masivas de misiles enemigos.\n📊 EXPECTATIVAS: Intercepción de hasta 12 misiles por turno de 5s, recarga rápida de capacitores y escudos Alpha.\n📏 MAGNITUD EN PUERTO: Eslora de 180 metros (equivalente a 2 campos de fútbol juntos) repleta de torretas antiaéreas y sensores de seguimiento.");

            AddPreset(ref idx, "💥 DD-Beam Lancer - Destructor Láser Espinal (9,500 t)", "1. Destructores (DD / DDG / PD)",
                "🎯 PROPÓSITO: Destructor cazador con cañón láser espinal de alto calibre.\n⚓ DOCTRINA: Diseñado para emboscadas a corta distancia a través de puntos de salto y combate en cerrado contra cruceros blindados.\n📊 EXPECTATIVAS: Gran potencia de perforación de blindaje, velocidad militar de 4,200 km/s y recarga de capacitores en 15s.\n📏 MAGNITUD EN PUERTO: Casco afilado de 190 metros de largo cuya espina dorsal está ocupada enteramente por el focalizador del cañón láser gigante.");

            AddPreset(ref idx, "🌌 DDJ Pathbreaker - Destructor Líder de Salto (10,000 t)", "1. Destructores (DD / DDG / PD)",
                "🎯 PROPÓSITO: Destructor nave insignia de salto equipado con motor de salto táctico.\n⚓ DOCTRINA: Guía a flotillas de destructores a través de puntos de salto no estabilizados sin depender de puertas estelares.\n📊 EXPECTATIVAS: Motor de salto militar para 10,000t, sensores de corto alcance y defensa antiaérea.\n📏 MAGNITUD EN PUERTO: 195 metros de longitud albergando la inmensa bobina de distorsión gravitacional trans-newtoniana.");

            AddPreset(ref idx, "📦 DDB Barrage - Destructor Emboscada Box Launchers (8,000 t)", "1. Destructores (DD / DDG / PD)",
                "🎯 PROPÓSITO: Destructor de ataque relámpago con lanzadores desechables Box Launchers.\n⚓ DOCTRINA: Descarga toda su munición en un único turno masivo de saturación y se retira a reabastecer a la base orbital.\n📊 EXPECTATIVAS: Salva devastadora de 24 misiles simultáneos a un costo de construcción mínimo.\n📏 MAGNITUD EN PUERTO: 175 metros de eslora cubiertos de bahías de lanzamiento selladas listas para ráfagas relámpago.");

            // 2. Cruceros
            AddPreset(ref idx, "💥 CA Vindicator - Crucero Pesado Beam de Línea (20,000 t)", "2. Cruceros (CA / CC / CG / BC)",
                "🎯 PROPÓSITO: Crucero pesado de línea con batería principal de armas de energía y blindaje reforzado.\n⚓ DOCTRINA: Buque insignia de línea de batalla. Sostiene el combate directo contra naves de guerra alienígenas.\n📊 EXPECTATIVAS: 6 capas de blindaje, generadores de escudo Alpha, 4 baterías láser pesadas y autonomía de 24 meses.\n📏 MAGNITUD EN PUERTO: 270 metros de longitud (tan largo como un portaaviones supercarrier de la Tierra o la Torre Eiffel echada). Una fortaleza que intimida a cualquier invasor.");

            AddPreset(ref idx, "📡 CC Oracle - Crucero de Mando y Sensores AWACS (18,000 t)", "2. Cruceros (CA / CC / CG / BC)",
                "🎯 PROPÓSITO: Crucero de mando, guerra electrónica y alerta temprana AWACS.\n⚓ DOCTRINA: Permanece en el centro del comando imperial escaneando el sistema con sensores pasivos/activos de ultra alcance.\n📊 EXPECTATIVAS: Detección activa a más de 200Mkm, coordinación de flota y centro de oficiales superiores.\n📏 MAGNITUD EN PUERTO: 255 metros de eslora erizada de cúpulas de sensores térmicos y reflectores activos gigantes.");

            AddPreset(ref idx, "🚀 CG Titan - Crucero Lanzamisiles Pesado (22,000 t)", "2. Cruceros (CA / CC / CG / BC)",
                "🎯 PROPÓSITO: Crucero pesado de bombardeo y combate de misiles de largo alcance.\n⚓ DOCTRINA: Lanza ataques de saturación contra flotas de combate enemigas y bases estelares.\n📊 EXPECTATIVAS: Pañoles masivos de misiles tamaño 6, sensores de control de tiro de 120Mkm y 6 capas de blindaje.\n📏 MAGNITUD EN PUERTO: 280 metros de largo equipados con bahías de pañoles automatizadas para almacenar cientos de proyectiles pesados.");

            AddPreset(ref idx, "⚡ BC Stalker - Crucero de Batalla Rápido (25,000 t)", "2. Cruceros (CA / CC / CG / BC)",
                "🎯 PROPÓSITO: Crucero de batalla rápido diseñado para cazar y exterminar cruceros enemigos.\n⚓ DOCTRINA: Combina la velocidad de un destructor (5,500 km/s) con la potencia de fuego de un acorazado.\n📊 EXPECTATIVAS: Alta movilidad estratégica, baterías de plasma/láser y escudos de energía integrados.\n📏 MAGNITUD EN PUERTO: 300 metros de eslora con una relación longitud/manga estilizada diseñada para albergar gigantescos motores militares.");

            AddPreset(ref idx, "🌌 CJ Aether Gate - Crucero de Salto de Flota (25,000 t)", "2. Cruceros (CA / CC / CG / BC)",
                "🎯 PROPÓSITO: Crucero de salto de flota principal.\n⚓ DOCTRINA: Abre brechas gravitacionales para permitir el paso de cruceros pesados y portaaviones en sistemas inexplorados.\n📊 EXPECTATIVAS: Motor de salto trans-newtoniano de 25,000t con eficiencia de escuadrón.\n📏 MAGNITUD EN PUERTO: 300 metros de estructura acorazada dominados por el núcleo de distorsión de hiper-salto.");

            // 3. Portaaviones
            AddPreset(ref idx, "🚢 CV Valhalla - Superportaaviones de Flota (40,000 t)", "3. Portaaviones (CV / CVL / CVE)",
                "🎯 PROPÓSITO: Superportaaviones de flota con capacidad para más de 30 cazas y corbetas.\n⚓ DOCTRINA: Centro neurálgico del poder espacial imperial. Proyecta fuerza a sistemas enteros mediante alas embarcadas.\n📊 EXPECTATIVAS: Hangares masivos, talleres de reparación embarcados, 6 capas de blindaje y escudos pesados.\n📏 MAGNITUD EN PUERTO: Con 365 metros de eslora y 40,000t, supera la altura del rascacielos Empire State Building. Alberga verdaderas ciudades interiores y hangares de múltiples cubiertas.");

            AddPreset(ref idx, "🛡️ CVL Dauntless - Portaaviones Ligero de Escolta (15,000 t)", "3. Portaaviones (CV / CVL / CVE)",
                "🎯 PROPÓSITO: Portaaviones ligero de escolta para protección de convoyes y exploraciones.\n⚓ DOCTRINA: Alberga 6-8 cazas interceptores para repeler agresiones sorpresa a convoyes civiles.\n📊 EXPECTATIVAS: Tamaño reducido (15,000t), bajo consumo de combustible y mantenimiento económico.\n📏 MAGNITUD EN PUERTO: 235 metros de eslora con pista de lanzamiento interna sellada y ascensores magnéticos.");

            AddPreset(ref idx, "🐝 CVE Hive - Nodriza de Lanchas de Asalto / FAC Tender (25,000 t)", "3. Portaaviones (CV / CVL / CVE)",
                "🎯 PROPÓSITO: Nodriza militar de lanchas de ataque rápido (FAC Tender).\n⚓ DOCTRINA: Reabastece, repara y transporta 4 corbetas/FAC a través de saltos de sistema.\n📊 EXPECTATIVAS: Instalaciones de repostaje rápido, depósitos de Sorium y hangares modulares.\n📏 MAGNITUD EN PUERTO: 300 metros de eslora con bahías de atracamiento ventrales preparadas para fijar corbetas enteras.");

            AddPreset(ref idx, "🏰 CVB Iron Haven - Portaaviones Blindado de Asalto (30,000 t)", "3. Portaaviones (CV / CVL / CVE)",
                "🎯 PROPÓSITO: Portaaviones blindado de asalto frontal.\n⚓ DOCTRINA: Despliega fuerzas en sectores de intenso fuego enemigo sin arriesgar la integridad del buque.\n📊 EXPECTATIVAS: 8 capas de blindaje de neutronium, doble matriz de escudos y hangares reforzados.\n📏 MAGNITUD EN PUERTO: 320 metros de longitud recubiertos con las placas de armadura más gruesas de la flota imperial.");

            AddPreset(ref idx, "🤖 CVD Nexus - Portadrones Automatizado (10,000 t)", "3. Portaaviones (CV / CVL / CVE)",
                "🎯 PROPÓSITO: Portadrones automatizado de vanguardia.\n⚓ DOCTRINA: Despliega enjambres de drones kamikaze y sondas de reconocimiento en territorio hostil.\n📊 EXPECTATIVAS: Operación con tripulación reducida, enlace de telemetría remota y alta velocidad de despliegue.\n📏 MAGNITUD EN PUERTO: 195 metros de casco automatizado optimizado para la descarga rápida de sondas avanzadas.");

            // 4. Cazas y Parásitas
            AddPreset(ref idx, "🐝 F-01 Hornet - Caza Interceptor Gauss (250 t)", "4. Cazas y Naves Parásitas",
                "🎯 PROPÓSITO: Caza interceptor Gauss ultra-rápido (250t).\n⚓ DOCTRINA: Despliegue enjambre desde portaaviones para interceptar salvas de misiles o ametrallar cazas enemigos.\n📊 EXPECTATIVAS: Velocidad extrema de 8,000 km/s, sin requerir tanques de combustible de largo alcance.\n📏 MAGNITUD EN PUERTO: 28 metros de longitud (mismas dimensiones que un caza de combate moderno F-22 Raptor o Su-57). Cabe ágilmente en los ascensores de hangar.");

            AddPreset(ref idx, "🚀 B-01 Viper - Caza Torpedeo Misiles (500 t)", "4. Cazas y Naves Parásitas",
                "🎯 PROPÓSITO: Caza bombardero de torpedos y misiles pesados (500t).\n⚓ DOCTRINA: Ataques relámpago contra naves capitales enemigas lanzando torpedos a bocajarro y regresando al nodriza.\n📊 EXPECTATIVAS: Elevado impacto por nave, costo mínimo de producción y maniobrabilidad en combate.\n📏 MAGNITUD EN PUERTO: 42 metros de largo (similar a un transbordador espacial). Diseñado para lanzar sus torpedos en pasadas a corta distancia.");

            AddPreset(ref idx, "💥 FB-02 Sabre - Caza Cañonero Beam (500 t)", "4. Cazas y Naves Parásitas",
                "🎯 PROPÓSITO: Caza cañonero de energía láser/plasma (500t).\n⚓ DOCTRINA: Combate cerrado a corta distancia desgastando escudos y capas de blindaje superficial enemigas.\n📊 EXPECTATIVAS: Alta precisión angular, capacitores ligeros integrados.\n📏 MAGNITUD EN PUERTO: 42 metros de largo equipado con focalizadores térmicos ligeros en sus alas.");

            AddPreset(ref idx, "🔍 R-01 Eye - Caza de Reconocimiento Pasivo (200 t)", "4. Cazas y Naves Parásitas",
                "🎯 PROPÓSITO: Sonda de reconocimiento pasivo furtivo (200t).\n⚓ DOCTRINA: Explora sectores desconocidos del sistema sin activar radares para mantener el sigilo absoluto.\n📊 EXPECTATIVAS: Sensores térmicos y electromagnéticos de alta sensibilidad en un casco diminuto.\n📏 MAGNITUD EN PUERTO: 24 metros de longitud recubiertos de pintura absorvente anti-IR.");

            AddPreset(ref idx, "💣 D-01 Wasp - Drone Kamikaze / Señuelo (150 t)", "4. Cazas y Naves Parásitas",
                "🎯 PROPÓSITO: Drone kamikaze señuelo (150t).\n⚓ DOCTRINA: Atrae el fuego de las defensas de punto enemigas simulando ser un buque capital de mayor tamaño.\n📊 EXPECTATIVAS: Emisor de firma térmica amplificada y costo de producción insignificante.\n📏 MAGNITUD EN PUERTO: 20 metros de eslora automatizada sin soporte vital para tripulación.");

            // 5. Exploración y Ciencia
            AddPreset(ref idx, "🌌 ES-Grav Compass - Explorador Gravitacional Rápido (3,000 t)", "5. Exploración y Ciencia",
                "🎯 PROPÓSITO: Nave de exploración gravitacional de vanguardia (3,000t).\n⚓ DOCTRINA: Mapea puntos de salto en sistemas no cartografiados para la expansión imperial.\n📊 EXPECTATIVAS: Sensores gravimétricos de alta sensibilidad, motor de salto propio y autonomía de 36 meses.\n📏 MAGNITUD EN PUERTO: 120 metros de eslora (más largo que un campo de fútbol profesional). Equipado con laboratorios científicos completos.");

            AddPreset(ref idx, "🔍 ES-Geo Prospector - Explorador Geológico Celular (3,000 t)", "5. Exploración y Ciencia",
                "🎯 PROPÓSITO: Nave de prospectiva geológica planetaria (3,000t).\n⚓ DOCTRINA: Escanea asteroides, lunas y planetas identificando yacimientos de minerales trans-newtonianos.\n📊 EXPECTATIVAS: Sensores geológicos celulares, alta autonomía y bajo costo operativo.\n📏 MAGNITUD EN PUERTO: 120 metros de longitud albergando matrices de escáneres minerales de alta resolución.");

            AddPreset(ref idx, "🧭 GSV Pathfinder - Explorador Combinado de Largo Alcance (7,000 t)", "5. Exploración y Ciencia",
                "🎯 PROPÓSITO: Explorador espacial combinado de largo alcance (7,000t).\n⚓ DOCTRINA: Nave científica autónoma equipada con sensores gravimétricos y geológicos integrados.\n📊 EXPECTATIVAS: Cobertura total de exploración, motor de salto y capacidad de supervivencia en fronteras lejanas.\n📏 MAGNITUD EN PUERTO: 165 metros de eslora (tan alto/largo como la torre Monumento a Washington). Sostiene misiones aisladas durante años.");

            AddPreset(ref idx, "🛰️ Probe - Lancha Científica Parásita (300 t)", "5. Exploración y Ciencia",
                "🎯 PROPÓSITO: Lancha científica embarcable para reconocimiento exploratorio cercano (300t).\n⚓ DOCTRINA: Lanzada desde exploradores nodriza para investigar planetas de alto peligro ambiental.\n📊 EXPECTATIVAS: Sensores pasivos integrados y perfil de masa reducido.\n📏 MAGNITUD EN PUERTO: 32 metros de eslora construidos para descensos a atmósferas densas.");

            AddPreset(ref idx, "📡 SV-Scout Sentinel - Piquete Científico Alerta Temprana (4,000 t)", "5. Exploración y Ciencia",
                "🎯 PROPÓSITO: Piquete de vigilancia científica y alerta temprana (4,000t).\n⚓ DOCTRINA: Estacionado cerca de puntos de salto estratégicos para detectar intrusiones enemigas al instante.\n📊 EXPECTATIVAS: Sensores pasivos de largo alcance, emisores de baliza y motores de escape rápido.\n📏 MAGNITUD EN PUERTO: 135 metros de eslora dominados por una antena de alerta pasiva de ultra frecuencia.");

            // 6. Petroleros y Reabastecimiento
            AddPreset(ref idx, "⚡ AOF Endurance - Petrolero de Flota Rápido (25,000 t)", "6. Petroleros y Reabastecimiento",
                "🎯 PROPÓSITO: Petrolero de flota rápido para soporte de grupos de batalla en avance.\n⚓ DOCTRINA: Acompaña a las escuadras militares reabasteciendo combustible Sorium en pleno vuelo.\n📊 EXPECTATIVAS: Tanques de 5,000,000L, sistemas de transferencia ultra-rápida y velocidad militar.\n📏 MAGNITUD EN PUERTO: 300 metros de longitud equipados con gigantescos tanques de presión y mangueras de transferencia de alta velocidad.");

            AddPreset(ref idx, "⛽ AO Prometheus - Supertanquero Comercial Estratégico (60,000 t)", "6. Petroleros y Reabastecimiento",
                "🎯 PROPÓSITO: Supertanquero comercial estratégico (60,000t).\n⚓ DOCTRINA: Transporta reservas masivas de combustible entre la Metrópoli y las colonias exteriores.\n📊 EXPECTATIVAS: Capacidad para 25,000,000 Litros de Sorium y motores comerciales de alta eficiencia.\n📏 MAGNITUD EN PUERTO: Con 430 metros de eslora y 60,000t, supera holgadamente las dimensiones del rascacielos Empire State Building. Una arteria vital de la economía imperial.");

            AddPreset(ref idx, "🪐 AOH Harvester Primus - Cosechadora de Sorium Orbital (50,000 t)", "6. Petroleros y Reabastecimiento",
                "🎯 PROPÓSITO: Cosechadora orbital de Sorium en gigantes gaseosos (50,000t).\n⚓ DOCTRINA: Permanece en órbita de gigantes de gas refinando combustible Sorium de forma continua.\n📊 EXPECTATIVAS: Módulos de extracción de combustible, almacenamiento masivo y operación comercial.\n📏 MAGNITUD EN PUERTO: 400 metros de diámetro operativo albergando refinerías químicas de alta potencia.");

            AddPreset(ref idx, "📦 AOR Atlas Fleet - Aprovisionamiento Logístico Combinado (35,000 t)", "6. Petroleros y Reabastecimiento",
                "🎯 PROPÓSITO: Nave de aprovisionamiento logístico combinado (combustible + MSP + municiones).\n⚓ DOCTRINA: Sostiene campañas prolongadas de flotas de guerra en sectores alienígenas distantes.\n📊 EXPECTATIVAS: Tanques de Sorium, pañoles de munición VLS y repuestos MSP.\n📏 MAGNITUD EN PUERTO: 345 metros de eslora con grúas de carga magnética integradas.");

            AddPreset(ref idx, "🏰 Safehaven - Estación Tanque de Almacenamiento (80,000 t)", "6. Petroleros y Reabastecimiento",
                "🎯 PROPÓSITO: Estación orbital de almacenamiento de combustible sin propulsión (80,000t).\n⚓ DOCTRINA: Sirve como depósito nodriza en sistemas fronterizos para reabastecimiento rápido de flotas.\n📊 EXPECTATIVAS: Capacidad para 50,000,000L de Sorium a costo mínimo.\n📏 MAGNITUD EN PUERTO: 485 metros de estructura esférica orbital (tan grande como el tramo principal del Puente Golden Gate).");

            // 7. Naves de Misiles y Asedio
            AddPreset(ref idx, "💣 BBG Nemesis - Monitor de Misiles Asedio Planetario (20,000 t)", "7. Naves de Misiles y Asedio",
                "🎯 PROPÓSITO: Monitor de misiles de asedio planetario e instalaciones fijas.\n⚓ DOCTRINA: Destruye complejas defensas planetarias enemigas lanzando misiles pesados de ojiva masiva desde fuera del alcance enemigo.\n📊 EXPECTATIVAS: Lanzadores de gran tamaño, pañoles blindados y control de tiro corregido por telemetría.\n📏 MAGNITUD EN PUERTO: 270 metros de casco fuertemente protegido blindado contra represalias de baterías de superficie.");

            AddPreset(ref idx, "🛡️ FFG-AMM Shield - Fragata Antimisil Cobertura Área (6,000 t)", "7. Naves de Misiles y Asedio",
                "🎯 PROPÓSITO: Fragata ligera defensiva antimisil (AMM).\n⚓ DOCTRINA: Lanza pequeñas salvas de interceptores AMM para neutralizar misiles atacantes a 10Mkm.\n📊 EXPECTATIVAS: Lanzadores tamaño 1 de alta cadencia, radares de resolución 1 y costo reducido.\n📏 MAGNITUD EN PUERTO: 155 metros de eslora con radar de alta frecuencia en la superestructura central.");

            AddPreset(ref idx, "🚢 ML Trapdoor - Crucero Lanzaminas Espacial (12,000 t)", "7. Naves de Misiles y Asedio",
                "🎯 PROPÓSITO: Crucero sembrador de campos de minas espaciales (12,000t).\n⚓ DOCTRINA: Bloquea puntos de salto defensivos desplegando campos de minas autónomas invisibles.\n📊 EXPECTATIVAS: Pañol de minas espaciales de alta capacidad y sistemas de sembrado sigiloso.\n📏 MAGNITUD EN PUERTO: 210 metros de eslora con escotillas de eyectores de minas situadas en la sección de popa.");

            AddPreset(ref idx, "🚀 CG Universal - Crucero VLS Multipropósito (15,000 t)", "7. Naves de Misiles y Asedio",
                "🎯 PROPÓSITO: Crucero de lanzamiento vertical VLS multipropósito.\n⚓ DOCTRINA: Capaz de alternar en sus pañoles misiles ASM antibuque, AMM defensivos y sondas de reconocimiento.\n📊 EXPECTATIVAS: Versatilidad táctica total, velocidad crucero de 4,500 km/s.\n📏 MAGNITUD EN PUERTO: 235 metros de eslora con celdas de lanzamiento VLS alineadas a lo largo de toda su cubierta superior.");

            AddPreset(ref idx, "👻 SSG Shadow - Submarino Espacial Sigiloso de Misiles (8,000 t)", "7. Naves de Misiles y Asedio",
                "🎯 PROPÓSITO: Submarino espacial sigiloso de ataque relámpago (8,000t).\n⚓ DOCTRINA: Se infiltra tras las líneas enemigas con firmas térmicas/EM reducidas para emboscar transportes.\n📊 EXPECTATIVAS: 80% de reducción de firma por recubrimientos anti-ir y lanzadores Box Launchers.\n📏 MAGNITUD EN PUERTO: 175 metros de superficie negra mate absorvente de radar sin aristas reflejantes.");

            // 8. Combate Compacto y Corbetas
            AddPreset(ref idx, "⚡ FAC Strikefast - Lancha Torpedera Rápida (1,000 t)", "8. Combate Compacto y Corbetas",
                "🎯 PROPÓSITO: Lancha de ataque rápido de torpedos (FAC - 1,000t).\n⚓ DOCTRINA: Ataque en wolfpack de corta distancia contra buques de carga o naves descolgadas de la flota enemiga.\n📊 EXPECTATIVAS: Velocidad extrema (7,000 km/s), tamaño compacto para hangares de nodrizas.\n📏 MAGNITUD EN PUERTO: 72 metros de eslora (mismo tamaño que un avión comercial Boeing 747 Jumbo).");

            AddPreset(ref idx, "💥 Gunboat Warp Hammer - Cañonera Mesón de Salto (1,200 t)", "8. Combate Compacto y Corbetas",
                "🎯 PROPÓSITO: Cañonera rápida con cañón Mesón ignorador de blindaje (1,200t).\n⚓ DOCTRINA: Ignora escudos y armaduras atacando directamente los componentes internos de naves pesadas.\n📊 EXPECTATIVAS: Daño directo a sistemas internos a corta distancia.\n📏 MAGNITUD EN PUERTO: 78 metros de eslora construidos alrededor del cañón emisor de partículas mesónicas.");

            AddPreset(ref idx, "🛡️ Corvette Gargoyle - Corbeta Escolta Gauss (2,500 t)", "8. Combate Compacto y Corbetas",
                "🎯 PROPÓSITO: Corbeta de escolta armada con torreta Gauss (2,500t).\n⚓ DOCTRINA: Escolta ligera de convoyes mercantes contra cazas piratas o misiles solitarios.\n📊 EXPECTATIVAS: Cobertura de punto cercana, costo económico de construcción.\n📏 MAGNITUD EN PUERTO: 110 metros de longitud (más largo que un campo de fútbol) armados con ametralladoras electromagnéticas.");

            AddPreset(ref idx, "👮 Corvette Watchman - Corbeta Patrullera de Frontera (3,000 t)", "8. Combate Compacto y Corbetas",
                "🎯 PROPÓSITO: Corbeta patrullera de soberanía y policía colonial (3,000t).\n⚓ DOCTRINA: Mantiene la presencia militar y el orden público en colonias lejanas reduciendo el malestar civil.\n📊 EXPECTATIVAS: Autonomía de 24 meses, mantenimiento reducido y costo de producción bajo.\n📏 MAGNITUD EN PUERTO: 120 metros de eslora equipada con camarotes ampliados para patrullas prolongadas.");

            AddPreset(ref idx, "👻 Raider Specter - Cazador Sigiloso de Emboscada (2,000 t)", "8. Combate Compacto y Corbetas",
                "🎯 PROPÓSITO: Cazador sigiloso de interdicción y emboscada (2,000t).\n⚓ DOCTRINA: Operaciones encubiertas en sectores neutrales o disputados interrumpiendo rutas comerciales.\n📊 EXPECTATIVAS: Motores de bajo calor, sensores pasivos y cañón de energía liviano.\n📏 MAGNITUD EN PUERTO: 100 metros de eslora estilizada de color carbón táctico.");

            // 9. Formaciones Terrestres
            AddPreset(ref idx, "🪖 Marine Strike Battalion - Asalto Planetario Helitransportado", "9. Formaciones Terrestres",
                "🎯 PROPÓSITO: Batallón de infantería marina de asalto orbital.\n⚓ DOCTRINA: Fuerza de choque helitransportada para asegurar cabezas de playa en mundos enemigos hostiles.\n📊 EXPECTATIVAS: Alta movilidad táctica, armamento ligero de perforación y moral elevada.\n📏 MAGNITUD EN PUERTO: Despliegue terrestre compuesto por 500 hombres y vehículos blindados de desembarco.");

            AddPreset(ref idx, "🏛️ Garrison Battalion - Infantería de Guarnición y Policía", "9. Formaciones Terrestres",
                "🎯 PROPÓSITO: Batallón de guarnición, pacificación y policía militar.\n⚓ DOCTRINA: Mantiene la seguridad interna de colonias recién tomadas o en riesgo de rebelión civil.\n📊 EXPECTATIVAS: Bajo costo de mantenimiento y alta efectividad de pacificación.\n📏 MAGNITUD EN PUERTO: Formación de 800 efectivos terrestres con equipamiento antimotines y fortificaciones portátiles.");

            AddPreset(ref idx, "🚜 Heavy Armor Regiment - Regimiento Blindado de Ruptura", "9. Formaciones Terrestres",
                "🎯 PROPÓSITO: Regimiento de tanques pesados y vehículos blindados de ruptura.\n⚓ DOCTRINA: Destruye fortificaciones defensivas enemigas en combate terrestre de alta intensidad.\n📊 EXPECTATIVAS: Blindaje pesado terrestre, potencia de fuego de fuego directo.\n📏 MAGNITUD EN PUERTO: Columna de 120 tanques de combate pesados con cañones de masa gravitacional.");

            AddPreset(ref idx, "💣 Bombardment Artillery Element - Batería Artillería Pesada", "9. Formaciones Terrestres",
                "🎯 PROPÓSITO: Elemento de artillería pesada de apoyo de fuego de campamento.\n⚓ DOCTRINA: Proporciona cobertura artillera de largo alcance a unidades de infantería en combate.\n📊 EXPECTATIVAS: Daño de área y destrucción de fortificaciones sin contacto directo.\n📏 MAGNITUD EN PUERTO: 40 obuses móviles de calibres pesados capaces de pulverizar búnkeres alienígenas.");

            AddPreset(ref idx, "🛡️ Air & Orbital Defense Battery - Batería Antiaérea / Antidroga", "9. Formaciones Terrestres",
                "🎯 PROPÓSITO: Batería terrestre de defensa contra aeronaves y bombardeos orbitales.\n⚓ DOCTRINA: Protege centros industriales y tropas terrestres contra ataques aéreos enemigos.\n📊 EXPECTATIVAS: Sensores de seguimiento aéreo y cañones automáticos integrados.\n📏 MAGNITUD EN PUERTO: Batería fija de radares y cañones antiaéreos de tiro rápido.");

            // 10. Logística Modular
            AddPreset(ref idx, "📦 Freighter Atlas Heavy - Carguero Modular Pesado (50,000 t)", "10. Logística Modular",
                "🎯 PROPÓSITO: Carguero comercial pesado modular (50,000t).\n⚓ DOCTRINA: Traslado de instalaciones industriales, minas y fábricas entre planetas imperiales.\n📊 EXPECTATIVAS: Capacidad para 25,000t de carga por viaje, diseño comercial robusto y económico.\n📏 MAGNITUD EN PUERTO: Con 400 metros de eslora (más grande que el Empire State Building), sus bodegas pueden transportar una fábrica automatizada entera por viaje.");

            AddPreset(ref idx, "🪖 Troop Transport Colossus - Transporte Tropas Invasión (30,000 t)", "10. Logística Modular",
                "🎯 PROPÓSITO: Transporte masivo de tropas de desembarco (30,000t).\n⚓ DOCTRINA: Transporta regimientos blindados y divisiones de infantería para invasiones planetarias.\n📊 EXPECTATIVAS: Capacidad para 10,000t de fuerzas terrestres y cápsulas de desembarco atmosférico.\n📏 MAGNITUD EN PUERTO: 320 metros de largo equipados con bahías de desembarco de cápsulas orbitales rápidas.");

            AddPreset(ref idx, "⛏️ Mining Ship Titan Core - Minero Espacial de Asteroides (60,000 t)", "10. Logística Modular",
                "🎯 PROPÓSITO: Minero espacial autónomo de asteroides (60,000t).\n⚓ DOCTRINA: Extracción de mineral en cuerpos celestes sin atmósfera ni infraestructura colonial.\n📊 EXPECTATIVAS: Minas automatizadas integradas y bodegas de almacenamiento de minerales.\n📏 MAGNITUD EN PUERTO: 430 metros de eslora dominados por cortadores de plasma industrial y trituradoras de mineral.");

            AddPreset(ref idx, "🚜 Tugboat Hercules - Remolcador Espacial Despliegue Rápido (20,000 t)", "10. Logística Modular",
                "🎯 PROPÓSITO: Remolcador espacial militar/comercial (20,000t).\n⚓ DOCTRINA: Remolca estaciones orbitales, astilleros portátiles o naves averiadas sin motor.\n📊 EXPECTATIVAS: Rayos de remolque de alta potencia y propulsores de gran empuje.\n📏 MAGNITUD EN PUERTO: 270 metros de casco reforzado optimizado para la tracción de masas gigantescas.");

            AddPreset(ref idx, "🛠️ Maint Vessel Hephaestus - Taller Orbital y Dique Móvil (50,000 t)", "10. Logística Modular",
                "🎯 PROPÓSITO: Taller orbital y dique seco móvil de mantenimiento (50,000t).\n⚓ DOCTRINA: Estacionado en bases de avanzada para reparar y mantener buques militares sin astillero.\n📊 EXPECTATIVAS: Módulos de mantenimiento naval masivos y pañoles MSP de gran capacidad.\n📏 MAGNITUD EN PUERTO: 400 metros de eslora albergando talleres de fundición y brazos robóticos de reparación naval.");

            // User-saved custom presets
            LoadUserSavedPresetsIntoList(ref idx);

            FilterPresetsByCategory();
        }

        private void LoadUserSavedPresetsIntoList(ref int idx)
        {
            var userPresets = UserPresetService.LoadUserPresets();
            foreach (var up in userPresets)
            {
                _allPresetsList.Add(new PresetItem
                {
                    Index = idx++,
                    Title = $"💾 {up.PresetName}",
                    Category = "💾 Diseños del Usuario",
                    IsUserPreset = true,
                    TacticalDescription = string.IsNullOrWhiteSpace(up.TacticalDescription) ?
                        $"🎯 PROPÓSITO: Preset personalizado creado por el usuario ({up.ClassName}).\n⚓ DOCTRINA: Diseñado y configurado según doctrina personalizada del Comando Imperial.\n📊 EXPECTATIVAS: Configuración a medida con componentes locales." : up.TacticalDescription,
                    UserData = up
                });
            }
        }

        private void AddPreset(ref int index, string title, string category, string tacticalDescription = "")
        {
            _allPresetsList.Add(new PresetItem { Index = index++, Title = title, Category = category, TacticalDescription = tacticalDescription });
        }

        private void CmbPresetCategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterPresetsByCategory();
        }

        private void FilterPresetsByCategory()
        {
            if (CmbPresets == null || CmbPresetCategoryFilter == null) return;
            string selectedCat = CmbPresetCategoryFilter.SelectedItem?.ToString() ?? "📂 Todas las Categorías";

            List<PresetItem> filtered;
            if (CmbPresetCategoryFilter.SelectedIndex <= 0 || selectedCat.Contains("Todas las Categorías"))
            {
                filtered = new List<PresetItem>(_allPresetsList);
            }
            else
            {
                filtered = _allPresetsList.Where(p => p.Category.Equals(selectedCat, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            CmbPresets.ItemsSource = filtered;
            if (filtered.Count > 0)
            {
                CmbPresets.SelectedIndex = 0;
            }
        }

        public void LoadEmpireData(DatabaseService? dbService, int raceId)
        {
            _dbService = dbService;
            if (_dbService == null) return;

            if (TxtDbPath != null) TxtDbPath.Text = _dbService.DbPath;
            var empires = _dbService.GetEmpires();
            if (CmbEmpire != null)
            {
                CmbEmpire.ItemsSource = empires;
                var matchEmp = empires.FirstOrDefault(e => e.RaceID == raceId) ?? empires.FirstOrDefault();
                if (matchEmp != null)
                {
                    CmbEmpire.SelectedItem = matchEmp;
                }
            }

            bool onlyResearched = (CmbPaletteMode?.SelectedIndex ?? 0) == 0;
            LoadComponents(onlyResearched);
        }

        public void SetSelectedEmpire(Empire emp)
        {
            if (emp == null) return;
            if (CmbEmpire != null && CmbEmpire.ItemsSource != null)
            {
                foreach (Empire item in CmbEmpire.Items)
                {
                    if (item.RaceID == emp.RaceID)
                    {
                        CmbEmpire.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        public void LoadShipDesignIntoDesigner(ShipDesign design, string? tacticalDescription = null)
        {
            if (design == null) return;

            if (TxtClassName != null) TxtClassName.Text = string.IsNullOrWhiteSpace(design.ClassName) ? "Clase Importada" : design.ClassName;
            if (TxtArmorThickness != null) TxtArmorThickness.Text = Math.Max(1, design.ArmorThickness).ToString();
            if (TxtArmorWidth != null) TxtArmorWidth.Text = Math.Max(1, design.ArmorWidth).ToString();
            if (TxtDeploymentMonths != null) TxtDeploymentMonths.Text = Math.Max(1, design.PlannedDeploymentMonths).ToString();

            _selectedComponents.Clear();
            if (design.Components != null)
            {
                foreach (var item in design.Components)
                {
                    var comp = item.Component ?? new Component { ComponentName = item.ComponentName, TypeName = item.TypeName, ComponentSize = item.TotalHS, Cost = item.TotalCost };
                    _selectedComponents.Add(new SelectedComponentItem
                    {
                        Component = comp,
                        Quantity = Math.Max(1, item.Quantity)
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(tacticalDescription))
            {
                ParseTacticalDossier(tacticalDescription);
            }
            else
            {
                ParseTacticalDossier($"🎯 PROPÓSITO: Plano cargado directamente al Diseñador de Naves desde el juego / auditoría imperial.\n⚓ DOCTRINA: Inspección técnica y calibración en vivo de la clase {design.ClassName}.\n📊 EXPECTATIVAS: Componentes, masa y especificaciones importados con éxito.");
            }

            Recalculate();
        }

        private void InitializeDatabase(string path)
        {
            if (string.IsNullOrEmpty(path) || !System.IO.File.Exists(path)) return;
            _dbService = new DatabaseService(path);
            if (_dbService.TestConnection(out _))
            {
                var empires = _dbService.GetEmpires();
                CmbEmpire.ItemsSource = empires;
                if (empires.Count > 0)
                {
                    CmbEmpire.SelectedIndex = 0;
                }
                else
                {
                    LoadFallbackComponents();
                }
            }
            else
            {
                LoadFallbackComponents();
            }
        }

        private void CmbPaletteMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbPaletteMode == null || _dbService == null) return;
            bool onlyResearched = CmbPaletteMode.SelectedIndex == 0;
            LoadComponents(onlyResearched);
        }

        private void LoadComponents(bool onlyResearched)
        {
            if (_dbService == null) return;
            int raceId = SelectedRaceID;
            var comps = _dbService.GetResearchedComponents(raceId, onlyResearched);
            _allComponents.Clear();
            foreach (var c in comps) _allComponents.Add(c);
            FilterComponents();
        }

        private void LoadFallbackComponents()
        {
            _allComponents.Clear();
            var fallback = _dbService?.GetDefaultFallbackComponents() ?? new List<Component>();
            foreach (var c in fallback) _allComponents.Add(c);
            FilterComponents();
            PopulateInitialBlueprint();
        }

        private void PopulateInitialBlueprint()
        {
            _selectedComponents.Clear();
            var eng = _allComponents.FirstOrDefault(c => c.TypeName == "Engine") ?? _allComponents.FirstOrDefault();
            var fuel = _allComponents.FirstOrDefault(c => c.TypeName == "Fuel");
            var hab = _allComponents.FirstOrDefault(c => c.TypeName == "Habitation");
            var maint = _allComponents.FirstOrDefault(c => c.TypeName == "Maintenance");

            if (eng != null) _selectedComponents.Add(new SelectedComponentItem { Component = eng, Quantity = 2 });
            if (fuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = fuel, Quantity = 4 });
            if (maint != null) _selectedComponents.Add(new SelectedComponentItem { Component = maint, Quantity = 1 });

            AutoBalanceHabitationAndMaintenance();
            Recalculate();
        }

        private void CmbPresets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbPresets.SelectedItem is not PresetItem preset) return;

            _selectedComponents.Clear();

            // Populate Tactical Description into 4 Dossier Block Cards & Status Tags
            ParseTacticalDossier(preset.TacticalDescription);

            if (TxtPresetTypeTag != null && BdrPresetTypeTag != null)
            {
                if (preset.IsUserPreset)
                {
                    TxtPresetTypeTag.Text = "💾 Preset Personalizado del Usuario";
                    TxtPresetTypeTag.Foreground = (Brush)Application.Current.Resources["AccentGreenBrush"];
                    BdrPresetTypeTag.BorderBrush = (Brush)Application.Current.Resources["AccentGreenBrush"];
                }
                else
                {
                    TxtPresetTypeTag.Text = "📌 Preset Oficial Marina Imperial";
                    TxtPresetTypeTag.Foreground = (Brush)Application.Current.Resources["AccentCyanBrush"];
                    BdrPresetTypeTag.BorderBrush = (Brush)Application.Current.Resources["AccentCyanBrush"];
                }
            }

            if (preset.IsUserPreset && preset.UserData != null)
            {
                LoadUserPresetData(preset.UserData);
                return;
            }

            // Robust Component Provider Helpers
            Component GetEngine(bool isCommercial, double size = 10) =>
                _allComponents.FirstOrDefault(c => c.TypeName == "Engine" && (isCommercial ? c.ComponentName.ToLower().Contains("commercial") : !c.ComponentName.ToLower().Contains("commercial"))) ??
                _allComponents.FirstOrDefault(c => c.TypeName == "Engine") ??
                new Component { ComponentID = 901, ComponentName = isCommercial ? "Commercial Nuclear Engine (HS 50)" : "Magneto-Plasma Drive (HS 10)", TypeName = "Engine", ComponentSize = isCommercial ? 50 : size, Cost = 50, EnginePower = isCommercial ? 400 : 250, Crew = 5 };

            Component GetFuel(bool isLarge = false) =>
                _allComponents.FirstOrDefault(c => c.TypeName == "Fuel" && (isLarge ? c.ComponentSize >= 5 : c.ComponentSize <= 2)) ??
                _allComponents.FirstOrDefault(c => c.TypeName == "Fuel") ??
                new Component { ComponentID = 902, ComponentName = isLarge ? "Large Fuel Tank (250k Liters)" : "Standard Fuel Tank (50k Liters)", TypeName = "Fuel", ComponentSize = isLarge ? 5 : 1, Cost = 5, FuelCapacity = isLarge ? 250000 : 50000 };

            Component GetLaser() =>
                _allComponents.FirstOrDefault(c => c.TypeName.Contains("Beam") || c.TypeName.Contains("Weapon") || c.TypeName.Contains("Laser") || c.ComponentName.ToLower().Contains("laser")) ??
                new Component { ComponentID = 903, ComponentName = "15cm C3 Near-Ultraviolet Laser", TypeName = "Beam Weapon", ComponentSize = 4, Cost = 32, Crew = 8 };

            Component GetSensor(bool isPassive = false) =>
                _allComponents.FirstOrDefault(c => isPassive ? (c.TypeName.Contains("Passive") || c.ComponentName.ToLower().Contains("thermal") || c.ComponentName.ToLower().Contains("em")) : (c.TypeName.Contains("Active") || c.TypeName.Contains("Sensor"))) ??
                _allComponents.FirstOrDefault(c => c.TypeName.Contains("Sensor")) ??
                new Component { ComponentID = 904, ComponentName = isPassive ? "Thermal Sensor Array (TH-10)" : "Active Search Sensor Res-20 (50M km)", TypeName = isPassive ? "Passive Sensor" : "Active Sensor", ComponentSize = isPassive ? 2 : 5, Cost = isPassive ? 20 : 45, ActiveSensor = isPassive ? 0 : 50, PassiveSensor = isPassive ? 10 : 0, Crew = 2 };

            Component GetShield() =>
                _allComponents.FirstOrDefault(c => c.TypeName.Contains("Shield") || c.ComponentName.ToLower().Contains("shield")) ??
                new Component { ComponentID = 905, ComponentName = "Alpha Shield Generator", TypeName = "Shield", ComponentSize = 2, Cost = 15, ShieldStrength = 6, Crew = 2 };

            Component GetJump()
            {
                var j = _allComponents.FirstOrDefault(c => c.TypeName == "Jump Drive" || c.ComponentName.ToLower().Contains("jump")) ??
                        new Component { ComponentID = 906, ComponentName = "Military Jump Drive (Max 10,000 Tons)", TypeName = "Jump Drive", ComponentSize = 10, Cost = 150, JumpRating = 3, JumpMaxHS = 500, Crew = 8 };
                if (j.JumpMaxHS <= 0) j.JumpMaxHS = Math.Max((int)(j.ComponentSize * 150), 2000);
                return j;
            }

            Component GetMagazine() =>
                _allComponents.FirstOrDefault(c => c.TypeName.Contains("Magazine") || c.TypeName.Contains("Launcher") || c.ComponentName.ToLower().Contains("missile")) ??
                new Component { ComponentID = 907, ComponentName = "Size 6 Missile Magazine (Capacity 120)", TypeName = "Magazine", ComponentSize = 6, Cost = 24, MissileCapacity = 120, Crew = 3 };

            string title = preset.Title;
            int spaceIdx = title.IndexOf(' ');
            TxtClassName.Text = spaceIdx >= 0 ? title.Substring(spaceIdx + 1).Trim() : title;

            // Preset configuration logic based on title keywords or category
            if (title.Contains("DDG Artemis") || title.Contains("Artemis"))
            {
                TxtArmorThickness.Text = "4"; TxtArmorWidth.Text = "12";
                _selectedComponents.Add(new SelectedComponentItem { Component = GetEngine(false), Quantity = 4 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetFuel(true), Quantity = 6 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetMagazine(), Quantity = 3 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetSensor(false), Quantity = 1 });
            }
            else if (title.Contains("DD-PD Aegis") || title.Contains("Aegis-G"))
            {
                TxtArmorThickness.Text = "4"; TxtArmorWidth.Text = "10";
                _selectedComponents.Add(new SelectedComponentItem { Component = GetEngine(false), Quantity = 3 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetFuel(true), Quantity = 5 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetLaser(), Quantity = 4 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetSensor(false), Quantity = 1 });
            }
            else if (title.Contains("CA Vindicator") || title.Contains("Vindicator") || title.Contains("Crucero Pesado"))
            {
                TxtArmorThickness.Text = "6"; TxtArmorWidth.Text = "16";
                _selectedComponents.Add(new SelectedComponentItem { Component = GetEngine(false), Quantity = 6 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetFuel(true), Quantity = 10 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetLaser(), Quantity = 4 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetShield(), Quantity = 2 });
            }
            else if (title.Contains("CV Valhalla") || title.Contains("Valhalla") || title.Contains("Superportaaviones"))
            {
                TxtArmorThickness.Text = "6"; TxtArmorWidth.Text = "24";
                _selectedComponents.Add(new SelectedComponentItem { Component = GetEngine(true), Quantity = 8 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetFuel(true), Quantity = 16 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetShield(), Quantity = 4 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetSensor(false), Quantity = 2 });
            }
            else if (title.Contains("F-01 Hornet") || title.Contains("Hornet") || title.Contains("Caza"))
            {
                TxtArmorThickness.Text = "1"; TxtArmorWidth.Text = "4";
                _selectedComponents.Add(new SelectedComponentItem { Component = GetEngine(false, 2), Quantity = 1 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetFuel(false), Quantity = 1 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetLaser(), Quantity = 1 });
            }
            else if (title.Contains("ES-Grav Compass") || title.Contains("Explorador Gravitacional"))
            {
                TxtArmorThickness.Text = "2"; TxtArmorWidth.Text = "8";
                _selectedComponents.Add(new SelectedComponentItem { Component = GetEngine(false), Quantity = 2 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetFuel(true), Quantity = 4 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetJump(), Quantity = 1 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetSensor(true), Quantity = 1 });
            }
            else if (title.Contains("ES-Geo Prospector") || title.Contains("Explorador Geológico"))
            {
                TxtArmorThickness.Text = "2"; TxtArmorWidth.Text = "8";
                _selectedComponents.Add(new SelectedComponentItem { Component = GetEngine(false), Quantity = 2 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetFuel(true), Quantity = 4 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetSensor(true), Quantity = 2 });
            }
            else if (title.Contains("GSV Pathfinder") || title.Contains("Pathfinder"))
            {
                TxtArmorThickness.Text = "3"; TxtArmorWidth.Text = "10";
                _selectedComponents.Add(new SelectedComponentItem { Component = GetEngine(false), Quantity = 3 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetFuel(true), Quantity = 6 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetJump(), Quantity = 1 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetSensor(false), Quantity = 1 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetSensor(true), Quantity = 1 });
            }
            else if (title.Contains("Probe") || title.Contains("Lancha Científica"))
            {
                TxtArmorThickness.Text = "1"; TxtArmorWidth.Text = "4";
                _selectedComponents.Add(new SelectedComponentItem { Component = GetEngine(false, 2), Quantity = 1 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetFuel(false), Quantity = 2 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetSensor(true), Quantity = 1 });
            }
            else if (title.Contains("SV-Scout Sentinel") || title.Contains("Sentinel"))
            {
                TxtArmorThickness.Text = "2"; TxtArmorWidth.Text = "8";
                _selectedComponents.Add(new SelectedComponentItem { Component = GetEngine(false), Quantity = 2 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetFuel(true), Quantity = 4 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetSensor(false), Quantity = 2 });
            }
            else if (title.Contains("AOF Endurance") || title.Contains("Prometheus") || title.Contains("Petrolero") || title.Contains("Supertanquero") || title.Contains("Cosechadora") || title.Contains("Harvester"))
            {
                TxtArmorThickness.Text = "2"; TxtArmorWidth.Text = "14";
                _selectedComponents.Add(new SelectedComponentItem { Component = GetEngine(true), Quantity = 6 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetFuel(true), Quantity = 16 });
            }
            else if (title.Contains("BBG Nemesis") || title.Contains("FFG-AMM") || title.Contains("Lanzamisiles") || title.Contains("Asedio"))
            {
                TxtArmorThickness.Text = "5"; TxtArmorWidth.Text = "14";
                _selectedComponents.Add(new SelectedComponentItem { Component = GetEngine(false), Quantity = 4 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetFuel(true), Quantity = 8 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetMagazine(), Quantity = 4 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetSensor(false), Quantity = 1 });
            }
            else if (title.Contains("FAC Strikefast") || title.Contains("Corvette") || title.Contains("Gunboat") || title.Contains("Raider"))
            {
                TxtArmorThickness.Text = "2"; TxtArmorWidth.Text = "6";
                _selectedComponents.Add(new SelectedComponentItem { Component = GetEngine(false), Quantity = 2 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetFuel(false), Quantity = 3 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetLaser(), Quantity = 2 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetSensor(false), Quantity = 1 });
            }
            else if (title.Contains("Freighter Atlas") || title.Contains("Troop Transport") || title.Contains("Mining Ship") || title.Contains("Tugboat") || title.Contains("Maint Vessel") || title.Contains("Hephaestus") || title.Contains("Carguero"))
            {
                TxtArmorThickness.Text = "2"; TxtArmorWidth.Text = "12";
                _selectedComponents.Add(new SelectedComponentItem { Component = GetEngine(true), Quantity = 4 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetFuel(true), Quantity = 10 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetSensor(false), Quantity = 1 });
            }
            else
            {
                // General Fallback for any other preset title
                TxtArmorThickness.Text = "2"; TxtArmorWidth.Text = "10";
                _selectedComponents.Add(new SelectedComponentItem { Component = GetEngine(true), Quantity = 2 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetFuel(true), Quantity = 4 });
                _selectedComponents.Add(new SelectedComponentItem { Component = GetSensor(false), Quantity = 1 });
            }

            // GUARANTEED ZERO-WARNING BALANCE CALCULATOR
            AutoBalanceHabitationAndMaintenance();
            Recalculate();
        }

        private void LoadUserPresetData(UserPresetData ud)
        {
            TxtClassName.Text = ud.ClassName;
            TxtDeploymentMonths.Text = ud.PlannedDeploymentMonths.ToString();
            TxtArmorThickness.Text = ud.ArmorThickness.ToString();
            TxtArmorWidth.Text = ud.ArmorWidth.ToString();

            ParseTacticalDossier(ud.TacticalDescription);

            _selectedComponents.Clear();
            foreach (var item in ud.Components)
            {
                var comp = _allComponents.FirstOrDefault(x => x.ComponentID == item.ComponentID) ??
                           _allComponents.FirstOrDefault(x => x.ComponentName.Equals(item.ComponentName, StringComparison.OrdinalIgnoreCase));

                if (comp != null)
                {
                    _selectedComponents.Add(new SelectedComponentItem { Component = comp, Quantity = item.Quantity });
                }
            }

            AutoBalanceHabitationAndMaintenance();
            Recalculate();
        }

        private void BtnSaveUserPreset_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedComponents.Count == 0)
            {
                MessageBox.Show("Por favor añade componentes al plano antes de guardar como preset.", "Plano Vacío", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string presetName = TxtClassName.Text?.Trim() ?? "Mi Clase Personalizada";
            if (string.IsNullOrEmpty(presetName)) presetName = "Mi Clase Personalizada";

            int.TryParse(TxtDeploymentMonths.Text, out int depM);
            int.TryParse(TxtArmorThickness.Text, out int armorT);
            int.TryParse(TxtArmorWidth.Text, out int armorW);

            string combinedTacticalDesc = CombineTacticalDossier();

            var userPreset = new UserPresetData
            {
                PresetName = presetName,
                ClassName = presetName,
                PlannedDeploymentMonths = Math.Max(1, depM),
                ArmorThickness = Math.Max(1, armorT),
                ArmorWidth = Math.Max(1, armorW),
                IsMilitary = CurrentDesign.IsMilitary,
                TacticalDescription = combinedTacticalDesc,
                Components = _selectedComponents.Select(x => new UserPresetComponentItem
                {
                    ComponentID = x.Component.ComponentID,
                    ComponentName = x.Component.ComponentName,
                    TypeName = x.Component.TypeName,
                    Quantity = x.Quantity
                }).ToList()
            };

            if (UserPresetService.SaveUserPreset(userPreset, out string msg))
            {
                MessageBox.Show(msg, "Preset del Usuario Guardado", MessageBoxButton.OK, MessageBoxImage.Information);
                InitializePresets();
                CmbPresetCategoryFilter.SelectedIndex = 11; // Select "💾 Diseños del Usuario"
            }
            else
            {
                MessageBox.Show(msg, "Error de Guardado", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AutoBalanceHabitationAndMaintenance()
        {
            var hab = _allComponents.FirstOrDefault(c => c.TypeName == "Habitation") ??
                      _allComponents.FirstOrDefault(c => c.ComponentName.ToLower().Contains("crew quarters"));

            var maint = _allComponents.FirstOrDefault(c => c.TypeName == "Maintenance") ??
                        _allComponents.FirstOrDefault(c => c.ComponentName.ToLower().Contains("engineering"));

            int totalCrewReq = 0;
            double totalHS = 0;
            foreach (var item in _selectedComponents)
            {
                totalHS += item.TotalHS;
                if (!item.Component.TypeName.Equals("Habitation", StringComparison.OrdinalIgnoreCase) && 
                    !item.Component.ComponentName.ToLower().Contains("crew quarters"))
                {
                    totalCrewReq += item.Component.Crew * item.Quantity;
                }
            }

            int habQuantityNeeded = Math.Max(1, (int)Math.Ceiling(totalCrewReq / 50.0));
            if (hab != null)
            {
                var existingHab = _selectedComponents.FirstOrDefault(x => x.Component.TypeName == "Habitation" || 
                                                                          x.Component.ComponentName.ToLower().Contains("crew quarters"));
                if (existingHab != null)
                {
                    existingHab.Quantity = habQuantityNeeded;
                }
                else
                {
                    _selectedComponents.Add(new SelectedComponentItem { Component = hab, Quantity = habQuantityNeeded });
                }
            }

            // Maintenance auto balance
            bool isMilitaryComp = _selectedComponents.Any(x => 
                x.Component.TypeName.ToLower().Contains("engine") && !x.Component.ComponentName.ToLower().Contains("commercial") ||
                x.Component.TypeName.ToLower().Contains("beam") || x.Component.TypeName.ToLower().Contains("weapon") ||
                x.Component.TypeName.ToLower().Contains("active") || x.Component.TypeName.ToLower().Contains("shield"));

            if (isMilitaryComp && maint != null)
            {
                int engineeringNeeded = Math.Max(1, (int)Math.Ceiling(totalHS / 50.0));
                var existingMaint = _selectedComponents.FirstOrDefault(x => x.Component.TypeName == "Maintenance" || 
                                                                             x.Component.ComponentName.ToLower().Contains("engineering"));
                if (existingMaint != null)
                {
                    existingMaint.Quantity = engineeringNeeded;
                }
                else
                {
                    _selectedComponents.Add(new SelectedComponentItem { Component = maint, Quantity = engineeringNeeded });
                }
            }
        }

        private void BtnBrowseDb_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Database Files (*.db)|*.db|All Files (*.*)|*.*",
                Title = "Seleccionar AuroraDB.db"
            };
            if (dlg.ShowDialog() == true)
            {
                TxtDbPath.Text = dlg.FileName;
                InitializeDatabase(dlg.FileName);
            }
        }

        private void CmbEmpire_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool onlyResearched = (CmbPaletteMode?.SelectedIndex ?? 0) == 0;
            LoadComponents(onlyResearched);
        }

        private void CmbCategoryFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterComponents();
        }

        private void TxtSearchComponent_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterComponents();
        }

        private void FilterComponents()
        {
            var query = TxtSearchComponent.Text?.Trim().ToLower() ?? string.Empty;
            int catIdx = CmbCategoryFilter?.SelectedIndex ?? 0;

            _filteredComponents.Clear();
            foreach (var c in _allComponents)
            {
                bool matchesQuery = string.IsNullOrEmpty(query) || 
                                   c.ComponentName.ToLower().Contains(query) || 
                                   c.TypeName.ToLower().Contains(query);

                bool matchesCategory = catIdx switch
                {
                    1 => c.TypeName.Equals("Engine", StringComparison.OrdinalIgnoreCase),
                    2 => c.TypeName.Equals("Fuel", StringComparison.OrdinalIgnoreCase),
                    3 => c.TypeName.Equals("Habitation", StringComparison.OrdinalIgnoreCase),
                    4 => c.TypeName.Equals("Maintenance", StringComparison.OrdinalIgnoreCase),
                    5 => c.TypeName.Contains("Sensor", StringComparison.OrdinalIgnoreCase) || c.TypeName.Contains("Active") || c.TypeName.Contains("Passive"),
                    6 => c.TypeName.Contains("Beam", StringComparison.OrdinalIgnoreCase) || c.TypeName.Contains("Weapon", StringComparison.OrdinalIgnoreCase) || c.TypeName.Contains("Laser", StringComparison.OrdinalIgnoreCase),
                    7 => c.TypeName.Contains("Magazine", StringComparison.OrdinalIgnoreCase) || c.TypeName.Contains("Launcher", StringComparison.OrdinalIgnoreCase),
                    8 => c.TypeName.Contains("Shield", StringComparison.OrdinalIgnoreCase) || c.TypeName.Contains("Armor", StringComparison.OrdinalIgnoreCase),
                    9 => c.TypeName.Contains("Jump", StringComparison.OrdinalIgnoreCase),
                    _ => true
                };

                if (matchesQuery && matchesCategory)
                {
                    _filteredComponents.Add(c);
                }
            }
        }

        private void DgComponentPalette_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DgComponentPalette.SelectedItem is Component comp)
            {
                var existing = _selectedComponents.FirstOrDefault(x => x.Component.ComponentID == comp.ComponentID);
                if (existing != null)
                {
                    existing.Quantity++;
                    DgSelectedComponents.Items.Refresh();
                }
                else
                {
                    _selectedComponents.Add(new SelectedComponentItem { Component = comp, Quantity = 1 });
                }
                Recalculate();
            }
        }

        private void BtnRemoveComponent_Click(object sender, RoutedEventArgs e)
        {
            if (DgSelectedComponents.SelectedItem is SelectedComponentItem item)
            {
                _selectedComponents.Remove(item);
                Recalculate();
            }
        }

        private void DgSelectedComponents_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(Recalculate), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void OnDesignInputChanged(object sender, RoutedEventArgs e)
        {
            Recalculate();
        }

        private void OnDesignInputChanged(object sender, TextChangedEventArgs e)
        {
            Recalculate();
        }

        private void BtnExportAurora_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedComponents.Count == 0)
            {
                MessageBox.Show("Por favor añade componentes al plano antes de exportar.", "Plano Vacío", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Recalculate();
            int raceId = SelectedRaceID;
            if (raceId <= 0 && _dbService != null)
            {
                var empires = _dbService.GetEmpires();
                raceId = empires.FirstOrDefault()?.RaceID ?? 0;
            }

            if (BlueprintExportService.ExportClassToAuroraDb(TxtDbPath.Text, CurrentDesign, raceId, out string msg))
            {
                MessageBox.Show(msg, "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(msg, "Error de Exportación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExportResearch_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedComponents.Count == 0)
            {
                MessageBox.Show("Por favor añade componentes al plano antes de exportar a I+D.", "Plano Vacío", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Recalculate();
            int raceId = SelectedRaceID;
            if (raceId <= 0 && _dbService != null)
            {
                var empires = _dbService.GetEmpires();
                raceId = empires.FirstOrDefault()?.RaceID ?? 0;
            }

            if (BlueprintExportService.ExportClassAsResearchProject(TxtDbPath.Text, CurrentDesign, raceId, out string msg))
            {
                MessageBox.Show(msg, "🔬 Proyecto de Prototipo Creado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(msg, "Error de Exportación", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCopyReport_Click(object sender, RoutedEventArgs e)
        {
            Recalculate();
            string textReport = BlueprintExportService.GenerateAuroraTextReport(CurrentDesign);
            Clipboard.SetText(textReport);
            MessageBox.Show("📋 Ficha técnica de la nave copiada al portapapeles en formato oficial de Aurora 4X.", "Copiado al Portapapeles", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnAutoBalanceCrew_Click(object sender, RoutedEventArgs e)
        {
            AutoBalanceHabitationAndMaintenance();
            Recalculate();
        }

        private void OnAutoBalanceCrewChecked(object sender, RoutedEventArgs e)
        {
            Recalculate();
        }

        private void OnAutoBalanceCrewUnchecked(object sender, RoutedEventArgs e)
        {
            Recalculate();
        }

        public void Recalculate()
        {
            if (TxtClassName == null) return;

            if (ChkAutoBalanceCrew?.IsChecked == true)
            {
                AutoBalanceHabitationAndMaintenance();
            }

            CurrentDesign.ClassName = TxtClassName?.Text ?? "Nueva Clase";
            int.TryParse(TxtDeploymentMonths?.Text, out int depMonths);
            CurrentDesign.PlannedDeploymentMonths = Math.Max(1, depMonths);

            int.TryParse(TxtArmorThickness?.Text, out int thickness);
            int.TryParse(TxtArmorWidth?.Text, out int width);
            CurrentDesign.ArmorThickness = Math.Max(1, thickness);
            CurrentDesign.ArmorWidth = Math.Max(1, width);

            CurrentDesign.Components = _selectedComponents.ToList();
            _calcEngine.RecalculateDesign(CurrentDesign);

            UpdateTelemetryDashboard();
        }

        private void UpdateTelemetryDashboard()
        {
            if (LblTonnage == null || LblSpeed == null || LblSignatures == null || 
                LblCost == null || LblCrew == null || LblFuelCap == null || 
                LblFuelCons == null || LblRangeKm == null || LblRangeAu == null || 
                LblMSP == null || LblFailureRate == null || LblMaintLife == null || 
                IcMinerals == null || IcWarnings == null || BdrValidation == null || LblValidationTitle == null)
            {
                return;
            }

            // Update Military Status Badge UI
            if (BdrMilitaryStatus != null && TxtMilitaryStatus != null)
            {
                if (CurrentDesign.IsMilitary)
                {
                    TxtMilitaryStatus.Text = "⚔️ CLASIFICACIÓN MILITAR";
                    TxtMilitaryStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 107, 107)); // Bright Coral Red
                    BdrMilitaryStatus.Background = new SolidColorBrush(Color.FromRgb(51, 26, 26)); // Dark Red Background
                    BdrMilitaryStatus.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 68, 68)); // Bright Red Border
                    ToolTipService.SetToolTip(BdrMilitaryStatus, "⚔️ CLASIFICACIÓN MILITAR: Clasificada automáticamente como Militar según reglas de Aurora 4X por contener motores militares, armas, escudos o sensores activos.");
                }
                else
                {
                    TxtMilitaryStatus.Text = "🚢 CLASIFICACIÓN COMERCIAL";
                    TxtMilitaryStatus.Foreground = new SolidColorBrush(Color.FromRgb(0, 240, 255)); // Bright Cyan
                    BdrMilitaryStatus.Background = new SolidColorBrush(Color.FromRgb(10, 36, 45)); // Dark Cyan Background
                    BdrMilitaryStatus.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 240, 255)); // Bright Cyan Border
                    ToolTipService.SetToolTip(BdrMilitaryStatus, "🚢 CLASIFICACIÓN COMERCIAL: Clasificada automáticamente como Comercial. Utiliza únicamente motores comerciales y carece de armamento o sensores militares.");
                }
            }

            // Update Visual Ship Scale Silhouette & Side-by-Side Real World Magnitude HUD
            UpdateSideBySideScaleHUD(CurrentDesign.TotalTonnage);

            LblTonnage.Text = $"{CurrentDesign.TotalTonnage:N0} Tons ({CurrentDesign.TotalHS:F1} HS)";
            LblSpeed.Text = $"{CurrentDesign.MaxSpeedKmS:N0} km/s";
            LblSignatures.Text = $"Térmica: {CurrentDesign.ThermalSignature:N0} | EM: {CurrentDesign.EMSignature:N0}";
            LblCost.Text = $"{CurrentDesign.TotalCostBP:N1} BP";
            LblCrew.Text = $"{CurrentDesign.TotalCrewRequired} / {CurrentDesign.CrewQuartersProvidedHS}";

            LblFuelCap.Text = $"{CurrentDesign.TotalFuelLiters:N0} Litros";
            LblFuelCons.Text = $"{CurrentDesign.FuelConsumptionLitersPerHour:N1} L/h";
            LblRangeKm.Text = $"{CurrentDesign.RangeBillionKm:N2} Billones km";
            LblRangeAu.Text = $"{CurrentDesign.RangeAU:F1} AU ({CurrentDesign.RangeLightYears:F3} AL)";

            LblMSP.Text = $"{CurrentDesign.TotalMSP:N0} MSP";
            LblFailureRate.Text = $"{CurrentDesign.AnnualFailureRate * 100.0:F1} %";
            LblMaintLife.Text = $"{CurrentDesign.MaintenanceLifeYears:F1} Años (MTBF: {CurrentDesign.MTBFMonths:F1} m)";

            TutorTooltipService.AttachToolTip(LblTonnage, "HS - Hull Size (Tamaño de Casco)", "🚀 TUTOR NAVAL: Desplazamiento y HS");
            TutorTooltipService.AttachToolTip(LblSpeed, "Speed", "⚡ TUTOR NAVAL: Velocidad de Navegación");
            TutorTooltipService.AttachToolTip(LblSignatures, "TCS - Thermal & Cross Section (Firma Térmica)", "🛰️ TUTOR NAVAL: Firma Térmica y EM");
            TutorTooltipService.AttachToolTip(LblMSP, "DCR - Damage Control Rating (Control de Daños)", "🛠️ TUTOR NAVAL: Repuestos MSP y Control de Daños");
            TutorTooltipService.AttachToolTip(LblFuelCap, "Sorium", "⛽ TUTOR NAVAL: Tanques de Combustible LPH");
            TutorTooltipService.AttachToolTip(LblCrew, "Population", "🏠 TUTOR NAVAL: Habitabilidad y Tripulación");

            var minList = new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>("Duranium", CurrentDesign.Minerals.Duranium),
                new KeyValuePair<string, double>("Sorium", CurrentDesign.Minerals.Sorium),
                new KeyValuePair<string, double>("Neutronium", CurrentDesign.Minerals.Neutronium),
                new KeyValuePair<string, double>("Corundium", CurrentDesign.Minerals.Corundium),
                new KeyValuePair<string, double>("Uridium", CurrentDesign.Minerals.Uridium),
                new KeyValuePair<string, double>("Gallicite", CurrentDesign.Minerals.Gallicite),
                new KeyValuePair<string, double>("Tritium", CurrentDesign.Minerals.Tritium),
                new KeyValuePair<string, double>("Boronide", CurrentDesign.Minerals.Boronide)
            }.Where(x => x.Value > 0).ToList();

            IcMinerals.ItemsSource = minList;

            bool isValid = CurrentDesign.Warnings.Count == 0;
            if (isValid)
            {
                LblValidationTitle.Text = "✅ DISEÑO DE NAVE VALIDADO";
                LblValidationTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 240, 255)); // Cyan
                BdrValidation.Background = new SolidColorBrush(Color.FromRgb(10, 36, 26)); // Glowing Dark Emerald Green
                BdrValidation.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 136)); // Bright Emerald Green
            }
            else
            {
                LblValidationTitle.Text = "⚠️ VALIDACIÓN Y ALERTAS DE DISEÑO";
                LblValidationTitle.Foreground = new SolidColorBrush(Color.FromRgb(255, 187, 51)); // Amber
                BdrValidation.Background = new SolidColorBrush(Color.FromRgb(31, 13, 13)); // Dark Red
                BdrValidation.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 68, 68)); // Bright Red
            }

            var displayList = new List<ValidationDisplayItem>();
            if (isValid)
            {
                displayList.Add(new ValidationDisplayItem { Message = "✅ Diseño de nave validado correctamente sin advertencias.", ColorHex = "#55FF55" });
                foreach (var sug in CurrentDesign.Suggestions)
                {
                    displayList.Add(new ValidationDisplayItem { Message = sug, ColorHex = "#FFFF88" });
                }
            }
            else
            {
                foreach (var warn in CurrentDesign.Warnings)
                {
                    displayList.Add(new ValidationDisplayItem { Message = warn, ColorHex = "#FF8888" });
                }
                foreach (var sug in CurrentDesign.Suggestions)
                {
                    displayList.Add(new ValidationDisplayItem { Message = sug, ColorHex = "#FFFF88" });
                }
            }

            IcWarnings.ItemsSource = displayList;

            UpdateShipyardCompatibilityCard();
        }

        private void UpdateShipyardCompatibilityCard()
        {
            if (LblShipyardMatchStatus == null || LblShipyardRetoolInfo == null) return;

            if (_dbService == null)
            {
                LblShipyardMatchStatus.Text = "⚠️ Base de datos no conectada.";
                LblShipyardRetoolInfo.Text = "Retooling: Indeterminado";
                return;
            }

            int raceId = SelectedRaceID;
            if (raceId <= 0) return;

            var shipyards = _dbService.GetShipyards(raceId);
            double tonnage = CurrentDesign.TotalTonnage;
            bool isMilitary = CurrentDesign.IsMilitary;

            int targetSyType = isMilitary ? 1 : 2; // 1 Naval, 2 Commercial
            string syTypeName = isMilitary ? "Naval" : "Comercial";

            var matchingSy = shipyards.FirstOrDefault(s => s.CapacityTons >= tonnage && s.SYType == targetSyType);
            if (matchingSy == null)
            {
                matchingSy = shipyards.FirstOrDefault(s => s.CapacityTons >= tonnage);
            }

            if (matchingSy != null)
            {
                LblShipyardMatchStatus.Text = $"✅ {matchingSy.ShipyardName}\nCapacidad: {matchingSy.CapacityTons:N0}t (Requerido: {tonnage:N0}t)";
                LblShipyardMatchStatus.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 136));

                double retoolBP = Math.Round(CurrentDesign.TotalCostBP * 0.25, 0);
                double retoolMonths = Math.Round((retoolBP / Math.Max(100.0, matchingSy.BuildSpeedBPPerYear)) * 12.0, 1);
                LblShipyardRetoolInfo.Text = $"Retooling estimado: {retoolBP:N0} BP (~{retoolMonths:F1} Meses de gradas)";
            }
            else
            {
                double maxCap = shipyards.Count > 0 ? shipyards.Max(s => s.CapacityTons) : 0;
                LblShipyardMatchStatus.Text = $"⚠️ Ningún Astillero {syTypeName} tiene suficiente capacidad.\n(Capacidad Máx: {maxCap:N0}t vs Nave: {tonnage:N0}t)";
                LblShipyardMatchStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 180, 0));

                LblShipyardRetoolInfo.Text = "Amplía la capacidad de astillero en Operaciones Astillero.";
            }
        }

        private void UpdateSideBySideScaleHUD(double tons)
        {
            if (tons <= 0) tons = 1000;
            double hs = CurrentDesign.TotalHS;
            if (hs <= 0) hs = tons / 50.0;

            // Physical Dimensions Math (Metros)
            // L = 42 * (tons / 1000)^(0.38) * 1.85
            double lengthM = Math.Round(42.0 * Math.Pow(tons / 1000.0, 0.38) * 1.85, 0);
            if (lengthM < 15) lengthM = 15;
            double widthM = Math.Round(lengthM * 0.22, 0);
            double volumeM3 = Math.Round(tons * 1.45, 0);

            if (TxtScaleDimensionsSummary != null)
            {
                TxtScaleDimensionsSummary.Text = $"Eslora: {lengthM:N0}m | Manga: {widthM:N0}m | Vol: {volumeM3:N0}m³";
            }

            // Real World Reference Object (Auto-matched by nearest physical length)
            (string objName, double refLength, double refWidth, string catName) = GetRealWorldReferenceObject(tons, lengthM);

            if (TxtShipCardHeader != null) TxtShipCardHeader.Text = $"🚀 TU NAVE: {lengthM:N0}m";
            if (TxtRealWorldHeader != null) TxtRealWorldHeader.Text = $"{objName}: {refLength:N0}m";

            if (TxtShipGaugeLabel != null) TxtShipGaugeLabel.Text = $"{lengthM:N0}m (Eslora)";
            if (TxtWorldGaugeLabel != null) TxtWorldGaugeLabel.Text = $"{refLength:N0}m";

            // Dynamic Shared Ruler Scaling (Max track width ~320px)
            double maxMetersOnRuler = Math.Max(lengthM, refLength) * 1.25;
            if (maxMetersOnRuler < 100) maxMetersOnRuler = 100;

            double trackMaxPx = 320.0;
            double shipPx = Math.Max(25, Math.Min(trackMaxPx, (lengthM / maxMetersOnRuler) * trackMaxPx));
            double worldPx = Math.Max(25, Math.Min(trackMaxPx, (refLength / maxMetersOnRuler) * trackMaxPx));

            if (BdrShipGauge != null) BdrShipGauge.Width = shipPx;
            if (BdrWorldGauge != null) BdrWorldGauge.Width = worldPx;

            // Ruler Ticks Calibration
            double t1 = Math.Round(maxMetersOnRuler * 0.25, 0);
            double t2 = Math.Round(maxMetersOnRuler * 0.50, 0);
            double t3 = Math.Round(maxMetersOnRuler * 0.75, 0);
            double t4 = Math.Round(maxMetersOnRuler, 0);

            if (TxtRulerTick1 != null) TxtRulerTick1.Text = $"{t1:F0}m";
            if (TxtRulerTick2 != null) TxtRulerTick2.Text = $"{t2:F0}m";
            if (TxtRulerTick3 != null) TxtRulerTick3.Text = $"{t3:F0}m";
            if (TxtRulerTick4 != null) TxtRulerTick4.Text = $"{t4:F0}m+";

            // Synthesis Line
            double ratio = Math.Round((lengthM / refLength) * 100.0, 0);
            string cleanObjName = objName.Replace("🏛️ ", "").Replace("🚢 ", "").Replace("🗽 ", "").Replace("✈️ ", "").Replace("🏙️ ", "").Replace("🗼 ", "").Replace("⚽ ", "").Replace("🌉 ", "");
            string shipName = TxtClassName?.Text?.Trim() ?? "Tu nave";

            if (TxtSideBySideSynthesis != null)
            {
                if (ratio >= 95 && ratio <= 105)
                {
                    TxtSideBySideSynthesis.Text = $"💡 COMPARACIÓN DIRECTA: La nave '{shipName}' ({lengthM:N0}m) tiene una longitud prácticamente idéntica a {cleanObjName} ({refLength:N0}m).";
                }
                else if (ratio < 95)
                {
                    TxtSideBySideSynthesis.Text = $"💡 COMPARACIÓN DIRECTA: La nave '{shipName}' ({lengthM:N0}m) representa el {ratio}% del tamaño de {cleanObjName} ({refLength:N0}m).";
                }
                else
                {
                    TxtSideBySideSynthesis.Text = $"💡 COMPARACIÓN DIRECTA: La nave '{shipName}' ({lengthM:N0}m) es un {ratio - 100}% más larga que {cleanObjName} ({refLength:N0}m).";
                }
            }
        }

        public class RealWorldObjectItem
        {
            public string Name { get; set; } = string.Empty;
            public double LengthM { get; set; }
            public double WidthM { get; set; }
            public string Category { get; set; } = string.Empty;
            public string DisplayTitle => LengthM > 0 ? $"{Name} ({LengthM:N0}m)" : Name;
        }

        private List<RealWorldObjectItem> _realWorldObjects = new List<RealWorldObjectItem>();

        private void InitializeRealWorldObjectsCatalog()
        {
            _realWorldObjects = new List<RealWorldObjectItem>
            {
                new RealWorldObjectItem { Name = "⭐ Auto-Seleccionar según Tamaño", LengthM = 0, WidthM = 0, Category = "Auto" },
                // Micro & Cazas (4m - 25m)
                new RealWorldObjectItem { Name = "🚗 Un Coche Utilitario", LengthM = 4, WidthM = 1.8, Category = "Vehículo Urbano" },
                new RealWorldObjectItem { Name = "🚙 Un Coche Familiar / SUV", LengthM = 5, WidthM = 2.0, Category = "Vehículo Urbano" },
                new RealWorldObjectItem { Name = "🚐 Una Furgoneta de Reparto", LengthM = 6, WidthM = 2.2, Category = "Vehículo Comercial" },
                new RealWorldObjectItem { Name = "🚚 Un Camión Pequeño de Mudanzas", LengthM = 8, WidthM = 2.5, Category = "Vehículo Pesado" },
                new RealWorldObjectItem { Name = "🚌 Un Autobús Urbano Estándar", LengthM = 12, WidthM = 2.5, Category = "Transporte Urbano" },
                new RealWorldObjectItem { Name = "🚛 Un Camión Tráiler con Remolque", LengthM = 16.5, WidthM = 2.5, Category = "Transporte Pesado" },
                new RealWorldObjectItem { Name = "🚌 Un Autobús Articulado Doble", LengthM = 18, WidthM = 2.5, Category = "Transporte Urbano" },
                new RealWorldObjectItem { Name = "🐋 Un Cachalote Marino", LengthM = 19, WidthM = 4.0, Category = "Fauna Marina" },
                new RealWorldObjectItem { Name = "✈️ Un Avión Privado / Business Jet", LengthM = 20, WidthM = 15, Category = "Aeronave Pequeña" },
                new RealWorldObjectItem { Name = "🎾 Una Pista de Tenis (Longitud)", LengthM = 23.7, WidthM = 11, Category = "Instalación Deportiva" },
                new RealWorldObjectItem { Name = "🏊 Una Piscina Semiolímpica", LengthM = 25, WidthM = 12.5, Category = "Instalación Deportiva" },

                // Lanchas / Cañoneras / Corbetas (26m - 60m)
                new RealWorldObjectItem { Name = "🏀 Una Pista de Baloncesto (Longitud)", LengthM = 28, WidthM = 15, Category = "Instalación Deportiva" },
                new RealWorldObjectItem { Name = "🦕 Un Dinosaurio Diplodocus Completo", LengthM = 30, WidthM = 4, Category = "Fauna Prehistórica" },
                new RealWorldObjectItem { Name = "🐋 Una Ballena Azul Gigante", LengthM = 30, WidthM = 5, Category = "Fauna Marina" },
                new RealWorldObjectItem { Name = "✈️ Un Avión de Pasajeros Airbus A320", LengthM = 37.5, WidthM = 34, Category = "Avión de Pasajeros" },
                new RealWorldObjectItem { Name = "✈️ Un Avión Comercial Boeing 737", LengthM = 39.5, WidthM = 35.8, Category = "Avión de Pasajeros" },
                new RealWorldObjectItem { Name = "🏊 Una Piscina Olímpica Oficial", LengthM = 50, WidthM = 25, Category = "Instalación Deportiva" },
                new RealWorldObjectItem { Name = "🗼 El Arco de Triunfo de París (Acostado)", LengthM = 50, WidthM = 45, Category = "Monumento Famoso" },
                new RealWorldObjectItem { Name = "⛪ La Torre de Pisa (Acostada)", LengthM = 57, WidthM = 15, Category = "Monumento Famoso" },

                // Fragatas / Exploradores / Destructores (61m - 150m)
                new RealWorldObjectItem { Name = "✈️ Un Avión Concorde de Pasajeros", LengthM = 61.6, WidthM = 25.6, Category = "Avión Supersónico" },
                new RealWorldObjectItem { Name = "🚩 El Ancho de un Campo de Fútbol", LengthM = 68, WidthM = 105, Category = "Estadio Deportivo" },
                new RealWorldObjectItem { Name = "🚀 El Cohete Falcon 9 (Acostado)", LengthM = 70, WidthM = 3.7, Category = "Lanzador Espacial" },
                new RealWorldObjectItem { Name = "🏛️ El Partenón de Atenas (Longitud)", LengthM = 70, WidthM = 31, Category = "Monumento Histórico" },
                new RealWorldObjectItem { Name = "✈️ Un Avión Boeing 747 Jumbo", LengthM = 70.6, WidthM = 64.4, Category = "Avión Comercial" },
                new RealWorldObjectItem { Name = "✈️ Un Avión Airbus A380 Gigante", LengthM = 72.7, WidthM = 79.8, Category = "Avión de Pasajeros" },
                new RealWorldObjectItem { Name = "🗽 La Estatua de la Libertad (Acostada)", LengthM = 93, WidthM = 40, Category = "Monumento Famoso" },
                new RealWorldObjectItem { Name = "🕰️ La Torre Big Ben (Acostada)", LengthM = 96, WidthM = 12, Category = "Monumento Histórico" },
                new RealWorldObjectItem { Name = "⚽ Un Campo de Fútbol Reglamentario", LengthM = 105, WidthM = 68, Category = "Estadio Deportivo" },
                new RealWorldObjectItem { Name = "🛸 Estación Espacial Internacional (ISS)", LengthM = 109, WidthM = 73, Category = "Estación Espacial" },
                new RealWorldObjectItem { Name = "🏈 Un Campo de Fútbol Americano", LengthM = 110, WidthM = 49, Category = "Estadio Deportivo" },
                new RealWorldObjectItem { Name = "⛪ La Catedral de Notre Dame (Longitud)", LengthM = 128, WidthM = 48, Category = "Monumento Histórico" },
                new RealWorldObjectItem { Name = "🎡 La Gran Noria London Eye (Acostada)", LengthM = 135, WidthM = 120, Category = "Atracción Famosa" },
                new RealWorldObjectItem { Name = "🏛️ La Gran Pirámide de Guiza (Altura)", LengthM = 138.8, WidthM = 230, Category = "Maravilla del Mundo" },

                // Cruceros / Batalla / Lanzamisiles (151m - 300m)
                new RealWorldObjectItem { Name = "🌉 El Puente de Vizcaya (Sección Colgante)", LengthM = 160, WidthM = 12, Category = "Gran Puente" },
                new RealWorldObjectItem { Name = "🚢 Un Ferri Transmediterránea de Pasajeros", LengthM = 170, WidthM = 26, Category = "Barco Comercial" },
                new RealWorldObjectItem { Name = "🏛️ El Coliseo Romano (De Extremo a Extremo)", LengthM = 189, WidthM = 156, Category = "Monumento Histórico" },
                new RealWorldObjectItem { Name = "🚢 Un Buque Mercante Portacontenedores Mediano", LengthM = 200, WidthM = 30, Category = "Barco de Carga" },
                new RealWorldObjectItem { Name = "🏈 Dos Campos de Fútbol Alineados", LengthM = 210, WidthM = 68, Category = "Medida Deportiva" },
                new RealWorldObjectItem { Name = "⛪ La Basílica de San Pedro en el Vaticano", LengthM = 218, WidthM = 150, Category = "Monumento Religioso" },
                new RealWorldObjectItem { Name = "🏛️ El Capitolio de los EE.UU. (Longitud)", LengthM = 229, WidthM = 106, Category = "Edificio Político" },
                new RealWorldObjectItem { Name = "🚢 El Transatlántico RMS Titanic (Eslora)", LengthM = 269, WidthM = 28, Category = "Barco Famoso" },

                // Acorazados / Superportaaviones / Cargueros (301m - 600m)
                new RealWorldObjectItem { Name = "🏈 Tres Campos de Fútbol Alineados", LengthM = 315, WidthM = 68, Category = "Medida Deportiva" },
                new RealWorldObjectItem { Name = "🗼 La Torre Eiffel de París (Acostada)", LengthM = 330, WidthM = 125, Category = "Monumento Famoso" },
                new RealWorldObjectItem { Name = "🚢 Un Portaaviones Gigante Supercarrier", LengthM = 333, WidthM = 77, Category = "Gran Buque Naval" },
                new RealWorldObjectItem { Name = "🚢 El Transatlántico Queen Mary 2", LengthM = 345, WidthM = 41, Category = "Crucero de Lujo" },
                new RealWorldObjectItem { Name = "🏭 La Presa Hoover Dam (De Extremo a Extremo)", LengthM = 379, WidthM = 200, Category = "Infraestructura Hidroeléctrica" },
                new RealWorldObjectItem { Name = "🚢 Un Petrolero Supertanquero ULCC", LengthM = 380, WidthM = 68, Category = "Super-Carguero" },
                new RealWorldObjectItem { Name = "🚢 El Portacontenedores Ever Given", LengthM = 400, WidthM = 59, Category = "Super-Carguero" },
                new RealWorldObjectItem { Name = "🏙️ El Rascacielos Empire State (Acostado)", LengthM = 443, WidthM = 130, Category = "Edificio Gigante" },
                new RealWorldObjectItem { Name = "🏙️ Las Torres Gemelas Petronas (Acostadas)", LengthM = 452, WidthM = 100, Category = "Super-Rascacielos" },
                new RealWorldObjectItem { Name = "🌉 La Sección Principal del Puente Golden Gate", LengthM = 500, WidthM = 30, Category = "Gran Puente" },
                new RealWorldObjectItem { Name = "🏙️ El Rascacielos Taipei 101 (Acostado)", LengthM = 508, WidthM = 100, Category = "Super-Rascacielos" },
                new RealWorldObjectItem { Name = "🏈 Cinco Campos de Fútbol Alineados", LengthM = 525, WidthM = 68, Category = "Medida Deportiva" },

                // Súper-Naves / Estaciones / Diques DREADNOUGHT (601m - 4,000m+)
                new RealWorldObjectItem { Name = "🏙️ El Rascacielos Burj Khalifa de Dubái (Acostado)", LengthM = 828, WidthM = 180, Category = "Torre Gigante" },
                new RealWorldObjectItem { Name = "🏈 Ocho Campos de Fútbol Alineados", LengthM = 840, WidthM = 68, Category = "Medida Deportiva" },
                new RealWorldObjectItem { Name = "📏 Un Kilómetro Completo de Eslora", LengthM = 1000, WidthM = 200, Category = "Medida Estándar" },
                new RealWorldObjectItem { Name = "🌉 El Puente de Brooklyn Completo", LengthM = 1825, WidthM = 26, Category = "Gran Puente" },
                new RealWorldObjectItem { Name = "🏁 Dos Kilómetros de Pista / Circuito", LengthM = 2000, WidthM = 400, Category = "Medida Estándar" },
                new RealWorldObjectItem { Name = "🏔️ El Monolito del Monte Uluru / Ayers Rock", LengthM = 3600, WidthM = 2400, Category = "Monolito Geológico Gigante" },
                new RealWorldObjectItem { Name = "🌉 El Puente Akashi Kaikyō Completo", LengthM = 3911, WidthM = 35, Category = "Puente Colgante Gigante" }
            };

            if (CmbRealWorldReference != null)
            {
                CmbRealWorldReference.ItemsSource = _realWorldObjects;
                CmbRealWorldReference.DisplayMemberPath = "DisplayTitle";
                CmbRealWorldReference.SelectedIndex = 0;
            }
        }

        private void CmbRealWorldReference_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSideBySideScaleHUD(CurrentDesign.TotalTonnage);
        }

        private (string Name, double LengthM, double WidthM, string Category) GetRealWorldReferenceObject(double tonnage, double shipLengthM)
        {
            if (CmbRealWorldReference?.SelectedItem is RealWorldObjectItem selectedObj && selectedObj.LengthM > 0)
            {
                return (selectedObj.Name, selectedObj.LengthM, selectedObj.WidthM, selectedObj.Category);
            }

            // Automatic Nearest Length Match from the 60+ Catalog!
            if (_realWorldObjects != null && _realWorldObjects.Count > 1)
            {
                var closest = _realWorldObjects
                    .Where(o => o.LengthM > 0)
                    .OrderBy(o => Math.Abs(o.LengthM - shipLengthM))
                    .FirstOrDefault();

                if (closest != null)
                {
                    return (closest.Name, closest.LengthM, closest.WidthM, closest.Category);
                }
            }

            return ("🏛️ El Coliseo Romano", 189, 156, "Monumento Histórico");
        }

        private void ParseTacticalDossier(string fullText)
        {
            if (string.IsNullOrWhiteSpace(fullText))
            {
                if (TxtDossierPurpose != null) TxtDossierPurpose.Text = "Escribe aquí el propósito y función de diseño de esta nave...";
                if (TxtDossierDoctrine != null) TxtDossierDoctrine.Text = "Describe aquí la doctrina operativa y rol táctico en flota...";
                if (TxtDossierExpectations != null) TxtDossierExpectations.Text = "Indica expectativas de combate, armamento y límites...";
                if (TxtDossierScaleProfile != null) TxtDossierScaleProfile.Text = "Describe la presencia física de la nave en puerto espacial...";
                return;
            }

            string purpose = string.Empty;
            string doctrine = string.Empty;
            string expectations = string.Empty;
            string scaleProfile = string.Empty;

            string[] lines = fullText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                string trimmed = line.Trim();
                if (trimmed.Contains("🎯 PROPÓSITO:"))
                    purpose = trimmed.Replace("🎯 PROPÓSITO:", "").Trim();
                else if (trimmed.Contains("⚓ DOCTRINA:"))
                    doctrine = trimmed.Replace("⚓ DOCTRINA:", "").Trim();
                else if (trimmed.Contains("📊 EXPECTATIVAS:"))
                    expectations = trimmed.Replace("📊 EXPECTATIVAS:", "").Trim();
                else if (trimmed.Contains("📏 MAGNITUD EN PUERTO:") || trimmed.Contains("📏 MAGNITUD:"))
                    scaleProfile = trimmed.Replace("📏 MAGNITUD EN PUERTO:", "").Replace("📏 MAGNITUD:", "").Trim();
                else
                {
                    if (string.IsNullOrEmpty(purpose)) purpose += " " + trimmed;
                    else if (string.IsNullOrEmpty(doctrine)) doctrine += " " + trimmed;
                    else expectations += " " + trimmed;
                }
            }

            if (TxtDossierPurpose != null) TxtDossierPurpose.Text = string.IsNullOrWhiteSpace(purpose) ? "Diseño multi-rol del Comando Imperial." : purpose.Trim();
            if (TxtDossierDoctrine != null) TxtDossierDoctrine.Text = string.IsNullOrWhiteSpace(doctrine) ? "Despliegue estándar en grupo de batalla." : doctrine.Trim();
            if (TxtDossierExpectations != null) TxtDossierExpectations.Text = string.IsNullOrWhiteSpace(expectations) ? "Operación continua según parámetros de plano." : expectations.Trim();
            if (TxtDossierScaleProfile != null) TxtDossierScaleProfile.Text = string.IsNullOrWhiteSpace(scaleProfile) ? "Estructura de casco estandarizada para atracamiento orbital." : scaleProfile.Trim();
        }

        private string CombineTacticalDossier()
        {
            string purpose = TxtDossierPurpose?.Text?.Trim() ?? string.Empty;
            string doctrine = TxtDossierDoctrine?.Text?.Trim() ?? string.Empty;
            string expectations = TxtDossierExpectations?.Text?.Trim() ?? string.Empty;
            string scaleProfile = TxtDossierScaleProfile?.Text?.Trim() ?? string.Empty;

            return $"🎯 PROPÓSITO: {purpose}\n⚓ DOCTRINA: {doctrine}\n📊 EXPECTATIVAS: {expectations}\n📏 MAGNITUD EN PUERTO: {scaleProfile}";
        }
    }
}
