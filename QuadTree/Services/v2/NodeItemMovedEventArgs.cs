namespace qt_benchmark.QuadTree.Services.v2
{
    public class NodeItemMovedEventArgs
    {
        public readonly string NodeItemId;
        public readonly string NewNodePath;
        public readonly string PreviousNodePath;

        public NodeItemMovedEventArgs(string nodeItemId, string newNodePath, string previousNodePath)
        {
            NodeItemId = nodeItemId;
            NewNodePath = newNodePath;
            PreviousNodePath = previousNodePath;
        }
    }
}