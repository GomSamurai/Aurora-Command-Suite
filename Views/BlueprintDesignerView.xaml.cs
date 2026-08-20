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
            AddPreset(ref idx, "🚀 DDG Artemis - Destructor Lanzamisiles de Flota (9,000 t)", "1. Destructores (DD / DDG / PD)");
            AddPreset(ref idx, "🛡️ DD-PD Aegis-G - Destructor Escolta Defensa de Punto (8,500 t)", "1. Destructores (DD / DDG / PD)");
            AddPreset(ref idx, "💥 DD-Beam Lancer - Destructor Láser Espinal (9,500 t)", "1. Destructores (DD / DDG / PD)");
            AddPreset(ref idx, "🌌 DDJ Pathbreaker - Destructor Líder de Salto (10,000 t)", "1. Destructores (DD / DDG / PD)");
            AddPreset(ref idx, "📦 DDB Barrage - Destructor Emboscada Box Launchers (8,000 t)", "1. Destructores (DD / DDG / PD)");

            // 2. Cruceros
            AddPreset(ref idx, "💥 CA Vindicator - Crucero Pesado Beam de Línea (20,000 t)", "2. Cruceros (CA / CC / CG / BC)");
            AddPreset(ref idx, "📡 CC Oracle - Crucero de Mando y Sensores AWACS (18,000 t)", "2. Cruceros (CA / CC / CG / BC)");
            AddPreset(ref idx, "🚀 CG Titan - Crucero Lanzamisiles Pesado (22,000 t)", "2. Cruceros (CA / CC / CG / BC)");
            AddPreset(ref idx, "⚡ BC Stalker - Crucero de Batalla Rápido (25,000 t)", "2. Cruceros (CA / CC / CG / BC)");
            AddPreset(ref idx, "🌌 CJ Aether Gate - Crucero de Salto de Flota (25,000 t)", "2. Cruceros (CA / CC / CG / BC)");

            // 3. Portaaviones
            AddPreset(ref idx, "🚢 CV Valhalla - Superportaaviones de Flota (40,000 t)", "3. Portaaviones (CV / CVL / CVE)");
            AddPreset(ref idx, "🛡️ CVL Dauntless - Portaaviones Ligero de Escolta (15,000 t)", "3. Portaaviones (CV / CVL / CVE)");
            AddPreset(ref idx, "🐝 CVE Hive - Nodriza de Lanchas de Asalto / FAC Tender (25,000 t)", "3. Portaaviones (CV / CVL / CVE)");
            AddPreset(ref idx, "🏰 CVB Iron Haven - Portaaviones Blindado de Asalto (30,000 t)", "3. Portaaviones (CV / CVL / CVE)");
            AddPreset(ref idx, "🤖 CVD Nexus - Portadrones Automatizado (10,000 t)", "3. Portaaviones (CV / CVL / CVE)");

            // 4. Cazas y Parásitas
            AddPreset(ref idx, "🐝 F-01 Hornet - Caza Interceptor Gauss (250 t)", "4. Cazas y Naves Parásitas");
            AddPreset(ref idx, "🚀 B-01 Viper - Caza Torpedeo Misiles (500 t)", "4. Cazas y Naves Parásitas");
            AddPreset(ref idx, "💥 FB-02 Sabre - Caza Cañonero Beam (500 t)", "4. Cazas y Naves Parásitas");
            AddPreset(ref idx, "🔍 R-01 Eye - Caza de Reconocimiento Pasivo (200 t)", "4. Cazas y Naves Parásitas");
            AddPreset(ref idx, "💣 D-01 Wasp - Drone Kamikaze / Señuelo (150 t)", "4. Cazas y Naves Parásitas");

            // 5. Exploración y Ciencia
            AddPreset(ref idx, "🌌 ES-Grav Compass - Explorador Gravitacional Rápido (3,000 t)", "5. Exploración y Ciencia");
            AddPreset(ref idx, "🔍 ES-Geo Prospector - Explorador Geológico Celular (3,000 t)", "5. Exploración y Ciencia");
            AddPreset(ref idx, "🧭 GSV Pathfinder - Explorador Combinado de Largo Alcance (7,000 t)", "5. Exploración y Ciencia");
            AddPreset(ref idx, "🛰️ Probe - Lancha Científica Parásita (300 t)", "5. Exploración y Ciencia");
            AddPreset(ref idx, "📡 SV-Scout Sentinel - Piquete Científico Alerta Temprana (4,000 t)", "5. Exploración y Ciencia");

            // 6. Petroleros y Reabastecimiento
            AddPreset(ref idx, "⚡ AOF Endurance - Petrolero de Flota Rápido (25,000 t)", "6. Petroleros y Reabastecimiento");
            AddPreset(ref idx, "⛽ AO Prometheus - Supertanquero Comercial Estratégico (60,000 t)", "6. Petroleros y Reabastecimiento");
            AddPreset(ref idx, "🪐 AOH Harvester Primus - Cosechadora de Sorium Orbital (50,000 t)", "6. Petroleros y Reabastecimiento");
            AddPreset(ref idx, "📦 AOR Atlas Fleet - Aprovisionamiento Logístico Combinado (35,000 t)", "6. Petroleros y Reabastecimiento");
            AddPreset(ref idx, "🏰 Safehaven - Estación Tanque de Almacenamiento (80,000 t)", "6. Petroleros y Reabastecimiento");

            // 7. Naves de Misiles y Asedio
            AddPreset(ref idx, "💣 BBG Nemesis - Monitor de Misiles Asedio Planetario (20,000 t)", "7. Naves de Misiles y Asedio");
            AddPreset(ref idx, "🛡️ FFG-AMM Shield - Fragata Antimisil Cobertura Área (6,000 t)", "7. Naves de Misiles y Asedio");
            AddPreset(ref idx, "🚢 ML Trapdoor - Crucero Lanzaminas Espacial (12,000 t)", "7. Naves de Misiles y Asedio");
            AddPreset(ref idx, "🚀 CG Universal - Crucero VLS Multipropósito (15,000 t)", "7. Naves de Misiles y Asedio");
            AddPreset(ref idx, "👻 SSG Shadow - Submarino Espacial Sigiloso de Misiles (8,000 t)", "7. Naves de Misiles y Asedio");

            // 8. Combate Compacto y Corbetas
            AddPreset(ref idx, "⚡ FAC Strikefast - Lancha Torpedera Rápida (1,000 t)", "8. Combate Compacto y Corbetas");
            AddPreset(ref idx, "💥 Gunboat Warp Hammer - Cañonera Mesón de Salto (1,200 t)", "8. Combate Compacto y Corbetas");
            AddPreset(ref idx, "🛡️ Corvette Gargoyle - Corbeta Escolta Gauss (2,500 t)", "8. Combate Compacto y Corbetas");
            AddPreset(ref idx, "👮 Corvette Watchman - Corbeta Patrullera de Frontera (3,000 t)", "8. Combate Compacto y Corbetas");
            AddPreset(ref idx, "👻 Raider Specter - Cazador Sigiloso de Emboscada (2,000 t)", "8. Combate Compacto y Corbetas");

            // 9. Formaciones Terrestres
            AddPreset(ref idx, "🪖 Marine Strike Battalion - Asalto Planetario Helitransportado", "9. Formaciones Terrestres");
            AddPreset(ref idx, "🏛️ Garrison Battalion - Infantería de Guarnición y Policía", "9. Formaciones Terrestres");
            AddPreset(ref idx, "🚜 Heavy Armor Regiment - Regimiento Blindado de Ruptura", "9. Formaciones Terrestres");
            AddPreset(ref idx, "💣 Bombardment Artillery Element - Batería Artillería Pesada", "9. Formaciones Terrestres");
            AddPreset(ref idx, "🛡️ Air & Orbital Defense Battery - Batería Antiaérea / Antidroga", "9. Formaciones Terrestres");

            // 10. Logística Modular
            AddPreset(ref idx, "📦 Freighter Atlas Heavy - Carguero Modular Pesado (50,000 t)", "10. Logística Modular");
            AddPreset(ref idx, "🪖 Troop Transport Colossus - Transporte Tropas Invasión (30,000 t)", "10. Logística Modular");
            AddPreset(ref idx, "⛏️ Mining Ship Titan Core - Minero Espacial de Asteroides (60,000 t)", "10. Logística Modular");
            AddPreset(ref idx, "🚜 Tugboat Hercules - Remolcador Espacial Despliegue Rápido (20,000 t)", "10. Logística Modular");
            AddPreset(ref idx, "🛠️ Maint Vessel Hephaestus - Taller Orbital y Dique Móvil (50,000 t)", "10. Logística Modular");

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
                    UserData = up
                });
            }
        }

        private void AddPreset(ref int index, string title, string category)
        {
            _allPresetsList.Add(new PresetItem { Index = index++, Title = title, Category = category });
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
            if (CmbPresets.SelectedItem is not PresetItem preset || _allComponents.Count == 0) return;

            _selectedComponents.Clear();

            if (preset.IsUserPreset && preset.UserData != null)
            {
                LoadUserPresetData(preset.UserData);
                return;
            }

            int idx = preset.Index;

            var commEng = _allComponents.FirstOrDefault(c => c.ComponentName.ToLower().Contains("commercial")) ?? 
                          _allComponents.FirstOrDefault(c => c.TypeName == "Engine");
            var milEng = _allComponents.FirstOrDefault(c => c.TypeName == "Engine" && !c.ComponentName.ToLower().Contains("commercial")) ?? 
                         _allComponents.FirstOrDefault(c => c.TypeName == "Engine");

            var stdFuel = _allComponents.FirstOrDefault(c => c.TypeName == "Fuel" && c.ComponentSize <= 2) ?? 
                          _allComponents.FirstOrDefault(c => c.TypeName == "Fuel");
            var lrgFuel = _allComponents.FirstOrDefault(c => c.TypeName == "Fuel" && c.ComponentSize >= 5) ?? stdFuel;

            var laser = _allComponents.FirstOrDefault(c => c.TypeName.Contains("Beam") || c.TypeName.Contains("Weapon") || c.TypeName.Contains("Laser"));
            var sensor = _allComponents.FirstOrDefault(c => c.TypeName.Contains("Sensor") || c.TypeName.Contains("Active"));
            var shield = _allComponents.FirstOrDefault(c => c.TypeName.Contains("Shield"));
            var jump = _allComponents.FirstOrDefault(c => c.TypeName.Contains("Jump"));
            var mag = _allComponents.FirstOrDefault(c => c.TypeName.Contains("Magazine") || c.TypeName.Contains("Launcher"));

            switch (idx)
            {
                case 0: // Carguero Comercial Estándar
                    TxtClassName.Text = "Carguero Comercial Estándar MK-I";
                    TxtArmorThickness.Text = "1";
                    TxtArmorWidth.Text = "10";
                    if (commEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = commEng, Quantity = 4 });
                    if (lrgFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = lrgFuel, Quantity = 8 });
                    break;

                case 1: // Carguero de Colonias
                    TxtClassName.Text = "Transporte de Colonos Horizonte";
                    TxtArmorThickness.Text = "2";
                    TxtArmorWidth.Text = "12";
                    if (commEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = commEng, Quantity = 6 });
                    if (lrgFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = lrgFuel, Quantity = 12 });
                    break;

                case 2: // Estación Minera Orbital Vulcano
                    TxtClassName.Text = "Estación Minera Orbital Vulcano";
                    TxtArmorThickness.Text = "2";
                    TxtArmorWidth.Text = "14";
                    if (commEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = commEng, Quantity = 2 });
                    if (lrgFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = lrgFuel, Quantity = 6 });
                    break;

                case 3: // Refinería Móvil y Tanquero
                    TxtClassName.Text = "Nave Refinería de Sorium Prometeo";
                    TxtArmorThickness.Text = "1";
                    TxtArmorWidth.Text = "12";
                    if (commEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = commEng, Quantity = 4 });
                    if (lrgFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = lrgFuel, Quantity = 16 });
                    break;

                case 4: // Buque Geológico de Exploración
                    TxtClassName.Text = "Buque Geológico de Exploración Vigía";
                    TxtArmorThickness.Text = "2";
                    TxtArmorWidth.Text = "8";
                    if (milEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = milEng, Quantity = 2 });
                    if (lrgFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = lrgFuel, Quantity = 4 });
                    if (sensor != null) _selectedComponents.Add(new SelectedComponentItem { Component = sensor, Quantity = 1 });
                    break;

                case 5: // Buque Gravitacional de Salto
                    TxtClassName.Text = "Explorador de Saltos Nebulosa";
                    TxtArmorThickness.Text = "2";
                    TxtArmorWidth.Text = "10";
                    if (milEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = milEng, Quantity = 3 });
                    if (lrgFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = lrgFuel, Quantity = 5 });
                    if (jump != null) _selectedComponents.Add(new SelectedComponentItem { Component = jump, Quantity = 1 });
                    break;

                case 6: // Buque de Rescate y Salvamento
                    TxtClassName.Text = "Remolcador de Salvamento Érido";
                    TxtArmorThickness.Text = "2";
                    TxtArmorWidth.Text = "12";
                    if (commEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = commEng, Quantity = 4 });
                    if (lrgFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = lrgFuel, Quantity = 10 });
                    break;

                case 7: // Destructor de Escolta Picket
                    TxtClassName.Text = "Destructor de Escolta Clase Vanguardia";
                    TxtArmorThickness.Text = "4";
                    TxtArmorWidth.Text = "12";
                    if (milEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = milEng, Quantity = 4 });
                    if (lrgFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = lrgFuel, Quantity = 6 });
                    if (laser != null) _selectedComponents.Add(new SelectedComponentItem { Component = laser, Quantity = 2 });
                    if (sensor != null) _selectedComponents.Add(new SelectedComponentItem { Component = sensor, Quantity = 1 });
                    break;

                case 8: // Fragata Lanzamisiles Ligera
                    TxtClassName.Text = "Fragata Lanzamisiles Relámpago";
                    TxtArmorThickness.Text = "3";
                    TxtArmorWidth.Text = "10";
                    if (milEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = milEng, Quantity = 3 });
                    if (lrgFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = lrgFuel, Quantity = 5 });
                    if (mag != null) _selectedComponents.Add(new SelectedComponentItem { Component = mag, Quantity = 2 });
                    if (sensor != null) _selectedComponents.Add(new SelectedComponentItem { Component = sensor, Quantity = 1 });
                    break;

                case 9: // Crucero Pesado de Haz Láser
                    TxtClassName.Text = "Crucero Pesado de Batalla Leviatán";
                    TxtArmorThickness.Text = "6";
                    TxtArmorWidth.Text = "16";
                    if (milEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = milEng, Quantity = 6 });
                    if (lrgFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = lrgFuel, Quantity = 10 });
                    if (laser != null) _selectedComponents.Add(new SelectedComponentItem { Component = laser, Quantity = 4 });
                    if (shield != null) _selectedComponents.Add(new SelectedComponentItem { Component = shield, Quantity = 2 });
                    break;

                case 10: // Fragata Anti-Misil / Defensa de Punto
                    TxtClassName.Text = "Fragata de Defensa de Punto Guardián";
                    TxtArmorThickness.Text = "3";
                    TxtArmorWidth.Text = "10";
                    if (milEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = milEng, Quantity = 3 });
                    if (lrgFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = lrgFuel, Quantity = 4 });
                    if (laser != null) _selectedComponents.Add(new SelectedComponentItem { Component = laser, Quantity = 3 });
                    if (sensor != null) _selectedComponents.Add(new SelectedComponentItem { Component = sensor, Quantity = 1 });
                    break;

                case 11: // Corbeta Sigilosa Stealth
                    TxtClassName.Text = "Corbeta de Infiltración Sombra";
                    TxtArmorThickness.Text = "2";
                    TxtArmorWidth.Text = "8";
                    if (milEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = milEng, Quantity = 2 });
                    if (stdFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = stdFuel, Quantity = 4 });
                    if (sensor != null) _selectedComponents.Add(new SelectedComponentItem { Component = sensor, Quantity = 1 });
                    break;

                case 12: // Nieve de Reconocimiento ELINT
                    TxtClassName.Text = "Piquete de Inteligencia Espectro";
                    TxtArmorThickness.Text = "2";
                    TxtArmorWidth.Text = "6";
                    if (milEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = milEng, Quantity = 2 });
                    if (stdFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = stdFuel, Quantity = 3 });
                    if (sensor != null) _selectedComponents.Add(new SelectedComponentItem { Component = sensor, Quantity = 2 });
                    break;

                case 13: // Portanaves Escolta
                    TxtClassName.Text = "Portanaves Escolta Olympus";
                    TxtArmorThickness.Text = "4";
                    TxtArmorWidth.Text = "18";
                    if (commEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = commEng, Quantity = 6 });
                    if (lrgFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = lrgFuel, Quantity = 12 });
                    break;

                case 14: // Caza Estelar Interceptor
                    TxtClassName.Text = "Caza Interceptor Halcón 250";
                    TxtArmorThickness.Text = "1";
                    TxtArmorWidth.Text = "4";
                    if (milEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = milEng, Quantity = 1 });
                    if (stdFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = stdFuel, Quantity = 1 });
                    if (laser != null) _selectedComponents.Add(new SelectedComponentItem { Component = laser, Quantity = 1 });
                    break;

                case 15: // Bombardero Espacial
                    TxtClassName.Text = "Bombardero Táctico Trueno";
                    TxtArmorThickness.Text = "1";
                    TxtArmorWidth.Text = "6";
                    if (milEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = milEng, Quantity = 1 });
                    if (stdFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = stdFuel, Quantity = 2 });
                    if (mag != null) _selectedComponents.Add(new SelectedComponentItem { Component = mag, Quantity = 1 });
                    break;

                case 16: // Barcaza de Desembarco de Tropas
                    TxtClassName.Text = "Transporte de Tropas Mirmidón";
                    TxtArmorThickness.Text = "4";
                    TxtArmorWidth.Text = "14";
                    if (commEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = commEng, Quantity = 4 });
                    if (lrgFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = lrgFuel, Quantity = 8 });
                    break;

                case 17: // Monitor Defensivo Bastión de Hierro
                    TxtClassName.Text = "Monitor Defensivo Bastión de Hierro";
                    TxtArmorThickness.Text = "10";
                    TxtArmorWidth.Text = "24";
                    if (commEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = commEng, Quantity = 2 });
                    if (lrgFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = lrgFuel, Quantity = 8 });
                    if (laser != null) _selectedComponents.Add(new SelectedComponentItem { Component = laser, Quantity = 6 });
                    if (shield != null) _selectedComponents.Add(new SelectedComponentItem { Component = shield, Quantity = 4 });
                    if (sensor != null) _selectedComponents.Add(new SelectedComponentItem { Component = sensor, Quantity = 2 });
                    break;

                case 18: // Puesto de Escucha y Alerta Temprana
                    TxtClassName.Text = "Estación de Alerta Temprana Ojo Celestial";
                    TxtArmorThickness.Text = "3";
                    TxtArmorWidth.Text = "12";
                    if (commEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = commEng, Quantity = 1 });
                    if (stdFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = stdFuel, Quantity = 4 });
                    if (sensor != null) _selectedComponents.Add(new SelectedComponentItem { Component = sensor, Quantity = 3 });
                    break;

                case 19: // Fortaleza Planetaria de Escudos
                    TxtClassName.Text = "Fortaleza Defensiva Aegis Prime";
                    TxtArmorThickness.Text = "12";
                    TxtArmorWidth.Text = "28";
                    if (commEng != null) _selectedComponents.Add(new SelectedComponentItem { Component = commEng, Quantity = 2 });
                    if (lrgFuel != null) _selectedComponents.Add(new SelectedComponentItem { Component = lrgFuel, Quantity = 10 });
                    if (shield != null) _selectedComponents.Add(new SelectedComponentItem { Component = shield, Quantity = 8 });
                    if (laser != null) _selectedComponents.Add(new SelectedComponentItem { Component = laser, Quantity = 8 });
                    break;
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

            var userPreset = new UserPresetData
            {
                PresetName = presetName,
                ClassName = presetName,
                PlannedDeploymentMonths = Math.Max(1, depM),
                ArmorThickness = Math.Max(1, armorT),
                ArmorWidth = Math.Max(1, armorW),
                IsMilitary = CurrentDesign.IsMilitary,
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
                CmbPresetCategoryFilter.SelectedIndex = 5; // Select "💾 Diseños del Usuario"
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

        public void Recalculate()
        {
            if (TxtClassName == null) return;

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

            // Update Visual Ship Scale Silhouette Bar
            if (BdrVisualShipSilhouette != null && TxtVisualShipTonnage != null && LblVisualScaleClass != null)
            {
                double tons = CurrentDesign.TotalTonnage;
                double hs = CurrentDesign.TotalHS;

                string hullCategory = "Corbeta";
                if (tons >= 100000) hullCategory = "Acorazado Estelar / Súper-Nave";
                else if (tons >= 50000) hullCategory = "Crucero Pesado / Dreadnought";
                else if (tons >= 25000) hullCategory = "Crucero Ligero / Batalla";
                else if (tons >= 10000) hullCategory = "Destructor Escuadra";
                else if (tons >= 5000) hullCategory = "Fragata de Escolta";
                else if (tons >= 2000) hullCategory = "Corbeta / Cañonera";
                else hullCategory = "Caza / Nave Ligera";

                LblVisualScaleClass.Text = $"Clase: {hullCategory} ({tons:N0}t / HS {hs:F0})";
                TxtVisualShipTonnage.Text = $"◄═══ {tons:N0} Tons (HS {hs:F0}) ═══►";

                double targetWidth = Math.Max(90, Math.Min(340, 90 + (Math.Min(tons, 100000) / 100000.0) * 250));
                BdrVisualShipSilhouette.Width = targetWidth;
            }

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
    }
}
