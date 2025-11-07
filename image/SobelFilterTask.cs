using System;

namespace Recognizer;
internal static class SobelFilterTask
{
    public static double[,] SobelFilter(double[,] g, double[,] sx)
    {
        var width = g.GetLength(0);
        var height = g.GetLength(1);
        var result = new double[width, height];
        
        // Создаем sy как транспонированную матрицу sx
        var sy = Transpose(sx);
        
        int radius = sx.GetLength(0) / 2;
        
        for (int x = radius; x < width - radius; x++)
            for (int y = radius; y < height - radius; y++)
            {
                // Вычисляем свертку с матрицей sx (производная по X)
                double gx = Convolve(g, sx, x, y, radius);
                
                // Вычисляем свертку с матрицей sy (производная по Y)
                double gy = Convolve(g, sy, x, y, radius);
                
                result[x, y] = Math.Sqrt(gx * gx + gy * gy);
            }
        return result;
    }
    
    private static double[,] Transpose(double[,] matrix)
    {
        int size = matrix.GetLength(0);
        var result = new double[size, size];
        
        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                result[j, i] = matrix[i, j];
                
        return result;
    }
    
    private static double Convolve(double[,] image, double[,] kernel, int x, int y, int radius)
    {
        double result = 0;
        int kernelSize = kernel.GetLength(0);
        
        for (int i = 0; i < kernelSize; i++)
            for (int j = 0; j < kernelSize; j++)
            {
                int imageX = x - radius + i;
                int imageY = y - radius + j;
                result += image[imageX, imageY] * kernel[i, j];
            }
                
        return result;
    }
}