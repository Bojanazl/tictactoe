using System;
using System.Windows;
using System.Windows.Controls;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FirstPlayerComboBox.Text) || string.IsNullOrEmpty(SymbolComboBox.Text))
            {
                MessageBox.Show("Fill in both boxes!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }

            this.Hide();

            string firstPlayer = FirstPlayerComboBox.Text;
            string playerSymbol = SymbolComboBox.Text;

            GameWindow gameWindow = new GameWindow(firstPlayer, playerSymbol);
            gameWindow.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}
