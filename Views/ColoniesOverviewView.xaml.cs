using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class ColoniesOverviewView : UserControl
    {
        private DatabaseService? _dbService;
        private int _currentRaceId;

        public ColoniesOverviewView()
        {
            InitializeComponent();
        }

        public void LoadColoniesData(DatabaseService? dbService, int raceId)
        {
            if (dbService == null) return;
            _dbService = dbService;
            _currentRaceId = raceId;

            RefreshData();
        }

        private void RefreshData()
        {
            if (_dbService == null) return;

            var colonies = _dbService.GetColonies(_currentRaceId);
            if (DgColonies != null) DgColonies.ItemsSource = colonies;
            if (LblColonyCount != null) LblColonyCount.Text = $"{colonies.Count} Asentamiento(s)";

            // Calculate totals across all colonies for all 11 trans-uranic minerals
            var global = new MineralRequirement();
            foreach (var c in colonies)
            {
                var m = c.MineralStockpiles;
                global.Duranium += m.Duranium;
                global.Sorium += m.Sorium;
                global.Neutronium += m.Neutronium;
                global.Corundium += m.Corundium;
                global.Uridium += m.Uridium;
                global.Gallicite += m.Gallicite;
                global.Boronide += m.Boronide;
                global.Mercassium += m.Mercassium;
                global.Vendarite += m.Vendarite;
                global.Corbomite += m.Corbomite;
                global.Tritium += m.Tritium;
            }

            double grandTotal = global.TotalCost;
            if (LblGlobalTotalMinerals != null) LblGlobalTotalMinerals.Text = $"{grandTotal:N0} t";

            // Build detailed 11 minerals list
            var minList = new List<MineralDetailItem>
            {
                new MineralDetailItem
                {
                    Name = "Duranium", Symbol = "Dur", Amount = global.Duranium,
                    PercentageOfEmpire = grandTotal > 0 ? (global.Duranium / grandTotal) * 100.0 : 0,
                    GameUtility = "Cascos navales, estructuras de blindaje, expansión de astilleros e infraestructura general.",
                    Status = global.Duranium > 10000 ? "✅ OPTIMA" : "⚠️ CRÍTICA", StatusColor = global.Duranium > 10000 ? "#55FF55" : "#FFB400"
                },
                new MineralDetailItem
                {
                    Name = "Gallicite", Symbol = "Gal", Amount = global.Gallicite,
                    PercentageOfEmpire = grandTotal > 0 ? (global.Gallicite / grandTotal) * 100.0 : 0,
                    GameUtility = "Motores espaciales, propulsores de combate, proyectiles de misiles y propulsión principal.",
                    Status = global.Gallicite > 10000 ? "✅ OPTIMA" : "⚠️ CRÍTICA", StatusColor = global.Gallicite > 10000 ? "#55FF55" : "#FFB400"
                },
                new MineralDetailItem
                {
                    Name = "Sorium", Symbol = "Sor", Amount = global.Sorium,
                    PercentageOfEmpire = grandTotal > 0 ? (global.Sorium / grandTotal) * 100.0 : 0,
                    GameUtility = "Materia prima para refino de combustible (LPH) y reactores de motores de salto.",
                    Status = global.Sorium > 5000 ? "✅ OPTIMA" : "⚠️ RECURSO CRÍTICO", StatusColor = global.Sorium > 5000 ? "#55FF55" : "#FF5555"
                },
                new MineralDetailItem
                {
                    Name = "Neutronium", Symbol = "Neu", Amount = global.Neutronium,
                    PercentageOfEmpire = grandTotal > 0 ? (global.Neutronium / grandTotal) * 100.0 : 0,
                    GameUtility = "Blindaje pesado de cascos de guerra, fortificación de silos e instalaciones defensivas.",
                    Status = global.Neutronium > 5000 ? "✅ OPTIMA" : "⚠️ REVISE EXTRACCIÓN", StatusColor = global.Neutronium > 5000 ? "#55FF55" : "#FFB400"
                },
                new MineralDetailItem
                {
                    Name = "Corundium", Symbol = "Cor", Amount = global.Corundium,
                    PercentageOfEmpire = grandTotal > 0 ? (global.Corundium / grandTotal) * 100.0 : 0,
                    GameUtility = "Lentes de láseres de energía, carronadas de plasma y lentes focalizadoras avanzadas.",
                    Status = global.Corundium > 5000 ? "✅ OPTIMA" : "⚠️ NOMINAL", StatusColor = "#55FF55"
                },
                new MineralDetailItem
                {
                    Name = "Uridium", Symbol = "Uri", Amount = global.Uridium,
                    PercentageOfEmpire = grandTotal > 0 ? (global.Uridium / grandTotal) * 100.0 : 0,
                    GameUtility = "Sensores de vigilancia, radares de control de tiro y ordenadores de salto.",
                    Status = global.Uridium > 5000 ? "✅ OPTIMA" : "⚠️ NOMINAL", StatusColor = "#55FF55"
                },
                new MineralDetailItem
                {
                    Name = "Boronide", Symbol = "Bor", Amount = global.Boronide,
                    PercentageOfEmpire = grandTotal > 0 ? (global.Boronide / grandTotal) * 100.0 : 0,
                    GameUtility = "Núcleos de reactores de energía, plantas auxiliares y condensadores de recarga.",
                    Status = global.Boronide > 2000 ? "✅ NOMINAL" : "⚠️ RESERVA BAJA", StatusColor = "#55FF55"
                },
                new MineralDetailItem
                {
                    Name = "Mercassium", Symbol = "Mer", Amount = global.Mercassium,
                    PercentageOfEmpire = grandTotal > 0 ? (global.Mercassium / grandTotal) * 100.0 : 0,
                    GameUtility = "Sistemas de Life Support, habitabilidad, investigación biológica y módulos médicos.",
                    Status = global.Mercassium > 1000 ? "✅ NOMINAL" : "⚠️ RESERVA BAJA", StatusColor = "#55FF55"
                },
                new MineralDetailItem
                {
                    Name = "Vendarite", Symbol = "Ven", Amount = global.Vendarite,
                    PercentageOfEmpire = grandTotal > 0 ? (global.Vendarite / grandTotal) * 100.0 : 0,
                    GameUtility = "Fabricación de repuestos MSP (Maintenance Supplies) y mantenimiento de maquinaria.",
                    Status = global.Vendarite > 1000 ? "✅ NOMINAL" : "⚠️ RESERVA BAJA", StatusColor = "#55FF55"
                },
                new MineralDetailItem
                {
                    Name = "Corbomite", Symbol = "Cbm", Amount = global.Corbomite,
                    PercentageOfEmpire = grandTotal > 0 ? (global.Corbomite / grandTotal) * 100.0 : 0,
                    GameUtility = "Generadores de escudos de defensivos, revestimiento de sigilo e inhibición de firmas.",
                    Status = global.Corbomite > 1000 ? "✅ NOMINAL" : "⚠️ RESERVA BAJA", StatusColor = "#55FF55"
                },
                new MineralDetailItem
                {
                    Name = "Tritium", Symbol = "Tri", Amount = global.Tritium,
                    PercentageOfEmpire = grandTotal > 0 ? (global.Tritium / grandTotal) * 100.0 : 0,
                    GameUtility = "Tubos de lanzamiento de misiles, cañones cinéticos pesados y municiones balísticas.",
                    Status = global.Tritium > 1000 ? "✅ NOMINAL" : "⚠️ RESERVA BAJA", StatusColor = "#55FF55"
                }
            };

            if (DgMineralDetails != null) DgMineralDetails.ItemsSource = minList;

            // Update Telemetry Bar
            var topMin = minList.OrderByDescending(m => m.Amount).FirstOrDefault();
            if (LblTopMineral != null && topMin != null)
                LblTopMineral.Text = $"{topMin.Name} ({topMin.Amount:N0} t)";

            var lowMin = minList.OrderBy(m => m.Amount).FirstOrDefault();
            if (LblBottleneckMineral != null && lowMin != null)
                LblBottleneckMineral.Text = $"{lowMin.Name} ({lowMin.Amount:N0} t)";

            // Select first colony by default
            if (colonies.Count > 0 && DgColonies != null && DgColonies.SelectedIndex < 0)
            {
                DgColonies.SelectedIndex = 0;
            }
        }

        private void DgColonies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgColonies.SelectedItem is ColonyInfo col)
            {
                UpdateColonyDossier(col);
            }
        }

        private void UpdateColonyDossier(ColonyInfo col)
        {
            if (LblColonyTitle == null || LblColonyStatus == null || LblColonyPop == null || LblColonyFuel == null || LblColonyGovernor == null || LblColonyInfra == null) return;

            LblColonyTitle.Text = $"{col.PopName} ({col.SystemName})";
            LblColonyStatus.Text = col.IsCapital ? "Capital Imperial | Colony Cost: 0.00 (Mundo Nativo)" : $"Asentamiento Colonial | Status: {col.TerraformStatus}";
            LblColonyPop.Text = $"{col.PopulationMillions:N2} Millones";
            LblColonyFuel.Text = $"{col.FuelStockpile:N0} Litros";

            if (_dbService != null)
            {
                var infra = _dbService.GetEmpireInfrastructure(_currentRaceId);
                double constFactories = infra.Where(i => i.Name.Contains("Construcción") || i.Name.Contains("Convencional")).Sum(i => i.Amount);
                double mines = infra.Where(i => i.Name.Contains("Mina")).Sum(i => i.Amount);
                double refineries = infra.Where(i => i.Name.Contains("Refinería")).Sum(i => i.Amount);

                LblColonyInfra.Text = $"{constFactories:N0} Fábricas | {mines:N0} Minas | {refineries:N0} Refinerías";
            }
            else
            {
                LblColonyInfra.Text = "Infraestructura Local Sincronizada";
            }

            LblColonyGovernor.Text = col.IsCapital ? "Gobernador General del Sector (+10.0% Minería)" : "Oficial Asignado";
        }
    }
}
