using System;

namespace qt_benchmark.QuadTree.Services.v1
{
	public struct WorldPosition
	{
		public const double DegToRad = Math.PI / 180;
		public const double RadToDeg = 180 / Math.PI;
		public double X { get; private set; }
		public double Y { get; private set; }

		public WorldPosition(double x, double y)
		{
			X = x;
			Y = y;
		}

		public GridPosition ToGrid()
		{
			var roundedX = Math.Round(X);
			var roundedY = Math.Round(Y);
			return new GridPosition(Convert.ToInt32(roundedX), Convert.ToInt32(roundedY));
		}

		public void Normalize()
		{
			var length = Length();
			if (length < double.Epsilon) return;
			X /= length;
			Y /= length;
		}

		public WorldPosition Normalized()
		{
			var result = new WorldPosition(X, Y);
			result.Normalize();
			return result;
		}
		public double Length() => Math.Sqrt(LengthSquare());
		public double LengthSquare()
		{
			var x = Math.Abs(X);
			var y = Math.Abs(Y);

			return x * x + y * y;
		}
		public static WorldPosition Zero => new WorldPosition(0, 0);
		public static double Dot(WorldPosition a, WorldPosition b) => a.X * b.X + a.Y * b.Y;
		public static double Angle(WorldPosition a, WorldPosition b) => Math.Acos(Dot(a.Normalized(), b.Normalized()));
		public static WorldPosition Rotate(WorldPosition a, double radians)
		{
			var ca = Math.Cos(radians);
			var sa = Math.Sin(radians);
			var x = ca * a.X - sa * a.Y;
			var y = sa * a.X + ca * a.Y;
			return new WorldPosition(x, y);
		}
		public static WorldPosition Lerp(WorldPosition a, WorldPosition b, double by) => new WorldPosition(a.X + (b.X - a.X) * by, a.Y + (b.Y - a.Y) * by);
		public static double Distance(WorldPosition a, WorldPosition b) => (a - b).Length();
		public static double DistanceSquare(WorldPosition a, WorldPosition b) => (a - b).LengthSquare();

		public override bool Equals(object obj)
		{
			return obj is WorldPosition position &&
				   X == position.X &&
				   Y == position.Y;
		}

		public override int GetHashCode()
		{
			int hashCode = 1861411795;
			hashCode = hashCode * -1521134295 + X.GetHashCode();
			hashCode = hashCode * -1521134295 + Y.GetHashCode();
			return hashCode;
		}

		public override string ToString() => $"{X:0.00};{Y:0.00}";

		public static WorldPosition operator +(WorldPosition a, WorldPosition b) => new WorldPosition(a.X + b.X, a.Y + b.Y);
		public static WorldPosition operator -(WorldPosition a) => new WorldPosition(-a.X, -a.Y);
		public static WorldPosition operator -(WorldPosition a, WorldPosition b) => a + (-b);
		public static WorldPosition operator *(WorldPosition a, double b) => new WorldPosition(a.X * b, a.Y * b);

	}
}
