using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using AuroraDesignSuite.Views;

namespace AuroraDesignSuite.Services
{
    public class AITacticalMonitorService
    {
        private DispatcherTimer? _timer;
        private readonly HashSet<string> _historyHashes = new HashSet<string>();
        private int _checkCycleCount = 0;

        public bool IsActive { get; set; } = false;
        public int IntervalSeconds { get; set; } = 90;
        public double AutoDismissSeconds { get; set; } = 10.0;
        public bool EnableSound { get; set; } = true;

        public bool EnableCritical { get; set; } = true;
        public bool EnableAdvice { get; set; } = true;
        public bool EnableAchievements { get; set; } = true;
        public bool EnableReports { get; set; } = true;

        public event Action<string, AlertType>? OnAlertGenerated;

        public void Start(DatabaseService dbService, AIService aiService, int raceId)
        {
            Stop();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(Math.Max(15, IntervalSeconds))
            };

            _timer.Tick += (s, e) => PerformCheck(dbService, aiService, raceId);
            _timer.Start();
            IsActive = true;

            // Immediate initial check after start
            PerformCheck(dbService, aiService, raceId);
        }

        public void Stop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }
            IsActive = false;
        }

        public void TriggerTestAlert(AlertType type, string customMessage = "")
        {
            string msg = !string.IsNullOrEmpty(customMessage) ? customMessage : type switch
            {
                AlertType.Critical => "🚨 ¡ALERTA IMPERIAL! 15 Laboratorios de I+D se encuentran inactivos sin proyecto asignado.",
                AlertType.Achievement => "🏆 ¡HITO ALCANZADO! La población imperial en Earth ha superado exitosamente los 1.000 Millones de almas.",
                AlertType.Report => "📊 REGISTRO DE BALANCE: Superávit económico de +1.169 BP/año con 1.595 fábricas a pleno rendimiento.",
                _ => "💡 CONSEJO TÁCTICO: Desarrollar 'Motores Magneto-Plasma' incrementará la velocidad de maniobra naval en un +45%."
            };

            DispatchOverlay(msg, type);
        }

        public void PerformCheck(DatabaseService dbService, AIService aiService, int raceId)
        {
            if (dbService == null) return;
            _checkCycleCount++;

            try
            {
                // 1. Check Idle Research Facilities
                if (EnableCritical)
                {
                    var researchProjects = dbService.GetActiveResearchProjects(raceId);
                    var installations = dbService.GetPopulationInstallations(raceId);
                    var labs = installations.FirstOrDefault(i => i.InstallationName.Contains("Laboratorio"));

                    if (labs != null && labs.Amount > 0 && researchProjects.Count == 0)
                    {
                        string alertKey = "IDLE_LABS";
                        if (_historyHashes.Add(alertKey))
                        {
                            DispatchOverlay($"🚨 ¡Atención Comandante! Tienes {labs.Amount:N0} Laboratorios de I+D inactivos sin investigación asignada.", AlertType.Critical);
                            return;
                        }
                    }

                    // 2. Check Idle Industrial Construction Factories
                    var indProjects = dbService.GetIndustrialProjects(raceId);
                    var constFact = installations.FirstOrDefault(i => i.InstallationName.Contains("Fábrica de Construcción"));
                    if (constFact != null && constFact.Amount > 0 && indProjects.Count == 0)
                    {
                        string alertKey = "IDLE_FACTORIES";
                        if (_historyHashes.Add(alertKey))
                        {
                            DispatchOverlay($"🚨 ¡Alerta Industrial! Tus {constFact.Amount:N0} Fábricas de Construcción están sin cola de producción.", AlertType.Critical);
                            return;
                        }
                    }

                    // 3. Check Critical Mineral Stockpiles
                    var colonies = dbService.GetColonies(raceId);
                    var capital = colonies.FirstOrDefault(c => c.IsCapital) ?? colonies.FirstOrDefault();
                    if (capital != null && capital.MineralStockpiles.Duranium < 500.0)
                    {
                        string alertKey = "LOW_DURANIUM";
                        if (_historyHashes.Add(alertKey))
                        {
                            DispatchOverlay($"⚠️ ¡Déficit Mineral Crítico! La reserva de Duranio en {capital.PopName} ha descendido a {capital.MineralStockpiles.Duranium:N0} T.", AlertType.Critical);
                            return;
                        }
                    }
                }

                // 4. Check Imperial Achievements
                if (EnableAchievements)
                {
                    double popM = dbService.GetTotalEmpirePopulation(raceId);
                    if (popM >= 1000.0)
                    {
                        string alertKey = "ACHIEVE_1B_POP";
                        if (_historyHashes.Add(alertKey))
                        {
                            DispatchOverlay($"🏆 ¡HITO IMPERIAL LOGRADO! La población de tu imperio ha superado exitosamente los 1.000 Millones de almas ({popM:N1}M).", AlertType.Achievement);
                            return;
                        }
                    }
                }

                // 5. Check Tactical Advice & Recommendations
                if (EnableAdvice && _checkCycleCount % 2 == 0)
                {
                    var researchProjects = dbService.GetActiveResearchProjects(raceId);
                    if (researchProjects.Count > 0)
                    {
                        string alertKey = $"ADVICE_TECH_{researchProjects.Count}";
                        if (_historyHashes.Add(alertKey))
                        {
                            DispatchOverlay($"💡 CONSEJO DE I+D: Mantienes {researchProjects.Count} proyectos activos. Asegúrate de asignar científicos con bonificación coincidente (+25%).", AlertType.Advice);
                            return;
                        }
                    }
                }

                // 6. Periodic Balance Executive Report (Every 3 cycles ~ 4.5 mins)
                if (EnableReports && _checkCycleCount % 3 == 0)
                {
                    double popM = dbService.GetTotalEmpirePopulation(raceId);
                    var fleets = dbService.GetActiveFleets(raceId);
                    int activeFleets = fleets.Count(f => f.ShipCount > 0);

                    DispatchOverlay($"📊 BALANCE IMPERIAL: Población {popM:N1}M almas | {activeFleets} Flotas Navales activas en servicio. Estado del imperio estable.", AlertType.Report);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PerformCheck Error: {ex.Message}");
            }
        }

        private void DispatchOverlay(string message, AlertType type)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    var win = new AITacticalOverlayWindow();
                    win.ShowAlert(message, type, AutoDismissSeconds, EnableSound);
                    OnAlertGenerated?.Invoke(message, type);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DispatchOverlay Error: {ex.Message}");
                }
            });
        }
    }
}
