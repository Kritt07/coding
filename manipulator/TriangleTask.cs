using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Manipulation;

public class TriangleTask
{
    /// <summary>
    /// Возвращает угол (в радианах) между сторонами a и b в треугольнике со сторонами a, b, c 
    /// </summary>
    public static double GetABAngle(double a, double b, double c)
    {
        // Проверка на отрицательные длины и нулевые стороны
        if (a < 0 || b < 0 || c < 0)
        {
            return double.NaN;
        }
        
        // Проверка на вырожденный треугольник (нулевые стороны)
        if (a == 0 || b == 0)
        {
            // Если обе стороны нулевые и третья тоже - особый вырожденный случай
            if (a == 0 && b == 0 && c == 0)
                return 0; // Все точки совпадают - угол 0 градусов
            else if (a == 0 && b == 0)
                return double.NaN; // Невозможно определить угол
            
            // Если одна из сторон нулевая
            if (a == 0 && b > 0 && c == b) return 0; // Вырожденный в отрезок
            if (b == 0 && a > 0 && c == a) return 0; // Вырожденный в отрезок
            
            return double.NaN;
        }

        // нахождение угла при помощи теоремы косинусов
        var angle = Math.Acos((a * a + b * b - c * c) / (2 * a * b));
        return angle;
    }
}

[TestFixture]
public class TriangleTask_Tests
{
    [TestCase(3, 4, 5, Math.PI / 2)]
    [TestCase(1, 1, 1, Math.PI / 3)]
    [TestCase(150, 120, 60, 0.3897607327974747)]
    [TestCase(3, 4, 5, 1.5707963267948966)]
    [TestCase(1, 1, 2, Math.PI)]
    [TestCase(2, 1, 1, 0)]
    [TestCase(1, 2, 1, 0)]
    [TestCase(1, 1, 2.001, double.NaN)]
    [TestCase(1, 2.001, 1, double.NaN)]
    [TestCase(2.001, 1, 1, double.NaN)]
    [TestCase(0, 5, 5, double.NaN)]
    [TestCase(5, 0, 5, double.NaN)]
    [TestCase(5, 5, 0, 0)]
    [TestCase(-3, -2, -4, double.NaN)]
    [TestCase(3, 4, 5, 1.5707963267948966d)]
    [TestCase(1, 1, 2, 3.141592653589793d)]
    public void TestGetABAngle(double a, double b, double c, double expectedAngle)
    {
        var actual = TriangleTask.GetABAngle(a, b, c);
        if (double.IsNaN(expectedAngle))
            Assert.That(actual, Is.NaN);
        else
            ClassicAssert.AreEqual(expectedAngle, actual, 1e-10);
    }
}