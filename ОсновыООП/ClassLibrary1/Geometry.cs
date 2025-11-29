using System;

namespace Geometry
{
    public class Vector
    {
        public double X;
        public double Y;
    }

    public class Segment
    {
        public Vector Begin;
        public Vector End;
    }

    public static class Geometry
    {
        // Длина вектора
        public static double GetLength(Vector v)
        {
            return Math.Sqrt(v.X * v.X + v.Y * v.Y);
        }

        // Сложение векторов
        public static Vector Add(Vector a, Vector b)
        {
            return new Vector
            {
                X = a.X + b.X,
                Y = a.Y + b.Y
            };
        }

        // Длина сегмента
        public static double GetLength(Segment s)
        {
            double dx = s.End.X - s.Begin.X;
            double dy = s.End.Y - s.Begin.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Проверка, лежит ли точка p на сегменте s
        public static bool IsVectorInSegment(Vector p, Segment s)
        {
            // Направления Begin→p и Begin→End
            double cross = (p.X - s.Begin.X) * (s.End.Y - s.Begin.Y) -
                           (p.Y - s.Begin.Y) * (s.End.X - s.Begin.X);

            // Если cross ≠ 0 — точка не на линии (учитываем погрешность)
            if (Math.Abs(cross) > 1e-9)
                return false;

            // Проверка, что точка лежит между Begin и End (по диапазонам координат)
            double minX = Math.Min(s.Begin.X, s.End.X);
            double maxX = Math.Max(s.Begin.X, s.End.X);
            double minY = Math.Min(s.Begin.Y, s.End.Y);
            double maxY = Math.Max(s.Begin.Y, s.End.Y);

            return p.X >= minX - 1e-9 && p.X <= maxX + 1e-9 &&
                   p.Y >= minY - 1e-9 && p.Y <= maxY + 1e-9;
        }
    }
}