namespace Recognizer;

public static class GrayscaleTask
{
	/* 
	 * Переведите изображение в серую гамму.
	 * 
	 * original[x, y] - массив пикселей с координатами x, y. 
	 * Каждый канал R,G,B лежит в диапазоне от 0 до 255.
	 * 
	 * Получившийся массив должен иметь те же размеры, 
	 * grayscale[x, y] - яркость пикселя (x,y) в диапазоне от 0.0 до 1.0
	 *
	 * Используйте формулу:
	 * Яркость = (0.299*R + 0.587*G + 0.114*B) / 255
	 * 
	 * Почему формула именно такая — читайте в википедии 
	 * http://ru.wikipedia.org/wiki/Оттенки_серого
	 */

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
}