using System.Drawing;

namespace Recognizer;

public class Pixel(byte r, byte g, byte b)
{
	public Pixel(Color color) : this(color.R, color.G, color.B)
	{
	}

	public byte R { get; } = r;
	public byte G { get; } = g;
	public byte B { get; } = b;

	public override string ToString()
	{
		return $"Pixel({R}, {G}, {B})";
	}
}