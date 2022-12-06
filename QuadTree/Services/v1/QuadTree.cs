using System.Collections.Generic;

namespace qt_benchmark.QuadTree.Services.v1
{
	/// <summary>
	/// this class partitioned the room depending on the nummer of units in a certain area
	/// It splits up more and more depending on the number of units 
	/// This makes it more performant to check distances between units
	/// </summary>
	public sealed class QuadTree : IQuadTreeService
	{
		public QuadTreeNode RootNode { get; private set; }
		readonly QuadTreePool pool;

		public QuadTree(WorldPosition position, Size size, int poolSize, int nodeCapacity, int maxDepth)
		{
			pool = new QuadTreePool(poolSize, nodeCapacity, maxDepth);
			RootNode = pool.Get(position, size.Width * 0.5, 0, null);
		}

		public void Add(Agent agent) => RootNode.AddObject(agent);

		public void Update() { }

		public void RangeScan(WorldPosition position, double radius, HashSet<Agent> buffer)
		{
			buffer.Clear();

			RootNode.RangeScanQuads(position, radius, buffer);

			var sqrRadius = radius * radius;

			buffer.RemoveWhere(agent => WorldPosition.DistanceSquare(position, agent.position.ToWorld()) > sqrRadius);
		}

		public void Initialize() { }

		public void Query(Agent agent, float radius, Dictionary<int, Agent> allAgents, HashSet<Agent> buffer)
		{
			RangeScan(agent.position.ToWorld(), radius, buffer);
		}

		public void Insert(Agent agent) => Add(agent);
	}
}
