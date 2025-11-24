using System;
using Avalonia;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using static Manipulation.Manipulator;

namespace Manipulation;

public static class AnglesToCoordinatesTask
{
    public static Point[] GetJointPositions(double shoulder, double elbow, double wrist)
    {
		var angle1 = shoulder;
		var angle2 = angle1 + elbow - Math.PI;
		var angle3 = angle2 + wrist - Math.PI;

        var elbowPos = GetPoint(angle1, UpperArm, new Point(0, 0));
        var wristPos = GetPoint(angle2, Forearm, elbowPos);
        var palmEndPos = GetPoint(angle3, Palm, wristPos);
        return new [] { elbowPos, wristPos, palmEndPos };
    }

    public static Point GetPoint(double angle, float length, Point startPoint)
    {
        var x = (float)Math.Cos(angle) * length + startPoint.X;
        var y = (float)Math.Sin(angle) * length + startPoint.Y;
        return new Point(x, y);
    }
}

[TestFixture]
public class AnglesToCoordinatesTask_Tests
{
    // shoulder, elbow, wrist, expected x, expected y
    [TestCase(0, Math.PI, Math.PI, 
        UpperArm + Forearm + Palm, 0)]  // полностью вправо

    [TestCase(Math.PI, Math.PI, Math.PI, 
       -(UpperArm + Forearm + Palm), 0)] // полностью влево

    [TestCase(-Math.PI / 2, Math.PI / 2, Math.PI,
         Forearm + UpperArm - Palm, -(Forearm + Palm))] // вниз + сложные углы
    public void TestPalmEndPosition(
        double shoulder, double elbow, double wrist,
        double expectedX, double expectedY)
    {
        var joints = AnglesToCoordinatesTask.GetJointPositions(shoulder, elbow, wrist);

        Assert.That(joints[2].X, Is.EqualTo(expectedX).Within(1e-5), "Palm X mismatch");
        Assert.That(joints[2].Y, Is.EqualTo(expectedY).Within(1e-5), "Palm Y mismatch");

        // Проверяем длины сегментов
        Assert.That(Distance(new Point(0,0), joints[0]),
            Is.EqualTo(UpperArm).Within(1e-5), "Shoulder to Elbow length wrong");

        Assert.That(Distance(joints[0], joints[1]),
            Is.EqualTo(Forearm).Within(1e-5), "Elbow to Wrist length wrong");

        Assert.That(Distance(joints[1], joints[2]),
            Is.EqualTo(Palm).Within(1e-5), "Wrist to Palm length wrong");
    }

    private static double Distance(Point a, Point b)
    {
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
