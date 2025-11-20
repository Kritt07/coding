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

        var elbowPos = GetNextPoint(angle1, UpperArm, new Point(0, 0));
        var wristPos = GetNextPoint(angle2, Forearm, elbowPos);
        var palmEndPos = GetNextPoint(angle3, Palm, wristPos);
        return new [] { elbowPos, wristPos, palmEndPos };
    }

    public static Point GetNextPoint(double angle, float length, Point startPoint)
    {
        var x = (float)Math.Cos(angle) * length + startPoint.X;
        var y = (float)Math.Sin(angle) * length + startPoint.Y;
        return new Point(x, y);
    }
}

[TestFixture]
public class AnglesToCoordinatesTask_Tests
{
    [TestCase(Math.PI / 2, Math.PI, 3 * Math.PI, 0, Forearm + UpperArm + Palm)] 
    [TestCase(Math.PI / 2, Math.PI / 2, Math.PI / 2, Forearm, UpperArm - Palm)] 
    [TestCase(Math.PI / 2, Math.PI / 2, Math.PI, Forearm + Palm, UpperArm)]     
    [TestCase(Math.PI / 2, 3 * Math.PI / 2, 3 * Math.PI / 2, -Forearm, UpperArm - Palm)] 

    public void TestGetJointPositions(double shoulder, double elbow, double wrist, double palmEndX, double palmEndY)
    {
        var joints = AnglesToCoordinatesTask.GetJointPositions(shoulder, elbow, wrist);
        
        Assert.That(joints[2].X, Is.EqualTo(palmEndX).Within(1e-5), "palm endX");
        Assert.That(joints[2].Y, Is.EqualTo(palmEndY).Within(1e-5), "palm endY");
        Assert.That(GetDistance(joints[0], new Point(0, 0)), Is.EqualTo(UpperArm).Within(1e-5));
        Assert.That(GetDistance(joints[0], joints[1]), Is.EqualTo(Forearm).Within(1e-5));
        Assert.That(GetDistance(joints[1], joints[2]), Is.EqualTo(Palm).Within(1e-5));
    }

    public float GetDistance(Point a, Point b)
    {
        var differenceX = (a.X - b.X) * (a.X - b.X);
        var differenceY = (a.Y - b.Y) * (a.Y - b.Y);
        return (float)Math.Sqrt(differenceX + differenceY);
    }
}