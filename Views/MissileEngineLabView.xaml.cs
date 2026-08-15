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

        private readonly List<MissilePresetInfo> _allMissilePresets = new List<MissilePresetInfo>();
        private readonly List<EnginePresetInfo> _allEnginePresets = new List<EnginePresetInfo>();

        public MissileEngineLabView()
        {
            InitializeComponent();
            InitializePresets();
            RecalculateMissile();
            RecalculateEngine();
        }

        private void InitializePresets()
        {
            // Missile Presets
            _allMissilePresets.Add(new MissilePresetInfo { Name = "Víbora MK-I Antinave", Category = "🚀 Misiles Antinave (ASM)", SizeMSP = 6.0, EnginePercent = 40.0, PowerMod = 2.0, WarheadMSP = 2.0, FuelMSP = 1.0, Agility = 5.0 });
            _allMissilePresets.Add(new MissilePresetInfo { Name = "Tiburón Martillo Heavy ASM", Category = "🚀 Misiles Antinave (ASM)", SizeMSP = 12.0, EnginePercent = 50.0, PowerMod = 2.5, WarheadMSP = 4.0, FuelMSP = 2.0, Agility = 8.0 });
            _allMissilePresets.Add(new MissilePresetInfo { Name = "Leviatán Torpedo Antinave", Category = "🚀 Misiles Antinave (ASM)", SizeMSP = 20.0, EnginePercent = 45.0, PowerMod = 3.0, WarheadMSP = 8.0, FuelMSP = 3.0, Agility = 10.0 });

            _allMissilePresets.Add(new MissilePresetInfo { Name = "Interceptor Relámpago AMM", Category = "🛡️ Misiles Antimisil (AMM)", SizeMSP = 1.0, EnginePercent = 60.0, PowerMod = 3.5, WarheadMSP = 0.25, FuelMSP = 0.15, Agility = 15.0 });
            _allMissilePresets.Add(new MissilePresetInfo { Name = "Centinela Defensivo MK-II", Category = "🛡️ Misiles Antimisil (AMM)", SizeMSP = 2.0, EnginePercent = 55.0, PowerMod = 3.0, WarheadMSP = 0.5, FuelMSP = 0.3, Agility = 12.0 });

            _allMissilePresets.Add(new MissilePresetInfo { Name = "Sonda Recon Cero", Category = "🔍 Sondas de Reconocimiento Cero", SizeMSP = 4.0, EnginePercent = 30.0, PowerMod = 1.0, WarheadMSP = 0.1, FuelMSP = 2.5, Agility = 2.0 });

            // Engine Presets
            _allEnginePresets.Add(new EnginePresetInfo { Name = "Impulso Ionico Militar 500", Category = "⚡ Motores Militares de Alta Velocidad", IsMilitary = true, SizeHS = 10.0, PowerMod = 1.25, ThermalReduction = 1.0 });
            _allEnginePresets.Add(new EnginePresetInfo { Name = "Motor Caza Interceptor Swift", Category = "⚡ Motores Militares de Alta Velocidad", IsMilitary = true, SizeHS = 3.0, PowerMod = 2.5, ThermalReduction = 1.0 });
            _allEnginePresets.Add(new EnginePresetInfo { Name = "Postquemador Plasma Leviatán", Category = "⚡ Motores Militares de Alta Velocidad", IsMilitary = true, SizeHS = 25.0, PowerMod = 2.0, ThermalReduction = 1.0 });

            _allEnginePresets.Add(new EnginePresetInfo { Name = "Carguero Estelar C-500", Category = "📦 Motores Comerciales de Alta Autonomía", IsMilitary = false, SizeHS = 20.0, PowerMod = 0.8, ThermalReduction = 1.0 });
            _allEnginePresets.Add(new EnginePresetInfo { Name = "Impulso Tanquero Sorium", Category = "📦 Motores Comerciales de Alta Autonomía", IsMilitary = false, SizeHS = 50.0, PowerMod = 0.5, ThermalReduction = 1.0 });

            _allEnginePresets.Add(new EnginePresetInfo { Name = "Impulso Térmico Silencioso", Category = "👻 Motores Sigilosos de Baja Emisión", IsMilitary = true, SizeHS = 8.0, PowerMod = 1.0, ThermalReduction = 0.3 });

            // Populate Missile categories
            var mCategories = new List<string> { "🚀 Misiles Antinave (ASM)", "🛡️ Misiles Antimisil (AMM)", "🔍 Sondas de Reconocimiento Cero", "👤 Presets del Usuario" };
            CmbMissileCategory.ItemsSource = mCategories;
            CmbMissileCategory.SelectedIndex = 0;

            // Populate Engine categories
            var eCategories = new List<string> { "⚡ Motores Militares de Alta Velocidad", "📦 Motores Comerciales de Alta Autonomía", "👻 Motores Sigilosos de Baja Emisión", "👤 Presets del Usuario" };
            CmbEngineCategory.ItemsSource = eCategories;
            CmbEngineCategory.SelectedIndex = 0;
        }

        public void LoadData(DatabaseService dbService, int raceId)
        {
            _dbService = dbService;
            _currentRaceId = raceId;
            RefreshCatalog();
        }

        private void RefreshCatalog()
        {
            if (_dbService == null) return;

            var missiles = _dbService.GetSavedMissiles(_currentRaceId);
            DgSavedMissiles.ItemsSource = missiles;

            var engines = _dbService.GetSavedEngines(_currentRaceId);
            DgSavedEngines.ItemsSource = engines;
        }

        private void OnModeChanged(object sender, RoutedEventArgs e)
        {
            if (PnlMissileDesigner == null || PnlEngineDesigner == null || PnlCatalog == null) return;

            if (BtnModeMissiles?.IsChecked == true)
            {
                PnlMissileDesigner.Visibility = Visibility.Visible;
                PnlEngineDesigner.Visibility = Visibility.Collapsed;
                PnlCatalog.Visibility = Visibility.Collapsed;
            }
            else if (BtnModeEngines?.IsChecked == true)
            {
                PnlMissileDesigner.Visibility = Visibility.Collapsed;
                PnlEngineDesigner.Visibility = Visibility.Visible;
                PnlCatalog.Visibility = Visibility.Collapsed;
            }
            else if (BtnModeCatalog?.IsChecked == true)
            {
                PnlMissileDesigner.Visibility = Visibility.Collapsed;
                PnlEngineDesigner.Visibility = Visibility.Collapsed;
                PnlCatalog.Visibility = Visibility.Visible;
                RefreshCatalog();
            }
        }

        // ==================== MISSILE PRESETS ====================
        private void CmbMissileCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbMissileCategory.SelectedItem is not string category) return;

            var filtered = _allMissilePresets.Where(p => p.Category == category).ToList();
            CmbMissilePreset.ItemsSource = filtered;
            if (filtered.Count > 0)
            {
                CmbMissilePreset.SelectedIndex = 0;
            }
        }

        private void CmbMissilePreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbMissilePreset.SelectedItem is not MissilePresetInfo preset) return;

            TxtMissileName.Text = preset.Name;
            SldMissileSize.Value = preset.SizeMSP;
            SldEngineSize.Value = preset.EnginePercent;
            SldPowerMod.Value = preset.PowerMod;
            SldWarhead.Value = preset.WarheadMSP;
            SldFuel.Value = preset.FuelMSP;
            SldAgility.Value = preset.Agility;

            RecalculateMissile();
        }

        private void BtnSaveUserMissilePreset_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtMissileName.Text.Trim();
            if (string.IsNullOrEmpty(name)) name = "Misil del Usuario";

            var preset = new MissilePresetInfo
            {
                Name = name,
                Category = "👤 Presets del Usuario",
                SizeMSP = SldMissileSize.Value,
                EnginePercent = SldEngineSize.Value,
                PowerMod = SldPowerMod.Value,
                WarheadMSP = SldWarhead.Value,
                FuelMSP = SldFuel.Value,
                Agility = SldAgility.Value
            };

            _allMissilePresets.Add(preset);
            CmbMissileCategory.SelectedIndex = 3; // Switch to User category
            MessageBox.Show($"⭐ Preset '{name}' guardado en tus plantillas de usuario.", "Preset Guardado", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ==================== ENGINE PRESETS ====================
        private void CmbEngineCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbEngineCategory.SelectedItem is not string category) return;

            var filtered = _allEnginePresets.Where(p => p.Category == category).ToList();
            CmbEnginePreset.ItemsSource = filtered;
            if (filtered.Count > 0)
            {
                CmbEnginePreset.SelectedIndex = 0;
            }
        }

        private void CmbEnginePreset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbEnginePreset.SelectedItem is not EnginePresetInfo preset) return;

            TxtEngineName.Text = preset.Name;
            if (preset.IsMilitary) RbEngMil.IsChecked = true;
            else RbEngComm.IsChecked = true;

            SldEngSize.Value = preset.SizeHS;
            SldEngPowerMod.Value = preset.PowerMod;
            SldEngThermal.Value = preset.ThermalReduction;

            RecalculateEngine();
        }

        private void BtnSaveUserEnginePreset_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtEngineName.Text.Trim();
            if (string.IsNullOrEmpty(name)) name = "Motor del Usuario";

            var preset = new EnginePresetInfo
            {
                Name = name,
                Category = "👤 Presets del Usuario",
                IsMilitary = RbEngMil.IsChecked == true,
                SizeHS = SldEngSize.Value,
                PowerMod = SldEngPowerMod.Value,
                ThermalReduction = SldEngThermal.Value
            };

            _allEnginePresets.Add(preset);
            CmbEngineCategory.SelectedIndex = 3; // Switch to User category
            MessageBox.Show($"⭐ Preset '{name}' guardado en tus plantillas de usuario.", "Preset Guardado", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ==================== MODE 1: MISSILE CALCULATOR ====================
        private void OnMissileParamChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RecalculateMissile();
        }

        private void RecalculateMissile()
        {
            if (LblValMissileSize == null || SldMissileSize == null || SldEngineSize == null || 
                SldPowerMod == null || SldWarhead == null || SldFuel == null || SldAgility == null) return;

            double msp = SldMissileSize.Value;
            double enginePercent = SldEngineSize.Value;
            double powerMod = SldPowerMod.Value;
            double warheadMsp = SldWarhead.Value;
            double fuelMsp = SldFuel.Value;
            double agility = SldAgility.Value;

            if (LblValMissileSize != null) LblValMissileSize.Text = $"{msp:F1} MSP";
            if (LblValEngineSize != null) LblValEngineSize.Text = $"{enginePercent:F0}%";
            if (LblValPowerMod != null) LblValPowerMod.Text = $"{powerMod:F2}x";
            if (LblValWarhead != null) LblValWarhead.Text = $"{warheadMsp:F1} MSP";
            if (LblValFuel != null) LblValFuel.Text = $"{fuelMsp:F1} MSP";
            if (LblValAgility != null) LblValAgility.Text = $"{agility:F0}";

            // Calculations
            double engineMsp = msp * (enginePercent / 100.0);
            double enginePower = engineMsp * 25.0 * powerMod; // Base engine tech output
            double missileSpeed = Math.Round((enginePower / Math.Max(0.5, msp)) * 1000.0, 0);

            double warheadDamage = Math.Round(warheadMsp * 4.0, 1);
            double fuelCapacityLiters = fuelMsp * 2500.0;
            double fuelBurnRatePerHour = enginePower * 0.1;
            double flightTimeHours = fuelBurnRatePerHour > 0 ? fuelCapacityLiters / fuelBurnRatePerHour : 0;
            double flightTimeSeconds = Math.Round(flightTimeHours * 3600.0, 0);
            double maxRangeMillionKm = Math.Round((missileSpeed * flightTimeSeconds) / 1_000.0, 2);

            double bpCost = Math.Round((msp * 0.5) + (warheadDamage * 0.2) + (powerMod * 2.0), 2);

            if (LblResSize != null) LblResSize.Text = $"{msp:F1} MSP";
            if (LblResSpeed != null) LblResSpeed.Text = $"{missileSpeed:N0} km/s";
            if (LblResDamage != null) LblResDamage.Text = $"{warheadDamage:F1} Dmg";
            if (LblResRange != null) LblResRange.Text = $"{maxRangeMillionKm:N2} Mill. km";
            if (LblResFlightTime != null) LblResFlightTime.Text = $"{flightTimeSeconds:N0} Segundos";
            if (LblResCost != null) LblResCost.Text = $"{bpCost:F2} BP";

            // Armor Penetration Crater Depth Profile (Aurora 4X square root damage penetration rule)
            double craterDepth = Math.Round(Math.Sqrt(warheadDamage), 1);
            if (LblCrater1 != null) LblCrater1.Text = craterDepth >= 1.0 ? $"Penetrante ({craterDepth:F1})" : "Absorbido";
            if (LblCrater2 != null) LblCrater2.Text = craterDepth >= 2.0 ? $"Penetrante ({craterDepth:F1})" : "Parcial";
            if (LblCrater4 != null) LblCrater4.Text = craterDepth >= 4.0 ? $"Penetrante ({craterDepth:F1})" : "Superficial";
            if (LblCrater8 != null) LblCrater8.Text = craterDepth >= 8.0 ? $"Penetrante ({craterDepth:F1})" : "Absorbido";
            if (LblCrater12 != null) LblCrater12.Text = craterDepth >= 12.0 ? $"Penetrante ({craterDepth:F1})" : "Absorbido";

            // Launcher & Magazine Logistics
            if (LblLauncherSize != null) LblLauncherSize.Text = $"{msp:F1} MSP ({msp * 50:N0} Tons)";
            if (LblReloadTime != null) LblReloadTime.Text = $"{Math.Round(msp * 5.0, 0)} Segundos";
            if (LblMagCapacity != null) LblMagCapacity.Text = $"{Math.Floor(100.0 / Math.Max(0.5, msp)):N0} Misiles";

            // Hit probability matrix vs target speeds
            UpdateHitMatrix(missileSpeed, agility, msp);
            UpdatePkSimulation(missileSpeed, agility, msp);

            // Minerals
            var minerals = new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>("Gallicite (Motores)", bpCost * 0.5),
                new KeyValuePair<string, double>("Uridium (Sensores/Agilidad)", bpCost * 0.3),
                new KeyValuePair<string, double>("Duranium (Cuerpo/Ojiva)", bpCost * 0.2)
            };
            if (IcMissileMinerals != null) IcMissileMinerals.ItemsSource = minerals;
        }

        private void UpdatePkSimulation(double missileSpeed, double agility, double msp)
        {
            if (LblPkSlow == null || LblPkStd == null || LblPkFast == null || LblPkAdvice == null) return;

            double maneuverRating = (agility * 10.0) / Math.Max(1.0, msp);

            double CalculatePk(double targetSpeed)
            {
                if (targetSpeed <= 0) return 100.0;
                double ratio = (missileSpeed + (maneuverRating * 1000.0)) / targetSpeed;
                return Math.Min(100.0, Math.Max(5.0, ratio * 100.0));
            }

            double pkSlow = CalculatePk(3000);
            double pkStd = CalculatePk(6000);
            double pkFast = CalculatePk(12000);

            LblPkSlow.Text = $"{pkSlow:F0}% Pk";
            LblPkStd.Text = $"{pkStd:F0}% Pk";
            LblPkFast.Text = $"{pkFast:F0}% Pk";

            if (pkFast >= 80)
            {
                LblPkAdvice.Text = "✨ ¡Excelente rendimiento táctico! El misil posee suficiente velocidad y agilidad para destruir interceptores de alta velocidad (12,000 km/s) con un Pk superior al 80%.";
            }
            else if (pkStd >= 80)
            {
                LblPkAdvice.Text = "⚡ Rendimiento Antinave Óptimo: Efectivo contra fragatas y cargueros pesados estándar. Aumenta la agilidad si buscas interceptar misiles enemigos AMM.";
            }
            else
            {
                LblPkAdvice.Text = "⚠️ Precaución: Probabilidad de impacto reducida contra blancos móviles. Sube el Modificador de Potencia o los Puntos de Agilidad.";
            }
        }

        private void UpdateHitMatrix(double missileSpeed, double agility, double msp)
        {
            double maneuverRating = (agility * 10.0) / Math.Max(1.0, msp);

            double CalculateHitRate(double targetSpeed)
            {
                if (targetSpeed <= 0) return 100.0;
                double ratio = (missileSpeed + (maneuverRating * 1000.0)) / targetSpeed;
                return Math.Min(100.0, Math.Max(5.0, ratio * 100.0));
            }

            if (LblHit2k != null) LblHit2k.Text = $"{CalculateHitRate(2000):F1}%";
            if (LblHit5k != null) LblHit5k.Text = $"{CalculateHitRate(5000):F1}%";
            if (LblHit10k != null) LblHit10k.Text = $"{CalculateHitRate(10000):F1}%";
            if (LblHit15k != null) LblHit15k.Text = $"{CalculateHitRate(15000):F1}%";
            if (LblHit20k != null) LblHit20k.Text = $"{CalculateHitRate(20000):F1}%";
        }

        private void BtnSaveMissile_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null)
            {
                MessageBox.Show("No se encontró conexión activa con la base de datos de Aurora 4X.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = TxtMissileName.Text.Trim();
            if (string.IsNullOrEmpty(name)) name = "Misil Táctico Personalizado";

            double msp = SldMissileSize.Value;
            double enginePercent = SldEngineSize.Value;
            double powerMod = SldPowerMod.Value;
            double warheadMsp = SldWarhead.Value;
            double fuelMsp = SldFuel.Value;

            double engineMsp = msp * (enginePercent / 100.0);
            double enginePower = engineMsp * 25.0 * powerMod;
            double speed = Math.Round((enginePower / Math.Max(0.5, msp)) * 1000.0, 0);
            double damage = Math.Round(warheadMsp * 4.0, 1);
            double rangeMillionKm = Math.Round(((speed * (fuelMsp * 2500.0 / (enginePower * 0.1) * 3600.0)) / 1_000.0), 2);
            double cost = Math.Round((msp * 0.5) + (damage * 0.2) + (powerMod * 2.0), 2);

            if (_dbService.SaveMissileDesign(_currentRaceId, name, msp, speed, damage, rangeMillionKm / 1000.0, cost, out string msg))
            {
                MessageBox.Show(msg, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshCatalog();
            }
            else
            {
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ==================== MODE 2: SHIP ENGINE CALCULATOR ====================
        private void OnEngineParamChanged(object sender, RoutedEventArgs e)
        {
            RecalculateEngine();
        }

        private void RecalculateEngine()
        {
            if (LblValEngSize == null || SldEngSize == null || SldEngPowerMod == null || SldEngThermal == null) return;

            bool isMilitary = RbEngMil?.IsChecked == true;
            double hs = SldEngSize.Value;
            double powerMod = SldEngPowerMod.Value;
            double thermalFactor = SldEngThermal.Value;

            if (LblValEngSize != null) LblValEngSize.Text = $"{hs:F1} HS ({hs * 50:N0} t)";
            if (LblValEngPowerMod != null) LblValEngPowerMod.Text = $"{powerMod:F2}x";
            if (LblValEngThermal != null) LblValEngThermal.Text = $"{thermalFactor * 100:F0}% Emisión";

            // Calculations
            double baseEpPerHs = isMilitary ? 50.0 : 25.0;
            double totalEP = Math.Round(hs * baseEpPerHs * powerMod, 1);
            double fuelEfficiency = Math.Round(1.0 * Math.Pow(powerMod, 1.5), 2);
            double thermalSignature = Math.Round(totalEP * thermalFactor, 1);
            double bpCost = Math.Round(hs * 2.5 * powerMod, 1);
            int crew = Convert.ToInt32(Math.Ceiling(hs * 1.5));

            if (LblEngResPower != null) LblEngResPower.Text = $"{totalEP:N1} EP";
            if (LblEngResFuel != null) LblEngResFuel.Text = $"{fuelEfficiency:F2} L/EP/Hr";
            if (LblEngResThermal != null) LblEngResThermal.Text = $"{thermalSignature:N1} W";
            if (LblEngResCost != null) LblEngResCost.Text = $"{bpCost:N1} BP";
            if (LblEngResSize != null) LblEngResSize.Text = $"{hs * 50:N0} Tons";
            if (LblEngResCrew != null) LblEngResCrew.Text = $"{crew:N0} Personas";

            // Hull speed simulator
            if (LblSpeed1k != null) LblSpeed1k.Text = $"{Math.Round((totalEP / (1000.0 / 50.0)) * 1000.0, 0):N0} km/s";
            if (LblSpeed5k != null) LblSpeed5k.Text = $"{Math.Round((totalEP / (5000.0 / 50.0)) * 1000.0, 0):N0} km/s";
            if (LblSpeed10k != null) LblSpeed10k.Text = $"{Math.Round((totalEP / (10000.0 / 50.0)) * 1000.0, 0):N0} km/s";
            if (LblSpeed20k != null) LblSpeed20k.Text = $"{Math.Round((totalEP / (20000.0 / 50.0)) * 1000.0, 0):N0} km/s";
            if (LblSpeed50k != null) LblSpeed50k.Text = $"{Math.Round((totalEP / (50000.0 / 50.0)) * 1000.0, 0):N0} km/s";

            // Fuel burn endurance
            double burnRatePerHour = totalEP * fuelEfficiency;
            if (LblBurn50k != null) LblBurn50k.Text = burnRatePerHour > 0 ? $"{Math.Round((50000.0 / burnRatePerHour) / 24.0, 1)} Días" : "N/A";
            if (LblBurn100k != null) LblBurn100k.Text = burnRatePerHour > 0 ? $"{Math.Round((100000.0 / burnRatePerHour) / 24.0, 1)} Días" : "N/A";
            if (LblBurn250k != null) LblBurn250k.Text = burnRatePerHour > 0 ? $"{Math.Round((250000.0 / burnRatePerHour) / 24.0, 1)} Días" : "N/A";
            if (LblBurn1M != null) LblBurn1M.Text = burnRatePerHour > 0 ? $"{Math.Round((1000000.0 / burnRatePerHour) / 24.0, 1)} Días" : "N/A";

            // Enemy passive thermal detection range (in Million km)
            if (LblDet10 != null) LblDet10.Text = $"{Math.Round((thermalSignature / 10.0) * 10.0, 1)} Mill. km";
            if (LblDet25 != null) LblDet25.Text = $"{Math.Round((thermalSignature / 25.0) * 10.0, 1)} Mill. km";
            if (LblDet50 != null) LblDet50.Text = $"{Math.Round((thermalSignature / 50.0) * 10.0, 1)} Mill. km";
            if (LblDet100 != null) LblDet100.Text = $"{Math.Round((thermalSignature / 100.0) * 10.0, 1)} Mill. km";

            // Minerals
            var engMinerals = new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>("Gallicite (Propulsores)", bpCost * 0.70),
                new KeyValuePair<string, double>("Duranium (Chasis de Motor)", bpCost * 0.20),
                new KeyValuePair<string, double>("Uridium (Circuitos de Control)", bpCost * 0.10)
            };
            if (IcEngineMinerals != null) IcEngineMinerals.ItemsSource = engMinerals;
        }

        private void BtnSaveEngine_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null)
            {
                MessageBox.Show("No se encontró conexión activa con la base de datos de Aurora 4X.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = TxtEngineName.Text.Trim();
            if (string.IsNullOrEmpty(name)) name = "Motor Naval Personalizado";

            bool isComm = RbEngComm?.IsChecked == true;
            double hs = SldEngSize.Value;
            double powerMod = SldEngPowerMod.Value;
            double thermalFactor = SldEngThermal.Value;

            double baseEp = isComm ? 25.0 : 50.0;
            double totalEP = Math.Round(hs * baseEp * powerMod, 1);
            double fuelEff = Math.Round(1.0 * Math.Pow(powerMod, 1.5), 2);
            double thermal = Math.Round(totalEP * thermalFactor, 1);
            double cost = Math.Round(hs * 2.5 * powerMod, 1);

            if (_dbService.SaveEngineDesign(_currentRaceId, name, hs, totalEP, fuelEff, thermal, cost, isComm, out string msg))
            {
                MessageBox.Show(msg, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshCatalog();
            }
            else
            {
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
