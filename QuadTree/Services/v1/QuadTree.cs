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
        QuadTreePool pool;
        private readonly WorldPosition position;
        private readonly Size size;
        private readonly int poolSize;
        private readonly int nodeCapacity;
        private readonly int maxDepth;

        public QuadTree(WorldPosition position, Size size, int poolSize, int nodeCapacity, int maxDepth)
        {
            this.position = position;
            this.size = size;
            this.poolSize = poolSize;
            this.nodeCapacity = nodeCapacity;
            this.maxDepth = maxDepth;
            Reset();
        }

        public void Reset()
        {
            pool = new QuadTreePool(this, poolSize, nodeCapacity, maxDepth);
            RootNode = pool.Get(position, size.Width, 0, parent: null);
        }

        public void Add(Agent agent) => RootNode.AddObject(agent);

        public void Update()
        {
            foreach (var item in AgentToNodeLookup)
            {
                var agent = item.Key;
                var node = item.Value;

                if (!node.Quad.Contains(agent.position.ToWorld()))
                {
                    AgentToNodeLookup[agent].RemoveObject(agent);
                    RootNode.AddObject(agent);
                }
            }
        }

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
