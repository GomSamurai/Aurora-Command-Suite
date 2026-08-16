using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using HarmonyLib;
using Newtonsoft.Json;

namespace AuroraSpanish
{
    public class AuroraSpanish : AuroraPatch.Patch
    {
        public override string Description
        {
            get { return "Traducción Integral al Español (Aurora 4X)"; }
        }

        public override IEnumerable<string> Dependencies
        {
            get { return new string[] { }; }
        }

        private static Dictionary<string, string> uiDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static bool isInternalSetting = false;

        protected override void Loaded(Harmony harmony)
        {
            LoadDictionary();

            // 1. Patch Control.set_Text for real-time translation and layout handling
            try
            {
                var setTextMethod = typeof(Control).GetMethod("set_Text", BindingFlags.Public | BindingFlags.Instance);
                if (setTextMethod != null)
                {
                    var prefix = new HarmonyMethod(GetType().GetMethod("ControlSetTextPrefix", BindingFlags.NonPublic | BindingFlags.Static));
                    harmony.Patch(setTextMethod, prefix: prefix);
                    Log("Patched Control.set_Text successfully!");
                }
            }
            catch (Exception ex)
            {
                Log("Error patching Control.set_Text: " + ex.Message);
            }

            // 2. Patch Form constructors to make ALL windows resizable (Sizable border)
            try
            {
                var formConstructorPostfix = new HarmonyMethod(GetType().GetMethod("FormConstructorPostfix", BindingFlags.NonPublic | BindingFlags.Static));
                foreach (var type in AuroraAssembly.GetTypes().Where(t => typeof(Form).IsAssignableFrom(t)))
                {
                    foreach (var ctor in type.GetConstructors())
                    {
                        harmony.Patch(ctor, postfix: formConstructorPostfix);
                    }
                }
                Log("Patched Form constructors for resizable window borders!");
            }
            catch (Exception ex)
            {
                Log("Error patching form constructors: " + ex.Message);
            }

            // 3. Patch Graphics.DrawString for direct canvas/map text rendering
            try
            {
                var drawStringMethods = typeof(Graphics).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                                        .Where(m => m.Name == "DrawString");
                var drawPrefix = new HarmonyMethod(GetType().GetMethod("DrawStringPrefix", BindingFlags.NonPublic | BindingFlags.Static));
                foreach (var method in drawStringMethods)
                {
                    harmony.Patch(method, prefix: drawPrefix);
                }
                Log("Patched Graphics.DrawString successfully!");
            }
            catch (Exception ex)
            {
                Log("Error patching Graphics.DrawString: " + ex.Message);
            }

            Log("AuroraSpanish patch initialized completely!");
            StartLiveSyncPipeServer();
        }

        private static bool isPipeServerStarted = false;

        private static void StartLiveSyncPipeServer()
        {
            if (isPipeServerStarted) return;
            isPipeServerStarted = true;

            System.Threading.Thread thread = new System.Threading.Thread(delegate()
            {
                while (true)
                {
                    try
                    {
                        using (var server = new System.IO.Pipes.NamedPipeServerStream("AuroraCommandSuiteSyncPipe", System.IO.Pipes.PipeDirection.In))
                        {
                            server.WaitForConnection();
                            using (var reader = new System.IO.StreamReader(server))
                            {
                                string msg = reader.ReadLine();
                                if (!string.IsNullOrEmpty(msg))
                                {
                                    TriggerInGameRefresh();
                                }
                            }
                        }
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        public static void TriggerInGameRefresh()
        {
            try
            {
                if (Application.OpenForms.Count == 0) return;
                Form mainForm = Application.OpenForms[0];
                if (mainForm.InvokeRequired)
                {
                    mainForm.BeginInvoke(new Action(TriggerInGameRefresh));
                    return;
                }

                foreach (Form f in Application.OpenForms)
                {
                    if (f != null && !f.IsDisposed)
                    {
                        f.Invalidate(true);
                        f.Refresh();
                    }
                }
            }
            catch { }
        }

        private static void LoadDictionary()
        {
            try
            {
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "localization", "ui_strings_es.json");
                if (!File.Exists(jsonPath))
                {
                    jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Patches", "AuroraSpanish", "localization", "ui_strings_es.json");
                }

                if (File.Exists(jsonPath))
                {
                    string content = File.ReadAllText(jsonPath);
                    uiDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(content) ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    Log(string.Format("Loaded {0} UI translation strings from {1}", uiDictionary.Count, jsonPath));
                }
            }
            catch (Exception ex)
            {
                Log("Error loading UI dictionary: " + ex.Message);
            }
        }

        private static void ControlSetTextPrefix(Control __instance, ref string value)
        {
            if (isInternalSetting || string.IsNullOrEmpty(value)) return;

            try
            {
                isInternalSetting = true;
                value = TranslateString(value);

                // Enable AutoSize on CheckBoxes and RadioButtons to prevent text truncation
                if (__instance is CheckBox || __instance is RadioButton)
                {
                    __instance.AutoSize = true;
                }
            }
            catch { }
            finally
            {
                isInternalSetting = false;
            }
        }

        private static void DrawStringPrefix(ref string s)
        {
            if (string.IsNullOrEmpty(s)) return;

            try
            {
                s = TranslateString(s);
            }
            catch { }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        private static void FormConstructorPostfix(Form __instance)
        {
            // Make ALL forms in Aurora 4X resizable from window borders like standard Windows apps!
            try
            {
                __instance.FormBorderStyle = FormBorderStyle.Sizable;
                __instance.MaximizeBox = true;
            }
            catch { }

            // Inject Master Suite Button ONLY on the main map window
            try
            {
                __instance.Shown += delegate
                {
                    try
                    {
                        if (__instance.Controls.Find("btnMasterSuiteNav", true).Length > 0) return;

                        string formTypeName = __instance.GetType().Name.ToLower();
                        string formTitle = __instance.Text ?? "";

                        // Explicitly exclude all secondary dialogs and sub-windows
                        if (formTitle.StartsWith("Economía", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Economy", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Investigación", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Research", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Astilleros", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Shipyards", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Diseño", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Class", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Comandantes", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Commanders", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Flotas", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Fleets", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Eventos", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Events", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Sistemas", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Systems", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Industria", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Industry", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Minería", StringComparison.OrdinalIgnoreCase) ||
                            formTitle.StartsWith("Mining", StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }

                        // Skip modal dialogs or child controls
                        if (__instance.Modal || __instance.Parent != null) return;

                        // Identify the Main Tactical System Map Window
                        bool isMainMap = false;

                        if (formTitle.Contains("Racial") || formTitle.Contains("Riqueza") || 
                            formTitle.Contains("Wealth") || formTitle.Contains("Sol") || 
                            formTitle.Contains("System Map") || formTitle.Contains("Tactical") ||
                            formTitle.Contains("Imperio"))
                        {
                            isMainMap = true;
                        }
                        else if (formTypeName == "f5" || formTypeName == "hf" || formTypeName == "gu" || formTypeName == "bd")
                        {
                            isMainMap = true;
                        }
                        else if (System.Windows.Forms.Application.OpenForms.Count > 0 && __instance == System.Windows.Forms.Application.OpenForms[0])
                        {
                            isMainMap = true;
                        }

                        if (!isMainMap) return;

                        Button btnSuite = new Button
                        {
                            Name = "btnMasterSuiteNav",
                            Text = "🚀 AURORA COMMAND SUITE",
                            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                            BackColor = Color.FromArgb(13, 26, 38),
                            ForeColor = Color.FromArgb(0, 240, 255),
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(215, 26),
                            Location = new Point(Math.Max(10, __instance.ClientSize.Width - 225), 4),
                            Anchor = AnchorStyles.Top | AnchorStyles.Right,
                            Cursor = Cursors.Hand
                        };
                        btnSuite.FlatAppearance.BorderColor = Color.FromArgb(0, 240, 255);
                        btnSuite.Click += delegate
                        {
                            try
                            {
                                var suiteProc = System.Diagnostics.Process.GetProcessesByName("AuroraDesignSuite");
                                if (suiteProc.Length > 0)
                                {
                                    IntPtr handle = suiteProc[0].MainWindowHandle;
                                    if (handle != IntPtr.Zero)
                                    {
                                        ShowWindow(handle, 3); // 3 = SW_MAXIMIZE
                                        BringWindowToTop(handle);
                                        SetForegroundWindow(handle);
                                        SwitchToThisWindow(handle, true);
                                        return;
                                    }
                                }

                                string suiteExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AuroraDesignSuite.exe");
                                if (File.Exists(suiteExe))
                                {
                                    System.Diagnostics.Process.Start(suiteExe);
                                }
                            }
                            catch { }
                        };

                        __instance.Controls.Add(btnSuite);
                        btnSuite.BringToFront();

                        if (__instance.Controls.Find("btnSuiteRefreshNav", true).Length == 0)
                        {
                            Button btnRefreshSuite = new Button
                            {
                                Name = "btnSuiteRefreshNav",
                                Text = "🔄 REFRESCAR SUITE",
                                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                                BackColor = Color.FromArgb(13, 26, 38),
                                ForeColor = Color.FromArgb(255, 184, 108),
                                FlatStyle = FlatStyle.Flat,
                                Size = new Size(160, 26),
                                Location = new Point(Math.Max(10, __instance.ClientSize.Width - 392), 4),
                                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                                Cursor = Cursors.Hand
                            };
                            btnRefreshSuite.FlatAppearance.BorderColor = Color.FromArgb(255, 184, 108);
                            btnRefreshSuite.Click += delegate
                            {
                                TriggerInGameRefresh();
                                MessageBox.Show("🔄 Interfaz del juego refrescada con los datos de Aurora Command Suite.", "Sincronización Completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            };

                            __instance.Controls.Add(btnRefreshSuite);
                            btnRefreshSuite.BringToFront();
                        }
                    }
                    catch { }
                };
            }
            catch { }

            __instance.HandleCreated += delegate(object sender, EventArgs e)
            {
                Control control = sender as Control;
                if (control != null)
                {
                    TranslateControlRecursive(control);
                }
            };
        }

        private static void TranslateControlRecursive(Control control)
        {
            if (control == null) return;

            if (!string.IsNullOrEmpty(control.Text))
            {
                control.Text = TranslateString(control.Text);
            }

            if (control is CheckBox || control is RadioButton)
            {
                control.AutoSize = true;
            }

            if (control is TabControl)
            {
                TabControl tabControl = (TabControl)control;
                foreach (TabPage tab in tabControl.TabPages)
                {
                    if (!string.IsNullOrEmpty(tab.Text))
                    {
                        tab.Text = TranslateString(tab.Text);
                    }
                }
            }
            else if (control is ListView)
            {
                ListView listView = (ListView)control;
                foreach (ColumnHeader header in listView.Columns)
                {
                    if (!string.IsNullOrEmpty(header.Text))
                    {
                        header.Text = TranslateString(header.Text);
                    }
                }
            }

            foreach (Control child in control.Controls)
            {
                TranslateControlRecursive(child);
            }
        }

        public static string TranslateString(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            string trimmed = text.Trim();

            // 1. Direct match in dictionary
            string translated;
            if (uiDictionary.TryGetValue(trimmed, out translated))
            {
                return text.Replace(trimmed, translated);
            }

            // 2. Pattern matching and sub-string translation for compound UI labels
            string result = text;
            foreach (var kvp in uiDictionary)
            {
                if (kvp.Key.Length > 3 && result.Contains(kvp.Key))
                {
                    result = result.Replace(kvp.Key, kvp.Value);
                }
            }

            return result;
        }

        private static void Log(string message)
        {
            try
            {
                Console.WriteLine(string.Format("[AuroraSpanish] {0}", message));
            }
            catch { }
        }
    }
}
