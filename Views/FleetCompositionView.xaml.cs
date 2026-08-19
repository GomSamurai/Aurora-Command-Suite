using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class FleetCompositionView : UserControl
    {
        private readonly ObservableCollection<FleetCompositionItem> _fleetItems = new ObservableCollection<FleetCompositionItem>();
        private ShipDesign? _activeBlueprint;
        private DatabaseService? _dbService;
        private int _currentRaceId;

        public FleetCompositionView()
        {
            InitializeComponent();
            DgFleetItems.ItemsSource = _fleetItems;

            InitializePresetFormations();
        }

        private void InitializePresetFormations()
        {
            var presets = new List<string>
            {
                "📂 Seleccionar Formación Predefinida...",
                "🛡️ Escuadra de Escolta de Convoy (2 Cargueros + 1 Destructor)",
                "💥 Fuerza de Tarea Láser (1 Crucero Pesado + 2 Fragatas + 1 Tanquero)",
                "🔍 Flota de Exploración de Salto (1 Buque Gravitacional + 2 Exploradores)"
            };
            CmbPresetFormations.ItemsSource = presets;
            CmbPresetFormations.SelectedIndex = 0;
        }

        public void LoadEmpireClasses(DatabaseService? dbService, int raceId)
        {
            _dbService = dbService;
            _currentRaceId = raceId;

            if (_dbService != null)
            {
                var classes = _dbService.GetRaceClasses(raceId);
                CmbEmpireClasses.ItemsSource = classes;
                if (classes.Count > 0)
                {
                    CmbEmpireClasses.SelectedIndex = 0;
                }
                else
                {
                    CmbEmpireClasses.ItemsSource = null;
                }
            }
            else
            {
                CmbEmpireClasses.ItemsSource = null;
            }

            RecalculateFleet();
        }

        public void SetActiveBlueprint(ShipDesign blueprint)
        {
            _activeBlueprint = blueprint;
            if (TxtActiveDesignName != null)
            {
                TxtActiveDesignName.Text = blueprint.ClassName;
            }
        }

        private void BtnAddCurrentShip_Click(object sender, RoutedEventArgs e)
        {
            if (_activeBlueprint != null)
            {
                AddDesignToFleet(_activeBlueprint);
            }
            else
            {
                MessageBox.Show("Primero crea o selecciona un blueprint en el Diseñador de Naves.", "Atención", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnAddSelectedClass_Click(object sender, RoutedEventArgs e)
        {
            if (CmbEmpireClasses.SelectedItem is ShipClassSimpleInfo shipClass)
            {
                var design = new ShipDesign
                {
                    ClassName = shipClass.ClassName,
                    TotalHS = shipClass.SizeHS,
                    TotalCostBP = shipClass.CostBP,
                    MaxSpeedKmS = shipClass.MaxSpeedKmS,
                    TotalFuelLiters = shipClass.TotalFuelLiters,
                    RangeBillionKm = shipClass.RangeBillionKm,
                    TotalMSP = shipClass.TotalMSP,
                    TotalCrewRequired = shipClass.TotalCrewRequired,
                    ThermalSignature = shipClass.ThermalSignature,
                    EMSignature = shipClass.EMSignature,
                    IsMilitary = shipClass.IsMilitary
                };

                AddDesignToFleet(design);
            }
        }

        private void BtnImportActiveFleets_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null) return;

            var activeFleets = _dbService.GetActiveFleets(_currentRaceId);
            if (activeFleets.Count == 0)
            {
                MessageBox.Show("No se encontraron flotas activas en tu partida guardada de Aurora 4X.", "Sin Flotas Activas", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _fleetItems.Clear();
            int totalShipsImported = 0;

            foreach (var fleet in activeFleets)
            {
                foreach (var ship in fleet.Ships)
                {
                    var design = new ShipDesign
                    {
                        ClassName = ship.ClassName,
                        TotalHS = ship.Tonnage / 50.0,
                        TotalCostBP = Math.Round(ship.Tonnage * 0.08, 1),
                        MaxSpeedKmS = fleet.SpeedKmS > 0 ? fleet.SpeedKmS : 1000.0,
                        TotalFuelLiters = ship.MaxFuelLiters > 0 ? ship.MaxFuelLiters : 50000.0,
                        TotalMSP = ship.CurrentMSP > 0 ? ship.CurrentMSP : 100.0,
                        TotalCrewRequired = 30,
                        IsMilitary = true
                    };

                    AddDesignToFleet(design);
                    totalShipsImported++;
                }
            }

            MessageBox.Show($"🌌 ¡Importación completada con éxito!\nSe cargaron {activeFleets.Count} flotas operativas ({totalShipsImported} naves) directamente desde tu partida guardada de Aurora 4X con sus velocidades y especificaciones reales.", "Flotas Importadas de Aurora 4X", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddDesignToFleet(ShipDesign design)
        {
            var existing = _fleetItems.FirstOrDefault(x => x.Design.ClassName.Equals(design.ClassName, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.Count++;
                DgFleetItems.Items.Refresh();
            }
            else
            {
                _fleetItems.Add(new FleetCompositionItem { Design = design, Count = 1 });
            }
            RecalculateFleet();
        }

        private void CmbPresetFormations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = CmbPresetFormations.SelectedIndex;
            if (idx <= 0) return;

            _fleetItems.Clear();

            var commEng = new Component { ComponentName = "Commercial Engine 500", TypeName = "Engine", ComponentSize = 10, EnginePower = 500 };
            var milEng = new Component { ComponentName = "Military Engine 200", TypeName = "Engine", ComponentSize = 4, EnginePower = 200 };
            var fuel = new Component { ComponentName = "Fuel Tank Large", TypeName = "Fuel", ComponentSize = 5, FuelCapacity = 200000 };
            var laser = new Component { ComponentName = "15cm C3 Near-Ultraviolet Laser", TypeName = "Beam Weapon", ComponentSize = 3 };

            if (idx == 1) // Escuadra de Escolta de Convoy
            {
                var commShip = new ShipDesign { ClassName = "Carguero Comercial Estándar", TotalHS = 200, TotalCostBP = 300, MaxSpeedKmS = 2500, TotalFuelLiters = 400000, RangeBillionKm = 50, TotalMSP = 100, TotalCrewRequired = 40, IsMilitary = false };
                commShip.Components.Add(new SelectedComponentItem { Component = commEng, Quantity = 4 });

                var escortShip = new ShipDesign { ClassName = "Destructor de Escolta Vanguardia", TotalHS = 100, TotalCostBP = 450, MaxSpeedKmS = 4000, TotalFuelLiters = 150000, RangeBillionKm = 30, TotalMSP = 300, TotalCrewRequired = 180, IsMilitary = true };
                escortShip.Components.Add(new SelectedComponentItem { Component = milEng, Quantity = 4 });
                escortShip.Components.Add(new SelectedComponentItem { Component = laser, Quantity = 2 });

                _fleetItems.Add(new FleetCompositionItem { Design = commShip, Count = 2 });
                _fleetItems.Add(new FleetCompositionItem { Design = escortShip, Count = 1 });
            }
            else if (idx == 2) // Fuerza de Tarea Láser
            {
                var cruiser = new ShipDesign { ClassName = "Crucero Pesado Leviatán", TotalHS = 240, TotalCostBP = 1200, MaxSpeedKmS = 3500, TotalFuelLiters = 500000, RangeBillionKm = 40, TotalMSP = 600, TotalCrewRequired = 450, IsMilitary = true };
                var frigate = new ShipDesign { ClassName = "Fragata de Defensa Guardián", TotalHS = 70, TotalCostBP = 350, MaxSpeedKmS = 4200, TotalFuelLiters = 100000, RangeBillionKm = 25, TotalMSP = 200, TotalCrewRequired = 120, IsMilitary = true };
                var tanker = new ShipDesign { ClassName = "Nave Tanquero Prometeo", TotalHS = 180, TotalCostBP = 280, MaxSpeedKmS = 2800, TotalFuelLiters = 1500000, RangeBillionKm = 100, TotalMSP = 80, TotalCrewRequired = 50, IsMilitary = false };

                _fleetItems.Add(new FleetCompositionItem { Design = cruiser, Count = 1 });
                _fleetItems.Add(new FleetCompositionItem { Design = frigate, Count = 2 });
                _fleetItems.Add(new FleetCompositionItem { Design = tanker, Count = 1 });
            }
            else if (idx == 3) // Flota de Exploración de Salto
            {
                var jumpShip = new ShipDesign { ClassName = "Explorador de Saltos Nebulosa", TotalHS = 90, TotalCostBP = 500, MaxSpeedKmS = 3200, TotalFuelLiters = 200000, RangeBillionKm = 45, TotalMSP = 250, TotalCrewRequired = 110, IsMilitary = true, HasJumpDrive = true };
                var geoShip = new ShipDesign { ClassName = "Buque Geológico Vigía", TotalHS = 60, TotalCostBP = 320, MaxSpeedKmS = 3000, TotalFuelLiters = 150000, RangeBillionKm = 40, TotalMSP = 180, TotalCrewRequired = 80, IsMilitary = true };

                _fleetItems.Add(new FleetCompositionItem { Design = jumpShip, Count = 1 });
                _fleetItems.Add(new FleetCompositionItem { Design = geoShip, Count = 2 });
            }

            RecalculateFleet();
        }

        private void BtnRemoveFleetItem_Click(object sender, RoutedEventArgs e)
        {
            if (DgFleetItems.SelectedItem is FleetCompositionItem item)
            {
                _fleetItems.Remove(item);
                RecalculateFleet();
            }
        }

        private void DgFleetItems_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(RecalculateFleet), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void OnFleetInputChanged(object sender, TextChangedEventArgs e)
        {
            RecalculateFleet();
        }

        private void BtnSendToShipyards_Click(object sender, RoutedEventArgs e)
        {
            if (_fleetItems.Count == 0 || _dbService == null)
            {
                MessageBox.Show("Por favor añade naves a la flota antes de enviar órdenes a los astilleros.", "Flota Vacía", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var shipyards = _dbService.GetShipyards(_currentRaceId);
            if (shipyards.Count == 0)
            {
                MessageBox.Show("No se encontraron astilleros en la base de datos de tu imperio.", "Sin Astilleros", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int ordersSent = 0;
            foreach (var item in _fleetItems)
            {
                // Find matching shipyard with enough capacity and free slipway
                var targetSy = shipyards.FirstOrDefault(s => s.CapacityTons >= item.Design.TotalTonnage && s.FreeSlipways > 0) ??
                               shipyards.FirstOrDefault(s => s.CapacityTons >= item.Design.TotalTonnage);

                if (targetSy != null)
                {
                    for (int i = 1; i <= item.Count; i++)
                    {
                        string unitName = $"S.M.S. {item.Design.ClassName}-{i:D2}";
                        _dbService.AddShipyardTask(targetSy.ShipyardID, unitName, item.Design.TotalCostBP, out _);
                        ordersSent++;
                    }
                }
            }

            MessageBox.Show($"🚀 Se han registrado {ordersSent} orden(es) de construcción naval en los astilleros imperiales.", "Órdenes Enviadas con Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void RecalculateFleet()
        {
            if (LblFleetTonnage == null || LblFleetCost == null || LblFleetFuel == null || 
                LblFleetMSP == null || LblFleetCrew == null || LblCampaignFuelNeeded == null || 
                LblCampaignSoriumNeeded == null || IcFleetMinerals == null ||
                LblFleetSpeed == null || LblFleetRange == null || LblFleetThermal == null || LblFleetEM == null)
            {
                return;
            }

            double totalTonnage = 0;
            double totalCostBP = 0;
            double totalFuelLiters = 0;
            double totalMSP = 0;
            int totalCrew = 0;
            double totalThermal = 0;
            double totalEM = 0;

            double fleetMinSpeed = _fleetItems.Count > 0 ? double.MaxValue : 0;
            double fleetMinRange = _fleetItems.Count > 0 ? double.MaxValue : 0;
            double maxShipTonnageInFleet = 0;

            var aggregatedMinerals = new MineralRequirement();

            foreach (var item in _fleetItems)
            {
                int count = item.Count;
                totalTonnage += item.TotalTonnage;
                totalCostBP += item.TotalCostBP;
                totalFuelLiters += item.TotalFuelLiters;
                totalMSP += item.TotalMSP;
                totalCrew += item.TotalCrew;

                if (item.Design.TotalTonnage > maxShipTonnageInFleet)
                {
                    maxShipTonnageInFleet = item.Design.TotalTonnage;
                }

                if (item.Design.MaxSpeedKmS > 0 && item.Design.MaxSpeedKmS < fleetMinSpeed)
                {
                    fleetMinSpeed = item.Design.MaxSpeedKmS;
                }

                if (item.Design.RangeBillionKm > 0 && item.Design.RangeBillionKm < fleetMinRange)
                {
                    fleetMinRange = item.Design.RangeBillionKm;
                }

                totalThermal += item.Design.ThermalSignature * count;
                totalEM += item.Design.EMSignature * count;

                var m = item.Design.Minerals;
                aggregatedMinerals.Duranium += m.Duranium * count;
                aggregatedMinerals.Sorium += m.Sorium * count;
                aggregatedMinerals.Neutronium += m.Neutronium * count;
                aggregatedMinerals.Corundium += m.Corundium * count;
                aggregatedMinerals.Uridium += m.Uridium * count;
                aggregatedMinerals.Gallicite += m.Gallicite * count;
                aggregatedMinerals.Tritium += m.Tritium * count;
                aggregatedMinerals.Boronide += m.Boronide * count;
            }

            if (fleetMinSpeed == double.MaxValue) fleetMinSpeed = 0;
            if (fleetMinRange == double.MaxValue) fleetMinRange = 0;

            LblFleetTonnage.Text = $"{totalTonnage:N0} Tons";
            LblFleetCost.Text = $"{totalCostBP:N1} BP";
            LblFleetFuel.Text = $"{totalFuelLiters:N0} Litros";
            LblFleetMSP.Text = $"{totalMSP:N0} MSP";
            LblFleetCrew.Text = $"{totalCrew:N0} Personas";

            LblFleetSpeed.Text = $"{fleetMinSpeed:N0} km/s";
            LblFleetRange.Text = $"{fleetMinRange:N1} Billones km";
            LblFleetThermal.Text = $"{totalThermal:N0} W";
            LblFleetEM.Text = $"{totalEM:N0} W";

            // Campaign fuel computation
            double.TryParse(TxtCampaignDist?.Text, out double campaignDistBillionKm);
            if (campaignDistBillionKm <= 0) campaignDistBillionKm = 50;

            if (LblCampaignAU != null)
            {
                double au = (campaignDistBillionKm * 1_000_000_000.0) / 149_597_870_700.0;
                LblCampaignAU.Text = $"{au:F1} AU";
            }

            double fleetFuelNeededForCampaign = 0;
            foreach (var item in _fleetItems)
            {
                if (item.Design.RangeBillionKm > 0)
                {
                    double litersPerBillionKm = item.Design.TotalFuelLiters / item.Design.RangeBillionKm;
                    fleetFuelNeededForCampaign += (litersPerBillionKm * campaignDistBillionKm) * item.Count;
                }
            }

            LblCampaignFuelNeeded.Text = $"{fleetFuelNeededForCampaign:N0} Litros";
            double soriumNeededTons = fleetFuelNeededForCampaign / 2500.0;
            LblCampaignSoriumNeeded.Text = $"{soriumNeededTons:N1} Tons Sorium";

            // Minerals
            var list = new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>("Duranium", aggregatedMinerals.Duranium),
                new KeyValuePair<string, double>("Sorium", aggregatedMinerals.Sorium),
                new KeyValuePair<string, double>("Neutronium", aggregatedMinerals.Neutronium),
                new KeyValuePair<string, double>("Corundium", aggregatedMinerals.Corundium),
                new KeyValuePair<string, double>("Uridium", aggregatedMinerals.Uridium),
                new KeyValuePair<string, double>("Gallicite", aggregatedMinerals.Gallicite),
                new KeyValuePair<string, double>("Tritium", aggregatedMinerals.Tritium),
                new KeyValuePair<string, double>("Boronide", aggregatedMinerals.Boronide)
            }.Where(x => x.Value > 0).ToList();

            IcFleetMinerals.ItemsSource = list;

            // Evaluate Shipyard Feasibility Matrix
            if (LblShipyardFeasibilityStatus != null && _dbService != null)
            {
                var shipyards = _dbService.GetShipyards(_currentRaceId);
                double maxSyCapacity = shipyards.Count > 0 ? shipyards.Max(s => s.CapacityTons) : 0;

                if (maxShipTonnageInFleet == 0)
                {
                    LblShipyardFeasibilityStatus.Text = "🟢 Añade naves a la flota para evaluar la factibilidad en tus astilleros.";
                    LblShipyardFeasibilityStatus.Foreground = new SolidColorBrush(Color.FromRgb(85, 255, 85));
                }
                else if (maxShipTonnageInFleet <= maxSyCapacity)
                {
                    LblShipyardFeasibilityStatus.Text = $"🟢 Todos los astilleros imperiales cuentan con capacidad suficiente (Capacidad Máxima: {maxSyCapacity:N0} t vs Nave Mayor: {maxShipTonnageInFleet:N0} t).";
                    LblShipyardFeasibilityStatus.Foreground = new SolidColorBrush(Color.FromRgb(85, 255, 85));
                }
                else
                {
                    LblShipyardFeasibilityStatus.Text = $"⚠️ ATENCIÓN: La nave mayor de la flota ({maxShipTonnageInFleet:N0} t) supera la capacidad del astillero imperial más grande ({maxSyCapacity:N0} t). Se requiere ampliar gradas.";
                    LblShipyardFeasibilityStatus.Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0));
                }
            }
        }
    }
}
