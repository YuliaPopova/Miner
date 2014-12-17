using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace курс
{
    public partial class MainWindow : Window
    {

        private Button[,] btns; //массив кнопок
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += timer_Tick;
        }
        int N, M, NumBombs;

        void timer_Tick(object sender, EventArgs e)
        {
            int tim;
            int.TryParse(lblTimer.Content.ToString(), out tim);
            tim++;
            lblTimer.Content = tim.ToString();
        }

        public class NewPoint
        {
            public int x;
            public int y;
            public int Bombs;

            public NewPoint(int _x, int _y, int _b)
            {
                x = _x;
                y = _y;
                Bombs = _b;
            }
        }
        private void Window_Loaded_easy(object sender, RoutedEventArgs e)
        {
            N = 9;
            M = 9;
            NumBombs = 10;
            Сапер.Height = 360;
            Сапер.Width = 290;
            gridPanel.Width = Сапер.Width;
            Window_Loaded(sender, e);
        }

        private void Window_Loaded_normal(object sender, RoutedEventArgs e)
        {
            N = 16;
            M = 16;
            NumBombs = 40;
            Сапер.Height = 570;
            Сапер.Width = 500;
            gridPanel.Width = Сапер.Width;
            Window_Loaded(sender, e);
        }

        private void Window_Loaded_hard(object sender, RoutedEventArgs e)
        {
            N = 30;
            M = 16;
            NumBombs = 99;
            Window_Loaded(sender, e);
            Сапер.Height = 570;
            Сапер.Width = 920;
            gridPanel.Width = Сапер.Width;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapImage Image = new BitmapImage();
            Image.BeginInit();
            Image.UriSource = new Uri(@"/Images/new_game.jpg", UriKind.RelativeOrAbsolute); //загружаем картинку
            Image.EndInit();
            Image game = new Image(); //картинка для кнопки
            game.Source = Image; //Загруженная картинка
            btnMiner.Content = game;

            timer.Stop();
            lblTimer.Content = "0";
            lblFlags.Content = NumBombs;
            int[,] Mas = new int[N, M];
            SetBombs(Mas);
            CountBombs(Mas);
            btns = new Button[N, M];
            for (int i = 0; i < N; i++)
                for (int j = 0; j < M; j++)
                {
                    btns[i, j] = new Button();
                    NewPoint p = new NewPoint(i, j, Mas[i, j]);
                    btns[i, j].Tag = p;
                    btns[i, j].Content = "";
                    btns[i, j].Width = 30;
                    btns[i, j].Height = 30;

                    btns[i, j].Click += btnNew_Click;
                    btns[i, j].MouseRightButtonDown += btnRight_Button_Click;
                    Canvas.SetLeft(btns[i, j], i * btns[i, j].Width);
                    Canvas.SetTop(btns[i, j], j * btns[i, j].Height);
                    cnvMain.Width = btns[i, j].Width * N;
                    cnvMain.Height = btns[i, j].Height * M;
                    cnvMain.Children.Add(btns[i, j]);
                }
        }

        public void SetBombs(int[,] Mas)
        {
            for (int i = 0; i < NumBombs; i++)
            {
                Random rand = new Random();
                int x = rand.Next(0, N);
                int y = rand.Next(0, M);
                while (Mas[x, y] == -1)
                {
                    x = rand.Next(0, N);
                    y = rand.Next(0, M);
                }
                Mas[x, y] = -1;

            }
        }

        public void CountBombs(int[,] Mas)
        {

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    if (Mas[i, j] != -1)
                    {
                        if (i - 1 >= 0)
                        {
                            if (Mas[i - 1, j] == -1)
                                Mas[i, j]++;
                        }
                        if (i + 1 < N)
                        {
                            if (Mas[i + 1, j] == -1)
                                Mas[i, j]++;
                        }
                        if (j - 1 >= 0)
                        {
                            if (Mas[i, j - 1] == -1)
                                Mas[i, j]++;
                        }
                        if (j + 1 < M)
                        {
                            if (Mas[i, j + 1] == -1)
                                Mas[i, j]++;
                        }
                        if (j + 1 < M && i + 1 < N)
                        {
                            if (Mas[i + 1, j + 1] == -1)
                                Mas[i, j]++;
                        }
                        if (j - 1 >= 0 && i + 1 < N)
                        {
                            if (Mas[i + 1, j - 1] == -1)
                                Mas[i, j]++;
                        }
                        if (j + 1 < M && i - 1 >= 0)
                        {
                            if (Mas[i - 1, j + 1] == -1)
                                Mas[i, j]++;
                        }
                        if (j - 1 >= 0 && i - 1 >= 0)
                        {
                            if (Mas[i - 1, j - 1] == -1)
                                Mas[i, j]++;
                        }

                    }

                }
            }
        }

        public void ClickButton(Button button)
        {
            int fl;
            int.TryParse(lblFlags.Content.ToString(), out fl);
            if (button.Content != "" && button.IsEnabled)
            {
                fl++;
                lblFlags.Content = fl.ToString();
            }

            if ((button.Tag as NewPoint).Bombs == 0 && button.IsEnabled)
            {
                button.Content = "";
                button.IsEnabled = false;
                if ((button.Tag as NewPoint).x > 0)
                    ClickButton(btns[(button.Tag as NewPoint).x - 1, (button.Tag as NewPoint).y]); //вызов соседней кнопки
                if (((button.Tag as NewPoint).x > 0) && ((button.Tag as NewPoint).y > 0))
                    ClickButton(btns[(button.Tag as NewPoint).x - 1, (button.Tag as NewPoint).y - 1]);
                if ((button.Tag as NewPoint).y > 0)
                    ClickButton(btns[(button.Tag as NewPoint).x, (button.Tag as NewPoint).y - 1]);
                if (((button.Tag as NewPoint).x < N - 1) && ((button.Tag as NewPoint).y > 0))
                    ClickButton(btns[(button.Tag as NewPoint).x + 1, (button.Tag as NewPoint).y - 1]);
                if ((button.Tag as NewPoint).x < N - 1)
                    ClickButton(btns[(button.Tag as NewPoint).x + 1, (button.Tag as NewPoint).y]);
                if (((button.Tag as NewPoint).x < N - 1) && ((button.Tag as NewPoint).y < M - 1))
                    ClickButton(btns[(button.Tag as NewPoint).x + 1, (button.Tag as NewPoint).y + 1]);
                if ((button.Tag as NewPoint).y < M - 1)
                    ClickButton(btns[(button.Tag as NewPoint).x, (button.Tag as NewPoint).y + 1]);
                if (((button.Tag as NewPoint).x > 0) && ((button.Tag as NewPoint).y < M - 1))
                    ClickButton(btns[(button.Tag as NewPoint).x - 1, (button.Tag as NewPoint).y + 1]);

            }

            else
            {
                BitmapImage loadImage = new BitmapImage();
                loadImage.BeginInit();
                int b = (button.Tag as NewPoint).Bombs;
                loadImage.UriSource = new Uri(@"/Images/" + b.ToString() + ".jpg", UriKind.RelativeOrAbsolute); //загружаем картинку
                loadImage.EndInit();
                Image test = new Image(); //картинка для кнопки
                test.Source = loadImage; //Загруженная картинка
                button.Content = test;
                button.IsEnabled = false;
                return;
            }
        }

        void btnNew_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            if (((sender as Button).Tag as NewPoint).Bombs == -1)
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(@"/Images/lost.jpg", UriKind.RelativeOrAbsolute); //загружаем картинку
                img.EndInit();
                Image lost = new Image(); //картинка для кнопки
                lost.Source = img; //Загруженная картинка
                btnMiner.Content = lost;

                timer.Stop();
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < M; j++)
                    {
                        if ((btns[i, j].Tag as NewPoint).Bombs == -1)
                        {
                            Image image = new Image();
                            BitmapImage bombImage = new BitmapImage();
                            bombImage.BeginInit();
                            bombImage.UriSource = new Uri(@"/Images/bomb.jpg", UriKind.RelativeOrAbsolute); //загружаем картинку
                            bombImage.EndInit();
                            Image bomb = new Image(); //картинка для кнопки
                            bomb.Source = bombImage; //Загруженная картинка
                            btns[i, j].Content = bomb;
                        }
                        btns[i, j].IsEnabled = false;
                    }

                if (MessageBox.Show("You lost!!! Play again?", "The game is over", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    Window_Loaded(sender, e);
                else
                {
                    Close();
                    timer.Stop();
                }

            }
            else
            {
                ClickButton(sender as Button);
                int n = 0;
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < M; j++)
                    {
                        if (btns[i, j].IsEnabled)
                            n++;
                    }
                if (n == NumBombs)
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = new Uri(@"/Images/win.jpg", UriKind.RelativeOrAbsolute); //загружаем картинку
                    img.EndInit();
                    Image win = new Image(); //картинка для кнопки
                    win.Source = img; //Загруженная картинка
                    btnMiner.Content = win;
                    for (int i = 0; i < N; i++)
                        for (int j = 0; j < N; j++)
                        {
                            if (btns[i, j].IsEnabled && btns[i, j].Content == "")
                            {
                                BitmapImage bombImage = new BitmapImage();
                                bombImage.BeginInit();
                                bombImage.UriSource = new Uri(@"/Images/bomb.jpg", UriKind.RelativeOrAbsolute); //загружаем картинку
                                bombImage.EndInit();
                                Image bomb = new Image(); //картинка для кнопки
                                bomb.Source = bombImage; //Загруженная картинка
                                btns[i, j].Content = bomb;
                            }
                            btns[i, j].IsEnabled = false;
                        }
                    timer.Stop();
                    if (MessageBox.Show("You won!!! Your time is " + lblTimer.Content + " seconds. Play again?", "Congratulations!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        Window_Loaded(sender, e);
                    else
                    {
                        Close();
                        timer.Stop();
                    }
                }
            }

        }

        void btnRight_Button_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage flagImage = new BitmapImage();
            flagImage.BeginInit();
            flagImage.UriSource = new Uri(@"/Images/flag.jpg", UriKind.RelativeOrAbsolute); //загружаем картинку флаг
            flagImage.EndInit();
            Image flag = new Image(); //картинка для кнопки
            flag.Source = flagImage; //Загруженная картинка

            int fl;
            int.TryParse(lblFlags.Content.ToString(), out fl);
            if ((sender as Button).Content == "")
            {
                (sender as Button).Content = flag;
                fl--;
            }
            else
            {
                (sender as Button).Content = "";
                fl++;
            }
            lblFlags.Content = fl.ToString();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        } 
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Stop();
            if (MessageBox.Show("Do you really want to quit?", "Confirm Closing", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                e.Cancel = true;
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < M; j++)
                    {
                        if (btns[i, j].IsEnabled)
                        {
                            timer.Start();
                            return;
                        }
                    }
                    return;
                }
            }
        }

    }
}
