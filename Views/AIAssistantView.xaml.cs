using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AuroraDesignSuite.Models;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class AIAssistantView : UserControl
    {
        private DatabaseService? _dbService;
        private AIService? _aiService;
        private int _currentRaceId;
        private List<CustomPromptItem> _customPrompts = new List<CustomPromptItem>();
        private string _lastAiResponse = string.Empty;

        public AIAssistantView()
        {
            InitializeComponent();
            _aiService = new AIService();
            AddInitialWelcomeMessage();
        }

        public void LoadData(DatabaseService dbService, int raceId)
        {
            _dbService = dbService;
            _currentRaceId = raceId;
            if (_dbService == null || _aiService == null) return;

            _customPrompts = _aiService.LoadCustomPrompts();
            RefreshTelemetrySidebar();
            RefreshCustomPromptsUI();
        }

        private void RefreshTelemetrySidebar()
        {
            if (_dbService == null) return;

            var colonies = _dbService.GetColonies(_currentRaceId);
            var fleets = _dbService.GetActiveFleets(_currentRaceId);
            var infra = _dbService.GetEmpireInfrastructure(_currentRaceId);
            var research = _dbService.GetActiveResearchProjects(_currentRaceId);

            double totalPop = colonies.Sum(c => c.PopulationMillions);
            double totalRevenueBP = Math.Round(totalPop * 10.0, 0);

            if (LblAuditPop != null) LblAuditPop.Text = $"{totalPop:N2} M";
            if (LblAuditCapital != null) LblAuditCapital.Text = colonies.Count > 0 ? colonies.First().PopName : "Sol";
            if (LblAuditWealth != null) LblAuditWealth.Text = $"+{totalRevenueBP * 0.10:N0} BP";

            double planetaryFuel = colonies.Sum(c => c.FuelStockpile);
            double shipFuel = fleets.Sum(f => f.TotalFuelLiters);
            double totalFuel = planetaryFuel + shipFuel;
            if (LblAuditFuel != null)
            {
                LblAuditFuel.Text = $"{totalFuel:N0} L";
                LblAuditFuel.ToolTip = $"Reserva Total Imperio: {totalFuel:N0} L\n• Depósitos Planetarios: {planetaryFuel:N0} L\n• Combustible en Naves: {shipFuel:N0} L";
            }

            double constFactories = infra.Where(i => i.Name.Contains("Construcción") || i.Name.Contains("Convencional")).Sum(i => i.Amount);
            if (LblAuditFactories != null) LblAuditFactories.Text = $"{constFactories:N0} Fábricas";

            if (LblAuditResearch != null) LblAuditResearch.Text = $"{research.Count} Proyectos";

            int activeFleets = fleets.Count(f => f.ShipCount > 0);
            int totalShips = fleets.Sum(f => f.ShipCount);
            int totalFleets = fleets.Count;
            if (LblAuditFleets != null)
            {
                LblAuditFleets.Text = $"{activeFleets} Operativa ({totalShips} Nave)";
                LblAuditFleets.ToolTip = $"Flotas Operativas con naves: {activeFleets} ({totalShips} Nave activa)\nAgrupaciones registradas en base de datos: {totalFleets} ({totalFleets - activeFleets} asignaciones vacías)";
            }

            // Update welcome message dynamically with exact live telemetry
            if (PnlChatMessages != null && PnlChatMessages.Children.Count <= 1)
            {
                PnlChatMessages.Children.Clear();
                string capitalName = colonies.Count > 0 ? colonies.First().PopName : "Sol";
                AddChatMessage("🖥️ MATRIZ DE INTELIGENCIA IMPERIAL",
                    "Enlace AuroraDB.db establecido. Telemetría sincronizada en tiempo real:\n" +
                    $"• Población Total: {totalPop:N2}M ({capitalName})\n" +
                    $"• Capacidad Industrial: {constFactories:N0} Fábricas (Industria Convencional e Industrial)\n" +
                    $"• Reserva Combustible: {totalFuel:N0} L ({planetaryFuel:N0} L Planetarios + {shipFuel:N0} L en Naves)\n" +
                    $"• Flotas Operativas: {activeFleets} Flota ({totalShips} Nave activa)\n" +
                    $"• Infraestructura I+D: {research.Count} Proyectos Activos\n\n" +
                    "Matriz computacional en espera. Formula consultas de vector o selecciona una auditoría directa a la izquierda.",
                    isUser: false);
            }
        }

        private void RefreshCustomPromptsUI()
        {
            if (PnlCustomPromptsList == null) return;
            PnlCustomPromptsList.Children.Clear();

            foreach (var item in _customPrompts)
            {
                var border = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(11, 14, 20)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(28, 37, 54)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(6),
                    Margin = new Thickness(0, 0, 0, 4)
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var titleBlock = new TextBlock
                {
                    Text = item.Title,
                    Foreground = item.IsPreset ? new SolidColorBrush(Color.FromRgb(255, 180, 0)) : new SolidColorBrush(Color.FromRgb(85, 255, 85)),
                    FontWeight = FontWeights.Bold,
                    FontSize = 11,
                    VerticalAlignment = VerticalAlignment.Center,
                    ToolTip = item.PromptText
                };
                Grid.SetColumn(titleBlock, 0);

                var spActions = new StackPanel { Orientation = Orientation.Horizontal };
                Grid.SetColumn(spActions, 1);

                var btnRun = new Button
                {
                    Content = "⚡ Ejecutar",
                    Background = new SolidColorBrush(Color.FromRgb(28, 37, 54)),
                    Foreground = new SolidColorBrush(Color.FromRgb(0, 240, 255)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 240, 255)),
                    FontWeight = FontWeights.Bold,
                    FontSize = 10,
                    Padding = new Thickness(6, 2, 6, 2),
                    Margin = new Thickness(4, 0, 0, 0),
                    ToolTip = $"Ejecutar consulta: {item.PromptText}"
                };
                btnRun.Click += (s, e) => ExecuteQuery(item.PromptText);
                spActions.Children.Add(btnRun);

                if (!item.IsPreset)
                {
                    var btnDel = new Button
                    {
                        Content = "🗑️",
                        Background = new SolidColorBrush(Color.FromRgb(28, 37, 54)),
                        Foreground = new SolidColorBrush(Color.FromRgb(255, 85, 85)),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(255, 85, 85)),
                        FontSize = 10,
                        Padding = new Thickness(4, 2, 4, 2),
                        Margin = new Thickness(4, 0, 0, 0),
                        ToolTip = "Eliminar esta consulta personalizada"
                    };
                    btnDel.Click += (s, e) => DeleteCustomPrompt(item);
                    spActions.Children.Add(btnDel);
                }

                grid.Children.Add(titleBlock);
                grid.Children.Add(spActions);
                border.Child = grid;

                PnlCustomPromptsList.Children.Add(border);
            }
        }

        private void BtnSaveCustomPrompt_Click(object sender, RoutedEventArgs e)
        {
            string title = TxtCustomTitle.Text.Trim();
            string prompt = TxtCustomPrompt.Text.Trim();

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(prompt))
            {
                MessageBox.Show("Por favor introduce un Título y el Texto del Prompt para guardar la consulta.", "Consulta Incompleta", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var newItem = new CustomPromptItem
            {
                Title = title.StartsWith("⭐") ? title : $"⭐ {title}",
                PromptText = prompt,
                IsPreset = false
            };

            _customPrompts.Add(newItem);
            _aiService?.SaveCustomPrompts(_customPrompts);
            TxtCustomTitle.Clear();
            TxtCustomPrompt.Clear();
            RefreshCustomPromptsUI();
        }

        private void DeleteCustomPrompt(CustomPromptItem item)
        {
            _customPrompts.Remove(item);
            _aiService?.SaveCustomPrompts(_customPrompts);
            RefreshCustomPromptsUI();
        }

        private void AddInitialWelcomeMessage()
        {
            AddChatMessage("🖥️ MATRIZ DE INTELIGENCIA IMPERIAL",
                "Enlace AuroraDB.db establecido. Sincronizando vectores de telemetría...\n\n" +
                "Matriz computacional en espera. Formula consultas de vector o selecciona una auditoría directa a la izquierda.",
                isUser: false);
        }

        private void AddChatMessage(string senderName, string messageText, bool isUser)
        {
            var card = new Border
            {
                Background = isUser ? new SolidColorBrush(Color.FromRgb(15, 23, 42)) : new SolidColorBrush(Color.FromRgb(11, 14, 20)),
                BorderBrush = isUser ? new SolidColorBrush(Color.FromRgb(0, 240, 255)) : new SolidColorBrush(Color.FromRgb(0, 255, 136)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(8),
                Margin = new Thickness(0, 0, 0, 6)
            };

            var sp = new StackPanel();

            var header = new TextBlock
            {
                Text = senderName,
                Foreground = isUser ? new SolidColorBrush(Color.FromRgb(0, 240, 255)) : new SolidColorBrush(Color.FromRgb(0, 255, 136)),
                FontWeight = FontWeights.Bold,
                FontSize = 11,
                Margin = new Thickness(0, 0, 0, 4)
            };

            var body = new TextBlock
            {
                Text = messageText,
                Foreground = Brushes.White,
                FontSize = 11.5,
                TextWrapping = TextWrapping.Wrap
            };

            sp.Children.Add(header);
            sp.Children.Add(body);
            card.Child = sp;

            PnlChatMessages.Children.Add(card);
            ScvChat.ScrollToBottom();
        }

        private async void ExecuteQuery(string userText)
        {
            if (string.IsNullOrWhiteSpace(userText) || _dbService == null || _aiService == null) return;

            AddChatMessage("👨‍✈️ COMANDANTE IMPERIAL", userText, isUser: true);
            TxtUserInput.Clear();

            string context = _aiService.BuildImperialContextPrompt(_dbService, _currentRaceId);
            bool useOnline = ChkUseOnlineAI?.IsChecked == true;

            BtnSendQuery.IsEnabled = false;

            // Temporary loading indicator
            AddChatMessage("🖥️ MATRIZ DE INTELIGENCIA IMPERIAL", "⚡ Procesando vectores de consulta...", isUser: false);

            string aiResponse = await _aiService.AskAIAsync(userText, context, useOnline);
            _lastAiResponse = aiResponse;

            // Remove loading indicator
            if (PnlChatMessages.Children.Count > 0)
            {
                PnlChatMessages.Children.RemoveAt(PnlChatMessages.Children.Count - 1);
            }

            AddChatMessage("🖥️ MATRIZ DE INTELIGENCIA IMPERIAL", aiResponse, isUser: false);
            BtnSendQuery.IsEnabled = true;
        }

        private void BtnSendQuery_Click(object sender, RoutedEventArgs e)
        {
            ExecuteQuery(TxtUserInput.Text);
        }

        private void TxtUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExecuteQuery(TxtUserInput.Text);
            }
        }

        // Direct AI Tool Button Handlers
        private void BtnExportReport_Click(object sender, RoutedEventArgs e)
        {
            if (_dbService == null || _aiService == null) return;
            string exportPath = _aiService.ExportImperialReportToFile(_dbService, _currentRaceId, _lastAiResponse);
            MessageBox.Show($"Informe Ejecutivo de la Matriz exportado correctamente en:\n\n{exportPath}", "Informe Exportado", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnDefenseAudit_Click(object sender, RoutedEventArgs e) => ExecuteQuery("Realiza un auto-diagnóstico completo de vulnerabilidades defensivas, escudos, blindajes y cobertura de sensores activos.");
        private void BtnOptimizeProd_Click(object sender, RoutedEventArgs e) => ExecuteQuery("Simula y calcula la optimización de asignación de fábricas basada en los minerales exóticos disponibles.");
        private void BtnClearChat_Click(object sender, RoutedEventArgs e)
        {
            PnlChatMessages.Children.Clear();
            AddInitialWelcomeMessage();
        }

        // Built-in 1-Click Prompt Button Handlers
        private void BtnPromptGlobalReport_Click(object sender, RoutedEventArgs e) => ExecuteQuery("Genera un informe estratégico completo del estado global de mi imperio.");
        private void BtnPromptMinerals_Click(object sender, RoutedEventArgs e) => ExecuteQuery("Audita mis reservas de minerales exóticos e identifica qué recursos tienen déficit.");
        private void BtnPromptTech_Click(object sender, RoutedEventArgs e) => ExecuteQuery("Evalúa mis proyectos de investigación activos y recomiéndame las mejores tecnologías para investigar ahora.");
        private void BtnPromptNavy_Click(object sender, RoutedEventArgs e) => ExecuteQuery("Realiza un diagnóstico de la fuerza militar y logística de mis flotas en servicio.");
        private void BtnPromptExpansion_Click(object sender, RoutedEventArgs e) => ExecuteQuery("Examina los sistemas estelares descubiertos y dame un plan de expansión y terraformación colonial.");
    }
}
