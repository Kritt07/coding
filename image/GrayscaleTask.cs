using System;
using System.Diagnostics;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Recognizer;

public static class GrayscaleTask
{
    // Оригинальный метод
    public static double[,] ToGrayscale(Pixel[,] original)
    {
        var grayscale = new double[original.GetLength(0), original.GetLength(1)];
        var width = original.GetLength(0);
        var height = original.GetLength(1);

        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                var pixel = original[i, j];
                var r = pixel.R;
                var g = pixel.G;
                var b = pixel.B;
                grayscale[i, j] = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
            }
        }
        return grayscale;
    }

    // 1. GetLength вызывается один раз до циклов (уже так и есть в оригинале — но для ясности выделим как "оптимизированный")
    public static double[,] ToGrayscale_GetLengthOnce(Pixel[,] original)
    {
        int width = original.GetLength(0);
        int height = original.GetLength(1);
        var grayscale = new double[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var pixel = original[i, j];
                grayscale[i, j] = (0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B) / 255;
            }
        }
        return grayscale;
    }

    // 2. GetLength вызывается в условии цикла каждый раз
    public static double[,] ToGrayscale_GetLengthInLoop(Pixel[,] original)
    {
        var grayscale = new double[original.GetLength(0), original.GetLength(1)];

        for (int i = 0; i < original.GetLength(0); i++)
        {
            for (int j = 0; j < original.GetLength(1); j++)
            {
                var pixel = original[i, j];
                grayscale[i, j] = (0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B) / 255;
            }
        }
        return grayscale;
    }

    // Вспомогательная функция для вычисления яркости
    private static double ComputeLuminance(Pixel p)
    {
        return (0.299 * p.R + 0.587 * p.G + 0.114 * p.B) / 255;
    }

    // 3. Вычисление серого вынесено в отдельную функцию
    public static double[,] ToGrayscale_ExtractPixelFunction(Pixel[,] original)
    {
        int width = original.GetLength(0);
        int height = original.GetLength(1);
        var grayscale = new double[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                grayscale[i, j] = ComputeLuminance(original[i, j]);
            }
        }
        return grayscale;
    }

    [Test]
    public static void Measure(Action action)
    {
        var sw = Stopwatch.StartNew();
        action();
        sw.Stop();
        TestContext.WriteLine($"{sw.ElapsedMilliseconds} ms");
    }
}