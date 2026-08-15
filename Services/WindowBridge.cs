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
        /// </summary>
        public static bool FocusAuroraGame(out string statusMessage)
        {
            try
            {
                IntPtr targetHWnd = IntPtr.Zero;

                // 1. Enumerate top level windows to find Aurora game windows
                EnumWindows((hWnd, lParam) =>
                {
                    if (IsWindowVisible(hWnd))
                    {
                        StringBuilder sb = new StringBuilder(256);
                        GetWindowText(hWnd, sb, 256);
                        string title = sb.ToString();

                        if (!string.IsNullOrEmpty(title) && 
                            (title.StartsWith("Aurora") || title.Contains("System Map") || title.Contains("Tactical Map") || title.Contains("Commanders")))
                        {
                            targetHWnd = hWnd;
                            return false; // Stop enumeration
                        }
                    }
                    return true;
                }, IntPtr.Zero);

                if (targetHWnd != IntPtr.Zero)
                {
                    ShowWindow(targetHWnd, SW_RESTORE);
                    BringWindowToTop(targetHWnd);
                    SetForegroundWindow(targetHWnd);
                    SwitchToThisWindow(targetHWnd, true);
                    statusMessage = "🎮 Juego Aurora 4X enfocado en primer plano.";
                    return true;
                }

                // 2. Fallback: check processes
                Process[] processes = Process.GetProcessesByName("Aurora");
                if (processes.Length == 0) processes = Process.GetProcessesByName("AuroraPatch");
                if (processes.Length == 0) processes = Process.GetProcessesByName("Aurora4X");

                foreach (var proc in processes)
                {
                    if (proc.MainWindowHandle != IntPtr.Zero)
                    {
                        IntPtr handle = proc.MainWindowHandle;
                        ShowWindow(handle, SW_RESTORE);
                        BringWindowToTop(handle);
                        SetForegroundWindow(handle);
                        SwitchToThisWindow(handle, true);
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

                statusMessage = "⚠️ No se encontró el ejecutable de Aurora 4X.";
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
