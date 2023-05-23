using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1_14
{
    public class ImageSmoothing
    {
        public static Bitmap ApplySmoothing(Bitmap image)
        {
            int width = image.Width;
            int height = image.Height;

            // Создаем новое изображение для результата сглаживания
            Bitmap smoothed = new Bitmap(width, height);

            // Проходим по каждому пикселу изображения, кроме крайних границ
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    // Получаем значения цветов пикселей вокруг текущего пиксела
                    Color[] neighbors = {
                    image.GetPixel(x - 1, y - 1),
                    image.GetPixel(x, y - 1),
                    image.GetPixel(x + 1, y - 1),
                    image.GetPixel(x - 1, y),
                    image.GetPixel(x, y),
                    image.GetPixel(x + 1, y),
                    image.GetPixel(x - 1, y + 1),
                    image.GetPixel(x, y + 1),
                    image.GetPixel(x + 1, y + 1)
                };

                    // Вычисляем новое значение цвета текущего пиксела
                    int r = 0, g = 0, b = 0;
                    foreach (Color neighbor in neighbors)
                    {
                        r += neighbor.R;
                        g += neighbor.G;
                        b += neighbor.B;
                    }
                    r /= 9;
                    g /= 9;
                    b /= 9;

                    // Устанавливаем новый цвет текущего пиксела в результирующем изображении
                    smoothed.SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }

            return smoothed;
        }
    }
}
