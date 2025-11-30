using System.Drawing;
using System.Runtime.CompilerServices;
using Avalonia.Media;
using Geometry;
using Color = System.Drawing.Color;

// namespace можно оставить, если требуется
namespace GeometryPainting
{
    public static class SegmentExtensions
    {
        private static readonly ConditionalWeakTable<Segment, ColorHolder> table =
            new ConditionalWeakTable<Segment, ColorHolder>();

        private class ColorHolder
        {
            public Color Color = Color.Black; // Цвет по умолчанию
        }

        // Установка цвета
        public static void SetColor(this Segment segment, Color color)
        {
            table.GetOrCreateValue(segment).Color = color;
        }

        // Получение цвета
        public static Color GetColor(this Segment segment)
        {
            if (table.TryGetValue(segment, out var holder))
                return holder.Color;

            return Color.Black;
        }
    }
}
