using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class ActiveFleetsView : UserControl
    {
        private DatabaseService? _dbService;
        private int _currentRaceId;

        private ActiveFleet? _selectedFleet;
        private ActiveShip? _selectedShip;
        private List<ActiveFleet> _allFleets = new List<ActiveFleet>();

        public ActiveFleetsView()
        {
            InitializeComponent();
        }

        public void LoadData(DatabaseService dbService, int raceId)
        {
            _dbService = dbService;
            _currentRaceId = raceId;
            if (_dbService == null) return;

            RefreshData();
        }

        public void LoadFleetsData(DatabaseService dbService, int raceId) => LoadData(dbService, raceId);

        private void RefreshData()
        {
            if (_dbService == null) return;

            _allFleets = _dbService.GetActiveFleets(_currentRaceId);
            DgActiveFleets.ItemsSource = _allFleets;
            CboTargetFleets.ItemsSource = _allFleets;

            if (LblFleetsCountHeader != null) LblFleetsCountHeader.Text = $"{_allFleets.Count} Flotas Activas";

            if (_allFleets.Count > 0)
            {
                DgActiveFleets.SelectedIndex = 0;
            }
        }

        private void DgActiveFleets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgActiveFleets.SelectedItem is ActiveFleet fleet)
            {
                _selectedFleet = fleet;
                LblFleetShipsHeader.Text = $"🛸 NAVES ASIGNADAS A: {fleet.FleetName.ToUpper()}";
                DgActiveShips.ItemsSource = fleet.Ships;

                if (LblShipsCountHeader != null) LblShipsCountHeader.Text = $"{fleet.ShipCount} Naves";
                if (LblFleetSpeedLimit != null) LblFleetSpeedLimit.Text = $"{fleet.SpeedKmS:N0} km/s";

                // Range estimation
                double totalFuel = fleet.TotalFuelLiters;
                double lightYears = Math.Round((totalFuel / 15000.0) * 1.2, 1);
                if (LblFleetRangeLightYears != null) LblFleetRangeLightYears.Text = $"{lightYears:N1} Años Luz";

                // MSP estimation
                double totalMsp = fleet.Ships.Sum(s => s.CurrentMSP);
                if (LblFleetTotalMSP != null) LblFleetTotalMSP.Text = $"{totalMsp:N0} MSP";

                // Average Morale
                double avgMorale = fleet.Ships.Count > 0 ? fleet.Ships.Average(s => s.CrewMorale) : 100.0;
                if (LblFleetAvgMorale != null)
                {
                    LblFleetAvgMorale.Text = $"{avgMorale:F0}% (Óptima)";
                    LblFleetAvgMorale.Foreground = avgMorale >= 90 ? System.Windows.Media.Brushes.SpringGreen : System.Windows.Media.Brushes.Gold;
                }

                // Commander Dossier
                if (fleet.AssignedCommander != null)
                {
                    var cmdInfo = fleet.AssignedCommander;
                    if (LblCommanderName != null) LblCommanderName.Text = cmdInfo.FullTitleAndName;

                    if (cmdInfo.HasCommander)
                    {
                        if (LblCommanderHealth != null) LblCommanderHealth.Text = cmdInfo.HealthStatus;
                        if (LblCommanderLoyaltySeniority != null) LblCommanderLoyaltySeniority.Text = $"{cmdInfo.Loyalty:F0}% Lealtad | Antigüedad #{cmdInfo.Seniority}";
                        if (LblCommanderKills != null) LblCommanderKills.Text = $"{cmdInfo.MilitaryKillsTons:N0}t Militar | {cmdInfo.CommercialKillsTons:N0}t Comercial";
                        if (LblCommanderTraits != null) LblCommanderTraits.Text = cmdInfo.TraitsDisplay;
                        if (LblCommanderAllBonuses != null) LblCommanderAllBonuses.Text = cmdInfo.AllBonusesDisplay;
                    }
                    else
                    {
                        if (LblCommanderHealth != null) LblCommanderHealth.Text = "N/A (Flota Inactiva)";
                        if (LblCommanderLoyaltySeniority != null) LblCommanderLoyaltySeniority.Text = "N/A";
                        if (LblCommanderKills != null) LblCommanderKills.Text = "0t Militar | 0t Comercial";
                        if (LblCommanderTraits != null) LblCommanderTraits.Text = "Ninguno (Agrupación sin buques)";
                        if (LblCommanderAllBonuses != null) LblCommanderAllBonuses.Text = "0% (Escuadra sin buques)";
                    }
                }

                // Tactical Advisor
                if (LblTacticalAdvisorText != null)
                {
                    if (totalFuel == 0 && fleet.ShipCount > 0)
                    {
                        LblTacticalAdvisorText.Text = "⚠️ ALERTA TÁCTICA: La flota no dispone de combustible de Sorium. Utiliza el botón 'Reabastecer Combustible' para cargar los tanques desde el inventario colonial.";
                    }
                    else if (fleet.ShipCount == 0)
                    {
                        LblTacticalAdvisorText.Text = "💡 EVALUACIÓN TÁCTICA: Esta agrupación no tiene naves asignadas actualmente. Utiliza el panel de reasignación a la derecha para transferir buques.";
                    }
                    else
                    {
                        LblTacticalAdvisorText.Text = $"💡 EVALUACIÓN Y RECOMENDACIÓN TÁCTICA:\nActividad: {fleet.CurrentActivity}\nUbicación: {fleet.NearestColonyDisplay}\n\n{fleet.StrategicRecommendation}";
                    }
                }

                if (fleet.Ships.Count > 0)
                {
                    DgActiveShips.SelectedIndex = 0;
                }
            }
        }

        private void DgActiveShips_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgActiveShips.SelectedItem is ActiveShip ship)
            {
                _selectedShip = ship;
            }
        }

        private void BtnTransferShip_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null) return;
            if (_selectedShip == null)
            {
                MessageBox.Show("Por favor selecciona una nave de la tabla izquierda para transferir.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CboTargetFleets.SelectedItem is ActiveFleet targetFleet)
            {
                if (targetFleet.FleetID == _selectedFleet?.FleetID)
                {
                    MessageBox.Show("La nave ya está asignada a esta misma flota.", "Atención", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                if (_dbService.TransferShipToFleet(_currentRaceId, _selectedShip.ShipID, targetFleet.FleetID, targetFleet.FleetName, out string msg))
                {
                    string fullMsg = $"{msg}\n\n💡 NOTA DE CONEXIÓN EN VIVO:\nPara ver este cambio reflejado en la interfaz de Aurora 4X mientras el juego está abierto:\n• Avanza 1 incremento de tiempo (ej. 5 Segundos) en el juego, o\n• Cierra y vuelve a abrir la ventana de Naval/Fleets del juego.";
                    MessageBox.Show(fullMsg, "Transferencia Naval Registrada en DB", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshData();
                }
            }
            else
            {
                MessageBox.Show("Por favor selecciona una flota de destino del desplegable.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnRefuelFleet_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null || _selectedFleet == null) return;

            if (_dbService.RefuelFleet(_currentRaceId, _selectedFleet.FleetID, _selectedFleet.FleetName, out string msg))
            {
                string fullMsg = $"{msg}\n\n💡 NOTA DE CONEXIÓN EN VIVO:\nPara ver los tanques llenos en la pantalla del juego mientras está abierto, avanza 1 incremento de tiempo (ej. 5 Segundos) en Aurora 4X.";
                MessageBox.Show(fullMsg, "Reabastecimiento Naval", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshData();
            }
        }

        private void BtnReplenishMSP_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null || _selectedFleet == null) return;

            if (_dbService.ReplenishFleetMSP(_currentRaceId, _selectedFleet.FleetID, _selectedFleet.FleetName, out string msg))
            {
                string fullMsg = $"{msg}\n\n💡 NOTA DE CONEXIÓN EN VIVO:\nPara ver los suministros de repuesto en el juego, avanza 1 incremento de tiempo (ej. 5 Segundos) en Aurora 4X.";
                MessageBox.Show(fullMsg, "Mantenimiento y Repuestos", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshData();
            }
        }
    }
}
