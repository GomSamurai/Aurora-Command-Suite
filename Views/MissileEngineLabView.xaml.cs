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

            LoadCompanyNames();
            LoadEmpireNamingThemes();
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
            if (CmbSubTech4 != null) CmbSubTech4.ItemsSource = null;

            // Default visibility settings
            if (LblSubTech2 != null) LblSubTech2.Visibility = Visibility.Visible;
            if (CmbSubTech2 != null) CmbSubTech2.Visibility = Visibility.Visible;
            if (LblSubTech3 != null) LblSubTech3.Visibility = Visibility.Visible;
            if (CmbSubTech3 != null) CmbSubTech3.Visibility = Visibility.Visible;
            if (LblSubTech4 != null) LblSubTech4.Visibility = Visibility.Collapsed;
            if (CmbSubTech4 != null) CmbSubTech4.Visibility = Visibility.Collapsed;
            if (PnlParam1Container != null) PnlParam1Container.Visibility = Visibility.Visible;
            if (PnlParam2Container != null) PnlParam2Container.Visibility = Visibility.Visible;

            if (selectedCat.Contains("Engines") || selectedCat.Contains("Motores"))
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Tecnología de Propulsión (Engine Tech):";
                if (LblSubTech2 != null) LblSubTech2.Text = "Consumo de Combustible (Fuel Consumption):";
                if (LblSubTech3 != null) LblSubTech3.Text = "Reducción de Firma Térmica (Thermal Reduction):";

                if (LblParam1Title != null) LblParam1Title.Text = "Tamaño del Motor (HS / Tons):";
                if (LblParam2Title != null) LblParam2Title.Text = "Modificador de Potencia (Power Mod):";

                var engineTechs = _researchedTechs.Where(t => (t.TechTypeID == 40 || t.TechTypeID == 119 || t.Name.Contains("Engine")) && !t.Name.Contains("Jump Engine") && !t.Name.Contains("Fire Control")).ToList();
                if (engineTechs.Count == 0) engineTechs.Add(new ResearchedTechItem { TechID = 1, Name = "Nuclear Radioisotope Engine" });
                CmbSubTech1.ItemsSource = engineTechs; CmbSubTech1.SelectedIndex = 0;

                var fuelTechs = _researchedTechs.Where(t => t.TechTypeID == 65 || t.Name.Contains("Fuel Consumption")).ToList();
                if (fuelTechs.Count == 0) fuelTechs.Add(new ResearchedTechItem { TechID = 10, Name = "Fuel Consumption: 1 Litre per Engine Power Hour" });
                if (CmbSubTech2 != null) { CmbSubTech2.ItemsSource = fuelTechs; CmbSubTech2.SelectedIndex = 0; }

                var thermalTechs = _researchedTechs.Where(t => t.TechTypeID == 127 || t.Name.Contains("Thermal Reduction")).ToList();
                if (thermalTechs.Count == 0) thermalTechs.Add(new ResearchedTechItem { TechID = 20, Name = "Thermal Reduction: Signature 100% Normal" });
                if (CmbSubTech3 != null) { CmbSubTech3.ItemsSource = thermalTechs; CmbSubTech3.SelectedIndex = 0; }

                if (TxtProjectName != null) TxtProjectName.Text = "Nuclear Thermal Engine EP50";
            }
            else if (selectedCat.Contains("Lasers"))
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Tamaño Focal / Calibre (Laser Focal Size):";
                if (LblSubTech2 != null) LblSubTech2.Text = "Longitud de Onda del Láser (Wavelength):";
                if (LblSubTech3 != null) LblSubTech3.Text = "Tasa de Recarga de Condensador (Capacitor):";
                if (LblSubTech4 != null) { LblSubTech4.Text = "Tipo de Montaje / Cañón Focal:"; LblSubTech4.Visibility = Visibility.Visible; }
                if (CmbSubTech4 != null) CmbSubTech4.Visibility = Visibility.Visible;

                if (PnlParam1Container != null) PnlParam1Container.Visibility = Visibility.Collapsed;
                if (PnlParam2Container != null) PnlParam2Container.Visibility = Visibility.Collapsed;

                var focalTechs = _researchedTechs.Where(t => t.TechTypeID == 15 || t.Name.Contains("Focal Size")).ToList();
                if (focalTechs.Count == 0) focalTechs.Add(new ResearchedTechItem { TechID = 30, Name = "10cm Laser Focal Size" });
                CmbSubTech1.ItemsSource = focalTechs; CmbSubTech1.SelectedIndex = 0;

                var waveTechs = _researchedTechs.Where(t => t.TechTypeID == 3 || t.Name.Contains("Infrared") || t.Name.Contains("Laser")).ToList();
                if (waveTechs.Count == 0) waveTechs.Add(new ResearchedTechItem { TechID = 31, Name = "Infrared Laser" });
                if (CmbSubTech2 != null) { CmbSubTech2.ItemsSource = waveTechs; CmbSubTech2.SelectedIndex = 0; }

                var capTechs = _researchedTechs.Where(t => t.TechTypeID == 1 || t.Name.Contains("Capacitor")).ToList();
                if (capTechs.Count == 0) capTechs.Add(new ResearchedTechItem { TechID = 32, Name = "Capacitor Recharge Rate 1" });
                if (CmbSubTech3 != null) { CmbSubTech3.ItemsSource = capTechs; CmbSubTech3.SelectedIndex = 0; }

                if (CmbSubTech4 != null)
                {
                    CmbSubTech4.ItemsSource = new List<string> { "Standard Mount (Montaje Estándar)", "Spinal Mount 1.5x (Montaje Espinal Proa)", "Reduced Size 0.5 HS (Caza Parásito)" };
                    CmbSubTech4.SelectedIndex = 0;
                }

                if (TxtProjectName != null) TxtProjectName.Text = "Infrared Laser 10cm";
            }
            else if (selectedCat.Contains("Active Sensors"))
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Fuerza del Sensor Activo (Active Sensor Strength):";
                if (LblSubTech2 != null) LblSubTech2.Text = "Endurecimiento Electrónico (Electronic Hardening):";
                if (LblSubTech3 != null) LblSubTech3.Text = "Contra-contramedidas (ECCM):";

                if (LblParam1Title != null) LblParam1Title.Text = "Tamaño del Sensor (HS / Tons):";
                if (LblParam2Title != null) LblParam2Title.Text = "Resolución de Radar (Target Resolution):";

                var sensorTechs = _researchedTechs.Where(t => t.TechTypeID == 20 || t.TechTypeID == 152 || t.Name.Contains("Active Sensor")).ToList();
                if (sensorTechs.Count == 0) sensorTechs.Add(new ResearchedTechItem { TechID = 40, Name = "Conventional Active Sensor Strength 2" });
                CmbSubTech1.ItemsSource = sensorTechs; CmbSubTech1.SelectedIndex = 0;

                var hardTechs = _researchedTechs.Where(t => t.TechTypeID == 139 || t.Name.Contains("Electronic Hardening")).ToList();
                if (hardTechs.Count == 0) hardTechs.Add(new ResearchedTechItem { TechID = 41, Name = "Electronic Hardening Level 0" });
                if (CmbSubTech2 != null) { CmbSubTech2.ItemsSource = hardTechs; CmbSubTech2.SelectedIndex = 0; }

                var eccmTechs = _researchedTechs.Where(t => t.TechTypeID == 83 || t.Name.Contains("ECCM")).ToList();
                if (eccmTechs.Count == 0) eccmTechs.Add(new ResearchedTechItem { TechID = 42, Name = "Electronic Counter-countermeasures - 0" });
                if (CmbSubTech3 != null) { CmbSubTech3.ItemsSource = eccmTechs; CmbSubTech3.SelectedIndex = 0; }

                if (TxtProjectName != null) TxtProjectName.Text = "Active Search Sensor AS10-R100";
            }
            else if (selectedCat.Contains("Thermal Sensors"))
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Sensibilidad del Sensor Térmico (Thermal Sensitivity):";
                if (LblSubTech2 != null) LblSubTech2.Visibility = Visibility.Collapsed;
                if (CmbSubTech2 != null) CmbSubTech2.Visibility = Visibility.Collapsed;
                if (LblSubTech3 != null) LblSubTech3.Visibility = Visibility.Collapsed;
                if (CmbSubTech3 != null) CmbSubTech3.Visibility = Visibility.Collapsed;

                if (LblParam1Title != null) LblParam1Title.Text = "Tamaño del Sensor Térmico (HS / Tons):";
                if (PnlParam2Container != null) PnlParam2Container.Visibility = Visibility.Collapsed;

                var thermTechs = _researchedTechs.Where(t => t.TechTypeID == 19 || t.TechTypeID == 28 || t.Name.Contains("Thermal Sensor")).ToList();
                if (thermTechs.Count == 0) thermTechs.Add(new ResearchedTechItem { TechID = 45, Name = "Thermal Sensor Sensitivity 5" });
                CmbSubTech1.ItemsSource = thermTechs; CmbSubTech1.SelectedIndex = 0;

                if (TxtProjectName != null) TxtProjectName.Text = "Passive Thermal Array TH5-HS1";
            }
            else if (selectedCat.Contains("EM Detection"))
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Sensibilidad del Sensor EM (EM Sensitivity):";
                if (LblSubTech2 != null) LblSubTech2.Visibility = Visibility.Collapsed;
                if (CmbSubTech2 != null) CmbSubTech2.Visibility = Visibility.Collapsed;
                if (LblSubTech3 != null) LblSubTech3.Visibility = Visibility.Collapsed;
                if (CmbSubTech3 != null) CmbSubTech3.Visibility = Visibility.Collapsed;

                if (LblParam1Title != null) LblParam1Title.Text = "Tamaño del Sensor EM (HS / Tons):";
                if (PnlParam2Container != null) PnlParam2Container.Visibility = Visibility.Collapsed;

                var emTechs = _researchedTechs.Where(t => t.TechTypeID == 125 || t.Name.Contains("EM Sensor")).ToList();
                if (emTechs.Count == 0) emTechs.Add(new ResearchedTechItem { TechID = 46, Name = "EM Sensor Sensitivity 5" });
                CmbSubTech1.ItemsSource = emTechs; CmbSubTech1.SelectedIndex = 0;

                if (TxtProjectName != null) TxtProjectName.Text = "Passive EM Array EM5-HS1";
            }
            else if (selectedCat.Contains("Direct Fire Control"))
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Alcance del Control de Tiro (Range Rating):";
                if (LblSubTech2 != null) LblSubTech2.Text = "Velocidad de Seguimiento (Tracking Speed):";
                if (LblSubTech3 != null) LblSubTech3.Text = "Módulo ECCM Integrado:";

                if (LblParam1Title != null) LblParam1Title.Text = "Multiplicador de Velocidad de Seguimiento:";
                if (PnlParam2Container != null) PnlParam2Container.Visibility = Visibility.Collapsed;

                var rangeTechs = _researchedTechs.Where(t => t.TechTypeID == 17 || t.Name.Contains("Fire Control Range")).ToList();
                if (rangeTechs.Count == 0) rangeTechs.Add(new ResearchedTechItem { TechID = 47, Name = "Beam Fire Control Range 48,000 km" });
                CmbSubTech1.ItemsSource = rangeTechs; CmbSubTech1.SelectedIndex = 0;

                var speedTechs = _researchedTechs.Where(t => t.TechTypeID == 18 || t.Name.Contains("Tracking Speed")).ToList();
                if (speedTechs.Count == 0) speedTechs.Add(new ResearchedTechItem { TechID = 48, Name = "Tracking Speed 2,500 km/s" });
                if (CmbSubTech2 != null) { CmbSubTech2.ItemsSource = speedTechs; CmbSubTech2.SelectedIndex = 0; }

                if (CmbSubTech3 != null) { CmbSubTech3.ItemsSource = new List<string> { "Standard (Sin ECCM)", "ECCM-1 Integrado", "ECCM-2 Integrado" }; CmbSubTech3.SelectedIndex = 0; }

                if (TxtProjectName != null) TxtProjectName.Text = "Beam Fire Control BFC48-S2500";
            }
            else if (selectedCat.Contains("Missile Fire Control"))
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Alcance de Control de Misiles (Range):";
                if (LblSubTech2 != null) LblSubTech2.Text = "Velocidad de Seguimiento de Misiles:";
                if (LblSubTech3 != null) LblSubTech3.Text = "ECCM de Misil Integrado:";

                if (LblParam1Title != null) LblParam1Title.Text = "Multiplicador de Seguimiento:";
                if (PnlParam2Container != null) PnlParam2Container.Visibility = Visibility.Collapsed;

                var mfcRangeTechs = _researchedTechs.Where(t => t.TechTypeID == 152 || t.Name.Contains("Missile Fire Control")).ToList();
                if (mfcRangeTechs.Count == 0) mfcRangeTechs.Add(new ResearchedTechItem { TechID = 49, Name = "Missile Fire Control Range 10 Mkm" });
                CmbSubTech1.ItemsSource = mfcRangeTechs; CmbSubTech1.SelectedIndex = 0;

                if (CmbSubTech2 != null) { CmbSubTech2.ItemsSource = new List<string> { "Speed 5,000 km/s", "Speed 10,000 km/s", "Speed 20,000 km/s" }; CmbSubTech2.SelectedIndex = 0; }
                if (CmbSubTech3 != null) { CmbSubTech3.ItemsSource = new List<string> { "Básico (Sin ECCM)", "ECCM-1 Integrado" }; CmbSubTech3.SelectedIndex = 0; }

                if (TxtProjectName != null) TxtProjectName.Text = "Missile Fire Control MFC10M-S10k";
            }
            else if (selectedCat.Contains("Missile Launchers"))
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Tamaño de Lanzador (Missile Launcher Size):";
                if (LblSubTech2 != null) LblSubTech2.Text = "Tasa de Recarga y Reducción (Reload Rate):";
                if (LblSubTech3 != null) LblSubTech3.Text = "Protección de Pañol y Blindaje:";

                if (PnlParam1Container != null) PnlParam1Container.Visibility = Visibility.Collapsed;
                if (PnlParam2Container != null) PnlParam2Container.Visibility = Visibility.Collapsed;

                var launcherTechs = _researchedTechs.Where(t => t.TechTypeID == 10 || t.Name.Contains("Missile Launcher Size")).ToList();
                if (launcherTechs.Count == 0) launcherTechs.Add(new ResearchedTechItem { TechID = 50, Name = "Missile Launcher Size 6" });
                CmbSubTech1.ItemsSource = launcherTechs; CmbSubTech1.SelectedIndex = 0;

                var reloadTechs = _researchedTechs.Where(t => t.TechTypeID == 129 || t.Name.Contains("Reload")).ToList();
                if (reloadTechs.Count == 0) reloadTechs.Add(new ResearchedTechItem { TechID = 51, Name = "Standard Size and Reload Rate" });
                if (CmbSubTech2 != null) { CmbSubTech2.ItemsSource = reloadTechs; CmbSubTech2.SelectedIndex = 0; }

                if (CmbSubTech3 != null) { CmbSubTech3.ItemsSource = new List<string> { "Standard Box Launcher (100% Explosion Chance)", "Safe Launcher (70% Explosion Chance)" }; CmbSubTech3.SelectedIndex = 0; }

                if (TxtProjectName != null) TxtProjectName.Text = "Size 6 Missile Launcher";
            }
            else if (selectedCat.Contains("Gauss Cannon"))
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Precisión vs Tamaño (Gauss Size vs Accuracy):";
                if (LblSubTech2 != null) LblSubTech2.Text = "Tasa de Disparo (Rate of Fire):";
                if (LblSubTech3 != null) LblSubTech3.Text = "Montaje de Torreta / Caza:";

                if (LblParam1Title != null) LblParam1Title.Text = "Número de Cañones (Barrels):";
                if (PnlParam2Container != null) PnlParam2Container.Visibility = Visibility.Collapsed;

                var gaussTechs = _researchedTechs.Where(t => t.TechTypeID == 143 || t.Name.Contains("Gauss")).ToList();
                if (gaussTechs.Count == 0) gaussTechs.Add(new ResearchedTechItem { TechID = 52, Name = "Gauss Cannon Size vs Accuracy 1HS and 17%" });
                CmbSubTech1.ItemsSource = gaussTechs; CmbSubTech1.SelectedIndex = 0;

                if (CmbSubTech2 != null) { CmbSubTech2.ItemsSource = new List<string> { "1 Disparo / 5s", "2 Disparos / 5s", "4 Disparos / 5s" }; CmbSubTech2.SelectedIndex = 0; }
                if (CmbSubTech3 != null) { CmbSubTech3.ItemsSource = new List<string> { "Montaje Estándar", "Torreta Punto de Defensa (PD)" }; CmbSubTech3.SelectedIndex = 0; }

                if (TxtProjectName != null) TxtProjectName.Text = "Gauss Cannon R3-100";
            }
            else if (selectedCat.Contains("Shield"))
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Tasa de Regeneración de Escudos (Regeneration Rate):";
                if (LblSubTech2 != null) LblSubTech2.Visibility = Visibility.Collapsed;
                if (CmbSubTech2 != null) CmbSubTech2.Visibility = Visibility.Collapsed;
                if (LblSubTech3 != null) LblSubTech3.Visibility = Visibility.Collapsed;
                if (CmbSubTech3 != null) CmbSubTech3.Visibility = Visibility.Collapsed;

                if (LblParam1Title != null) LblParam1Title.Text = "Tamaño del Generador de Escudos (HS / Tons):";
                if (PnlParam2Container != null) PnlParam2Container.Visibility = Visibility.Collapsed;

                var shieldTechs = _researchedTechs.Where(t => t.TechTypeID == 215 || t.Name.Contains("Shield")).ToList();
                if (shieldTechs.Count == 0) shieldTechs.Add(new ResearchedTechItem { TechID = 60, Name = "Shield Regeneration Rate 1" });
                CmbSubTech1.ItemsSource = shieldTechs; CmbSubTech1.SelectedIndex = 0;

                if (TxtProjectName != null) TxtProjectName.Text = "Alpha Shield Generator MK-I";
            }
            else if (selectedCat.Contains("Power Plant") || selectedCat.Contains("Reactor"))
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Tecnología de Reactor (Power Plant Tech):";
                if (LblSubTech2 != null) LblSubTech2.Visibility = Visibility.Collapsed;
                if (CmbSubTech2 != null) CmbSubTech2.Visibility = Visibility.Collapsed;
                if (LblSubTech3 != null) LblSubTech3.Visibility = Visibility.Collapsed;
                if (CmbSubTech3 != null) CmbSubTech3.Visibility = Visibility.Collapsed;

                if (LblParam1Title != null) LblParam1Title.Text = "Capacidad de Producción de Energía (EU):";
                if (PnlParam2Container != null) PnlParam2Container.Visibility = Visibility.Collapsed;

                var reactorTechs = _researchedTechs.Where(t => t.Name.Contains("Reactor") || t.Name.Contains("Power Plant")).ToList();
                if (reactorTechs.Count == 0) reactorTechs.Add(new ResearchedTechItem { TechID = 70, Name = "Conventional Reactor R1" });
                CmbSubTech1.ItemsSource = reactorTechs; CmbSubTech1.SelectedIndex = 0;

                if (TxtProjectName != null) TxtProjectName.Text = "Fission Power Plant R10";
            }
            else if (selectedCat.Contains("Jump Engine"))
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Tipo de Motor de Salto (Jump Drive Tech):";
                if (LblSubTech2 != null) LblSubTech2.Visibility = Visibility.Collapsed;
                if (CmbSubTech2 != null) CmbSubTech2.Visibility = Visibility.Collapsed;
                if (LblSubTech3 != null) LblSubTech3.Visibility = Visibility.Collapsed;
                if (CmbSubTech3 != null) CmbSubTech3.Visibility = Visibility.Collapsed;

                if (LblParam1Title != null) LblParam1Title.Text = "Masa Máxima de Nave para Salto (Tons):";
                if (PnlParam2Container != null) PnlParam2Container.Visibility = Visibility.Collapsed;

                var jumpTechs = _researchedTechs.Where(t => t.TechTypeID == 169 || t.Name.Contains("Jump")).ToList();
                if (jumpTechs.Count == 0) jumpTechs.Add(new ResearchedTechItem { TechID = 80, Name = "Military Jump Drive" });
                CmbSubTech1.ItemsSource = jumpTechs; CmbSubTech1.SelectedIndex = 0;

                if (TxtProjectName != null) TxtProjectName.Text = "Military Jump Drive J10k-50";
            }
            else if (selectedCat.Contains("Magazine"))
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Neutralización de Pañol (Neutralization System):";
                if (LblSubTech2 != null) LblSubTech2.Text = "Alimentación de Munición (Feed Efficiency):";
                if (LblSubTech3 != null) LblSubTech3.Visibility = Visibility.Collapsed;
                if (CmbSubTech3 != null) CmbSubTech3.Visibility = Visibility.Collapsed;

                if (LblParam1Title != null) LblParam1Title.Text = "Capacidad de Almacenamiento (MSP):";
                if (PnlParam2Container != null) PnlParam2Container.Visibility = Visibility.Collapsed;

                var magTechs = _researchedTechs.Where(t => t.TechTypeID == 167 || t.Name.Contains("Magazine")).ToList();
                if (magTechs.Count == 0) magTechs.Add(new ResearchedTechItem { TechID = 85, Name = "Magazine Neutralization System - 70% Chance" });
                CmbSubTech1.ItemsSource = magTechs; CmbSubTech1.SelectedIndex = 0;

                if (CmbSubTech2 != null) { CmbSubTech2.ItemsSource = new List<string> { "Alimentación Estándar 75%", "Alimentación Avanzada 90%" }; CmbSubTech2.SelectedIndex = 0; }

                if (TxtProjectName != null) TxtProjectName.Text = "Ordnance Magazine 100 MSP";
            }
            else if (selectedCat.Contains("Cloak"))
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Eficiencia de Dispositivo de Sigilo (Cloak Tech):";
                if (LblSubTech2 != null) LblSubTech2.Visibility = Visibility.Collapsed;
                if (CmbSubTech2 != null) CmbSubTech2.Visibility = Visibility.Collapsed;
                if (LblSubTech3 != null) LblSubTech3.Visibility = Visibility.Collapsed;
                if (CmbSubTech3 != null) CmbSubTech3.Visibility = Visibility.Collapsed;

                if (LblParam1Title != null) LblParam1Title.Text = "Capacidad Máxima de Casco (HS):";
                if (PnlParam2Container != null) PnlParam2Container.Visibility = Visibility.Collapsed;

                var cloakTechs = _researchedTechs.Where(t => t.TechTypeID == 46 || t.Name.Contains("Cloak")).ToList();
                if (cloakTechs.Count == 0) cloakTechs.Add(new ResearchedTechItem { TechID = 90, Name = "Standard Cloaking Efficiency 80%" });
                CmbSubTech1.ItemsSource = cloakTechs; CmbSubTech1.SelectedIndex = 0;

                if (TxtProjectName != null) TxtProjectName.Text = "Active Cloaking Field Device MK-I";
            }
            else if (selectedCat.Contains("CIWS"))
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Velocidad de Seguimiento CIWS:";
                if (LblSubTech2 != null) LblSubTech2.Text = "Sistema de Control de Tiro CIWS:";
                if (LblSubTech3 != null) LblSubTech3.Visibility = Visibility.Collapsed;
                if (CmbSubTech3 != null) CmbSubTech3.Visibility = Visibility.Collapsed;

                if (LblParam1Title != null) LblParam1Title.Text = "Número de Montajes de Cañón:";
                if (PnlParam2Container != null) PnlParam2Container.Visibility = Visibility.Collapsed;

                var ciwsTechs = _researchedTechs.Where(t => t.TechTypeID == 43 || t.Name.Contains("CIWS")).ToList();
                if (ciwsTechs.Count == 0) ciwsTechs.Add(new ResearchedTechItem { TechID = 95, Name = "CIWS Speed Rating 5,000 km/s" });
                CmbSubTech1.ItemsSource = ciwsTechs; CmbSubTech1.SelectedIndex = 0;

                if (CmbSubTech2 != null) { CmbSubTech2.ItemsSource = new List<string> { "Phalanx Standard CIWS", "Advanced Automated Dual CIWS" }; CmbSubTech2.SelectedIndex = 0; }

                if (TxtProjectName != null) TxtProjectName.Text = "Phalanx Automated CIWS-S5000";
            }
            else
            {
                if (LblSubTech1 != null) LblSubTech1.Text = "Tecnología Base Investigada (AuroraDB.db):";
                if (LblSubTech2 != null) LblSubTech2.Text = "Modificador de Eficiencia:";
                if (LblSubTech3 != null) LblSubTech3.Text = "Módulo de Control Electrónico:";

                if (LblParam1Title != null) LblParam1Title.Text = "Tamaño del Componente (HS / Tons):";
                if (LblParam2Title != null) LblParam2Title.Text = "Modificador de Rendimiento / Calidad:";

                var generalTechs = _researchedTechs.Where(t => CategoryMatches(t, selectedCat)).ToList();
                if (generalTechs.Count == 0) generalTechs.Add(new ResearchedTechItem { TechID = 99, Name = $"Standard {selectedCat} Tech" });
                CmbSubTech1.ItemsSource = generalTechs; CmbSubTech1.SelectedIndex = 0;

                if (CmbSubTech2 != null) { CmbSubTech2.ItemsSource = new List<string> { "Estándar 100%", "Mejorado 120%", "Optimizado 150%" }; CmbSubTech2.SelectedIndex = 0; }
                if (CmbSubTech3 != null) { CmbSubTech3.ItemsSource = new List<string> { "Básico (Sin ECCM)", "ECCM-1", "ECCM-2 Avanzado" }; CmbSubTech3.SelectedIndex = 0; }

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

        private void LoadCompanyNames()
        {
            if (_dbService == null || CmbCompanyName == null) return;
            var companies = _dbService.GetCompanyNames(_currentRaceId);
            CmbCompanyName.ItemsSource = companies;
            if (companies.Count > 0) CmbCompanyName.SelectedIndex = 0;
        }

        private void CmbCompanyName_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private void BtnApplyCompany_Click(object sender, RoutedEventArgs e)
        {
            if (CmbCompanyName?.SelectedItem == null || TxtProjectName == null) return;
            string company = CmbCompanyName.SelectedItem.ToString()!;
            if (string.IsNullOrWhiteSpace(company) || company == "(Ninguna)") return;

            string currentName = TxtProjectName.Text;
            if (!currentName.StartsWith(company))
            {
                TxtProjectName.Text = $"{company} {currentName.Trim()}";
            }
        }

        private void LoadEmpireNamingThemes()
        {
            if (_dbService == null) return;

            var themes = _dbService.GetNamingThemes();
            var config = _dbService.GetEmpireNamingConfig(_currentRaceId);

            PopulateThemeCombo(CmbClassTheme, themes, config.ClassThemeID);
            PopulateThemeCombo(CmbSystemTheme, themes, config.SystemThemeID);
            PopulateThemeCombo(CmbDesignTheme, themes, config.DesignThemeID);
            PopulateThemeCombo(CmbGroundTheme, themes, config.GroundThemeID);
            PopulateThemeCombo(CmbMissileTheme, themes, config.MissileThemeID);
            PopulateThemeCombo(CmbNameTheme, themes, config.NameThemeID);
        }

        private void PopulateThemeCombo(ComboBox? combo, List<NamingThemeItem> themes, int currentThemeId)
        {
            if (combo == null) return;
            combo.ItemsSource = themes;
            var match = themes.FirstOrDefault(t => t.ThemeID == currentThemeId);
            if (match != null)
            {
                combo.SelectedItem = match;
            }
            else if (themes.Count > 0)
            {
                combo.SelectedIndex = 0;
            }
        }

        private void BtnSaveEmpireNamingConfig_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null) return;

            var config = new EmpireNamingConfig
            {
                RaceID = _currentRaceId,
                ClassThemeID = CmbClassTheme?.SelectedItem is NamingThemeItem c ? c.ThemeID : 0,
                SystemThemeID = CmbSystemTheme?.SelectedItem is NamingThemeItem s ? s.ThemeID : 0,
                DesignThemeID = CmbDesignTheme?.SelectedItem is NamingThemeItem d ? d.ThemeID : 0,
                GroundThemeID = CmbGroundTheme?.SelectedItem is NamingThemeItem g ? g.ThemeID : 0,
                MissileThemeID = CmbMissileTheme?.SelectedItem is NamingThemeItem m ? m.ThemeID : 0,
                NameThemeID = CmbNameTheme?.SelectedItem is NamingThemeItem n ? n.ThemeID : 0
            };

            bool success = _dbService.SaveEmpireNamingConfig(_currentRaceId, config, out string msg);
            if (success)
            {
                MessageBox.Show(msg, "Lotes de Nombres Actualizados", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
