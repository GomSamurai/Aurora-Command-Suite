using System;
using System.IO;
using System.Windows;

namespace AuroraDesignSuite
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                LogException(args.ExceptionObject as Exception);
            };

            DispatcherUnhandledException += (s, args) =>
            {
                LogException(args.Exception);
                args.Handled = true;
            };
        }

        private void LogException(Exception? ex)
        {
            if (ex == null) return;

            string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AuroraDesignSuite.log");
            string msg = $"[{DateTime.Now}] UNHANDLED EXCEPTION:\n{ex}\n\n";

            try
            {
                File.AppendAllText(logFile, msg);
            }
            catch { }

            MessageBox.Show($"Ha ocurrido un error en Aurora Design Suite:\n\n{ex.Message}\n\nDetalles guardados en:\n{logFile}",
                            "Error de Ejecución", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
