using System;
using Avalonia.Media;
using RefactorMe.Common;

namespace RefactorMe
{
    class TurtleGraphics
    {
        static float currentX, currentY;
        static IGraphics? graphicsEngine;

        public static void Initialize(IGraphics newGraphics)
        {
            graphicsEngine = newGraphics;
            //graphics.SmoothingMode = SmoothingMode.None;
            graphicsEngine.Clear(Colors.Black);
        }

        public static void SetPosition(float x0, float y0)
        { currentX = x0; currentY = y0; }

        public static void DrawStep(Pen pen, double length, double angle)
        {
            //Делает шаг длиной length в направлении angle и рисует пройденную траекторию
            var x1 = (float)(currentX + length * Math.Cos(angle));
            var y1 = (float)(currentY + length * Math.Sin(angle));
            graphicsEngine.DrawLine(pen, currentX, currentY, x1, y1);
            currentX = x1;
            currentY = y1;
        }

        public static void Move(double length, double angle)
        {
            currentX = (float)(currentX + length * Math.Cos(angle));
            currentY = (float)(currentY + length * Math.Sin(angle));
        }
    }

    public class ImpossibleSquare
    {
        static double sideLength;
        static double gapThickness;
        static double diagonalStepLength;
        static Pen yellowPen;

        public static void Draw(int width, int height, double rotationAngle, IGraphics graphics)
        {
            // rotationAngle пока не используется, но будет использоваться в будущем
            TurtleGraphics.Initialize(graphics);

            var size = Math.Min(width, height);

            // Common proportions
            sideLength = size * 0.375f;
            gapThickness = size * 0.04f;
            diagonalStepLength = gapThickness * Math.Sqrt(2);
            yellowPen = new Pen(Brushes.Yellow);

            var diagonalLength = Math.Sqrt(2) * (sideLength + gapThickness) / 2;
            var x0 = (float)(diagonalLength * Math.Cos(Math.PI / 4 + Math.PI)) + width / 2f;
            var y0 = (float)(diagonalLength * Math.Sin(Math.PI / 4 + Math.PI)) + height / 2f;

            TurtleGraphics.SetPosition(x0, y0);

            // Draw all four sides with transitions
            DrawSideAndTransition(0);
            DrawSideAndTransition(-Math.PI / 2);
            DrawSideAndTransition(Math.PI);
            DrawSideAndTransition(Math.PI / 2);
        }

        static void DrawSideAndTransition(double baseAngle)
        {
            TurtleGraphics.DrawStep(yellowPen, sideLength, baseAngle);
            TurtleGraphics.DrawStep(yellowPen, diagonalStepLength, baseAngle + Math.PI / 4);
            TurtleGraphics.DrawStep(yellowPen, sideLength, baseAngle + Math.PI);
            TurtleGraphics.DrawStep(yellowPen, sideLength - gapThickness, baseAngle + Math.PI / 2);

            TurtleGraphics.Move(gapThickness, baseAngle + Math.PI);
            TurtleGraphics.Move(diagonalStepLength, baseAngle + 3 * Math.PI / 4);
        }
    }
}