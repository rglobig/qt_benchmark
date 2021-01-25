namespace qt_benchmark.QuadTree.Services.v1
{
	public struct Direction
	{
		public int X { get; }
		public int Y { get; }

		public Direction(int x, int y)
		{
			X = x;
			Y = y;
		}

		public static Direction Zero => new Direction(0, 0);
		public static Direction Right => new Direction(1, 0);
		public static Direction Up => new Direction(0, 1);
		public GridPosition ToGrid() => new GridPosition(X, Y);
		public Direction Inverse() => new Direction(-X, -Y);
		public static bool operator ==(Direction a, Direction b) => a.X == b.X && a.Y == b.Y;
		public static bool operator !=(Direction a, Direction b) => !(a == b);

		public override bool Equals(object obj)
		{
			return obj is Direction direction &&
				   X == direction.X &&
				   Y == direction.Y;
		}

		public override int GetHashCode()
		{
			int hashCode = 1861411795;
			hashCode = hashCode * -1521134295 + X.GetHashCode();
			hashCode = hashCode * -1521134295 + Y.GetHashCode();
			return hashCode;
		}
	}
}
