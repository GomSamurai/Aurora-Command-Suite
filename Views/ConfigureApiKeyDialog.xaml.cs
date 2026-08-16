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

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string key = TxtApiKey.Text.Trim();
            if (string.IsNullOrWhiteSpace(key))
            {
                MessageBox.Show("Por favor introduce una clave Gemini API válida o pulsa 'Obtener Clave Gratis'.", "Clave No Válida", MessageBoxButton.OK, MessageBoxImage.Warning);
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
