using System.Collections.Generic;
using Avalonia.Controls;

namespace Recognizer;

internal static class MedianFilterTask
{
	enum PixelPosition
	{
		Center,
		Left,
		Right,
		Top,
		Bottom,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	public static double[,] MedianFilter(double[,] original)
	{
		if (original.Length == 1) return original;

		var newArray = new double[original.GetLength(0), original.GetLength(1)];
		var width = original.GetLength(0);
		var height = original.GetLength(1);

		for (int i = 0; i < width; i++)
			for (int j = 0; j < height; j++)
			{
				var currentPixelPosition = GetPixelPosition(i, j, width, height);
				var pixelsArray = new List<double>();

				if (width == 1)
					pixelsArray = GetListHorizontal(original, currentPixelPosition, i, j);
				else if (height == 1)
					pixelsArray = GetListVertical(original, currentPixelPosition, i, j);
				else
					pixelsArray = GetList3X3(original, currentPixelPosition, i, j);

				var median = GetMedian(pixelsArray);
				newArray[i, j] = median;
			}
		return newArray;
	}

	private static PixelPosition GetPixelPosition(int i, int j, int width, int height)
	{
		if (i == 0 && j == 0)
			return PixelPosition.TopLeft;
		else if (i == 0 && j == height - 1)
			return PixelPosition.TopRight;
		else if (i == width - 1 && j == 0)
			return PixelPosition.BottomLeft;
		else if (i == width - 1 && j == height - 1)
			return PixelPosition.BottomRight;
		else if (i == 0)
			return PixelPosition.Top;
		else if (i == width - 1)
			return PixelPosition.Bottom;
		else if (j == 0)
			return PixelPosition.Left;
		else if (j == height - 1)
			return PixelPosition.Right;

		return PixelPosition.Center;
	}

	public static double GetMedian(List<double> array)
	{
		array.Sort();
		if (array.Count % 2 == 0)
			return (array[array.Count / 2] + array[array.Count / 2 - 1]) / 2;
		else
			return array[array.Count / 2];
	}
	
	private static List<double> GetListHorizontal(double[,] original, PixelPosition position, int i, int j)
    {
        switch (position)
        {
			case PixelPosition.TopLeft: return new List<double>()
			{
				original[i, j],
				original[i, j + 1]
			};

			case PixelPosition.Top: return new List<double>()
			{
				original[i, j],
				original[i, j - 1],
				original[i, j + 1]
			};

			default: return new List<double>()
			{
				original[i, j],
				original[i, j - 1]
			};
        }
    }

	private static List<double> GetListVertical(double[,] original, PixelPosition position, int i, int j)
	{
		switch (position)
		{
			case PixelPosition.TopLeft: return new List<double>()
			{
				original[i, j],
				original[i + 1, j]
			};

			case PixelPosition.Left: return new List<double>()
			{
				original[i, j],
				original[i - 1, j],
				original[i + 1, j]
			};

			default: return new List<double>()
			{
				original[i, j],
				original[i - 1, j]
			};
		}
	}
	
	private static List<double> GetList3X3(double[,] original, PixelPosition position, int i, int j)
	{
		switch (position)
		{
			case PixelPosition.TopLeft: return new List<double>()
			{
				original[i, j],
				original[i, j + 1],
				original[i + 1, j],
				original[i + 1, j + 1]
			};

			case PixelPosition.TopRight: return new List<double>()
			{
				original[i, j],
				original[i, j - 1],
				original[i + 1, j],
				original[i + 1, j - 1]
			};

			case PixelPosition.BottomLeft: return new List<double>()
			{
				original[i, j],
				original[i, j + 1],
				original[i - 1, j],
				original[i - 1, j + 1]
			};

			case PixelPosition.BottomRight: return new List<double>()
			{
				original[i, j],
				original[i, j - 1],
				original[i - 1, j - 1],
				original[i - 1, j]
			};

			case PixelPosition.Top: return new List<double>()
			{
				original[i, j],
				original[i, j - 1],
				original[i, j + 1],
				original[i + 1, j - 1],
				original[i + 1, j],
				original[i + 1, j + 1]
			};

			case PixelPosition.Bottom: return new List<double>()
			{
				original[i, j],
				original[i, j - 1],
				original[i, j + 1],
				original[i - 1, j - 1],
				original[i - 1, j],
				original[i - 1, j + 1]
			};

			case PixelPosition.Left: return new List<double>()
			{
				original[i, j],
				original[i - 1, j],
				original[i + 1, j],
				original[i - 1, j + 1],
				original[i, j + 1],
				original[i + 1, j + 1]
			};

			case PixelPosition.Right: return new List<double>()
			{
				original[i, j],
				original[i - 1, j],
				original[i + 1, j],
				original[i - 1, j - 1],
				original[i, j - 1],
				original[i + 1, j - 1]
			};

			default: return new List<double>()
			{
				original[i, j],
				original[i - 1, j - 1],
				original[i - 1, j],
				original[i - 1, j + 1],
				original[i, j - 1],
				original[i, j + 1],
				original[i + 1, j - 1],
				original[i + 1, j],
				original[i + 1, j + 1]
			};
		}
	}
}
