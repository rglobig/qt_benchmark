using System.Numerics;

namespace qt_benchmark.QuadTree.Services.v1
{
	static class Helper
	{
		public static WorldPosition ToWorld(this Vector2 vector2)
			=> new WorldPosition(vector2.X, vector2.Y);
	}
}
