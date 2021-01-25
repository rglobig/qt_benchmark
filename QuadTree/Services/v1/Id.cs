namespace qt_benchmark.QuadTree.Services.v1
{
	public struct Id
	{
		public int Value { get; private set; }
		public Id(int value) => Value = value;
		public override bool Equals(object obj) => obj is Id id && Value == id.Value;
		public override int GetHashCode() => Value.GetHashCode();
		public static Id Invalid => new Id(-1);
		public static bool operator ==(Id a, Id b) => a.Value == b.Value;
		public static bool operator !=(Id a, Id b) => !(a == b);
		public override string ToString() => Value.ToString();
	}
}
