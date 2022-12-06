using System.Collections.Generic;

namespace qt_benchmark.QuadTree.Services.v1
{
	public sealed class QuadTreePool
	{
		readonly int nodeCapacity;
		readonly int maxDepth;
		readonly Stack<QuadTreeNode> nodes;

		public QuadTreePool(int initialSize, int nodeCapacity, int maxDepth)
		{
			nodes = new Stack<QuadTreeNode>(initialSize);

			for (int i = 0; i < initialSize; i++)
				nodes.Push(new QuadTreeNode(this, nodeCapacity, maxDepth));

			this.nodeCapacity = nodeCapacity;
			this.maxDepth = maxDepth;
		}

		public QuadTreeNode Get(WorldPosition worldPosition, double size, int treeHeight, QuadTreeNode parent)
		{
			var tree = nodes.Count > 0 ? nodes.Pop() : new QuadTreeNode(this, nodeCapacity, maxDepth);

			tree.SetUp(worldPosition, size, treeHeight, parent);
			return tree;
		}

		public void Return(QuadTreeNode tree) => nodes.Push(tree);
	}
}
