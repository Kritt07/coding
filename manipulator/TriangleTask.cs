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
        // Проверка на отрицательные или NaN значения
        if (a < 0 || b < 0 || c < 0 || double.IsNaN(a) || double.IsNaN(b) || double.IsNaN(c))
            return double.NaN;

        // Проверка неравенства треугольника (с учетом возможных очень малых погрешностей)
        if (a + b < c || a + c < b || b + c < a)
            return double.NaN;

        // Обработка вырожденных случаев, когда хотя бы одна сторона нулевая
        if (a == 0 || b == 0)
        {
            // Если a и b равны 0, то угол не определён, но c должно быть 0
            if (a == 0 && b == 0)
                return c == 0 ? 0 : double.NaN;

            // Одна из сторон a или b равна 0, тогда треугольник вырождается
            // Угол между сторонами равен 0, если оставшиеся две стороны равны (и сумма равна третьей)
            if (a == 0 && b == c) return 0;
            if (b == 0 && a == c) return 0;
            return double.NaN;
        }

        // При c == 0 стороны a и b должны совпадать (вырожденный "треугольник")
        if (c == 0)
            return a == b ? 0 : double.NaN;

        // Вычисление косинуса угла по теореме косинусов
        double cosValue = (a * a + b * b - c * c) / (2 * a * b);

        // Защита от выхода за пределы [-1, 1] из-за погрешностей округления
        cosValue = Math.Clamp(cosValue, -1.0, 1.0);

        return Math.Acos(cosValue);
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