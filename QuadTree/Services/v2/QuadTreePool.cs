using System.Collections.Generic;

namespace qt_benchmark.QuadTree.Services.v2
{
    public sealed class QuadTreePool<T> where T : Agent
    {
        readonly QuadTree quadTree;
        readonly Stack<QuadTreeNode<T>> nodes;

        public QuadTreePool(QuadTree quadTree, int initialSize)
        {
            nodes = new Stack<QuadTreeNode<T>>(initialSize);

            for (int i = 0; i < initialSize; i++)
                nodes.Push(QuadTreeNode<T>.CreateEmpty(quadTree, this));
            this.quadTree = quadTree;
        }

        public QuadTreeNode<T> Get(byte index, Square bounds, QuadTreeNode<T> parent, string fullPath)
        {
            var tree = nodes.Count > 0 ? nodes.Pop() : new QuadTreeNode<T>(index, bounds, quadTree, this, fullPath);
            tree.SetUp(index, bounds, parent, fullPath);
            return tree;
        }

        public void AddToPool(QuadTreeNode<T> tree) => nodes.Push(tree);
    }
}
