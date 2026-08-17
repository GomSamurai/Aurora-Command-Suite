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

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;

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
        /// Sends a native time step shortcut key directly to the Aurora 4X game process window.
        /// Falls back to SQLite database update if the game is not running.
        /// </summary>
        public static bool SendTimeStepToGame(double seconds, DatabaseService dbService, int raceId, out string statusMessage)
        {
            if (GetAuroraGameHandle(out IntPtr hWnd))
            {
                IntPtr vk = seconds switch
                {
                    5 => (IntPtr)0x70,        // F1 (+5 Seg)
                    30 => (IntPtr)0x71,       // F2 (+30 Seg)
                    300 => (IntPtr)0x72,      // F3 (+5 Min)
                    1200 => (IntPtr)0x73,     // F4 (+20 Min)
                    3600 => (IntPtr)0x74,     // F5 (+1 Hora)
                    10800 => (IntPtr)0x75,    // F6 (+3 Horas)
                    28800 => (IntPtr)0x76,    // F7 (+8 Horas)
                    86400 => (IntPtr)0x77,    // F8 (+1 Día)
                    432000 => (IntPtr)0x78,   // F9 (+5 Días)
                    2592000 => (IntPtr)0x79,  // F10 (+30 Días)
                    31536000 => (IntPtr)0x7A, // F11 (+1 Año)
                    _ => (IntPtr)0x77
                };

                PostMessage(hWnd, WM_KEYDOWN, vk, IntPtr.Zero);
                System.Threading.Thread.Sleep(30);
                PostMessage(hWnd, WM_KEYUP, vk, IntPtr.Zero);

                statusMessage = "⚡ Pulsación enviada nativamente a Aurora 4X en tiempo real.";
                return true;
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
