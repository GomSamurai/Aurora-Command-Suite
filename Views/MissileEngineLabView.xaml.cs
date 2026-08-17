using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class MissileEngineLabView : UserControl
    {
        private DatabaseService? _dbService;
        private int _currentRaceId;

        private List<CustomProjectItem> _allProjects = new List<CustomProjectItem>();
        private List<CustomProjectItem> _appUserPresets = new List<CustomProjectItem>();
        private List<ResearchedTechItem> _researchedTechs = new List<ResearchedTechItem>();

        public MissileEngineLabView()
        {
            InitializeComponent();
        }

        public void LoadData(DatabaseService dbService, int raceId)
        {
            _dbService = dbService;
            _currentRaceId = raceId;

            if (_dbService != null)
            {
                _researchedTechs = _dbService.GetResearchedTechsForRace(_currentRaceId);
            }

            PopulateCategoryTechs();
            RefreshCatalogData();
            CalculateCurrentProjectSpecs();
        }

        public void LoadLabData(DatabaseService dbService, int raceId) => LoadData(dbService, raceId);

        private void OnModeChanged(object sender, RoutedEventArgs e)
        {
            if (PnlProjectDesigner == null || PnlProjectCatalog == null) return;

            if (BtnModeDesigner != null && BtnModeDesigner.IsChecked == true)
            {
                PnlProjectDesigner.Visibility = Visibility.Visible;
                PnlProjectCatalog.Visibility = Visibility.Collapsed;
            }
            else
            {
                PnlProjectDesigner.Visibility = Visibility.Collapsed;
                PnlProjectCatalog.Visibility = Visibility.Visible;
                RefreshCatalogData();
            }
        }

        private void CmbMasterCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateCategoryTechs();
            CalculateCurrentProjectSpecs();
        }

        private void PopulateCategoryTechs()
        {
            if (CmbMasterCategory == null || CmbSubTech1 == null || CmbSubTech2 == null || CmbSubTech3 == null) return;

            string selectedCat = "Active Sensors";
            if (CmbMasterCategory.SelectedItem is ComboBoxItem item && item.Content != null)
            {
                selectedCat = item.Content.ToString()!;
            }

            CmbSubTech1.ItemsSource = null;
            CmbSubTech2.ItemsSource = null;
            CmbSubTech3.ItemsSource = null;

            // Filter tech for Sub-ComboBox 1 from DB
            var categoryTechs = _researchedTechs
                .Where(t => CategoryMatches(t, selectedCat))
                .ToList();

            if (categoryTechs.Count == 0)
            {
                // Fallback default researched options if specific tech is not yet researched
                categoryTechs.Add(new ResearchedTechItem
                {
                    TechID = 1,
                    Name = $"Standard {selectedCat} Technology",
                    CategoryID = 1,
                    TechTypeID = 1,
                    AdditionalInfo = 1.0
                });
            }

            CmbSubTech1.ItemsSource = categoryTechs;
            CmbSubTech1.SelectedIndex = 0;

            // Setup Sub Tech 2 & 3 based on category
            if (selectedCat.Contains("Engines") || selectedCat.Contains("Motores"))
            {
                LblSubTech1.Text = "Tipo de Propulsión Investigada:";
                LblSubTech2.Text = "Eficiencia de Combustible (L/EP/Hr):";
                LblSubTech3.Text = "Reducción de Firma Térmica:";

                CmbSubTech2.ItemsSource = new List<string> { "1.0 Litro por EP/Hora (Básico)", "0.8 Litros por EP/Hora", "0.6 Litros por EP/Hora (Avanzado)" };
                CmbSubTech2.SelectedIndex = 0;

                CmbSubTech3.ItemsSource = new List<string> { "Normal (100% Firma)", "Reducción 50%", "Reducción 25% (Sigiloso)" };
                CmbSubTech3.SelectedIndex = 0;

                if (TxtProjectName != null) TxtProjectName.Text = "Nuclear Thermal Engine EP50";
            }
            else if (selectedCat.Contains("Lasers"))
            {
                LblSubTech1.Text = "Focalización de Longitud de Onda (Wavelength):";
                LblSubTech2.Text = "Tasa de Recarga de Condensador (Capacitor):";
                LblSubTech3.Text = "Opticas Focalizadas y Lentes:";

                CmbSubTech2.ItemsSource = new List<string> { "Capacitor Recharge Rate 1 (1 EU/5s)", "Capacitor Recharge Rate 2 (2 EU/5s)", "Capacitor Recharge Rate 4" };
                CmbSubTech2.SelectedIndex = 0;

                CmbSubTech3.ItemsSource = new List<string> { "Estándar 10cm", "Focalizado 15cm", "Pesado 20cm" };
                CmbSubTech3.SelectedIndex = 0;

                if (TxtProjectName != null) TxtProjectName.Text = "Infrared Laser 15cm";
            }
            else
            {
                LblSubTech1.Text = "Tecnología Base Investigada (AuroraDB.db):";
                LblSubTech2.Text = "Modificador de Eficiencia:";
                LblSubTech3.Text = "Módulo de Control Electrónico:";

                CmbSubTech2.ItemsSource = new List<string> { "Estándar 100%", "Mejorado 120%", "Optimizado 150%" };
                CmbSubTech2.SelectedIndex = 0;

                CmbSubTech3.ItemsSource = new List<string> { "Básico (Sin ECCM)", "ECCM-1", "ECCM-2 Avanzado" };
                CmbSubTech3.SelectedIndex = 0;

                if (TxtProjectName != null) TxtProjectName.Text = $"{selectedCat.Replace("📡 ", "").Replace("⚙️ ", "").Replace("💥 ", "")} Component MK-I";
            }
        }

        private bool CategoryMatches(ResearchedTechItem tech, string categoryName)
        {
            string cat = categoryName.ToLower();
            string name = tech.Name.ToLower();
            string desc = tech.Description.ToLower();
            int type = tech.TechTypeID;

            if (cat.Contains("engine") || cat.Contains("motor"))
            {
                return type == 119 || type == 65 || type == 127 || type == 130 || type == 198 || type == 214
                       || (name.Contains("engine") && !name.Contains("jump engine") && !name.Contains("fire control"))
                       || (desc.Contains("engine") && !desc.Contains("jump engine"));
            }
            if (cat.Contains("active sensor"))
            {
                return type == 20 || type == 152 || (name.Contains("active") && name.Contains("sensor"));
            }
            if (cat.Contains("em detection"))
            {
                return type == 125 || (name.Contains("em") && name.Contains("sensor"));
            }
            if (cat.Contains("thermal sensor"))
            {
                return type == 28 || (name.Contains("thermal") && name.Contains("sensor"));
            }
            if (cat.Contains("laser"))
            {
                return type == 3 || type == 1 || type == 140 || name.Contains("laser") || desc.Contains("laser");
            }
            if (cat.Contains("shield"))
            {
                return type == 215 || name.Contains("shield") || desc.Contains("shield");
            }
            if (cat.Contains("missile launcher"))
            {
                return type == 10 || type == 129 || type == 216 || name.Contains("launcher");
            }
            if (cat.Contains("fire control"))
            {
                return type == 17 || type == 18 || name.Contains("fire control");
            }
            if (cat.Contains("jump engine"))
            {
                return type == 169 || name.Contains("jump drive") || name.Contains("jump engine");
            }
            if (cat.Contains("power plant") || cat.Contains("reactor"))
            {
                return name.Contains("reactor") || name.Contains("power plant") || desc.Contains("power plant");
            }
            if (cat.Contains("gauss"))
            {
                return type == 143 || name.Contains("gauss");
            }
            if (cat.Contains("meson"))
            {
                return name.Contains("meson");
            }
            if (cat.Contains("particle"))
            {
                return name.Contains("particle");
            }
            if (cat.Contains("railgun"))
            {
                return name.Contains("railgun");
            }
            if (cat.Contains("carronade"))
            {
                return name.Contains("carronade");
            }
            if (cat.Contains("ciws"))
            {
                return type == 43 || name.Contains("ciws");
            }
            if (cat.Contains("cloak"))
            {
                return type == 46 || name.Contains("cloak");
            }

            string targetKeyword = cat.Replace("📡", "").Replace("⚙️", "").Replace("💥", "").Replace("🛡️", "").Replace("🚀", "").Trim();
            return name.Contains(targetKeyword) || desc.Contains(targetKeyword);
        }

        private void OnParamChanged(object sender, SelectionChangedEventArgs e) => CalculateCurrentProjectSpecs();
        private void OnParamChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => CalculateCurrentProjectSpecs();

        private void CalculateCurrentProjectSpecs()
        {
            if (LblSpecSize == null || LblSpecCostRP == null || LblSpecCostBP == null || IcProjectMinerals == null) return;

            string selectedCat = "Active Sensors";
            if (CmbMasterCategory?.SelectedItem is ComboBoxItem item && item.Content != null)
            {
                selectedCat = item.Content.ToString()!;
            }

            double hs = SldParam1 != null ? SldParam1.Value : 1.0;
            double mult = SldParam2 != null ? SldParam2.Value : 1.0;

            if (LblValParam1 != null) LblValParam1.Text = $"{hs:F1} HS ({hs * 50.0:N0} t)";
            if (LblValParam2 != null) LblValParam2.Text = $"Mod {mult:F0}";

            double costRP = Math.Round(hs * 100.0 * mult, 0);
            double costBP = Math.Round(costRP / 50.0, 1);
            int crew = (int)Math.Max(1, hs * 2);
            int htk = (int)Math.Max(1, hs);

            LblSpecSize.Text = $"{hs:F1} HS ({hs * 50.0:N0} t)";
            LblSpecCostRP.Text = $"{costRP:N0} RP";
            LblSpecCostBP.Text = $"{costBP:F1} BP";
            LblSpecCrew.Text = $"{crew} Personas";
            LblSpecHTK.Text = $"{htk} HTK";

            string tech1Name = CmbSubTech1?.SelectedItem is ResearchedTechItem t ? t.Name : "Tecnología Investigada";

            if (selectedCat.Contains("Engines") || selectedCat.Contains("Motores"))
            {
                double ep = hs * 50.0 * mult;
                LblSpecPerformance.Text = $"{ep:N0} EP";
                LblSpecDescription.Text = $"Motor Naval ({tech1Name}) | Potencia Total: {ep:N0} EP | Masa: {hs * 50.0:N0}t | Consumo: {1.0 / mult:F2} L/EP/Hr | Firma Térmica: {ep:N0} W";
            }
            else if (selectedCat.Contains("Lasers"))
            {
                double damage = Math.Round(hs * 4.0 * mult, 1);
                LblSpecPerformance.Text = $"{damage:F1} Dmg";
                LblSpecDescription.Text = $"Láser Naval ({tech1Name}) | Calibre: {hs * 10:F0}cm | Daño Focal: {damage:F1} HP | Consumo Energía: {damage * 2:F0} EU | Rango Máximo: {hs * 100_000:N0} km";
            }
            else
            {
                double rangeMkm = hs * mult * 4.0;
                LblSpecPerformance.Text = $"{rangeMkm:F2} Mkm";
                LblSpecDescription.Text = $"Componente Táctico ({selectedCat}) | Basado en: {tech1Name} | Alcance Operativo: {rangeMkm:F2} Mkm | Resistencia: {htk} HTK";
            }

            // Minerals Breakdown
            var minerals = new Dictionary<string, double>
            {
                { "Duranium (Chasis Estructural)", Math.Round(costBP * 0.3, 1) },
                { "Corbonite (Blindaje Térmico)", Math.Round(costBP * 0.3, 1) },
                { "Uridium / Gallicite (Sistemas Avanzados)", Math.Round(costBP * 0.4, 1) }
            };
            IcProjectMinerals.ItemsSource = minerals;
        }

        private void BtnCreateProjectInDB_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null)
            {
                MessageBox.Show("Conexión con AuroraDB.db no disponible.", "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string name = TxtProjectName != null ? TxtProjectName.Text : "Nuevo Proyecto";
            double hs = SldParam1 != null ? SldParam1.Value : 1.0;
            double mult = SldParam2 != null ? SldParam2.Value : 1.0;
            double costRP = Math.Round(hs * 100.0 * mult, 0);
            double costBP = Math.Round(costRP / 50.0, 1);

            string category = CmbMasterCategory?.SelectedItem is ComboBoxItem item && item.Content != null ? item.Content.ToString()! : "Componente Naval";

            var newProject = new CustomProjectItem
            {
                Name = name,
                Category = category,
                Source = ProjectSource.Aurora4XGame,
                DevelopmentCostRP = costRP,
                BuildCostBP = costBP,
                SizeHS = hs,
                Crew = (int)Math.Max(1, hs * 2),
                HTK = (int)Math.Max(1, hs),
                SpecificationsSummary = LblSpecDescription != null ? LblSpecDescription.Text : "Proyecto personalizado"
            };

            bool success = _dbService.CreateCustomProjectInDatabase(_currentRaceId, newProject, out string msg);
            if (success)
            {
                MessageBox.Show(msg, "Proyecto Creado", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshCatalogData();
            }
            else
            {
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSaveAppPreset_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtProjectName != null ? TxtProjectName.Text : "Preset Usuario";
            double hs = SldParam1 != null ? SldParam1.Value : 1.0;
            double mult = SldParam2 != null ? SldParam2.Value : 1.0;
            double costRP = Math.Round(hs * 100.0 * mult, 0);
            double costBP = Math.Round(costRP / 50.0, 1);

            string category = CmbMasterCategory?.SelectedItem is ComboBoxItem item && item.Content != null ? item.Content.ToString()! : "Componente Naval";

            var userPreset = new CustomProjectItem
            {
                ProjectID = _appUserPresets.Count + 900000,
                Name = $"⭐ {name}",
                Category = category,
                Source = ProjectSource.AppUserPreset,
                DevelopmentCostRP = costRP,
                BuildCostBP = costBP,
                SizeHS = hs,
                Crew = (int)Math.Max(1, hs * 2),
                HTK = (int)Math.Max(1, hs),
                SpecificationsSummary = "Preset guardado por el usuario en la aplicación Aurora Command Suite."
            };

            _appUserPresets.Add(userPreset);
            MessageBox.Show($"⭐ Preset '{name}' guardado con éxito en la app.", "Preset Guardado", MessageBoxButton.OK, MessageBoxImage.Information);
            RefreshCatalogData();
        }

        private void RefreshCatalogData()
        {
            if (_dbService == null) return;

            _allProjects = _dbService.GetCustomProjects(_currentRaceId);
            _allProjects.AddRange(_appUserPresets);

            ApplyCatalogFilter();
        }

        private void CmbCatalogSourceFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyCatalogFilter();

        private void ApplyCatalogFilter()
        {
            if (DgProjectCatalog == null || LblCatalogTotalCount == null) return;

            int filterIndex = CmbCatalogSourceFilter != null ? CmbCatalogSourceFilter.SelectedIndex : 0;
            IEnumerable<CustomProjectItem> filtered = _allProjects;

            if (filterIndex == 1) // Only Game
            {
                filtered = _allProjects.Where(p => p.Source == ProjectSource.Aurora4XGame);
            }
            else if (filterIndex == 2) // Only App
            {
                filtered = _allProjects.Where(p => p.Source == ProjectSource.AppUserPreset);
            }

            var list = filtered.ToList();
            DgProjectCatalog.ItemsSource = list;
            LblCatalogTotalCount.Text = $"Proyectos Cargados: {list.Count}";
        }
    }
}
