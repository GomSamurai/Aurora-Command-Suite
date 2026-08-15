using System.Windows;

namespace AuroraDesignSuite.Views
{
    public partial class ConfirmActionDialog : Window
    {
        public ConfirmActionDialog(string title, string description, string costInfo, string durationInfo)
        {
            InitializeComponent();
            TxtTitle.Text = title;
            TxtDescription.Text = description;
            TxtCost.Text = costInfo;
            TxtDuration.Text = durationInfo;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
