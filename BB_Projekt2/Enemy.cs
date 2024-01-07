using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BB_Projekt2
{
    public class Enemy
    {
        Random random = new Random();
        public Rectangle Shape { get; }

        public Enemy()
        {
            Shape = new Rectangle
            {
                Width = 50,
                Height = 50,
                Fill = GetRandomImageBrush(), // Beállítjuk a képet amit random választottunk az enemyknek
            };
        }
        private string GetRandomImage()
        {
            int randomImageIndex = random.Next(1, 6); // random szám generálása 1-től 5-ig
            string imagePath = "";
            if (randomImageIndex == 5)
            {
                 imagePath = $"pack://application:,,,/Images/{randomImageIndex}.png"; // A kiválasztott számú kép
            }
            else
            {
                imagePath = $"pack://application:,,,/Images/{randomImageIndex}.jpg"; // A kiválasztott számú
            }
            return imagePath;

        }

        private ImageBrush GetRandomImageBrush()
        {
            string imagePath = GetRandomImage().ToString();

            ImageBrush imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(imagePath)), // A kép megkeresése az adott helyen
            };

            return imageBrush;
        }

    }
}
