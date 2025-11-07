using System;
using System.Collections.Generic;
using Avalonia.Controls;

public static class ThresholdFilterTask
{
    public static double[,] ThresholdFilter(double[,] original, double whitePixelsFraction)
    {
		var result = new double[original.GetLength(0), original.GetLength(1)];
		var width = original.GetLength(0);
		var height = original.GetLength(1);
        var T = GetT(original, whitePixelsFraction, width, height);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (original[i, j] >= T)
                    result[i, j] = 1.0;
                else
                    result[i, j] = 0.0;
            }
        }
        return result;
    }

	public static double GetT(double[,] original, double whitePixelsFraction, int width, int height)
	{
		if (whitePixelsFraction == 0) return double.MaxValue;
		if (whitePixelsFraction == 1.0) return double.MinValue;

		var sortedList = new List<double>();
		var totalPixels = width * height;
		var whitePixelsCount = (int)(whitePixelsFraction * totalPixels);

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				sortedList.Add(original[i, j]);
			}
		}

		sortedList.Sort();
		sortedList.Reverse();

		if (whitePixelsCount == 0)
			return sortedList[0] + 1e-10;
		else if (whitePixelsCount >= totalPixels)
			return sortedList[totalPixels - 1] - 1e-10;
		else
			return sortedList[whitePixelsCount - 1];
	}
}
