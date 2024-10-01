using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Threading;

namespace TicTacToe
{
    public partial class GameWindow : Window
    {
        private string currentPlayer;
        private string playerSymbol;
        private string computerSymbol;
        private Button[,] buttons = new Button[3, 3];
        private List<Button> winningButtons = new List<Button>();
        private int clicks;
        private DispatcherTimer timer;
        private int playerWins = 0;
        private int computerWins = 0;

        public GameWindow(string firstPlayer, string playerSymbol)
        {
            InitializeComponent();
            this.playerSymbol = playerSymbol;
            this.computerSymbol = playerSymbol == "X" ? "O" : "X";

            InitializeButtons();

            currentPlayer = firstPlayer == "Player" ? playerSymbol : computerSymbol;
            if (currentPlayer == computerSymbol)
            {
                ComputerMove();
            }

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.1)
            };
            timer.Tick += Timer_Tick;

            this.Closed += GameWindow_Closed;
        }


        private void GameWindow_Closed(object sender, EventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }


        private void InitializeButtons()
        {
            SolidColorBrush background = new SolidColorBrush(Color.FromArgb(0, 0, 168, 198)); // Transparent background/prozirna pozadina
            SolidColorBrush foreground = new SolidColorBrush(Color.FromRgb(30, 223, 170)); // Color for symbols/boja slova

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var button = new Button
                    {
                        FontSize = 36,
                        Background = background,
                        Foreground = foreground,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1)
                    };

                    button.Style = new Style(typeof(Button))
                    {
                        Setters =
                        {
                            new Setter(Button.BackgroundProperty, background),
                            new Setter(Button.ForegroundProperty, foreground)
                        },
                        Triggers =
                        {
                            new Trigger
                            {
                                Property = Button.IsMouseOverProperty,
                                Value = true,
                                Setters =
                                {
                                    new Setter(Button.BackgroundProperty, new SolidColorBrush(Colors.LightCyan)),
                                    new Setter(Button.EffectProperty, new DropShadowEffect
                                    {
                                        Color = Colors.Cyan,
                                        BlurRadius = 15,
                                        ShadowDepth = 0,
                                        Opacity = 0.2
                                    })
                                }
                            }
                        }
                    };

                    button.Click += Button_Click;
                    buttons[i, j] = button;
                    GameGrid.Children.Add(button);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null || !string.IsNullOrEmpty(button.Content?.ToString()))
            {
                return;
            }

            button.Content = playerSymbol;
            clicks++;

            if (CheckWinner(playerSymbol))
            {
                ShowResult($"{playerSymbol} wins!");
                return;
            }

            if (clicks == 9)
            {
                ShowResult("A tie!");
                return;
            }

            currentPlayer = computerSymbol;

            if (currentPlayer == computerSymbol)
            {
                ComputerMove();
                if (CheckWinner(computerSymbol))
                {
                    ShowResult($"{computerSymbol} wins!");
                }
                else if (clicks == 9)
                {
                    ShowResult("It's a tie!");
                }
                else
                {
                    currentPlayer = playerSymbol;
                }
            }
        }

        private void ComputerMove()
        {
            Random random = new Random();
            while (true)
            {
                int row = random.Next(3);
                int col = random.Next(3);
                Button button = buttons[row, col];
                if (button != null && string.IsNullOrEmpty(button.Content?.ToString()))
                {
                    button.Content = computerSymbol;
                    clicks++;
                    break;
                }
            }
        }

        private bool CheckWinner(string symbol)
        {
            for (int i = 0; i < 3; i++)
            {
                if (IsButtonContentEqual(buttons[i, 0], symbol) &&
                    IsButtonContentEqual(buttons[i, 1], symbol) &&
                    IsButtonContentEqual(buttons[i, 2], symbol))
                {
                    CreateListOfWinningButtons(buttons[i, 0], buttons[i, 1], buttons[i, 2]);
                    return true;
                }

                if (IsButtonContentEqual(buttons[0, i], symbol) &&
                    IsButtonContentEqual(buttons[1, i], symbol) &&
                    IsButtonContentEqual(buttons[2, i], symbol))
                {
                    CreateListOfWinningButtons(buttons[0, i], buttons[1, i], buttons[2, i]);
                    return true;
                }
            }

            if (IsButtonContentEqual(buttons[0, 0], symbol) &&
                IsButtonContentEqual(buttons[1, 1], symbol) &&
                IsButtonContentEqual(buttons[2, 2], symbol))
            {
                CreateListOfWinningButtons(buttons[0, 0], buttons[1, 1], buttons[2, 2]);
                return true;
            }

            if (IsButtonContentEqual(buttons[0, 2], symbol) &&
                IsButtonContentEqual(buttons[1, 1], symbol) &&
                IsButtonContentEqual(buttons[2, 0], symbol))
            {
                CreateListOfWinningButtons(buttons[0, 2], buttons[1, 1], buttons[2, 0]);
                return true;
            }

            return false;
        }

        private bool IsButtonContentEqual(Button button, string symbol)
        {
            return button.Content != null && button.Content.ToString() == symbol;
        }

        private void CreateListOfWinningButtons(Button one, Button two, Button three)
        {
            winningButtons.Add(one);
            winningButtons.Add(two);
            winningButtons.Add(three);
        }

        private void ShowResult(string message)
        {
            var glowEffect = new DropShadowEffect
            {
                Color = Colors.Cyan,
                BlurRadius = 20,
                ShadowDepth = 0,
                Opacity = 1.0
            };
            foreach (var button in winningButtons)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(75, 0, 130)); // Dark purple/tamno ljubicasta
                button.Effect = glowEffect;
            }

            if (message.Contains(playerSymbol))
            {
                playerWins++;
                PlayerWinsLabel.Content = playerWins.ToString();
            }
            else if (message.Contains(computerSymbol))
            {
                computerWins++;
                ComputerWinsLabel.Content = computerWins.ToString();
            }

            MessageBox.Show(message);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
            StartNewGame();
        }

        private void StartNewGame()
        {
            winningButtons.Clear();
            clicks = 0;

            foreach (var button in buttons)
            {
                button.Content = string.Empty;
                button.Background = new SolidColorBrush(Color.FromArgb(0, 0, 168, 198));
                button.Effect = null;
            }

            currentPlayer = playerSymbol;
            if (currentPlayer == computerSymbol)
            {
                ComputerMove();
            }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }
    }
}
