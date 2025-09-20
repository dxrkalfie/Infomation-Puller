using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace PullInfomation
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BrowseUnity_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Unity Asset Files (*.unity3d)|*.unity3d|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                UnityFileTextBox.Text = openFileDialog.FileName;

                try
                {
                    string fileContent = File.ReadAllText(openFileDialog.FileName);

                    // Extract PlayFab
                    PlayFabOutput.Text = ExtractPlayFabSettings(fileContent);

                    // Extract Photon
                    PhotonOutput.Text = ExtractPhotonSettings(fileContent);
                }
                catch (Exception ex)
                {
                    PlayFabOutput.Text = $"Error reading file: {ex.Message}";
                    PhotonOutput.Text = $"Error reading file: {ex.Message}";
                }
            }
        }

        private string ExtractPlayFabSettings(string content)
        {
            int index = content.IndexOf("PlayFabSharedSettings", StringComparison.OrdinalIgnoreCase);
            if (index == -1)
                index = content.IndexOf("FabShared", StringComparison.OrdinalIgnoreCase);

            if (index == -1)
                return "PlayFabSharedSettings or FabShared not found.";

            int start = index;
            int length = Math.Min(500, content.Length - start);
            string rawBlock = content.Substring(start, length);

            var cleaned = new string(rawBlock
                .Where(c => !char.IsControl(c) || c == '\r' || c == '\n')
                .ToArray());

            return cleaned;
        }

        private string ExtractPhotonSettings(string content)
        {
            int index = content.IndexOf("PhotonServerSettings", StringComparison.OrdinalIgnoreCase);
            if (index == -1)
                return "PhotonServerSettings not found.";

            string after = content.Substring(index);
            string cleaned = new string(after
                .Where(c => !char.IsControl(c) || c == '\r' || c == '\n')
                .ToArray());

            string[] words = cleaned.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(" ", words.Take(545));
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedContent is UIElement content)
            {
                var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300));
                content.BeginAnimation(UIElement.OpacityProperty, fade);
            }
        }

        private void TopBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
