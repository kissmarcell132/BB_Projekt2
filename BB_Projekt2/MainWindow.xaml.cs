using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
        bool isItNiccs = false;
        DispatcherTimer timer = new DispatcherTimer(); // Threadinggel érjük el
        bool moveLeft;
        bool moveRight;
        List<Rectangle> itemsForRemove = new List<Rectangle>();

        int difficultyNumber = 0;

        double enemySpawnTime = 0;
        int playerSpeed = 0;
        double fallingSpeed = 0;

        int damage = 0;
        int score = 0;

        private readonly Random random = new Random();
        private readonly DispatcherTimer enemySpawnTimer = new DispatcherTimer();

        Rect kaplonHitbox;

        public MainWindow()
        {
            InitializeComponent();
            PlaySound(@"pack://application:,,,/Sounds/gameStart.mp3");
        }

        private void PlaySound(string sound)
        {
            Uri uri = new Uri(sound);
            var player = new MediaPlayer();
            player.Open(uri);
            player.Play();
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
            
            ImageBrush kaplonImage = new ImageBrush();
            if (!isItNiccs)
                kaplonImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/kaplon.png"));
            else
                kaplonImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/laci.png"));
            kaplon_Player.Fill = kaplonImage;


            enemySpawnTimer.Interval = TimeSpan.FromSeconds(enemySpawnTime); //Hány másodpercenként spawnoljanak az enemyk
            enemySpawnTimer.Tick += SpawnEnemy;
            enemySpawnTimer.Start();
        }
        private void LoopForGame(object? sender, EventArgs e)
        {
            if (damage >= 100)
            {
                gameEnd();
            }
            scoreLbl.Content = $"Score: {score}";
            damageLbl.Content = $"Damage: {damage}";
            kaplonHitbox = new Rect(Canvas.GetLeft(kaplon_Player), Canvas.GetTop(kaplon_Player), kaplon_Player.Width, kaplon_Player.Height);
            MovePlayer();
            CheckCollisionsBeetweenEnemyAndBullet();
            CheckCollisionsBeetweenEnemyAndPlayer();;
            foreach (var item in gameCanvas.Children.OfType<Rectangle>())
            {
                if (item is Rectangle && (string)item.Tag == "bullet")
                {
                    Canvas.SetTop(item, Canvas.GetTop(item) - 20);

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
                    enemySpawnTime = 2; 
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
        private void gameHardener()
        {
            playerSpeed += 2;
            if (fallingSpeed > 0.25)
                fallingSpeed = fallingSpeed / 1.25;
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
            if (e.Key == Key.Right)
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
            Enemy enemy = new Enemy(isItNiccs); //Új ellenség létrehozása
            Canvas.SetLeft(enemy.Shape, random.Next((int)gameCanvas.ActualWidth - 50)); //Ellenség random pozícióba helyezése a canvason
            Canvas.SetTop(enemy.Shape, 0);
            enemy.Shape.Tag = "enemy";

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
            //gameCanvas.Children.Remove(difficultyChooser);
            difficultyChooser.Visibility = Visibility.Collapsed;
            countBack();
            gameStart();

        }

        private void mediumBTN_Click(object sender, RoutedEventArgs e)
        {
            difficultyNumber = 2;
            difficultyChooser.Visibility = Visibility.Collapsed;
            //gameCanvas.Children.Remove(difficultyChooser);
            countBack();
            gameStart();
        }

        private void hardBTN_Click(object sender, RoutedEventArgs e)
        {
            difficultyNumber = 3;
            difficultyChooser.Visibility = Visibility.Collapsed;
            //gameCanvas.Children.Remove(difficultyChooser);
            countBack();
            gameStart();

        }

        private void nitsBTN_Click(object sender, RoutedEventArgs e)
        {
            difficultyNumber = 4;
            difficultyChooser.Visibility = Visibility.Collapsed;
            //gameCanvas.Children.Remove(difficultyChooser);
            isItNiccs = true;
            countBack();
            gameStart();
        }

        private void countBack()
        {

        }

        private void CheckCollisionsBeetweenEnemyAndBullet()
        {
            //Megkeressük és bejárjuk az enemy taggel ellátott objektumokat
            foreach (var enemy in gameCanvas.Children.OfType<Rectangle>().Where(item => (string)item.Tag == "enemy"))
            {
                // enemy Hitbox
                Rect enemyHitbox = new Rect(Canvas.GetLeft(enemy), Canvas.GetTop(enemy), enemy.Width, enemy.Height);
                //Megkeressük és bejárjuk a bullet taggel ellátot rectangle-ket
                foreach (var bullet in gameCanvas.Children.OfType<Rectangle>().Where(item => (string)item.Tag == "bullet"))
                {   
                    // Meghatározzuk a lövedék hitboxát
                    Rect bulletHitbox = new Rect(Canvas.GetLeft(bullet), Canvas.GetTop(bullet), bullet.Width, bullet.Height);
                    // Ütközés észlelése az IntersectWith függvénnyel, amely megnézi hogy a két objektum összeér e
                    if (enemyHitbox.IntersectsWith(bulletHitbox))
                    {
                        // Objektumok hozzáadása a törlendő listához
                        itemsForRemove.Add(enemy);
                        itemsForRemove.Add(bullet);

                        // Pontszám növelése
                        score += 10;
                        if (score % 50 == 0)
                            gameHardener();
                    }
                }
            }
        }



        private void CheckCollisionsBeetweenEnemyAndPlayer()
        {
            // Bejárjuk a Canvas gyermekeit amiknek típusa Rectangle illetve enemy taggel vannak ellátva
            foreach (var enemy in gameCanvas.Children.OfType<Rectangle>().Where(item => (string)item.Tag == "enemy"))
            {   
                // Meghatározzuk az enemy Hitboxát
                Rect enemyHitbox = new Rect(Canvas.GetLeft(enemy), Canvas.GetTop(enemy), enemy.Width, enemy.Height);

                // Ütközés észlelése az IntersectWith függvénnyel, amely megnézi hogy a két objektum összeér e
                if (enemyHitbox.IntersectsWith(kaplonHitbox))
                {
                    // Hozzá adjuk a törlendő listához
                    itemsForRemove.Add(enemy);

                    // damage növelése
                    damage += 10;
                }
            }
        }

        private void gameEnd()
        {
            timer.Stop();
            enemySpawnTimer.Stop();
            List<string> messages = new List<string>();
            if(!isItNiccs)
            {
                messages.Add("Úgy néz ki Kaplonon nem csak a nők");
                messages.Add("hanem a kebabok is ki tudnak fogni");
                messages.Add($"Kedvenc csontvázunk {score/10} kebabot evett meg");
            }
            else
            {
                messages.Add("Látod barátocskám a saláta kódok néha");
                messages.Add("még rajtam is ki tudnak fogni");
                messages.Add($"Laci mester {score/10} saláta kódót tudott legyözni");
            }
            mainGrid.Children.Remove(gameCanvas);
            label1.Content = messages[0];
            label2.Content = messages[1];
            endGameLBL.Content = messages[2];
            endGameLBL.VerticalAlignment = VerticalAlignment.Center;
            endGameLBL.Visibility = closeBTN.Visibility = label1.Visibility = label2.Visibility = startOverBTN.Visibility = Visibility.Visible;
        }
        private void StartClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            System.Windows.Application.Current.Shutdown();
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
