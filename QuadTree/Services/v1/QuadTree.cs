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
		public Dictionary<Agent, QuadTreeNode> AgentToNodeLookup { get; private set; } = new Dictionary<Agent, QuadTreeNode>();

		public QuadTreeNode RootNode { get; private set; }
		readonly QuadTreePool pool;
		readonly Queue<Agent> updateRemoveQueueBuffer = new Queue<Agent>();
		readonly List<Agent> tempBuffer = new List<Agent>();

		public QuadTree(WorldPosition position, Size size, int poolSize, int nodeCapacity, int maxDepth)
		{
			pool = new QuadTreePool(this, poolSize, nodeCapacity, maxDepth);
			RootNode = pool.Get(position, size.Width * 0.5, 0, null);
		}

		public void Add(Agent agent) => RootNode.AddObject(agent);

		public void RemoveObject(Agent agent)
		{
			AgentToNodeLookup[agent].RemoveObject(agent);
			AgentToNodeLookup.Remove(agent);
		}

		public void Update()
		{
			foreach (var item in AgentToNodeLookup)
			{
				var agent = item.Key;
				var tree = item.Value;

				if (!tree.Quad.Contains(agent.position.ToWorld()))
				{
					updateRemoveQueueBuffer.Enqueue(agent);
				}
			}

			while (updateRemoveQueueBuffer.Count > 0)
			{
				var agent = updateRemoveQueueBuffer.Dequeue();
				AgentToNodeLookup[agent].RemoveObject(agent);
				RootNode.AddObject(agent);
			}
		}

		public void RangeScan(WorldPosition position, double radius, List<Agent> buffer)
		{
			buffer.Clear();

			RootNode.RangeScanQuads(position, radius, buffer);

			var sqrRadius = radius * radius;

			for (int i = buffer.Count - 1; i >= 0; i--)
			{
				var agent = buffer[i];

				if (WorldPosition.DistanceSquare(position, agent.position.ToWorld()) > sqrRadius)
				{
					buffer.Remove(agent);
				}
			}
		}

		public void Initialize() { }

		public void Query(Agent agent, float radius, Dictionary<int, Agent> allAgents, HashSet<Agent> buffer)
		{
			tempBuffer.Clear();
			RangeScan(agent.position.ToWorld(), radius, tempBuffer);
			foreach (var temp in tempBuffer)
			{
				buffer.Add(temp);
			}
		}

		public void Insert(Agent agent) => Add(agent);
	}
}
