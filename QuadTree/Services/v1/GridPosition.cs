using System;
using System.Collections.Generic;

namespace qt_benchmark.QuadTree.Services.v1
{
	public struct GridPosition
	{
		public int X { get; }
		public int Y { get; }

		public GridPosition(int x, int y)
		{
			X = x;
			Y = y;
		}

		public static GridPosition Zero => new GridPosition(0, 0);
		public static GridPosition Invalid => new GridPosition(-1, -1);

		public WorldPosition ToWorld() => new WorldPosition(X, Y);
		public Id ToId(int width) => new Id(Y * width + X);
		public static GridPosition FromId(Id id, int width) => new GridPosition(id.Value % width, id.Value / width);
		public static GridPosition operator +(GridPosition a, GridPosition b) => new GridPosition(a.X + b.X, a.Y + b.Y);
		public static GridPosition operator +(GridPosition a, Direction b) => new GridPosition(a.X + b.X, a.Y + b.Y);
		public static GridPosition operator +(GridPosition a, Size b) => new GridPosition(a.X + b.Width, a.Y + b.Height);
		public static GridPosition operator -(GridPosition a, GridPosition b) => new GridPosition(a.X - b.X, a.Y - b.Y);
		public static bool operator ==(GridPosition a, GridPosition b) => a.X == b.X && a.Y == b.Y;
		public static bool operator !=(GridPosition a, GridPosition b) => !(a == b);
		public override bool Equals(object obj)
		{
			return obj is GridPosition position &&
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
		public static double Length(GridPosition a, GridPosition b) => Math.Sqrt(LengthSquare(a, b));
		public static int LengthSquare(GridPosition a, GridPosition b)
		{
			var x = Math.Abs(b.X - a.X);
			var y = Math.Abs(b.Y - a.Y);
			var xSqr = x * x;
			var ySqr = y * y;
			return xSqr + ySqr;
		}

		public override string ToString() => $"{X};{Y}";

		public GridPosition[] GetNeighbors()
		{
			var topLeft = new GridPosition(X - 1, Y + 1);
			var top = new GridPosition(X, Y + 1);
			var topRight = new GridPosition(X + 1, Y + 1);
			var right = new GridPosition(X + 1, Y);
			var bottomRight = new GridPosition(X + 1, Y - 1);
			var bottom = new GridPosition(X, Y - 1);
			var bottomLeft = new GridPosition(X - 1, Y - 1);
			var left = new GridPosition(X - 1, Y);

			var result = new GridPosition[8];

			result[0] = topLeft;
			result[1] = top;
			result[2] = topRight;
			result[3] = right;
			result[4] = bottomRight;
			result[5] = bottom;
			result[6] = bottomLeft;
			result[7] = left;

			return result;
		}

		public void GetNeighbors(List<GridPosition> output, Func<GridPosition, bool> boundsCheck)
		{
			var topLeft = new GridPosition(X - 1, Y + 1);
			var top = new GridPosition(X, Y + 1);
			var topRight = new GridPosition(X + 1, Y + 1);
			var right = new GridPosition(X + 1, Y);
			var bottomRight = new GridPosition(X + 1, Y - 1);
			var bottom = new GridPosition(X, Y - 1);
			var bottomLeft = new GridPosition(X - 1, Y - 1);
			var left = new GridPosition(X - 1, Y);

			if (boundsCheck(topLeft)) output.Add(topLeft);
			if (boundsCheck(top)) output.Add(top);
			if (boundsCheck(topRight)) output.Add(topRight);
			if (boundsCheck(right)) output.Add(right);
			if (boundsCheck(bottomRight)) output.Add(bottomRight);
			if (boundsCheck(bottom)) output.Add(bottom);
			if (boundsCheck(bottomLeft)) output.Add(bottomLeft);
			if (boundsCheck(left)) output.Add(left);
		}
	}
}
