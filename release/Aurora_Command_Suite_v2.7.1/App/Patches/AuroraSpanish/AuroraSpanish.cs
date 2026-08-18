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

                        try
                        {
                            var methods = f.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                            foreach (var m in methods)
                            {
                                if (m.GetParameters().Length == 0 && 
                                   (m.Name.StartsWith("Populate", StringComparison.OrdinalIgnoreCase) ||
                                    m.Name.StartsWith("Refresh", StringComparison.OrdinalIgnoreCase) ||
                                    m.Name.StartsWith("Display", StringComparison.OrdinalIgnoreCase) ||
                                    m.Name.StartsWith("Load", StringComparison.OrdinalIgnoreCase) ||
                                    m.Name.Equals("Requery", StringComparison.OrdinalIgnoreCase)))
                                {
                                    m.Invoke(f, null);
                                }
                            }
                        }
                        catch { }
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

        private static bool ShouldInjectButtons(Form form)
        {
            if (form == null || form.IsDisposed) return false;
            if (form.Modal || form.Parent != null) return false;

            string typeName = form.GetType().Name;
            string title = (form.Text ?? "").Trim();

            // 1. Primary check: If form class name is Form1 (Aurora 4X main window class)
            if (typeName.Equals("Form1", StringComparison.OrdinalIgnoreCase) || typeName.Equals("MainForm", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // 2. Main Star Map Window title contains "Racial Riqueza" or "Racial Wealth" or "System Map"
            if (title.IndexOf("Racial Riqueza", StringComparison.OrdinalIgnoreCase) >= 0 ||
                title.IndexOf("Racial Wealth", StringComparison.OrdinalIgnoreCase) >= 0 ||
                title.IndexOf("System Map", StringComparison.OrdinalIgnoreCase) >= 0 ||
                title.Equals("Aurora", StringComparison.OrdinalIgnoreCase) ||
                title.StartsWith("Aurora 4X", StringComparison.OrdinalIgnoreCase))
            {
                if (!title.StartsWith("Create", StringComparison.OrdinalIgnoreCase) &&
                    !title.StartsWith("Crear", StringComparison.OrdinalIgnoreCase) &&
                    !title.StartsWith("Select", StringComparison.OrdinalIgnoreCase) &&
                    !title.StartsWith("Edit", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            // 3. Fallback: Check if Application.OpenForms[0] is this form AND title does NOT contain subwindow keywords
            try
            {
                if (Application.OpenForms != null && Application.OpenForms.Count > 0 && Application.OpenForms[0] == form)
                {
                    string[] excludeSubKeywords = new string[]
                    {
                        "Create", "Crear", "Select", "Seleccionar", "Edit", "Editar", "New", "Nuevo",
                        "Project", "Proyecto", "Naval", "Organización", "Organization", "Fuerzas", "Ground",
                        "Economía", "Economy", "Investigación", "Research", "Astilleros", "Shipyard",
                        "Diseño", "Design", "Comandantes", "Commanders", "Flotas", "Fleets", "Eventos", "Events",
                        "Cuerpos", "System Bodies", "Industria", "Industry", "Misil", "Missile", "Torreta", "Turret"
                    };

                    foreach (string kw in excludeSubKeywords)
                    {
                        if (title.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0) return false;
                    }

                    return true;
                }
            }
            catch { }

            return false;
        }

        private static void FormConstructorPostfix(Form __instance)
        {
            // Make ALL forms in Aurora 4X resizable from window borders like standard Windows apps!
            try
            {
                __instance.FormBorderStyle = FormBorderStyle.Sizable;
                __instance.MaximizeBox = true;
            }
            catch { }

            // Inject Master Suite Buttons ONLY on the main system map window
            try
            {
                __instance.Shown += delegate
                {
                    try
                    {
                        if (__instance.Controls.Find("btnMasterSuiteNav", true).Length > 0) return;

                        // Inject buttons on main map window and exclude known sub-windows
                        if (!ShouldInjectButtons(__instance)) return;

                        Button btnSuite = new Button
                        {
                            Name = "btnMasterSuiteNav",
                            Text = "🚀",
                            Font = new Font("Segoe UI Emoji", 10.0f, FontStyle.Bold),
                            BackColor = Color.FromArgb(13, 26, 38),
                            ForeColor = Color.FromArgb(0, 240, 255),
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(28, 26),
                            Location = new Point(Math.Max(10, __instance.ClientSize.Width - 34), 4),
                            Anchor = AnchorStyles.Top | AnchorStyles.Right,
                            Cursor = Cursors.Hand
                        };
                        btnSuite.FlatAppearance.BorderColor = Color.FromArgb(0, 240, 255);
                        ToolTip tipSuite = new ToolTip();
                        tipSuite.SetToolTip(btnSuite, "Abrir / Enfocar Aurora Command Suite");

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
                                Text = "🔄",
                                Font = new Font("Segoe UI Emoji", 10.0f, FontStyle.Bold),
                                BackColor = Color.FromArgb(13, 26, 38),
                                ForeColor = Color.FromArgb(255, 184, 108),
                                FlatStyle = FlatStyle.Flat,
                                Size = new Size(28, 26),
                                Location = new Point(Math.Max(10, __instance.ClientSize.Width - 66), 4),
                                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                                Cursor = Cursors.Hand
                            };
                            btnRefreshSuite.FlatAppearance.BorderColor = Color.FromArgb(255, 184, 108);
                            ToolTip tipRefresh = new ToolTip();
                            tipRefresh.SetToolTip(btnRefreshSuite, "Refrescar Sincronización con Aurora Command Suite");

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
