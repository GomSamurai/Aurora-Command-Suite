using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string currentDir = Directory.GetCurrentDirectory();
                string[] candidates = new[]
                {
                    Path.GetFullPath(Path.Combine(baseDir, "..", "AuroraPatch.exe")),
                    Path.GetFullPath(Path.Combine(baseDir, "..", "Aurora.exe")),
                    Path.GetFullPath(Path.Combine(currentDir, "..", "AuroraPatch.exe")),
                    Path.GetFullPath(Path.Combine(currentDir, "..", "Aurora.exe")),
                    Path.GetFullPath(Path.Combine(baseDir, "Aurora.exe"))
                };

                string? gameLauncherPath = candidates.FirstOrDefault(f => !string.IsNullOrEmpty(f) && File.Exists(f));
                if (!string.IsNullOrEmpty(gameLauncherPath))
                {
                    string targetDir = Path.GetDirectoryName(gameLauncherPath) ?? "";
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = gameLauncherPath,
                        WorkingDirectory = targetDir
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

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        private const uint BM_CLICK = 0x00F5;
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;

        /// <summary>
        /// Attempts to locate a child button control inside Aurora 4X matching the time step text and clicks it natively via BM_CLICK.
        /// </summary>
        public static bool TriggerChildButton(IntPtr parentHWnd, string[] buttonTextMatches)
        {
            IntPtr targetButtonHWnd = IntPtr.Zero;

            EnumChildWindows(parentHWnd, (hWnd, lParam) =>
            {
                StringBuilder sb = new StringBuilder(256);
                GetWindowText(hWnd, sb, 256);
                string text = sb.ToString().Trim();

                if (!string.IsNullOrEmpty(text))
                {
                    foreach (var match in buttonTextMatches)
                    {
                        if (text.Equals(match, StringComparison.OrdinalIgnoreCase) || 
                            text.Contains(match, StringComparison.OrdinalIgnoreCase))
                        {
                            targetButtonHWnd = hWnd;
                            return false; // Found matching control!
                        }
                    }
                }
                return true;
            }, IntPtr.Zero);

            if (targetButtonHWnd != IntPtr.Zero)
            {
                SendMessage(targetButtonHWnd, BM_CLICK, IntPtr.Zero, IntPtr.Zero);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Retrieves the window handle of the active Aurora 4X game window.
        /// </summary>
        public static bool GetAuroraGameHandle(out IntPtr targetHWnd)
        {
            targetHWnd = IntPtr.Zero;
            uint currentAppPid = (uint)Process.GetCurrentProcess().Id;
            IntPtr foundHandle = IntPtr.Zero;

            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    GetWindowThreadProcessId(hWnd, out uint procId);
                    if (procId == currentAppPid) return true;

                    StringBuilder sb = new StringBuilder(256);
                    GetWindowText(hWnd, sb, 256);
                    string title = sb.ToString();

                    if (title.Contains("AURORA MASTER COMMAND SUITE") || title.Contains("Aurora Design Suite"))
                        return true;

                    try
                    {
                        var proc = Process.GetProcessById((int)procId);
                        string pname = proc.ProcessName.ToLower();
                        if (pname == "aurora" || pname == "aurorapatch" || pname == "aurora4x")
                        {
                            foundHandle = hWnd;
                            return false;
                        }
                    }
                    catch { }

                    if (!string.IsNullOrEmpty(title) &&
                        (title.StartsWith("Aurora v") || title.StartsWith("Aurora 2") || title.Contains("System Map") || title.Contains("Tactical Map")))
                    {
                        foundHandle = hWnd;
                        return false;
                    }
                }
                return true;
            }, IntPtr.Zero);

            if (foundHandle != IntPtr.Zero)
            {
                targetHWnd = foundHandle;
                return true;
            }

            Process[] processes = Process.GetProcessesByName("Aurora");
            if (processes.Length == 0) processes = Process.GetProcessesByName("AuroraPatch");
            if (processes.Length == 0) processes = Process.GetProcessesByName("Aurora4X");

            foreach (var proc in processes)
            {
                if (proc.Id != currentAppPid && proc.MainWindowHandle != IntPtr.Zero)
                {
                    targetHWnd = proc.MainWindowHandle;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Sends a native time step button click directly to the Aurora 4X game process window using BM_CLICK.
        /// Does NOT send function keys (F1-F12) to avoid opening game windows like Economy (F2).
        /// Falls back to SQLite database update if the game is not running.
        /// </summary>
        public static bool SendTimeStepToGame(double seconds, DatabaseService dbService, int raceId, out string statusMessage)
        {
            if (GetAuroraGameHandle(out IntPtr hWnd))
            {
                string[] matches = seconds switch
                {
                    5 => new[] { "5 Sec", "5Sec", "5 s", "5s" },
                    30 => new[] { "30 Sec", "30Sec", "30 s", "30s" },
                    300 => new[] { "5 Min", "5Min", "5 m", "5m" },
                    1200 => new[] { "20 Min", "20Min", "20 m", "20m" },
                    3600 => new[] { "1 Hour", "1 Hr", "1Hour", "1Hr", "1 h", "1h" },
                    10800 => new[] { "3 Hours", "3 Hr", "3Hours", "3Hr", "3 h", "3h" },
                    28800 => new[] { "8 Hours", "8 Hr", "8Hours", "8Hr", "8 h", "8h" },
                    86400 => new[] { "1 Day", "1Day", "1 d", "1d" },
                    432000 => new[] { "5 Days", "5Days", "5 d", "5d" },
                    2592000 => new[] { "30 Days", "30Days", "30 d", "30d" },
                    31536000 => new[] { "1 Year", "1 Yr", "1Year", "1Yr", "1 y", "1y" },
                    _ => new[] { "1 Day", "1Day", "1 d", "1d" }
                };

                // Try clicking the native WinForms button directly!
                if (TriggerChildButton(hWnd, matches))
                {
                    statusMessage = "⚡ Clic enviado nativamente al botón de Aurora 4X sin interferir con atajos de teclado.";
                    return true;
                }

                // If button control handle not found, fall back to SQLite time step to avoid firing unwanted F-keys!
                bool success = dbService.AdvanceGameTimeSeconds(raceId, seconds, out _, out _);
                if (success)
                {
                    statusMessage = "⚡ Tiempo avanzado directamente en el registro de partida.";
                    return true;
                }
            }

            // Fallback: Standalone DB advancement
            bool dbSuccess = dbService.AdvanceGameTimeSeconds(raceId, seconds, out _, out _);
            if (dbSuccess)
            {
                statusMessage = "⚙️ Aurora 4X no detectado. Tiempo actualizado en SQLite en modo simulación independiente.";
                return true;
            }

            statusMessage = "⚠️ No se pudo avanzar el tiempo.";
            return false;
        }
    }
}
