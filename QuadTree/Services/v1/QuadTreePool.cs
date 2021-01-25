using System.Collections.Generic;

namespace qt_benchmark.QuadTree.Services.v1
{
	public sealed class QuadTreePool
	{
		readonly QuadTree quadTree;
		readonly int nodeCapacity;
		readonly int maxDepth;
		readonly Stack<QuadTreeNode> quadTrees;

		public QuadTreePool(QuadTree quadTree, int initialSize, int nodeCapacity, int maxDepth)
		{
			if (quadTrees == null)
				quadTrees = new Stack<QuadTreeNode>(initialSize);

			for (int i = 0; i < initialSize; i++)
				quadTrees.Push(new QuadTreeNode(quadTree, this, nodeCapacity, maxDepth));

			this.quadTree = quadTree;
			this.nodeCapacity = nodeCapacity;
			this.maxDepth = maxDepth;
		}

		public QuadTreeNode Get(WorldPosition worldPosition, double halfDim, int treeHeight, QuadTreeNode parent)
		{
			var tree = quadTrees.Count > 0 ? quadTrees.Pop() : new QuadTreeNode(this.quadTree, this, nodeCapacity, maxDepth);

			tree.SetUp(worldPosition, halfDim, treeHeight, parent);
			return tree;
		}

		public void Return(QuadTreeNode tree) => quadTrees.Push(tree);
	}
}
