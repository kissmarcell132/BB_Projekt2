using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace BB_Projekt2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer(); // Threadinggel érjük el
        bool moveLeft;
        bool moveRight;
        List<Rectangle> itemsForRemove = new List<Rectangle>();


        int playerSpeed = 5;

        Rect kaplonHitbox;

        public MainWindow()
        {
            InitializeComponent();

            
            timer.Interval = TimeSpan.FromMilliseconds(1); // 20 millisekundumonként frissít
            timer.Tick += LoopForGame; // hozzá adjuk a LoopForGame fügvényt így azt hívja meg folyamatosan
            timer.Start(); // el kell indítani a játékot

            gameCanvas.Focus();
            ImageBrush bg = new ImageBrush();

            bg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/background.jpg"));
            bg.TileMode = TileMode.FlipXY; // kicsi a kép így többször kell berakni a TileMode pedig a módja hogy hogyan rakja a képet
            bg.Viewport = new Rect(0, 0, 0.2, 0.2);
            bg.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;

            gameCanvas.Background = bg;

            ImageBrush kaplonImage = new ImageBrush();
            kaplonImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/kaplon.png"));
            kaplon_Player.Fill = kaplonImage;
        }

        private void LoopForGame(object? sender, EventArgs e)
        {
            kaplonHitbox = new Rect(Canvas.GetLeft(kaplon_Player), Canvas.GetTop(kaplon_Player), kaplon_Player.Width, kaplon_Player.Height);
            MovePlayer();

            foreach (var item in gameCanvas.Children.OfType<Rectangle>())
            {
                if (item is Rectangle && (string)item.Tag == "bullet")
                {
                    Canvas.SetTop(item, Canvas.GetTop(item) - 20);

                    Rect bulletHitbox = new Rect(Canvas.GetLeft(item), Canvas.GetTop(item), item.Width, item.Height);
                    if (Canvas.GetTop(item) < 10)
                        itemsForRemove.Add(item);
                }
            }

            Remover();
            

        }

        private void Remover()
        {
            foreach (Rectangle rect in itemsForRemove)
            {
                gameCanvas.Children.Remove(rect);
            }
        }

        private void MovePlayer()
        {
            if (moveLeft && Canvas.GetLeft(kaplon_Player) > 0)
            {
                Canvas.SetLeft(kaplon_Player, Canvas.GetLeft(kaplon_Player) - playerSpeed);
            }

            if (moveRight && Canvas.GetLeft(kaplon_Player) + 69 < 500)
            {
                Canvas.SetLeft(kaplon_Player, Canvas.GetLeft(kaplon_Player) + playerSpeed);
            }
        }

        private void gameCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
                moveLeft = true;
            if(e.Key == Key.Right)
                moveRight = true;
        }

        private void gameCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
                moveLeft = false;
            if (e.Key == Key.Right)
                moveRight = false;
            if (e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle()
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.WhiteSmoke,
                    Stroke = Brushes.Black

                };

                Canvas.SetLeft(newBullet, Canvas.GetLeft(kaplon_Player) + kaplon_Player.Width / 2 - 2);
                Canvas.SetTop(newBullet, Canvas.GetTop(kaplon_Player) - newBullet.Height);

                gameCanvas.Children.Add(newBullet);
            }
        }

        private void SpawnEnemy()
        {

        }
    }
}
