namespace qt_benchmark.QuadTree.Services.v1
{
	public struct Size
	{
		public int Width { get; }
		public int Height { get; }

		public Size(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public Size OnlyWidth() => new Size(Width, 0);
		public Size OnlyHeight() => new Size(0, Height);
		public static Size Zero => new Size(0, 0);
		public static Size Invalid => new Size(-1, -1);
		public static Size operator +(Size a, Size b) => new Size(a.Width + b.Width, a.Height + b.Height);
		public static bool operator ==(Size a, Size b) => a.Width == b.Width && a.Height == b.Height;
		public static bool operator !=(Size a, Size b) => !(a == b);
		public override bool Equals(object obj)
		{
			return obj is Size position &&
				   Width == position.Width &&
				   Height == position.Height;
		}
		public override int GetHashCode()
		{
			int hashCode = 1861411795;
			hashCode = hashCode * -1521134295 + Width.GetHashCode();
			hashCode = hashCode * -1521134295 + Height.GetHashCode();
			return hashCode;
		}
	}
}
