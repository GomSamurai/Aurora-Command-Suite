using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class CommandersHQView : UserControl
    {
        private DatabaseService? _dbService;
        private int _currentRaceId;
        private List<CommanderInfo> _allCommanders = new List<CommanderInfo>();

        public CommanderInfo? SelectedCommander => DgCommanders?.SelectedItem as CommanderInfo;

        public CommandersHQView()
        {
            InitializeComponent();
            InitializeFilters();
        }

        private void InitializeFilters()
        {
            var roles = new List<string>
            {
                "📂 Todos los Comandantes",
                "🎓 Científicos e Investigadores",
                "⚓ Oficiales Navales y Capitanes",
                "🏛️ Gobernadores Planetarios",
                "⚔️ Comandantes Terrestres"
            };
            CmbRoleFilter.ItemsSource = roles;
            CmbRoleFilter.SelectedIndex = 0;

            var statuses = new List<string>
            {
                "📂 Todos los Estados",
                "🟢 Solo Asignados",
                "⚪ Solo Disponibles"
            };
            CmbStatusFilter.ItemsSource = statuses;
            CmbStatusFilter.SelectedIndex = 0;
        }

        public void LoadCommandersData(DatabaseService? dbService, int raceId)
        {
            _dbService = dbService;
            _currentRaceId = raceId;
            RefreshCommanders();
        }

        public void RefreshCommanders()
        {
            if (_dbService == null) return;

            int selId = SelectedCommander?.CommanderID ?? 0;
            _allCommanders = _dbService.GetCommanders(_currentRaceId);

            // Update Staff Telemetry
            LblStaffTotal.Text = _allCommanders.Count.ToString();
            LblStaffScientists.Text = _allCommanders.Count(c => c.CommanderType == 1).ToString();
            LblStaffNaval.Text = _allCommanders.Count(c => c.CommanderType == 2).ToString();
            LblStaffGovernors.Text = _allCommanders.Count(c => c.CommanderType == 3).ToString();

            double avgSeniority = _allCommanders.Count > 0 ? _allCommanders.Average(c => c.Seniority) : 0;
            LblStaffAvgSeniority.Text = $"{avgSeniority:F1} Rating";

            FilterCommanders(selId);
        }

        private void FilterCommanders(int targetId = 0)
        {
            if (DgCommanders == null) return;

            var query = TxtSearchCommander?.Text?.Trim().ToLower() ?? string.Empty;
            int roleIdx = CmbRoleFilter?.SelectedIndex ?? 0;
            int statusIdx = CmbStatusFilter?.SelectedIndex ?? 0;

            var filtered = _allCommanders.Where(c =>
            {
                bool matchesQuery = string.IsNullOrEmpty(query) || 
                                   c.Name.ToLower().Contains(query) || 
                                   c.Title.ToLower().Contains(query) || 
                                   c.AssignmentLocation.ToLower().Contains(query);

                bool matchesRole = roleIdx switch
                {
                    1 => c.CommanderType == 1, // Scientist
                    2 => c.CommanderType == 2, // Naval
                    3 => c.CommanderType == 3, // Governor
                    4 => c.CommanderType == 4, // Ground
                    _ => true
                };

                bool matchesStatus = statusIdx switch
                {
                    1 => c.IsAssigned,
                    2 => !c.IsAssigned,
                    _ => true
                };

                return matchesQuery && matchesRole && matchesStatus;
            }).ToList();

            DgCommanders.ItemsSource = filtered;

            if (filtered.Count > 0)
            {
                var target = filtered.FirstOrDefault(c => c.CommanderID == targetId) ?? filtered[0];
                DgCommanders.SelectedItem = target;
            }
            else
            {
                UpdateDossier(null);
            }
        }

        private void DgCommanders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDossier(SelectedCommander);
        }

        private void UpdateDossier(CommanderInfo? cmdr)
        {
            if (cmdr == null)
            {
                TxtDossierIcon.Text = "🎖️";
                LblDossierName.Text = "Ningún Comandante Seleccionado";
                LblDossierTitle.Text = "-";
                LblDossierLocation.Text = "-";
                LblDossierSeniority.Text = "0 Rating";
                LblDossierPromotion.Text = "0 / 100";
                PbPromotion.Value = 0;
                LblDossierLoyalty.Text = "0%";
                PbLoyalty.Value = 0;
                IcDetailedBonuses.ItemsSource = null;
                CalculateLeadershipImpact();
                return;
            }

            TxtDossierIcon.Text = cmdr.RoleIcon;
            LblDossierName.Text = cmdr.Name;
            LblDossierTitle.Text = cmdr.Title;
            LblDossierLocation.Text = cmdr.AssignmentLocation;
            LblDossierSeniority.Text = $"{cmdr.Seniority:N0} Rating";

            LblDossierPromotion.Text = $"{cmdr.PromotionScore:F0} / 100";
            PbPromotion.Value = Math.Min(100, cmdr.PromotionScore);

            LblDossierLoyalty.Text = $"{cmdr.LoyaltyRating:F0}%";
            PbLoyalty.Value = Math.Min(100, cmdr.LoyaltyRating);

            IcDetailedBonuses.ItemsSource = cmdr.DetailedBonuses;

            // Custom calculator labels depending on role
            if (LblImpactInputLabel != null)
            {
                LblImpactInputLabel.Text = cmdr.CommanderType switch
                {
                    1 => "Puntos de I+D Base del Lab (RP/año):",
                    3 => "Producción de Minería / Industria Base (tons):",
                    2 => "Velocidad Base de la Flota (km/s):",
                    _ => "Poder de Combate Terrestre Base (BP):"
                };
            }

            CalculateLeadershipImpact();
        }

        private void TxtSearchCommander_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterCommanders();
        }

        private void CmbRoleFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterCommanders();
        }

        private void CmbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterCommanders();
        }

        private void OnImpactInputChanged(object sender, TextChangedEventArgs e)
        {
            CalculateLeadershipImpact();
        }

        private void CalculateLeadershipImpact()
        {
            if (TxtImpactBase == null || LblImpactExtra == null || LblImpactTotal == null) return;

            double.TryParse(TxtImpactBase.Text, out double baseVal);
            if (baseVal <= 0) baseVal = 1000;

            double topBonusPct = 15.0; // Default baseline bonus
            if (SelectedCommander != null && SelectedCommander.DetailedBonuses.Count > 0)
            {
                topBonusPct = SelectedCommander.DetailedBonuses[0].ValuePercent;
            }

            double extra = baseVal * (topBonusPct / 100.0);
            double total = baseVal + extra;

            LblImpactExtra.Text = $"+{extra:N1} (+{topBonusPct:F1}%)";
            LblImpactTotal.Text = $"{total:N1}";
        }

        private void BtnPromote_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCommander == null || _dbService == null)
            {
                MessageBox.Show("Selecciona un comandante de la lista antes de conceder una promoción.", "Comandante No Seleccionado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"¿Confirmas la concesión de ascenso y promoción de rango para '{SelectedCommander.Name}' ({SelectedCommander.Title})?", "Confirmar Promoción", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (_dbService.PromoteCommander(SelectedCommander.CommanderID, out string msg))
                {
                    MessageBox.Show(msg, "Promoción Otorgada", MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshCommanders();
                }
                else
                {
                    MessageBox.Show(msg, "Error de Promoción", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnAssignTask_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCommander == null || _dbService == null)
            {
                MessageBox.Show("Selecciona un comandante para asignar un nuevo destino.", "Comandante No Seleccionado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string defaultLoc = SelectedCommander.CommanderType switch
            {
                1 => "Laboratorio de Investigación Láser N° 1",
                3 => "Gobernación de la Colonia Sol-3 (Tierra)",
                2 => "Flota Principal - S.M.S. Numancia",
                _ => "1ª División de Infantería Mecanizada"
            };

            if (_dbService.AssignCommanderLocation(SelectedCommander.CommanderID, defaultLoc, out string msg))
            {
                MessageBox.Show(msg, "Destino Asignado", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshCommanders();
            }
        }

        private void BtnAutoAssign_Click(object sender, RoutedEventArgs e)
        {
            if (_allCommanders == null || _allCommanders.Count == 0)
            {
                MessageBox.Show("No hay oficiales cargados en el estado mayor.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var unassigned = _allCommanders.Where(c => !c.IsAssigned).ToList();
            if (unassigned.Count == 0)
            {
                unassigned = _allCommanders.Take(10).ToList();
            }

            var topScientists = _allCommanders.Where(c => c.CommanderType == 1).OrderByDescending(c => c.DetailedBonuses.Count > 0 ? c.DetailedBonuses[0].ValuePercent : 0).Take(3).ToList();
            var topNaval = _allCommanders.Where(c => c.CommanderType == 2).OrderByDescending(c => c.DetailedBonuses.Count > 0 ? c.DetailedBonuses[0].ValuePercent : 0).Take(3).ToList();

            string report = "⚡ RECOMENDACIONES INTELIGENTES DE ASIGNACIÓN (AURORADB.DB)\n\n";
            report += "👨‍🔬 TOP CIENTÍFICOS PARA PROYECTOS DE I+D CLAVE:\n";
            foreach (var s in topScientists)
            {
                report += $"• {s.Name}: {s.PrimaryBonusDisplay} ({s.Title})\n";
            }

            report += "\n⚓ TOP OFICIALES NAVALES PARA MANDOS DE FLOTA:\n";
            foreach (var n in topNaval)
            {
                report += $"• {n.Name}: {n.PrimaryBonusDisplay} ({n.Title})\n";
            }

            report += $"\n💡 Oficiales Disponibles Escaneados: {unassigned.Count} de {_allCommanders.Count} oficiales.";

            MessageBox.Show(report, "⚡ Asignador Automático de Alto Mando", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
