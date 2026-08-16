using System;
using System.Diagnostics;
using System.Windows;
using AuroraDesignSuite.Services;

namespace AuroraDesignSuite.Views
{
    public partial class ConfigureApiKeyDialog : Window
    {
        public ConfigureApiKeyDialog()
        {
            InitializeComponent();
            TxtApiKey.Text = ApiKeyManager.GetApiKey();
        }

        private void BtnGetFreeKey_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://aistudio.google.com/app/apikey",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo abrir el navegador automáticamente: {ex.Message}\n\nVisita manualmente: https://aistudio.google.com/app/apikey", "Atención", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void BtnTestKey_Click(object sender, RoutedEventArgs e)
        {
            string rawKey = TxtApiKey.Text;
            string key = ApiKeyManager.CleanApiKey(rawKey);

            if (string.IsNullOrWhiteSpace(key))
            {
                MessageBox.Show("Introduce una clave API para realizar la prueba de conexión.", "Clave Vacía", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            BtnTestKey.IsEnabled = false;
            BtnTestKey.Content = "⏳ Probando...";

            try
            {
                using var client = new System.Net.Http.HttpClient();
                string[] models = new[] { "gemini-pro-latest", "gemini-flash-latest", "gemini-3.5-flash", "gemini-3.7-flash", "gemini-1.5-flash" };
                bool success = false;
                string lastErr = "";

                foreach (var model in models)
                {
                    string url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={key}";
                    string payload = "{\"contents\":[{\"parts\":[{\"text\":\"Hola, responde OK.\"}]}]}";
                    using var content = new System.Net.Http.StringContent(payload, System.Text.Encoding.UTF8, "application/json");

                    var resp = await client.PostAsync(url, content);
                    string body = await resp.Content.ReadAsStringAsync();

                    if (resp.IsSuccessStatusCode)
                    {
                        success = true;
                        MessageBox.Show($"✅ ¡CONEXIÓN CON GEMINI API COMPLETADA CON ÉXITO!\n\nModelo enlazado: '{model}'.\nTu clave está 100% activa y funcionando correctamente.", "Prueba Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                    else
                    {
                        lastErr = $"[Modelo {model}] HTTP {(int)resp.StatusCode}: {body}";
                    }
                }

                if (!success)
                {
                    MessageBox.Show($"❌ GOOGLE GEMINI API DEVUELVE ERROR:\n\n{lastErr}\n\n💡 Revisa que la clave provenga de aistudio.google.com/app/apikey y esté habilitada.", "Error en la Prueba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de red al conectar con los servidores de Google: {ex.Message}", "Error de Conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BtnTestKey.IsEnabled = true;
                BtnTestKey.Content = "🧪 Probar Conexión";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string key = ApiKeyManager.CleanApiKey(TxtApiKey.Text);
            if (string.IsNullOrWhiteSpace(key))
            {
                MessageBox.Show("Por favor introduce una clave Gemini API válida o pulsa 'Obtener Clave Gratis'.", "Clave No Válida", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (key.StartsWith("sk-", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("⚠️ La clave introducida comienza por 'sk-...'. Ese es el formato de OpenAI o Anthropic.\n\nPara usar la IA en esta aplicación se requiere una clave gratuita de Google Gemini.\n\nPulsa en el botón '🌐 Obtener Clave Gratis' para generar tu clave Gemini en 10 segundos.", "Formato de API Key No Compatible", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (ApiKeyManager.SaveApiKey(key))
            {
                MessageBox.Show("✅ Clave Gemini API guardada con éxito de forma local en tu equipo.", "API Key Configurada", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Error al guardar la clave en el archivo local de configuración.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
