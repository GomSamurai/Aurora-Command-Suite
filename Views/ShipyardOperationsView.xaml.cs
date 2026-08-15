using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class ShipyardOperationsView : UserControl
    {
        private DatabaseService? _dbService;
        private int _currentRaceId;

        public ShipyardComplexInfo? SelectedShipyard => DgShipyards?.SelectedItem as ShipyardComplexInfo;

        public ShipyardOperationsView()
        {
            InitializeComponent();
            CalculateWorkbenchEstimates();
        }

        public void LoadShipyardsData(DatabaseService? dbService, int raceId)
        {
            _dbService = dbService;
            _currentRaceId = raceId;
            RefreshShipyards();
        }

        public void RefreshShipyards()
        {
            if (_dbService == null || DgShipyards == null) return;

            int selectedSyId = SelectedShipyard?.ShipyardID ?? 0;
            var shipyards = _dbService.GetShipyards(_currentRaceId);
            DgShipyards.ItemsSource = shipyards;

            var tele = _dbService.GetShipyardTelemetry(_currentRaceId);
            LblTotalNavalCap.Text = $"{tele.TotalNavalCapacityTons:N0} t";
            LblTotalCommCap.Text = $"{tele.TotalCommercialCapacityTons:N0} t";
            LblSlipwaysStatus.Text = $"{tele.FreeSlipways} Libre(s) / {tele.TotalNavalSlipways + tele.TotalCommercialSlipways} Totales";
            LblAnnualBPOutput.Text = $"{tele.TotalAnnualBPOutput:N0} BP/año";
            LblGovBonus.Text = $"+{tele.GovernorBonusPercent:F1} %";

            if (shipyards.Count > 0)
            {
                var target = shipyards.FirstOrDefault(s => s.ShipyardID == selectedSyId) ?? shipyards[0];
                DgShipyards.SelectedItem = target;
            }
            else
            {
                DgShipyardTasks.ItemsSource = new List<ShipyardTaskInfo>();
                UpdateMineralRequirements(new ShipyardComplexInfo());
            }
        }

        private void DgShipyards_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedShipyard != null)
            {
                LblTasksHeader.Text = $"🔨 TAREAS DE CONSTRUCCIÓN NAVAL Y RETOOLING EN: {SelectedShipyard.ShipyardName.ToUpper()}";
                DgShipyardTasks.ItemsSource = SelectedShipyard.Tasks;
                UpdateMineralRequirements(SelectedShipyard);

                // Auto populate calculator with selected shipyard stats
                TxtCalcBPRate.Text = SelectedShipyard.BuildSpeedBPPerYear.ToString("F0");
            }
        }

        private void UpdateMineralRequirements(ShipyardComplexInfo sy)
        {
            LblMinDuranium.Text = $"{sy.TotalDuraniumNeeded:N1} t";
            LblMinNeutronium.Text = $"{sy.TotalNeutroniumNeeded:N1} t";
            LblMinGallicite.Text = $"{sy.TotalGalliciteNeeded:N1} t";
            LblMinUridium.Text = $"{sy.TotalUridiumNeeded:N1} t";
        }

        private void BtnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedShipyard == null || _dbService == null)
            {
                MessageBox.Show("Selecciona un astillero en la lista antes de enviar una orden de construcción.", "Astillero No Seleccionado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedShipyard.FreeSlipways <= 0)
            {
                MessageBox.Show($"El astillero '{SelectedShipyard.ShipyardName}' no tiene gradas libres en este momento ({SelectedShipyard.ActiveTasksCount}/{SelectedShipyard.Slipways} ocupadas).", "Gradas Ocupadas", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var availableClasses = _dbService.GetRaceClasses(_currentRaceId);
            if (availableClasses.Count == 0)
            {
                // Fallback demo classes if DB empty
                availableClasses.Add(new ShipClassSimpleInfo { ClassName = "Fragata Relámpago", SizeHS = 80, CostBP = 200 });
                availableClasses.Add(new ShipClassSimpleInfo { ClassName = "Carguero Comercial Estándar", SizeHS = 200, CostBP = 350 });
                availableClasses.Add(new ShipClassSimpleInfo { ClassName = "Destructor Vanguardia", SizeHS = 100, CostBP = 280 });
            }

            var dlg = new NewShipyardOrderDialog(SelectedShipyard, availableClasses)
            {
                Owner = Window.GetWindow(this)
            };

            if (dlg.ShowDialog() == true && dlg.SelectedClass != null)
            {
                if (_dbService.AddShipyardTask(SelectedShipyard.ShipyardID, dlg.UnitName, dlg.SelectedClass.CostBP, out string msg))
                {
                    MessageBox.Show(msg, "Orden de Construcción Registrada", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshShipyards();
                }
                else
                {
                    MessageBox.Show(msg, "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnCancelTask_Click(object sender, RoutedEventArgs e)
        {
            if (DgShipyardTasks.SelectedItem is not ShipyardTaskInfo task || _dbService == null)
            {
                MessageBox.Show("Selecciona una tarea de construcción activa en la lista para cancelarla.", "Tarea No Seleccionada", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"¿Confirmas que deseas cancelar la construcción de '{task.UnitName}'?\nSe liberará la grada de construcción en el astillero.", "Confirmar Cancelación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (_dbService.DeleteShipyardTask(task.TaskID, out string msg))
                {
                    MessageBox.Show(msg, "Tarea Cancelada", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshShipyards();
                }
                else
                {
                    MessageBox.Show(msg, "Error al Cancelar", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnRetoolShipyard_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedShipyard == null || _dbService == null)
            {
                MessageBox.Show("Selecciona un astillero para re-equipar.", "Astillero No Seleccionado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var availableClasses = _dbService.GetRaceClasses(_currentRaceId);
            if (availableClasses.Count == 0) return;

            string targetClassName = availableClasses[0].ClassName;
            double retoolBP = Math.Round(availableClasses[0].CostBP * 0.25, 1);

            var res = MessageBox.Show($"Confirmar re-equipamiento (Retooling) de '{SelectedShipyard.ShipyardName}' para la clase '{targetClassName}'?\n\n• Costo estimado: {retoolBP} BP\n• Días estimados: {Math.Round((retoolBP / SelectedShipyard.BuildSpeedBPPerYear) * 365, 0)} días", "Confirmar Retooling", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                if (_dbService.AddShipyardTask(SelectedShipyard.ShipyardID, $"🔧 Retooling: {targetClassName}", retoolBP, out string msg))
                {
                    MessageBox.Show($"✅ Tarea de Retooling agregada con éxito al astillero {SelectedShipyard.ShipyardName}.", "Retooling En Marcha", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshShipyards();
                }
            }
        }

        private void BtnExpandShipyard_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedShipyard == null)
            {
                MessageBox.Show("Selecciona un astillero para proyectar su expansión.", "Astillero No Seleccionado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dlg = new ConfirmActionDialog(
                "🔨 CONFIRMAR EXPANSIÓN DE ASTILLERO",
                $"¿Estás seguro de que deseas autorizar la orden de expansión para '{SelectedShipyard.ShipyardName}'?\nSe registrará la construcción de la Grada N° {SelectedShipyard.Slipways + 1} o +5,000 Tons en el complejo industrial de tu colonia.",
                "120 BP en Industria Planetaria (Duranium + Neutronium)",
                "90 Días (3 Meses)"
            )
            {
                Owner = Window.GetWindow(this)
            };

            if (dlg.ShowDialog() == true && _dbService != null)
            {
                if (_dbService.AddIndustrialProject(_currentRaceId, $"Ampliar Astillero: {SelectedShipyard.ShipyardName}", 1.0, out string msg))
                {
                    MessageBox.Show(msg, "Proyecto de Expansión Registrado en AuroraDB", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshShipyards();
                }
                else
                {
                    MessageBox.Show(msg, "Error de Base de Datos", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void OnCalcInputChanged(object sender, TextChangedEventArgs e)
        {
            CalculateWorkbenchEstimates();
        }

        private void CalculateWorkbenchEstimates()
        {
            if (TxtCalcTonnage == null || TxtCalcCostBP == null || TxtCalcBPRate == null || 
                LblCalcBuildDays == null || LblCalcRetoolBP == null || LblCalcRetoolDays == null)
            {
                return;
            }

            double.TryParse(TxtCalcTonnage.Text, out double tons);
            double.TryParse(TxtCalcCostBP.Text, out double costBP);
            double.TryParse(TxtCalcBPRate.Text, out double bpRate);

            if (bpRate <= 0) bpRate = 500;
            if (costBP <= 0) costBP = 100;

            double buildDays = (costBP / bpRate) * 365.0;
            double buildMonths = buildDays / 30.0;
            LblCalcBuildDays.Text = $"{buildDays:N0} Días ({buildMonths:F1} m)";

            double retoolCostBP = costBP * 0.25;
            double retoolDays = (retoolCostBP / bpRate) * 365.0;

            LblCalcRetoolBP.Text = $"{retoolCostBP:N1} BP";
            LblCalcRetoolDays.Text = $"{retoolDays:N0} Días";
        }
    }
}
