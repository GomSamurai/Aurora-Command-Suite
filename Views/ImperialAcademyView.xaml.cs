using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace AuroraDesignSuite.Views
{
    public partial class ImperialAcademyView : UserControl
    {
        public class CodexItem
        {
            public string Title { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string Body { get; set; } = string.Empty;
        }

        private List<CodexItem> _allItems = new List<CodexItem>();

        public ImperialAcademyView()
        {
            InitializeComponent();
            LoadCodexDatabase();
            FilterList("");
        }

        private void LoadCodexDatabase()
        {
            _allItems = new List<CodexItem>
            {
                new CodexItem
                {
                    Title = "🌌 Lección 1: Primeros Pasos en Sol",
                    Category = "🎓 Tutorial Interactivo para Principiantes",
                    Body = "Al comenzar en la Tierra (Sistema Sol), dispones de población civil, industrias convencionales y laboratorios.\n\n" +
                           "PASOS RECOMENDADOS:\n" +
                           "1. Convierte industrias convencionales a Fábricas de Construcción para multiplicar la producción x10.\n" +
                           "2. Asigna científicos a proyectos de I+D (Trans-Newtonian Technology, Sensors, Engines).\n" +
                           "3. Construye tu primera Catapulta de Masa (Mass Driver) para recibir minerales de otros cuerpos celestes."
                },
                new CodexItem
                {
                    Title = "⛏️ Lección 2: Minería Trans-Newtoniana",
                    Category = "🎓 Tutorial Interactivo para Principiantes",
                    Body = "Los 11 minerales exóticos son la sangre de tu imperio:\n" +
                           "• Duranium: Estructura de edificios y cascos navales.\n" +
                           "• Sorium: Refinado exclusivo para combustible espacial.\n" +
                           "• Neutronium: Blindaje pesado de naves de combate.\n" +
                           "• Gallicite: Motores y propulsión espacial.\n" +
                           "• Uridium: Sensores, radares y sistemas ópticos.\n\n" +
                           "CONSEJO TÁCTICO: Despliega Minas Automatizadas en asteroides y usa Mass Drivers orientados hacia la Tierra."
                },
                new CodexItem
                {
                    Title = "🛠️ Lección 3: Ingeniería Naval Básica",
                    Category = "🎓 Tutorial Interactivo para Principiantes",
                    Body = "Reglas fundamentales del Diseñador de Naves:\n" +
                           "1. Tamaño de Casco (HS): 1 HS = 50 toneladas.\n" +
                           "2. Motores Comerciales vs Militares: Motores comerciales (< 50% potencia por HS, tamaño > 25 HS) no sufren averías mecánicas.\n" +
                           "3. Mantenimiento (MSP): Las naves militares sufren averías en travesías largas si no llevan pañoles MSP y suficientes DCR."
                },
                new CodexItem
                {
                    Title = "⛽ Lección 4: Autonomía y Logística de Combustible",
                    Category = "🎓 Tutorial Interactivo para Principiantes",
                    Body = "El combustible determina el radio de acción de tus flotas.\n" +
                           "• El consumo depende directamente de la potencia del motor y la velocidad alcanzada.\n" +
                           "• Construye Refinerías de Sorium en colonias con yacimientos de Sorium para garantizar reservas continuas."
                },
                new CodexItem
                {
                    Title = "🚀 Lección 5: Doctrina de Misiles vs Armas de Energía",
                    Category = "🎓 Tutorial Interactivo para Principiantes",
                    Body = "DOCTRINA DE MISILES:\n" +
                           "• Alcance extremo (decenas de millones de km).\n" +
                           "• Requiere pañoles de munición (Magazines) y fábricas de armamento.\n\n" +
                           "DOCTRINA DE ENERGÍA (Láseres, Cañones Gauss, Plasma):\n" +
                           "• Sin consumo de munición en combate.\n" +
                           "• Requiere plantas de energía (Reactores) y recarga de capacitores."
                },
                new CodexItem
                {
                    Title = "🌍 Lección 6: Terraformación Colonial",
                    Category = "🎓 Tutorial Interactivo para Principiantes",
                    Body = "Reducir el Costo Colonial a 0.00 permite eliminar la necesidad de Infraestructura Poblacional.\n" +
                           "1. Añade Oxígeno entre 0.10 y 0.30 atm.\n" +
                           "2. Ajusta la temperatura inyectando gases de invernadero (GHG) o refrigerantes.\n" +
                           "3. Mantén la presión total por debajo del límite respirable."
                },
                new CodexItem
                {
                    Title = "HS - Hull Size (Tamaño de Casco)",
                    Category = "📖 Diccionario Códex Táctico",
                    Body = "💡 TAMAÑO DE CASCO (HS):\nUnidad fundamental de desplazamiento en Aurora 4X. 1 HS equivale exactamente a 50 toneladas métricas.\n\n" +
                           "• Cazas: < 10 HS (500 toneladas).\n" +
                           "• Corbetas / Fragatas: 20 - 100 HS (1,000 - 5,000 toneladas).\n" +
                           "• Destructores / Cruceros: 100 - 400 HS (5,000 - 20,000 toneladas).\n" +
                           "• Acorazados / Cargueros Pesados: > 500 HS (25,000+ toneladas)."
                },
                new CodexItem
                {
                    Title = "TCS - Thermal & Cross Section (Firma Térmica)",
                    Category = "📖 Diccionario Códex Táctico",
                    Body = "💡 FIRMA TÉRMICA Y SECCIÓN EFICAZ (TCS):\nDetermina la visibilidad de tu nave ante los radares y sensores pasivos térmicos enemigos.\n\n" +
                           "• A mayor tamaño y potencia de motores, mayor es la firma TCS.\n" +
                           "• Componentes de sigilo (Stealth) reducen la firma TCS facilitando emboscadas."
                },
                new CodexItem
                {
                    Title = "DCR - Damage Control Rating (Control de Daños)",
                    Category = "📖 Diccionario Códex Táctico",
                    Body = "💡 PUNTUACIÓN DE CONTROL DE DAÑOS (DCR):\nCapacidad operativa de los equipos de control de averías a bordo.\n\n" +
                           "• Un DCR elevado permite reparar múltiples sistemas destruidos simultáneamente durante el combate utilizando repuestos MSP."
                },
                new CodexItem
                {
                    Title = "Retooling (Re-equipamiento de Astillero)",
                    Category = "📖 Diccionario Códex Táctico",
                    Body = "💡 RE-EQUIPAMIENTO DE ASTILLERO (RETOOLING):\nProceso industrial mediante el cual un astillero reconfigura sus gradas para fabricar una nueva clase de nave.\n\n" +
                           "• Si la nueva clase es un derivado o variante de la clase anterior, el costo y tiempo de retooling se reduce drásticamente."
                }
            };
        }

        private void FilterList(string query)
        {
            query = (query ?? "").Trim().ToLower();
            var filtered = string.IsNullOrEmpty(query)
                ? _allItems
                : _allItems.Where(x => x.Title.ToLower().Contains(query) || x.Category.ToLower().Contains(query) || x.Body.ToLower().Contains(query)).ToList();

            LstCodexItems.ItemsSource = filtered;
            if (filtered.Count > 0)
            {
                LstCodexItems.SelectedIndex = 0;
            }
            else
            {
                LblArticleTitle.Text = "Sin Resultados";
                LblArticleCategory.Text = "🔍 Búsqueda";
                LblArticleBody.Text = "No se encontraron lecciones o términos que coincidan con la búsqueda.";
            }
        }

        private void TxtSearchCodex_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterList(TxtSearchCodex.Text);
        }

        private void LstCodexItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstCodexItems.SelectedItem is CodexItem selected)
            {
                LblArticleTitle.Text = selected.Title;
                LblArticleCategory.Text = selected.Category;
                LblArticleBody.Text = selected.Body;
            }
        }
    }
}
