using System;
using System.Numerics;

namespace qt_benchmark.QuadTree.Services.v2
{
    public interface INodeItem
    {
        event Action<NodeItemMovedEventArgs> OnNodeItemPathUpdated;

        string ItemId { get; }
        string NodePath { get; }

        void SetPath(string nodePath);
        Vector2 GetPosition();
    }
}