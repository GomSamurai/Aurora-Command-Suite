using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace AuroraDesignSuite.Services
{
    public static class WindowBridge
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

        private const int SW_RESTORE = 9;

        /// <summary>
        /// Locates the running Aurora 4X game process and brings its window to the foreground.
        /// </summary>
        public static bool FocusAuroraGame(out string statusMessage)
        {
            try
            {
                // Try finding process by name
                Process[] processes = Process.GetProcessesByName("Aurora");
                if (processes.Length == 0)
                {
                    // Fallback to checking executable path or window title
                    processes = Process.GetProcessesByName("Aurora4X");
                }

                if (processes.Length > 0)
                {
                    IntPtr handle = processes[0].MainWindowHandle;
                    if (handle == IntPtr.Zero)
                    {
                        handle = FindWindow(null, "Aurora");
                    }

                    if (handle != IntPtr.Zero)
                    {
                        ShowWindow(handle, SW_RESTORE);
                        BringWindowToTop(handle);
                        SetForegroundWindow(handle);
                        statusMessage = "🎮 Enfocado juego Aurora 4X en primer plano.";
                        return true;
                    }
                }

                // Fallback: try finding window by title "Aurora"
                IntPtr windowHandle = FindWindow(null, "Aurora");
                if (windowHandle != IntPtr.Zero)
                {
                    ShowWindow(windowHandle, SW_RESTORE);
                    BringWindowToTop(windowHandle);
                    SetForegroundWindow(windowHandle);
                    statusMessage = "🎮 Enfocado juego Aurora 4X (vía FindWindow).";
                    return true;
                }

                statusMessage = "⚠️ No se encontró la ventana de Aurora 4X en ejecución. Inicia el juego desde abrir_aurora_design_suite.bat.";
                return false;
            }
            catch (Exception ex)
            {
                statusMessage = $"⚠️ Error al enfocar Aurora 4X: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Locates the running Aurora Master Suite application and brings it to the foreground.
        /// </summary>
        public static bool FocusMasterSuite()
        {
            try
            {
                Process[] processes = Process.GetProcessesByName("AuroraDesignSuite");
                if (processes.Length > 0)
                {
                    IntPtr handle = processes[0].MainWindowHandle;
                    if (handle != IntPtr.Zero)
                    {
                        ShowWindow(handle, SW_RESTORE);
                        BringWindowToTop(handle);
                        SetForegroundWindow(handle);
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
