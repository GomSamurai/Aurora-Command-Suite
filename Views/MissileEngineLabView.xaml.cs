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

            if (selectedCat.Contains("Engines") || selectedCat.Contains("Motores"))
            {
                LblSubTech1.Text = "Tecnología de Propulsión (Engine Tech):";
                LblSubTech2.Text = "Consumo de Combustible (Fuel Consumption):";
                LblSubTech3.Text = "Reducción de Firma Térmica (Thermal Reduction):";

                // SubTech 1: Engine Propulsion Techs (Type 40, 119)
                var engineTechs = _researchedTechs
                    .Where(t => (t.TechTypeID == 40 || t.TechTypeID == 119 || t.Name.Contains("Engine")) 
                                && !t.Name.Contains("Jump Engine") && !t.Name.Contains("Fire Control") 
                                && !t.Name.Contains("Fuel Consumption") && !t.Name.Contains("Thermal Reduction")
                                && !t.Name.Contains("Power Modifier") && !t.Name.Contains("Engine Size"))
                    .ToList();

                if (engineTechs.Count == 0)
                {
                    engineTechs.Add(new ResearchedTechItem { TechID = 1, Name = "Nuclear Radioisotope Engine" });
                    engineTechs.Add(new ResearchedTechItem { TechID = 2, Name = "Conventional Engine" });
                }
                CmbSubTech1.ItemsSource = engineTechs;
                CmbSubTech1.SelectedIndex = 0;

                // SubTech 2: Fuel Consumption (Type 65)
                var fuelTechs = _researchedTechs.Where(t => t.TechTypeID == 65 || t.Name.Contains("Fuel Consumption")).ToList();
                if (fuelTechs.Count == 0) fuelTechs.Add(new ResearchedTechItem { TechID = 10, Name = "Fuel Consumption: 1 Litre per Engine Power Hour" });
                CmbSubTech2.ItemsSource = fuelTechs;
                CmbSubTech2.SelectedIndex = 0;

                // SubTech 3: Thermal Reduction (Type 127)
                var thermalTechs = _researchedTechs.Where(t => t.TechTypeID == 127 || t.Name.Contains("Thermal Reduction")).ToList();
                if (thermalTechs.Count == 0) thermalTechs.Add(new ResearchedTechItem { TechID = 20, Name = "Thermal Reduction: Signature 100% Normal" });
                CmbSubTech3.ItemsSource = thermalTechs;
                CmbSubTech3.SelectedIndex = 0;

                if (TxtProjectName != null) TxtProjectName.Text = "Nuclear Thermal Engine EP50";
            }
            else if (selectedCat.Contains("Lasers"))
            {
                LblSubTech1.Text = "Tamaño Focal / Calibre (Laser Focal Size):";
                LblSubTech2.Text = "Longitud de Onda del Láser (Wavelength):";
                LblSubTech3.Text = "Tasa de Recarga de Condensador (Capacitor):";

                // SubTech 1: Focal Size (Type 15)
                var focalTechs = _researchedTechs.Where(t => t.TechTypeID == 15 || t.Name.Contains("Focal Size")).ToList();
                if (focalTechs.Count == 0) focalTechs.Add(new ResearchedTechItem { TechID = 30, Name = "10cm Laser Focal Size" });
                CmbSubTech1.ItemsSource = focalTechs;
                CmbSubTech1.SelectedIndex = 0;

                // SubTech 2: Wavelength (Type 3)
                var waveTechs = _researchedTechs.Where(t => t.TechTypeID == 3 || t.Name.Contains("Infrared") || t.Name.Contains("Laser")).ToList();
                if (waveTechs.Count == 0) waveTechs.Add(new ResearchedTechItem { TechID = 31, Name = "Infrared Laser" });
                CmbSubTech2.ItemsSource = waveTechs;
                CmbSubTech2.SelectedIndex = 0;

                // SubTech 3: Capacitor Recharge Rate (Type 1)
                var capTechs = _researchedTechs.Where(t => t.TechTypeID == 1 || t.Name.Contains("Capacitor")).ToList();
                if (capTechs.Count == 0) capTechs.Add(new ResearchedTechItem { TechID = 32, Name = "Capacitor Recharge Rate 1" });
                CmbSubTech3.ItemsSource = capTechs;
                CmbSubTech3.SelectedIndex = 0;

                if (TxtProjectName != null) TxtProjectName.Text = "Infrared Laser 10cm";
            }
            else if (selectedCat.Contains("Active Sensors"))
            {
                LblSubTech1.Text = "Fuerza del Sensor Activo (Active Sensor Strength):";
                LblSubTech2.Text = "Endurecimiento Electrónico (Electronic Hardening):";
                LblSubTech3.Text = "Contra-contramedidas (ECCM):";

                // SubTech 1: Active Sensor Strength (Type 20, 152)
                var sensorTechs = _researchedTechs.Where(t => t.TechTypeID == 20 || t.TechTypeID == 152 || t.Name.Contains("Active Sensor")).ToList();
                if (sensorTechs.Count == 0) sensorTechs.Add(new ResearchedTechItem { TechID = 40, Name = "Conventional Active Sensor Strength 2" });
                CmbSubTech1.ItemsSource = sensorTechs;
                CmbSubTech1.SelectedIndex = 0;

                // SubTech 2: Hardening (Type 139)
                var hardTechs = _researchedTechs.Where(t => t.TechTypeID == 139 || t.Name.Contains("Electronic Hardening")).ToList();
                if (hardTechs.Count == 0) hardTechs.Add(new ResearchedTechItem { TechID = 41, Name = "Electronic Hardening Level 0" });
                CmbSubTech2.ItemsSource = hardTechs;
                CmbSubTech2.SelectedIndex = 0;

                // SubTech 3: ECCM (Type 83)
                var eccmTechs = _researchedTechs.Where(t => t.TechTypeID == 83 || t.Name.Contains("ECCM")).ToList();
                if (eccmTechs.Count == 0) eccmTechs.Add(new ResearchedTechItem { TechID = 42, Name = "Electronic Counter-countermeasures - 0" });
                CmbSubTech3.ItemsSource = eccmTechs;
                CmbSubTech3.SelectedIndex = 0;

                if (TxtProjectName != null) TxtProjectName.Text = "Active Search Sensor AS10-R100";
            }
            else if (selectedCat.Contains("Missile Launchers"))
            {
                LblSubTech1.Text = "Tamaño de Lanzador (Missile Launcher Size):";
                LblSubTech2.Text = "Tasa de Recarga y Reducción (Reload Rate):";
                LblSubTech3.Text = "Probabilidad de Explosión de Lanzador:";

                // SubTech 1: Launcher Size (Type 10)
                var launcherTechs = _researchedTechs.Where(t => t.TechTypeID == 10 || t.Name.Contains("Missile Launcher Size")).ToList();
                if (launcherTechs.Count == 0) launcherTechs.Add(new ResearchedTechItem { TechID = 50, Name = "Missile Launcher Size 6" });
                CmbSubTech1.ItemsSource = launcherTechs;
                CmbSubTech1.SelectedIndex = 0;

                // SubTech 2: Reload (Type 129)
                var reloadTechs = _researchedTechs.Where(t => t.TechTypeID == 129 || t.Name.Contains("Reload")).ToList();
                if (reloadTechs.Count == 0) reloadTechs.Add(new ResearchedTechItem { TechID = 51, Name = "Standard Size and Reload Rate" });
                CmbSubTech2.ItemsSource = reloadTechs;
                CmbSubTech2.SelectedIndex = 0;

                CmbSubTech3.ItemsSource = new List<string> { "Standard Box Launcher (100% Explosion)", "Safe Launcher (70% Explosion)" };
                CmbSubTech3.SelectedIndex = 0;

                if (TxtProjectName != null) TxtProjectName.Text = "Size 6 Missile Launcher";
            }
            else
            {
                LblSubTech1.Text = "Tecnología Base Investigada (AuroraDB.db):";
                LblSubTech2.Text = "Modificador de Eficiencia:";
                LblSubTech3.Text = "Módulo de Control Electrónico:";

                var generalTechs = _researchedTechs.Where(t => CategoryMatches(t, selectedCat)).ToList();
                if (generalTechs.Count == 0) generalTechs.Add(new ResearchedTechItem { TechID = 99, Name = $"Standard {selectedCat} Tech" });
                CmbSubTech1.ItemsSource = generalTechs;
                CmbSubTech1.SelectedIndex = 0;

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

            // Calculate Research Time Estimation
            double monthsEst = Math.Max(0.1, Math.Round(costRP / 200.0, 1));
            if (LblEstResearchTime != null) LblEstResearchTime.Text = $"{monthsEst:F1} Meses (1 Lab)";

            // Update Thermal Signature Advice
            if (LblThermalSignatureAdvice != null)
            {
                if (selectedCat.Contains("Engines") || selectedCat.Contains("Motores"))
                {
                    double ep = hs * 50.0 * mult;
                    LblThermalSignatureAdvice.Text = $"{ep:N0} W ({ep / 10.0:N0} Mkm IR)";
                }
                else
                {
                    LblThermalSignatureAdvice.Text = "Baja Firma (< 10 W)";
                }
            }

            // Update Doctrinal Advisor Guidance
            if (LblAdvisorGuidance != null)
            {
                if (selectedCat.Contains("Engines") || selectedCat.Contains("Motores"))
                {
                    LblAdvisorGuidance.Text = "💡 DOCTRINA DE PROPULSIÓN NAVAL: Motores de mayor tamaño en HS obtienen mejor consumo de combustible por EP. Para cargueros comerciales, mantén el modificador de potencia en 1.0x o inferior.";
                }
                else if (selectedCat.Contains("Lasers"))
                {
                    LblAdvisorGuidance.Text = "💡 DOCTRINA DE ARMAS DE ENERGÍA: Los láseres causan daño penetrante en columna. A mayor calibre focal, mayor penetración de blindaje en combate a corta/media distancia.";
                }
                else if (selectedCat.Contains("Active Sensors"))
                {
                    LblAdvisorGuidance.Text = "💡 DOCTRINA DE SENSORES: Ajusta la resolución (Res) al tipo de amenaza. Res 1 detecta misiles/cazas (0.1 MSP a 1 HS), mientras que Res 100 detecta cruceros de 5,000t a máxima distancia.";
                }
                else if (selectedCat.Contains("Missile Launchers"))
                {
                    LblAdvisorGuidance.Text = "💡 DOCTRINA DE MISILES: Los tubos de lanzamiento deben coincidir exactamente con el tamaño MSP de la munición. Lanzadores reducidos disminuyen la masa pero aumentan el tiempo de recarga.";
                }
                else
                {
                    LblAdvisorGuidance.Text = "💡 DOCTRINA GENERAL DE I+D: Maximiza la eficiencia tecnológica investigando prerrequisitos en el Árbol Tecnológico antes de prototipar componentes pesados.";
                }
            }

            // Update Simulation Matrix
            if (LblSimCol1Title != null && LblSimCol1Value != null)
            {
                if (selectedCat.Contains("Engines") || selectedCat.Contains("Motores"))
                {
                    double ep = hs * 50.0 * mult;
                    LblSimCol1Title.Text = "Casco 1,000 t";
                    LblSimCol1Value.Text = $"{ep * 1000.0 / 1000.0:N0} km/s";

                    LblSimCol2Title.Text = "Casco 5,000 t";
                    LblSimCol2Value.Text = $"{ep * 1000.0 / 5000.0:N0} km/s";

                    LblSimCol3Title.Text = "Casco 10,000 t";
                    LblSimCol3Value.Text = $"{ep * 1000.0 / 10000.0:N0} km/s";

                    LblSimCol4Title.Text = "Casco 25,000 t";
                    LblSimCol4Value.Text = $"{ep * 1000.0 / 25000.0:N0} km/s";
                }
                else if (selectedCat.Contains("Active Sensors"))
                {
                    double maxRangeKm = 2.0 * mult * hs * 40000.0;
                    LblSimCol1Title.Text = "Vs Caza (250t)";
                    LblSimCol1Value.Text = $"{maxRangeKm / 20.0 / 1_000_000.0:F2} Mkm";

                    LblSimCol2Title.Text = "Vs Corbeta (1,000t)";
                    LblSimCol2Value.Text = $"{maxRangeKm / 5.0 / 1_000_000.0:F2} Mkm";

                    LblSimCol3Title.Text = "Vs Fragata (5,000t)";
                    LblSimCol3Value.Text = $"{maxRangeKm / 1_000_000.0:F2} Mkm";

                    LblSimCol4Title.Text = "Vs Nave Capital (50k t)";
                    LblSimCol4Value.Text = $"{maxRangeKm * 3.16 / 1_000_000.0:F2} Mkm";
                }
                else
                {
                    LblSimCol1Title.Text = "Rango 10,000 km";
                    LblSimCol1Value.Text = "100% Eficacia";

                    LblSimCol2Title.Text = "Rango 50,000 km";
                    LblSimCol2Value.Text = "85% Eficacia";

                    LblSimCol3Title.Text = "Rango 100,000 km";
                    LblSimCol3Value.Text = "60% Eficacia";

                    LblSimCol4Title.Text = "Rango 200,000 km";
                    LblSimCol4Value.Text = "25% Eficacia";
                }
            }
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
