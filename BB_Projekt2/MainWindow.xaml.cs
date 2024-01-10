using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
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

        int difficultyNumber = 0;

        double enemySpawnTime = 0;
        int playerSpeed = 0;
        int fallingSpeed = 0;

        int damage = 0;
        int score = 0;

        private readonly Random random = new Random();
        private readonly DispatcherTimer enemySpawnTimer = new DispatcherTimer();

        Rect kaplonHitbox;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void gameStart()
        {
            getDifficulty(difficultyNumber);
            kaplon_Player.Visibility = Visibility.Visible;
            damageLbl.Visibility = Visibility.Visible;
            scoreLbl.Visibility = Visibility.Visible;

            timer.Interval = TimeSpan.FromMilliseconds(10); // 10 millisekundumonként frissít
            timer.Tick += LoopForGame; // hozzá adjuk a LoopForGame fügvényt így azt hívja meg folyamatosan
            timer.Start(); // el kell indítani a játékot

            gameCanvas.Focus();
            //ImageBrush bg = new ImageBrush();

            //bg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/background.jpg"));
            //bg.TileMode = TileMode.FlipXY; // kicsi a kép így többször kell berakni a TileMode pedig a módja hogy hogyan rakja a képet
            //bg.Viewport = new Rect(0, 0, 0.2, 0.2);
            //bg.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;

            //gameCanvas.Background = bg;

            ImageBrush kaplonImage = new ImageBrush();
            kaplonImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/kaplon.png"));
            kaplon_Player.Fill = kaplonImage;



            enemySpawnTimer.Interval = TimeSpan.FromSeconds(enemySpawnTime); //Hány másodpercenként spawnoljanak az enemyk
            enemySpawnTimer.Tick += SpawnEnemy;
            enemySpawnTimer.Start();
        }
        private void LoopForGame(object? sender, EventArgs e)
        {
            scoreLbl.Content = $"Score: {score}";
            damageLbl.Content = $"Damage: {damage}";
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
                if(item is Rectangle && Canvas.GetTop(item) > 500)
                {
                    damage += 10;
                    itemsForRemove.Add(item);
                }

                
            }
            Remover();
        }

        private void getDifficulty(int difficulty)
        {
            switch (difficulty)
            {
                case 1:
                    enemySpawnTime = 2.5;
                    playerSpeed = 10;
                    fallingSpeed = 15;
                    break;
                case 2:
                    enemySpawnTime = 1.5;
                    playerSpeed = 15;
                    fallingSpeed = 10;
                    break;
                case 3:
                    enemySpawnTime = 1;
                    playerSpeed = 20;
                    fallingSpeed = 8;
                    break;
                case 4:
                    enemySpawnTime = 0.3;
                    playerSpeed = 25;
                    fallingSpeed = 4;
                    break;

            }
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

            if (moveRight && Canvas.GetLeft(kaplon_Player) + 69  < 500)
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
                    Fill = Brushes.Yellow,
                    Stroke = Brushes.OrangeRed

                };

                Canvas.SetLeft(newBullet, Canvas.GetLeft(kaplon_Player) + kaplon_Player.Width / 2 - 2);
                Canvas.SetTop(newBullet, Canvas.GetTop(kaplon_Player) - newBullet.Height);

                gameCanvas.Children.Add(newBullet);
            }
        }

        private void SpawnEnemy(object sender, EventArgs e)
        {
            Enemy enemy = new Enemy(); //Új ellenség létrehozása
            Canvas.SetLeft(enemy.Shape, random.Next((int)gameCanvas.ActualWidth - 50)); //Ellenség random pozícióba helyezése a canvason
            Canvas.SetTop(enemy.Shape, 0);

            gameCanvas.Children.Add(enemy.Shape); //Ellenség hozzáadása a canvashoz
           
            MoveEnemy(enemy); //Ellenség mozgásának animálása
        }

        private void MoveEnemy(Enemy enemy)
        {
            
            var storyboard = new System.Windows.Media.Animation.Storyboard(); //Egy storyboard készítése az animáció készítéséhez

            // Create a DoubleAnimation to move the enemy vertically
            var animation = new System.Windows.Media.Animation.DoubleAnimation //Az ellenség vertikális mozgásához szükséges
            {
                From = Canvas.GetTop(enemy.Shape),
                To = gameCanvas.ActualHeight,
                Duration = TimeSpan.FromSeconds(fallingSpeed), // Egy időtartam megadása
            };

            // Az animációk beállítása és hozzáadása a storyboardhoz
            System.Windows.Media.Animation.Storyboard.SetTarget(animation, enemy.Shape); 
            System.Windows.Media.Animation.Storyboard.SetTargetProperty(animation, new PropertyPath(Canvas.TopProperty));
            storyboard.Children.Add(animation);

            // Animáció elindítása
            storyboard.Begin();
        }

        private void easyBTN_Click(object sender, RoutedEventArgs e)
        {
            difficultyNumber = 1;
            gameCanvas.Children.Remove(difficultyChooser);
            gameStart();

        }

        private void mediumBTN_Click(object sender, RoutedEventArgs e)
        {
            difficultyNumber = 2;
            gameCanvas.Children.Remove(difficultyChooser);
            gameStart();
        }

        private void hardBTN_Click(object sender, RoutedEventArgs e)
        {
            difficultyNumber = 3;
            gameCanvas.Children.Remove(difficultyChooser);
            gameStart();

        }

        private void nitsBTN_Click(object sender, RoutedEventArgs e)
        {
            difficultyNumber = 4;
            gameCanvas.Children.Remove(difficultyChooser);
            gameStart();
        }
    }
}
