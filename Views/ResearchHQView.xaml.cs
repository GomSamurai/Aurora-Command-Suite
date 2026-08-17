using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class ResearchHQView : UserControl
    {
        private DatabaseService? _dbService;
        private int _currentRaceId;

        private List<TechTreeItemInfo> _allTechs = new List<TechTreeItemInfo>();
        private TechTreeItemInfo? _selectedTech;
        private ResearchProjectInfo? _selectedActiveProject;

        public ResearchHQView()
        {
            InitializeComponent();
        }

        public void LoadResearchData(DatabaseService dbService, int raceId)
        {
            _dbService = dbService;
            _currentRaceId = raceId;

            // Categories
            var categories = new List<string>
            {
                "Todas las Categorías",
                "⚡ Potencia y Propulsión",
                "💥 Energía y Láseres",
                "🚀 Misiles y Cinéticas",
                "📡 Sensores y Control",
                "🧬 Biología y Ciencias",
                "🏗️ Construcción y Logística"
            };
            CmbTechCategory.ItemsSource = categories;
            CmbTechCategory.SelectedIndex = 0;

            RefreshData();
        }

        private void RefreshData()
        {
            if (_dbService == null) return;

            var activeProjects = _dbService.GetActiveResearchProjects(_currentRaceId);
            DgResearchProjects.ItemsSource = activeProjects;

            var infra = _dbService.GetEmpireInfrastructure(_currentRaceId);
            double totalEmpireLabs = infra.Where(i => i.Name.Contains("Laboratorio") || i.Name.Contains("Research")).Sum(i => i.Amount);
            if (totalEmpireLabs <= 0) totalEmpireLabs = 17; // Default from DB population

            int labsInUse = activeProjects.Sum(p => p.FacilitiesCount);
            int freeLabs = Math.Max(0, (int)totalEmpireLabs - labsInUse);
            double pctInUse = totalEmpireLabs > 0 ? (labsInUse / totalEmpireLabs) * 100.0 : 0;

            if (LblActiveLabsCount != null) LblActiveLabsCount.Text = $"{labsInUse} Labs en Uso";
            if (LblGlobalTotalLabs != null) LblGlobalTotalLabs.Text = $"{totalEmpireLabs:N0} Labs";
            if (LblGlobalLabsInUse != null) LblGlobalLabsInUse.Text = $"{labsInUse} / {freeLabs} ({pctInUse:F0}%)";

            var scientists = _dbService.GetScientists(_currentRaceId);
            double avgBonus = scientists.Count > 0 ? scientists.Average(s => s.BonusPercent) : 25.0;
            if (LblGlobalAvgBonus != null) LblGlobalAvgBonus.Text = $"+{avgBonus:F1}% Promedio";

            double baseRpPerLab = 200.0;
            double globalTotalRpYear = Math.Round(labsInUse * baseRpPerLab * (1.0 + (avgBonus / 100.0)), 0);
            if (LblGlobalTotalRpYear != null) LblGlobalTotalRpYear.Text = $"{globalTotalRpYear:N0} RP/Año";

            _allTechs = _dbService.GetTechTree(_currentRaceId);
            CmbSelectTech.ItemsSource = _allTechs;
            FilterTechTree();

            CmbScientists.ItemsSource = scientists;
            if (scientists.Count > 0) CmbScientists.SelectedIndex = 0;
            if (_allTechs.Count > 0) CmbSelectTech.SelectedIndex = 0;
        }

        private bool _isSyncingTechSelection = false;

        private void FilterTechTree()
        {
            if (_allTechs == null || DgTechTree == null) return;

            string searchText = TxtSearchTech?.Text.Trim().ToLower() ?? "";
            string category = CmbTechCategory?.SelectedItem as string ?? "Todas las Categorías";

            var filtered = _allTechs.AsEnumerable();

            if (category != "Todas las Categorías")
            {
                filtered = filtered.Where(t => t.CategoryName == category);
            }

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filtered = filtered.Where(t => t.TechName.ToLower().Contains(searchText) || t.CategoryName.ToLower().Contains(searchText));
            }

            DgTechTree.ItemsSource = filtered.ToList();
        }

        private void TxtSearchTech_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterTechTree();
        }

        private void CmbTechCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterTechTree();
        }

        private void CmbSelectTech_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isSyncingTechSelection) return;
            if (CmbSelectTech.SelectedItem is TechTreeItemInfo tech)
            {
                _isSyncingTechSelection = true;
                _selectedTech = tech;
                _selectedActiveProject = null;
                DgTechTree.SelectedItem = tech;
                _isSyncingTechSelection = false;
                RecalculateSimulation();
            }
        }

        private void DgTechTree_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isSyncingTechSelection) return;
            if (DgTechTree.SelectedItem is TechTreeItemInfo tech)
            {
                _isSyncingTechSelection = true;
                _selectedTech = tech;
                _selectedActiveProject = null;
                CmbSelectTech.SelectedItem = tech;
                _isSyncingTechSelection = false;
                RecalculateSimulation();
            }
        }

        private void DgResearchProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgResearchProjects.SelectedItem is ResearchProjectInfo proj)
            {
                _selectedActiveProject = proj;
                _selectedTech = null;
                SldLabs.Value = proj.FacilitiesCount;
                RecalculateSimulation();
            }
        }

        private void CmbScientists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RecalculateSimulation();
        }

        private void OnAssignmentParamChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LblValLabs != null && SldLabs != null)
            {
                LblValLabs.Text = $"{SldLabs.Value:F0} Labs";
            }
            RecalculateSimulation();
        }

        private void RecalculateSimulation()
        {
            if (LblSimRpYear == null || LblSimBonus == null || LblSimEstimatedTime == null || SldLabs == null) return;

            int labs = Convert.ToInt32(SldLabs.Value);
            double baseRpPerLab = 200.0; // Standard RP output per lab per year

            double scientistBonusPercent = 0.0;
            bool isMatchingField = false;

            if (CmbScientists.SelectedItem is ScientistInfo scientist)
            {
                scientistBonusPercent = scientist.BonusPercent;
                if (_selectedTech != null && scientist.FieldName == _selectedTech.CategoryName)
                {
                    isMatchingField = true;
                    scientistBonusPercent += 10.0; // Matching field bonus
                }
            }

            double totalRpYear = Math.Round(labs * baseRpPerLab * (1.0 + (scientistBonusPercent / 100.0)), 0);

            double rpNeeded = 1000.0;
            if (_selectedTech != null) rpNeeded = _selectedTech.DevelopCost;
            else if (_selectedActiveProject != null) rpNeeded = Math.Max(100.0, _selectedActiveProject.RPRequired - _selectedActiveProject.RPAssigned);

            double yearsNeeded = totalRpYear > 0 ? rpNeeded / totalRpYear : 0;
            double monthsNeeded = Math.Round(yearsNeeded * 12.0, 1);
            int totalDays = Convert.ToInt32(Math.Round(yearsNeeded * 365.0));

            LblSimRpYear.Text = $"{totalRpYear:N0} RP/Año";
            LblSimBonus.Text = isMatchingField ? $"+{scientistBonusPercent:F0}% (¡Especialidad Coincidente!)" : $"+{scientistBonusPercent:F0}% (Básico)";

            if (totalDays < 30)
            {
                LblSimEstimatedTime.Text = $"{totalDays} Días";
            }
            else if (totalDays < 365)
            {
                LblSimEstimatedTime.Text = $"{monthsNeeded:F1} Meses ({totalDays} Días)";
            }
            else
            {
                int yrs = totalDays / 365;
                int mths = (totalDays % 365) / 30;
                LblSimEstimatedTime.Text = $"{yrs} Año(s) y {mths} Mes(es) ({totalDays} Días)";
            }

            if (LblResearchHint != null)
            {
                LblResearchHint.Text = isMatchingField
                    ? "✨ ¡Excelente emparejamiento! El científico posee la misma especialidad científica de la tecnología, acelerando el desarrollo un +10% adicional."
                    : "💡 Consejo: Selecciona un científico cuya especialidad coincida con la rama del proyecto para recibir la bonificación de aceleración técnica máxima.";
            }

            UpdateScientistDossier();
            UpdateTacticalTechDossier();
        }

        private void UpdateTacticalTechDossier()
        {
            if (LblTacticalTechName == null || LblTacticalCategory == null || TxtTacticalDescription == null || LblTacticalCost == null) return;

            if (_selectedTech != null)
            {
                LblTacticalTechName.Text = _selectedTech.TechName;
                LblTacticalCategory.Text = $"Categoría: {_selectedTech.CategoryName}";
                TxtTacticalDescription.Text = _selectedTech.Description;
                LblTacticalCost.Text = $"{_selectedTech.DevelopCost:N0} RP";
            }
            else if (_selectedActiveProject != null)
            {
                LblTacticalTechName.Text = $"[ACTIVO] {_selectedActiveProject.TechName}";
                LblTacticalCategory.Text = $"Especialidad: {_selectedActiveProject.ScientistFieldDisplay}";
                TxtTacticalDescription.Text = TechDescriptionResolver.ResolveDescription(_selectedActiveProject.TechName, _selectedActiveProject.ScientistFieldDisplay);
                LblTacticalCost.Text = $"{_selectedActiveProject.RPRequired:N0} RP ({_selectedActiveProject.ProgressDisplay})";
            }
            else
            {
                LblTacticalTechName.Text = "Tecnología Seleccionada";
                LblTacticalCategory.Text = "Categoría: General";
                TxtTacticalDescription.Text = "Selecciona una tecnología del desplegable o del árbol para consultar su guía táctica completa, utilidades en juego y componentes desbloqueados.";
                LblTacticalCost.Text = "0 RP";
            }
        }

        private void UpdateScientistDossier()
        {
            if (LblDossierScientistName == null || LblDossierField == null || LblDossierBonus == null || LblDossierMaxLabs == null || LblDossierMatchStatus == null || BrdDossierMatchStatus == null) return;

            if (CmbScientists.SelectedItem is ScientistInfo scientist)
            {
                LblDossierScientistName.Text = scientist.Name;
                LblDossierRating.Text = $"Antigüedad: {scientist.Seniority} Rating | Lealtad: {scientist.Loyalty:F0}% | ID #{scientist.CommanderID}";
                LblDossierField.Text = string.IsNullOrWhiteSpace(scientist.FieldName) ? "General" : scientist.FieldName;
                LblDossierBonus.Text = $"+{scientist.BonusPercent:F1}%";
                LblDossierMaxLabs.Text = $"{scientist.MaxLabs} Labs Max (Admin Rating)";

                // Adjust Slider Maximum to match scientist's real max labs capacity
                if (SldLabs != null)
                {
                    SldLabs.Maximum = Math.Max(1, scientist.MaxLabs);
                    if (SldLabs.Value > SldLabs.Maximum) SldLabs.Value = SldLabs.Maximum;
                }

                bool isMatch = _selectedTech != null && scientist.FieldName == _selectedTech.CategoryName;
                if (isMatch)
                {
                    BrdDossierMatchStatus.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(13, 40, 24));
                    BrdDossierMatchStatus.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 136));
                    LblDossierMatchStatus.Text = $"⚡ COINCIDENCIA IDEAL (+{scientist.BonusPercent:F0}% BONUS APLICADO)";
                    LblDossierMatchStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 136));
                }
                else
                {
                    BrdDossierMatchStatus.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(43, 26, 26));
                    BrdDossierMatchStatus.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 180, 0));
                    LblDossierMatchStatus.Text = $"⚠️ ESPECIALIDAD DISTINTA (BONIFICACIÓN BÁSICA DE +{scientist.BonusPercent:F0}%)";
                    LblDossierMatchStatus.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 180, 0));
                }
            }
            else
            {
                LblDossierScientistName.Text = "Sin Científico Seleccionado";
                LblDossierRating.Text = "Selecciona un científico de la lista para ver su dossier.";
                LblDossierField.Text = "Ninguna";
                LblDossierBonus.Text = "+0,0%";
                LblDossierMaxLabs.Text = "0 Labs";
                LblDossierMatchStatus.Text = "⚠️ SIN CIENTÍFICO ASIGNADO";
            }
        }

        private void BtnAssignResearch_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null) return;
            if (_selectedTech == null)
            {
                MessageBox.Show("Por favor selecciona una tecnología investigable del árbol tecnológico.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int labs = Convert.ToInt32(SldLabs.Value);
            int commanderId = (CmbScientists.SelectedItem is ScientistInfo s) ? s.CommanderID : 0;

            if (_dbService.AssignResearchProject(_currentRaceId, _selectedTech.TechSystemID, labs, commanderId, out string msg))
            {
                MessageBox.Show(msg, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshData();
            }
            else
            {
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelResearch_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null) return;
            if (_selectedActiveProject == null)
            {
                MessageBox.Show("Por favor selecciona un proyecto activo de la lista para cancelar.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_dbService.CancelResearchProject(_currentRaceId, _selectedActiveProject.ProjectID, out string msg))
            {
                MessageBox.Show(msg, "Proyecto Cancelado", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshData();
            }
            else
            {
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAdd1Lab_Click(object sender, RoutedEventArgs e) => ModifySelectedProjectLabs(1);
        private void BtnRemove1Lab_Click(object sender, RoutedEventArgs e) => ModifySelectedProjectLabs(-1);
        private void BtnAdd5Labs_Click(object sender, RoutedEventArgs e) => ModifySelectedProjectLabs(5);
        private void BtnRemove5Labs_Click(object sender, RoutedEventArgs e) => ModifySelectedProjectLabs(-5);

        private void ModifySelectedProjectLabs(int delta)
        {
            if (_dbService == null || _selectedActiveProject == null)
            {
                MessageBox.Show("Por favor selecciona un proyecto activo de la lista para modificar sus laboratorios asignados.", "Atención", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (_dbService.UpdateResearchProjectLabs(_selectedActiveProject.ProjectID, delta, out string msg))
            {
                RefreshData();
            }
            else
            {
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
