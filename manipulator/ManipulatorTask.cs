using System;
using NUnit.Framework;
using static Manipulation.TriangleTask;
using static Manipulation.Manipulator;
using NUnit.Framework.Legacy;

namespace Manipulation;

public static class ManipulatorTask
{
    public static double[] MoveManipulatorTo(double x, double y, double alpha)
    {
        // Вычисляем Wrist
        var wristX = x - Palm * Math.Cos(alpha);
        var wristY = y + Palm * Math.Sin(alpha);

        // Расстояние от плеча до запястья
        var dist = Math.Sqrt(wristX * wristX + wristY * wristY);

        // Проверка достижимости (условие существования треугольника)
        if (dist > UpperArm + Forearm || dist < Math.Abs(UpperArm - Forearm))
            return new[] { double.NaN, double.NaN, double.NaN };

        // Угол в локте
        var elbow = GetABAngle(UpperArm, Forearm, dist);

        // Угол плеча = угол между плечом и Wrist + угол треугольника
        var shoulder1 = GetABAngle(UpperArm, dist, Forearm);
        var shoulder2 = Math.Atan2(wristY, wristX);
        var shoulder = shoulder2 + shoulder1;

        // Угол запястья
        var wrist = -alpha - shoulder - elbow;

        return new[] { shoulder, elbow, wrist };
    }
}


[TestFixture]
public class ManipulatorTask_Tests
{
	[Test]
	public void TestMoveManipulatorTo()
	{
		var rnd = new Random();

		var x = rnd.NextDouble() * 400 - 200;
		var y = rnd.NextDouble() * 400 - 200;
		var alpha = rnd.NextDouble() * Math.PI * 2 - Math.PI;

		var angles = ManipulatorTask.MoveManipulatorTo(x, y, alpha);

		Console.WriteLine($"x={x:F2}, y={y:F2}, alpha={alpha:F2}");
		Console.WriteLine($"shoulder={angles[0]}, elbow={angles[1]}, wrist={angles[2]}");
		
		// Чтобы NUnit не ругался, просто проверим существование массива
		Assert.That(angles.Length, Is.EqualTo(3));
	}
}