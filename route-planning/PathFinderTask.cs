using System.Drawing;
using Avalonia.Controls;

namespace RoutePlanning;

public static class PathFinderTask
{
    private static int[] bestOrder;
    private static double minLength = double.MaxValue;

	public static int[] FindBestCheckpointsOrder(Point[] checkpoints)
	{
		bestOrder = MakeTrivialPermutation(checkpoints.Length);
		minLength = checkpoints.GetPathLength(bestOrder);

		var currentOrder = new int[checkpoints.Length];
		currentOrder[0] = 0;

		GeneratePermutations(checkpoints, currentOrder, 1, 0.0);

		return bestOrder;
	}

	private static void GeneratePermutations(Point[] checkpoints, int[] currentOrder, int position, double currentLength)
	{
		if (position == currentOrder.Length)
		{
			if (currentLength < minLength)
			{
				minLength = currentLength;
				currentOrder.CopyTo(bestOrder, 0);
			}
			return;
		}

		for (int i = 1; i < currentOrder.Length; i++)
		{
			if (CheckPointIsFound(currentOrder, position, i))
				continue;

			currentOrder[position] = i;
			double newLength = CalculateNewLength(checkpoints, currentOrder, position, currentLength);

			if (newLength < minLength)
				GeneratePermutations(checkpoints, currentOrder, position + 1, newLength);
		}
	}

	private static bool CheckPointIsFound(int[] currentOrder, int position, int i)
	{
		for (int j = 0; j < position; j++)
			if (currentOrder[j] == i)
				return true;
		return false;
	}
	
	private static double CalculateNewLength(Point[] checkpoints, int[] currentOrder, int position, double currentLength)
    {
        Point previousPoint = checkpoints[currentOrder[position - 1]];
		Point nextPoint = checkpoints[currentOrder[position]];
		return currentLength + previousPoint.DistanceTo(nextPoint);
    }
	
    private static int[] MakeTrivialPermutation(int size)
    {
        var bestOrder = new int[size];
        for (var i = 0; i < bestOrder.Length; i++)
            bestOrder[i] = i;
        return bestOrder;
    }
}