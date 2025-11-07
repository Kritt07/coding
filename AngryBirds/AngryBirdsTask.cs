using System;

namespace AngryBirds;

public static class AngryBirdsTask
{
    public static double FindSightAngle(double v, double distance)
    {
        const double G = 9.8;

        return Math.Asin((distance * G) / (v * v)) / 2;
    }
}