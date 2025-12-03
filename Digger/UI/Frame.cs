using System.Collections.Generic;
using System.IO;
using System.Timers;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting;
using Avalonia.Threading;
using Digger.Architecture;

namespace Digger.UI;

public class Frame : UserControl
{
    private readonly Dictionary<string, Bitmap> bitmaps = new();
    private readonly GameState gameState;
    private int tickCount;

    public Frame()
    {
        gameState = new GameState();
        var imagesDirectory = new DirectoryInfo("Images");
        foreach (var e in imagesDirectory.GetFiles("*.png"))
            bitmaps[e.Name] = new Bitmap(e.FullName);
        var timer = new Timer();
        timer.Interval = 15;
        timer.Elapsed += TimerTick;
        timer.Start();
    }

    public override void Render(DrawingContext e)
    {
        var shift = new Vector(0, GameState.ElementSize);
        e.FillRectangle(
            Brushes.Black, new Rect(0, GameState.ElementSize, GameState.ElementSize * Game.MapWidth,
                GameState.ElementSize * Game.MapHeight));
        foreach (var a in gameState.Animations)
            e.DrawImage(bitmaps[a.Creature.GetImageFileName()], new Rect(a.Location + shift,
                new Size(GameState.ElementSize, GameState.ElementSize)));
        var text = Game.Scores.ToString();

        var textLayout = new TextLayout(
	        text,
	        typeface: Typeface.Default,
	        fontSize: 20,
	        foreground: Brushes.Green,
	        textAlignment: TextAlignment.Center,
	        textWrapping: TextWrapping.Wrap,
	        maxWidth: double.PositiveInfinity
        );

        textLayout.Draw(e, new Point(0, 0));
    }

    private void TimerTick(object sender, ElapsedEventArgs args)
    {
	    if (tickCount == 0) 
		    gameState.BeginAct();

	    foreach (var anim in gameState.Animations)
		    anim.Location = new Point(anim.Location.X + 4 * anim.Command.DeltaX, 
			    anim.Location.Y + 4 * anim.Command.DeltaY);

	    if (tickCount == 7)
		    gameState.EndAct();

	    tickCount++;
	    if (tickCount == 8) tickCount = 0;

	    Dispatcher.UIThread.Post(InvalidateVisual);
    }
}