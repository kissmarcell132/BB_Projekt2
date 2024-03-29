﻿
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
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
        public Enemy(bool isIt)
        {
            Shape = new Rectangle
            {
                Width = 70,
                Height = 70,
                Fill = GetRandomImageBrush(isIt), // Beállítjuk a képet amit random választottunk az enemyknek
            };
        }

        private ImageBrush GetRandomImageBrush(bool isIt)
        {
            string imagePath = "";
            if (!isIt)
            {
                int randomImageIndex = random.Next(1, 6); // random szám generálása 1-től 5-ig
                imagePath = $"pack://application:,,,/images/{randomImageIndex}.png"; // A kiválasztott számú kép
            }
            else
                imagePath = $"pack://application:,,,/images/lettuce.png"; // A kiválasztott számú kép
            ImageBrush imageBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(imagePath)), // A kép megkeresése az adott helyen
            };

            return imageBrush;
        }

    }
}
