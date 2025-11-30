using System;

namespace Geometry
{
    public class Vector
    {
        public double X;
        public double Y;

        // Длина текущего вектора
        public double GetLength()
        {
            return Geometry.GetLength(this);
        }

        // Сложение текущего вектора с другим
        public Vector Add(Vector other)
        {
            return Geometry.Add(this, other);
        }

        // Проверка: принадлежит ли текущая точка сегменту
        public bool Belongs(Segment s)
        {
            return Geometry.IsVectorInSegment(this, s);
        }
    }

    public class Segment
    {
        public Vector Begin;
        public Vector End;

        // Длина сегмента
        public double GetLength()
        {
            return Geometry.GetLength(this);
        }

        // Проверка: содержит ли сегмент указанную точку
        public bool Contains(Vector p)
        {
            return Geometry.IsVectorInSegment(p, this);
        }
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

        // Проверка принадлежности точки p отрезку s
        public static bool IsVectorInSegment(Vector p, Segment s)
        {
            // Проверка коллинеарности
            double cross = (p.X - s.Begin.X) * (s.End.Y - s.Begin.Y) -
                           (p.Y - s.Begin.Y) * (s.End.X - s.Begin.X);

            if (Math.Abs(cross) > 1e-9)
                return false;

            // Проверка попадания в диапазоны координат
            double minX = Math.Min(s.Begin.X, s.End.X);
            double maxX = Math.Max(s.Begin.X, s.End.X);
            double minY = Math.Min(s.Begin.Y, s.End.Y);
            double maxY = Math.Max(s.Begin.Y, s.End.Y);

            return p.X >= minX - 1e-9 && p.X <= maxX + 1e-9 &&
                   p.Y >= minY - 1e-9 && p.Y <= maxY + 1e-9;
        }
    }
}
