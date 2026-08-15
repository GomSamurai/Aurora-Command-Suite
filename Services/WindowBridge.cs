using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
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

        [DllImport("user32.dll")]
        private static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        private const int SW_MAXIMIZE = 3;
        private const int SW_RESTORE = 9;

        /// <summary>
        /// Locates the running Aurora 4X game process or window and brings it to the foreground.
        /// Strictly excludes AuroraDesignSuite itself to prevent self-focusing.
        /// </summary>
        public static bool FocusAuroraGame(out string statusMessage)
        {
            try
            {
                IntPtr targetHWnd = IntPtr.Zero;
                uint currentAppPid = (uint)Process.GetCurrentProcess().Id;

                // 1. Enumerate top level windows to find Aurora GAME windows (excluding AuroraDesignSuite)
                EnumWindows((hWnd, lParam) =>
                {
                    if (IsWindowVisible(hWnd))
                    {
                        GetWindowThreadProcessId(hWnd, out uint procId);

                        // Skip our own process!
                        if (procId == currentAppPid) return true;

                        StringBuilder sb = new StringBuilder(256);
                        GetWindowText(hWnd, sb, 256);
                        string title = sb.ToString();

                        // Skip if title belongs to our suite
                        if (title.Contains("AURORA MASTER COMMAND SUITE") || title.Contains("Aurora Design Suite"))
                            return true;

                        // Check process name
                        try
                        {
                            var proc = Process.GetProcessById((int)procId);
                            string pname = proc.ProcessName.ToLower();

                            if (pname == "aurora" || pname == "aurorapatch" || pname == "aurora4x")
                            {
                                targetHWnd = hWnd;
                                return false; // Found game window!
                            }
                        }
                        catch { }

                        // Fallback title check for Aurora game windows
                        if (!string.IsNullOrEmpty(title) &&
                            (title.StartsWith("Aurora v") || title.StartsWith("Aurora 2") || title.Contains("System Map") || title.Contains("Tactical Map") || title.Contains("Commanders")))
                        {
                            targetHWnd = hWnd;
                            return false;
                        }
                    }
                    return true;
                }, IntPtr.Zero);

                if (targetHWnd != IntPtr.Zero)
                {
                    ShowWindow(targetHWnd, SW_MAXIMIZE);
                    BringWindowToTop(targetHWnd);
                    SetForegroundWindow(targetHWnd);
                    SwitchToThisWindow(targetHWnd, true);

                    // Minimize our suite so the game is fully visible
                    if (Application.Current != null && Application.Current.MainWindow != null)
                    {
                        Application.Current.MainWindow.WindowState = WindowState.Minimized;
                    }

                    statusMessage = "🎮 Juego Aurora 4X enfocado en primer plano.";
                    return true;
                }

                // 2. Fallback: check processes excluding our own
                Process[] processes = Process.GetProcessesByName("Aurora");
                if (processes.Length == 0) processes = Process.GetProcessesByName("AuroraPatch");
                if (processes.Length == 0) processes = Process.GetProcessesByName("Aurora4X");

                foreach (var proc in processes)
                {
                    if (proc.Id != currentAppPid && proc.MainWindowHandle != IntPtr.Zero)
                    {
                        IntPtr handle = proc.MainWindowHandle;
                        ShowWindow(handle, SW_MAXIMIZE);
                        BringWindowToTop(handle);
                        SetForegroundWindow(handle);
                        SwitchToThisWindow(handle, true);

                        if (Application.Current != null && Application.Current.MainWindow != null)
                        {
                            Application.Current.MainWindow.WindowState = WindowState.Minimized;
                        }

                        statusMessage = "🎮 Juego Aurora 4X enfocado en primer plano.";
                        return true;
                    }
                }

                // 3. Auto-launch game if not running!
                string gameLauncherPath = @"c:\VSCODE\Aurora271Full\AuroraPatch.exe";
                if (File.Exists(gameLauncherPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = gameLauncherPath,
                        WorkingDirectory = @"c:\VSCODE\Aurora271Full"
                    });
                    statusMessage = "🚀 Iniciando juego Aurora 4X v2.7.1 automáticamente...";
                    return true;
                }

                statusMessage = "⚠️ No se encontró la instalación de Aurora 4X.";
                return false;
            }
            catch (Exception ex)
            {
                statusMessage = $"⚠️ Error al enfocar Aurora 4X: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Locates the running Aurora Master Suite application and brings it to the foreground MAXIMIZED.
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
                        ShowWindow(handle, SW_MAXIMIZE);
                        BringWindowToTop(handle);
                        SetForegroundWindow(handle);
                        SwitchToThisWindow(handle, true);
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
