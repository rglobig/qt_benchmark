using System.Collections.Generic;

namespace qt_benchmark.QuadTree.Services.v1
{
	public sealed class QuadTreeNode
	{
		public Quad Quad { get; private set; }

		public QuadTreeNode TopRight { get; private set; }
		public QuadTreeNode TopLeft { get; private set; }
		public QuadTreeNode BottomRight { get; private set; }
		public QuadTreeNode BottomLeft { get; private set; }
		public List<Agent> Agents { get; private set; }

		int CurrentHeight;

		//is null when this node is the root
		QuadTreeNode parent;

		readonly List<Agent> mergeBuffer;
		readonly int capacity;
		readonly int maxDepth;
		readonly QuadTree quadTree;
		readonly QuadTreePool pool;

		public QuadTreeNode(QuadTree quadTree, QuadTreePool pool, int capacity, int maxDepth)
		{
			this.quadTree = quadTree;
			this.pool = pool;
			this.capacity = capacity;
			this.maxDepth = maxDepth;
			Agents = new List<Agent>(capacity);
			mergeBuffer = new List<Agent>();
		}

		public void SetUp(WorldPosition position, double halfRange, int height, QuadTreeNode parent)
		{
			Quad = new Quad(position, halfRange);
			CurrentHeight = height;
			this.parent = parent;
			Agents.Clear();
		}

		public void AddObject(Agent agent)
		{

			if (HasChildren())
				AddObjectToSubTrees(agent);
			else
			{
				Agents.Add(agent);

				quadTree.AgentToNodeLookup[agent] = this;

				if (Agents.Count > capacity && CurrentHeight <= maxDepth)
					SplitUp();
			}
		}
		public void SplitUp()
		{
			TopRight = pool.Get(WorldPosition.Lerp(Quad.Center, Quad.TopRight, 0.5), Quad.HalfDimension * 0.5, CurrentHeight + 1, this);
			TopLeft = pool.Get(WorldPosition.Lerp(Quad.Center, Quad.TopLeft, 0.5), Quad.HalfDimension * 0.5, CurrentHeight + 1, this);
			BottomRight = pool.Get(WorldPosition.Lerp(Quad.Center, Quad.BottomRight, 0.5), Quad.HalfDimension * 0.5, CurrentHeight + 1, this);
			BottomLeft = pool.Get(WorldPosition.Lerp(Quad.Center, Quad.BottomLeft, 0.5), Quad.HalfDimension * 0.5, CurrentHeight + 1, this);

			for (int i = Agents.Count - 1; i >= 0; i--)
			{
				AddObjectToSubTrees(Agents[i]);
				Agents.Remove(Agents[i]);
			}
		}

		public void RemoveObject(Agent agent)
		{
			Agents.Remove(agent);
			parent?.MergeChilds();
		}
		public void RangeScanQuads(WorldPosition position, double range, List<Agent> buffer)
		{
			if (new Quad(position, range).Intersect(Quad))
			{
				buffer.AddRange(Agents);
				TopRight?.RangeScanQuads(position, range, buffer);
				TopLeft?.RangeScanQuads(position, range, buffer);
				BottomRight?.RangeScanQuads(position, range, buffer);
				BottomLeft?.RangeScanQuads(position, range, buffer);
			}
		}

		bool HasChildren() => TopRight != null || TopLeft != null || BottomRight != null || BottomLeft != null;

		/// <summary>
		/// help method to sort units in the right subtree based on thier position in the quad
		/// </summary>
		void AddObjectToSubTrees(Agent agent)
		{
			var pos = agent.position.ToWorld();

			if (pos.X < Quad.Center.X)
			{
				if (pos.Y < Quad.Center.Y)
				{
					BottomLeft.AddObject(agent);
				}
				else
				{
					TopLeft.AddObject(agent);
				}
			}
			else
			{
				if (pos.Y < Quad.Center.Y)
				{
					BottomRight.AddObject(agent);
				}
				else
				{
					TopRight.AddObject(agent);
				}
			}
		}

		void MergeChilds()
		{
			if (!HasChildren())
				return;

			mergeBuffer.Clear();

			GetAgentsInChildrenAll(mergeBuffer);

			if (mergeBuffer.Count <= capacity)
			{
				Agents.AddRange(mergeBuffer);

				foreach (var item in Agents)
				{
					quadTree.AgentToNodeLookup[item] = this;
				}

				pool.Return(TopRight);
				pool.Return(TopLeft);
				pool.Return(BottomRight);
				pool.Return(BottomLeft);

				TopRight = null;
				TopLeft = null;
				BottomRight = null;
				BottomLeft = null;

				parent?.MergeChilds();
			}
		}

		void GetAgentsInChildrenAll(List<Agent> buffer)
		{
			if (TopRight.HasChildren())
				TopRight.GetAgentsInChildrenAll(buffer);
			else
				buffer.AddRange(TopRight.Agents);

			if (TopLeft.HasChildren())
				TopLeft.GetAgentsInChildrenAll(buffer);
			else
				buffer.AddRange(TopLeft.Agents);

			if (BottomRight.HasChildren())
				BottomRight.GetAgentsInChildrenAll(buffer);
			else
				buffer.AddRange(BottomRight.Agents);

			if (BottomLeft.HasChildren())
				BottomLeft.GetAgentsInChildrenAll(buffer);
			else
				buffer.AddRange(BottomLeft.Agents);
		}
	}
}
