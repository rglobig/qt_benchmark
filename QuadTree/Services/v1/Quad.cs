using System;

namespace qt_benchmark.QuadTree.Services.v1
{
	public struct Quad
	{
		public WorldPosition Center { get; private set; }
		public WorldPosition TopRight { get; private set; }
		public WorldPosition TopLeft { get; private set; }
		public WorldPosition BottomRight { get; private set; }
		public WorldPosition BottomLeft { get; private set; }
		public double HalfDimension { get; private set; }
		double Size { get;  set; }

		/// <summary>
		/// Creates a quad from the center with the radius in all directions.
		/// </summary>
		public Quad(WorldPosition center, double radius)
		{
			Center = center;
			Size = radius * 2;

			HalfDimension = radius;
			TopRight = new WorldPosition(center.X + HalfDimension, center.Y + HalfDimension);
			TopLeft = new WorldPosition(center.X - HalfDimension, center.Y + HalfDimension);
			BottomRight = new WorldPosition(center.X + HalfDimension, center.Y - HalfDimension);
			BottomLeft = new WorldPosition(center.X - HalfDimension, center.Y - HalfDimension);
		}

		public bool Intersect(Quad quad)
		{
			bool val;

			var val1 = (Math.Abs(Center.X - quad.Center.X) < Size * 0.5 + quad.Size * 0.5);
			var val2 = (Math.Abs(Center.Y - quad.Center.Y) < Size * 0.5 + quad.Size * 0.5);

			val = val1 && val2;

			return val;
		}
		public bool Contains(WorldPosition pos)
		{
			return (BottomLeft.X <= pos.X && pos.X <= TopRight.X) && (BottomLeft.Y <= pos.Y && pos.Y <= TopRight.Y);
		}
	}
}
