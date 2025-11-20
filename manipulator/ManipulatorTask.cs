using System;
using NUnit.Framework;
using static Manipulation.TriangleTask;
using static Manipulation.Manipulator;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;

namespace Manipulation;

public static class ManipulatorTask
{
	/// <summary>
	/// Возвращает массив углов (shoulder, elbow, wrist),
	/// необходимых для приведения эффектора манипулятора в точку x и y 
	/// с углом между последним суставом и горизонталью, равному alpha (в радианах)
	/// См. чертеж manipulator.png!
	/// </summary>
	public struct Point
	{
		public double X { get; }
		public double Y { get; }
		
		public Point(double x, double y)
		{
			X = x;
			Y = y;
		}
	}
	
	public static double[] MoveManipulatorTo(double x, double y, double alpha)
	{
		// для удобства можно обозначить точки так: A, B, C, D
		// которые идут последовательно от shoulder к alpha
		var a = new Point(0, 0);
		var d = new Point(x, y);

		// нахождение координат точки C
		var cx = x - Math.Cos(alpha) * Palm;
		var cy = y - Math.Sin(alpha) * Palm;
		Point c = new Point(cx, cy);

        var ac = Distance(a, c);
		var elbow = GetABAngle(UpperArm, Forearm, ac);

		// Шаг 1: Находим проекцию AH
        var ah = (UpperArm * UpperArm + ac * ac - Forearm * Forearm) / (2 * ac);
        
        // Шаг 2: Находим длину высоты BH
        var bh = Math.Sqrt(UpperArm * UpperArm - ah * ah);

		// Шаг 3: Находим координаты точки H
		var ratio = ah / ac;
        var hx = a.X + ratio * (c.X - a.X);
        var hy = a.Y + ratio * (c.Y - a.Y);
		var h = new Point(hx, hy);

		// Шаг 4: Находим два возможных положения точки B
        // Перпендикулярный вектор (нормализованный)
        
		var solutions = GetPointsB(a, c, h, ac, bh);
		var b = SelectCorrectSolution(solutions, a, c, elbow);

		var bd = Distance(b, d);
		var wrist = GetABAngle(Forearm, Palm, bd);

		var q = new Point(cx, 0);
		var cq = Distance(c, q);

		var shoulderFirstPart = GetABAngle(UpperArm, ac, Forearm);
		var shoulder = shoulderFirstPart + Math.Asin(cq / UpperArm);

		return new[] { shoulder, elbow, wrist };
	}

	public static double Distance(Point p1, Point p2)
    {
        double dx = p2.X - p1.X;
        double dy = p2.Y - p1.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

	public static List<Point> GetPointsB(Point a, Point c, Point h, double ac, double bh)
    {
		var solutions = new List<Point>();

        var dx = c.X - a.X;
        var dy = c.Y - a.Y;
        
        var perpendicularX = -dy / ac;
        var perpendicularY = dx / ac;
        
        // Первое решение
        var b1x = h.X + bh * perpendicularX;
        var b1y = h.Y + bh * perpendicularY;
        solutions.Add(new Point(b1x, b1y));
        
        // Второе решение
        var b2x = h.X - bh * perpendicularX;
        var b2y = h.Y - bh * perpendicularY;
        solutions.Add(new Point(b2x, b2y));

		return solutions;
    }

	public static double CalculateAngleB(Point a, Point b, Point c)
    {
        // Вектора BA и BC
        double baX = a.X - b.X;
        double baY = a.Y - b.Y;
        double bcX = c.X - b.X;
        double bcY = c.Y - b.Y;
        
        // Скалярное произведение
        double dotProduct = baX * bcX + baY * bcY;
        
        // Длины векторов
        double lengthBA = Math.Sqrt(baX * baX + baY * baY);
        double lengthBC = Math.Sqrt(bcX * bcX + bcY * bcY);
        
        // Косинус угла
        double cosAngle = dotProduct / (lengthBA * lengthBC);
        
        // Ограничиваем значение косинуса для избежания ошибок округления
        cosAngle = Math.Max(-1.0, Math.Min(1.0, cosAngle));
        
        // Возвращаем угол в градусах
        return Math.Acos(cosAngle);
    }

	public static Point SelectCorrectSolution(List<Point> solutions, Point a, Point c, double elbow, double tolerance = 1.0)
    {
        foreach (var candidateB in solutions)
        {
			var expectedAngleB = CalculateAngleB(a, candidateB, c);
            if (Math.Abs(elbow - expectedAngleB) <= tolerance)
                return candidateB;
        }

        // Если ни одно решение не подходит, возвращаем первое
        return solutions[0];
    }
}

[TestFixture]
public class ManipulatorTask_Tests
{
	[Test]
	public void TestMoveManipulatorTo()
	{
		Assert.Fail("Write randomized test here!");
	}
}