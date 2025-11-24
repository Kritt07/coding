using System;
using System.Globalization;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;

namespace Manipulation;

public static class VisualizerTask
{
	public static double X = 220;
	public static double Y = -100;
	public static double Alpha = 0.05;
	public static double Wrist = 2 * Math.PI / 3;
	public static double Elbow = 3 * Math.PI / 4;
	public static double Shoulder = Math.PI / 2;

	public static Brush UnreachableAreaBrush = new SolidColorBrush(Color.FromArgb(255, 255, 230, 230));
	public static Brush ReachableAreaBrush = new SolidColorBrush(Color.FromArgb(255, 230, 255, 230));
	public static Pen ManipulatorPen = new Pen(Brushes.Black, 3);
	public static Brush JointBrush = new SolidColorBrush(Colors.Gray);

    public static void KeyDown(Visual visual, KeyEventArgs key)
    {
        const double delta = 0.05;

        switch (key.Key)
        {
            case Key.Q:
                Shoulder += delta;
                break;

            case Key.A:
                Shoulder -= delta;
                break;

            case Key.W:
                Elbow += delta;
                break;

            case Key.S:
                Elbow -= delta;
                break;

            default:
                return; // Ничего не делаем — не наша клавиша
        }

        // Пересчитываем Wrist
        Wrist = -Alpha - Shoulder - Elbow;

        visual.InvalidateVisual();
    }

    public static void MouseMove(Visual visual, PointerEventArgs e)
    {
        // Получаем координаты мыши в системе окна
        var windowPos = e.GetPosition(visual);

        // Позиция плечевого сустава в окне
        var shoulderPos = GetShoulderPos(visual);

        // Переводим координаты в логические (математические)
        var mathPos = ConvertWindowToMath(windowPos, shoulderPos);

        // Сохраняем логические координаты мыши
        X = mathPos.X;
        Y = mathPos.Y;

        // Пересчитываем углы манипулятора
        UpdateManipulator();

        // Перерисовываем
        visual.InvalidateVisual();
    }

    public static void MouseWheel(Visual visual, PointerWheelEventArgs e)
    {
        const double delta = 0.05;

        // Изменяем Alpha пропорционально прокрутке колеса
        Alpha += e.Delta.Y * delta;

        // Пересчитываем манипулятор
        UpdateManipulator();

        // Перерисовка
        visual.InvalidateVisual();
    }

    public static void UpdateManipulator()
    {
        // Пытаемся найти углы для текущих X, Y, Alpha
        var angles = ManipulatorTask.MoveManipulatorTo(X, Y, Alpha);

        // Если хоть один угол стал NaN — манипулятор не может дотянуться,
        // и тогда мы НЕ обновляем суставы (манипулятор "замирает")
        if (double.IsNaN(angles[0]) ||
            double.IsNaN(angles[1]) ||
            double.IsNaN(angles[2]))
            return;

        // Если решение есть — обновляем углы суставов
        Shoulder = angles[0];
        Elbow = angles[1];
        Wrist = angles[2];
    }

    public static void DrawManipulator(DrawingContext context, Point shoulderPos)
    {
        var joints = AnglesToCoordinatesTask.GetJointPositions(Shoulder, Elbow, Wrist);

        DrawReachableZone(context, ReachableAreaBrush, UnreachableAreaBrush, shoulderPos, joints);

        var formattedText = new FormattedText(
            $"X={X:0}, Y={Y:0}, Alpha={Alpha:0.00}",
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            Typeface.Default,
            18,
            Brushes.DarkRed
        )
        {
            TextAlignment = TextAlignment.Left
        };
        context.DrawText(formattedText, new Point(10, 10));

        // Переводим логические координаты суставов в оконные
        var shoulderWindow = shoulderPos;
        var elbowWindow = ConvertMathToWindow(joints[0], shoulderPos);
        var wristWindow = ConvertMathToWindow(joints[1], shoulderPos);
        var effectorWindow = ConvertMathToWindow(joints[2], shoulderPos);

        // Рисуем сегменты манипулятора
        context.DrawLine(ManipulatorPen, shoulderWindow, elbowWindow);
        context.DrawLine(ManipulatorPen, elbowWindow, wristWindow);
        context.DrawLine(ManipulatorPen, wristWindow, effectorWindow);

        // Рисуем суставы (небольшие кружки, например радиус 5)
        const double r = 5;
        context.DrawEllipse(JointBrush, null, shoulderWindow, r, r);
        context.DrawEllipse(JointBrush, null, elbowWindow, r, r);
        context.DrawEllipse(JointBrush, null, wristWindow, r, r);
        context.DrawEllipse(JointBrush, null, effectorWindow, r, r);
    }

    private static void DrawReachableZone(
		DrawingContext context,
		Brush reachableBrush,
		Brush unreachableBrush,
		Point shoulderPos,
		Point[] joints)
	{
		var rmin = Math.Abs(Manipulator.UpperArm - Manipulator.Forearm);
		var rmax = Manipulator.UpperArm + Manipulator.Forearm;
		var mathCenter = new Point(joints[2].X - joints[1].X, joints[2].Y - joints[1].Y);
		var windowCenter = ConvertMathToWindow(mathCenter, shoulderPos);
		context.DrawEllipse(reachableBrush,
			null,
			new Point(windowCenter.X, windowCenter.Y),
			rmax, rmax);
		context.DrawEllipse(unreachableBrush,
			null,
			new Point(windowCenter.X, windowCenter.Y),
			rmin, rmin);
	}

	public static Point GetShoulderPos(Visual visual)
	{
		return new Point(visual.Bounds.Width / 2, visual.Bounds.Height / 2);
	}

	public static Point ConvertMathToWindow(Point mathPoint, Point shoulderPos)
	{
		return new Point(mathPoint.X + shoulderPos.X, shoulderPos.Y - mathPoint.Y);
	}

	public static Point ConvertWindowToMath(Point windowPoint, Point shoulderPos)
	{
		return new Point(windowPoint.X - shoulderPos.X, shoulderPos.Y - windowPoint.Y);
	}
}