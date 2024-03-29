﻿using System.Collections.Generic;

namespace qt_benchmark.QuadTree.Services.v1
{
	public sealed class QuadTreeNode
	{
		public Quad Quad { get; private set; }

		public QuadTreeNode TopRight { get; private set; }
		public QuadTreeNode TopLeft { get; private set; }
		public QuadTreeNode BottomRight { get; private set; }
		public QuadTreeNode BottomLeft { get; private set; }
		public HashSet<Agent> Agents { get; private set; }

		int CurrentHeight;

		//is null when this node is the root
		QuadTreeNode parent;

		readonly HashSet<Agent> mergeBuffer;
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
			Agents = new HashSet<Agent>(capacity);
			mergeBuffer = new HashSet<Agent>();
		}

		public void SetUp(WorldPosition position, double size, int height, QuadTreeNode parent)
		{
			Quad = new Quad(position, size * 0.5);
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
			TopRight = pool.Get(WorldPosition.Lerp(Quad.Center, Quad.TopRight, 0.5), Quad.HalfDimension, CurrentHeight + 1, this);
			TopLeft = pool.Get(WorldPosition.Lerp(Quad.Center, Quad.TopLeft, 0.5), Quad.HalfDimension, CurrentHeight + 1, this);
			BottomRight = pool.Get(WorldPosition.Lerp(Quad.Center, Quad.BottomRight, 0.5), Quad.HalfDimension, CurrentHeight + 1, this);
			BottomLeft = pool.Get(WorldPosition.Lerp(Quad.Center, Quad.BottomLeft, 0.5), Quad.HalfDimension, CurrentHeight + 1, this);

			foreach(var agent in Agents)
			{
                AddObjectToSubTrees(agent);
            }
			Agents.Clear();
		}

		public void RemoveObject(Agent agent)
		{
			Agents.Remove(agent);
			parent?.MergeChilds();
		}
		public void RangeScanQuads(WorldPosition position, double range, HashSet<Agent> buffer)
		{
			if (new Quad(position, range).Intersect(Quad))
			{
				buffer.UnionWith(Agents);
				TopRight?.RangeScanQuads(position, range, buffer);
				TopLeft?.RangeScanQuads(position, range, buffer);
				BottomRight?.RangeScanQuads(position, range, buffer);
				BottomLeft?.RangeScanQuads(position, range, buffer);
			}
		}

		bool HasChildren() => TopRight != null || TopLeft != null || BottomRight != null || BottomLeft != null;

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
				Agents.UnionWith(mergeBuffer);

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

		void GetAgentsInChildrenAll(HashSet<Agent> buffer)
		{
			if (TopRight.HasChildren())
				TopRight.GetAgentsInChildrenAll(buffer);
			else
				buffer.UnionWith(TopRight.Agents);

			if (TopLeft.HasChildren())
				TopLeft.GetAgentsInChildrenAll(buffer);
			else
				buffer.UnionWith(TopLeft.Agents);

			if (BottomRight.HasChildren())
				BottomRight.GetAgentsInChildrenAll(buffer);
			else
				buffer.UnionWith(BottomRight.Agents);

			if (BottomLeft.HasChildren())
				BottomLeft.GetAgentsInChildrenAll(buffer);
			else
				buffer.UnionWith(BottomLeft.Agents);
		}
	}
}
