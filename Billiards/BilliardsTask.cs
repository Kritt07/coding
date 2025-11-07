using Avalonia.OpenGL.Angle;

namespace Billiards;

public static class BilliardsTask
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="directionRadians">Угол направления движения шара</param>
    /// <param name="wallInclinationRadians">Угол</param>
    /// <returns></returns>
    public static double BounceWall(double directionRadians, double wallInclinationRadians)
    {
        //TODO
        var angle = (wallInclinationRadians * 2.0) - directionRadians;
        return angle;
    }
}