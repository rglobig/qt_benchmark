using qt_benchmark.QuadTree.Services.v1;
using System;
using System.Collections.Generic;
using System.Text;

namespace qt_benchmark.QuadTree.Services.v2
{
    public class QuadTree : IQuadTreeService
    {
        private QuadTreeNode<Agent> _root;
        private readonly Square bounds;
        private QuadTreePool<Agent> _pool;

        public Dictionary<Agent, QuadTreeNode<Agent>> AgentToNodeLookup { get; private set; } = new Dictionary<Agent, QuadTreeNode<Agent>>();


        public QuadTree(Square bounds)
        {

            this.bounds = bounds;
            Reset();
        }

        public void Reset()
        {
            _pool = new QuadTreePool<Agent>(this, 2000);
            _root = _pool.Get(0, bounds, parent: null, string.Empty);
        }

        /// <summary>
        /// Tries to get the exact node by its prefix, if allowAncestor is on and the node path is not found, the nearest ancertor will be returned instead
        /// </summary>
        /// <param name="query"></param>
        /// <param name="allowAncestor"></param>
        /// <returns></returns>
        public QuadTreeNode<Agent> GetNodeByPrefix(string query, bool allowAncestor = true)
        {
            var node = _root;
            for (int i = 0; i < query.Length; i++)
            {
                var index = query[i];
                if (node[index] == null)
                    return allowAncestor ? node : null;
                node = node[index];
            }

            return node;
        }


        public void Initialize()
        {
        }

        public void Update()
        {
            foreach (var item in AgentToNodeLookup)
            {
                var agent = item.Key;
                var node = item.Value;

                if (!node.Bounds.Contains(agent.position))
                {
                    AgentToNodeLookup[agent].Remove(agent);
                    Insert(agent);
                }
            }
        }

        public void Query(Agent agent, float radius, Dictionary<int, Agent> allAgents, HashSet<Agent> buffer)
        {
            var quadPath = agent.nodeId;
            InnerQuerySet(quadPath, buffer);

            var position = agent.position.ToWorld();
            buffer.RemoveWhere(agent => WorldPosition.DistanceSquare(position, agent.position.ToWorld()) > radius * radius);
        }

        public void Insert(Agent item)
        {
            //var node = 
            _root.Insert(item, "");
            //if (node != null)
            //{
            //    AgentToNodeLookup[item] = node;
            //}
            //item.OnMove = Update;
        }

        public void Update(Agent item)
        {
            var pathBuilder = new StringBuilder();
            pathBuilder.Append(item.nodeId);

            var itemPosition = item.position;

            var node = GetNodeByPrefix(item.nodeId);
            if (node == null)
            {
                return;
            }

            node.Remove(item);

            var currNode = GetNodeByPrefix(pathBuilder.ToString());
            while (currNode != null && !currNode.Bounds.Contains(itemPosition))
            {
                pathBuilder.Remove(pathBuilder.Length - 1, 1);
            }

            var ancestor = GetNodeByPrefix(pathBuilder.ToString());
            if (ancestor != null) { return; }
            ancestor.Insert(item, pathBuilder.ToString());
        }

        private IEnumerable<Agent> InnerQuery(string quadId, Func<Agent, bool> condition = null, int anscestorLevel = 0)
        {
            var startAncestorPath = quadId.Substring(0, quadId.Length - anscestorLevel);
            var startNode = GetNodeByPrefix(startAncestorPath);
            if (startNode == null)
            {
                yield break;
            }

            var queue = new Queue<QuadTreeNode<Agent>>();
            queue.Enqueue(startNode);


            foreach (var surroundingNodeId in QuaternaryUtils.GetSurroundingNodes(startAncestorPath))
            {
                var surroundingNode = GetNodeByPrefix(surroundingNodeId);
                if (surroundingNode != null)
                {
                    queue.Enqueue(surroundingNode);
                }
            }

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                foreach (var content in node.Contents)
                {
                    if (condition != null && condition(content))
                    {
                        yield return content;
                    }
                    else
                    {
                        yield return content;
                    }
                }

                if (node.IsLeaf)
                    continue;
                foreach (var subNode in node.Nodes)
                {
                    queue.Enqueue(subNode);
                }
            }
        }

        // fill an external hashset directly rather than iterate through the contents again
        private void InnerQuerySet(string quadId, HashSet<Agent> buffer, int anscestorLevel = 0)
        {
            var startAncestorPath = quadId.Substring(0, quadId.Length - anscestorLevel);
            var startNode = GetNodeByPrefix(startAncestorPath);
            if (startNode == null)
            {
                return;
            }

            var queue = new Queue<QuadTreeNode<Agent>>();
            queue.Enqueue(startNode);

            foreach (var surroundingNodeId in QuaternaryUtils.GetSurroundingNodes(startAncestorPath))
            {
                var surroundingNode = GetNodeByPrefix(surroundingNodeId);
                if (surroundingNode != null)
                {
                    queue.Enqueue(surroundingNode);
                }
            }

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                buffer.UnionWith(node.Contents);

                if (node.IsLeaf)
                    continue;
                foreach (var subNode in node.Nodes)
                {
                    queue.Enqueue(subNode);
                }
            }
        }

        /// <summary>
        /// Queries the node by a direction and a union tyoe
        /// </summary>
        /// <param name="quadId">queried quad path</param>
        /// <param name="directionType">which direction to query</param>
        /// <returns></returns>
        public IEnumerable<Agent> QueryOuterByDirection(string quadId, DirectionType directionType, Func<Agent, bool> condition = null)
        {
            var queue = new Queue<QuadTreeNode<Agent>>();

            foreach (var outerPath in QuaternaryUtils.GetOuterNodePathsForPath(quadId, directionType))
            {
                    var currNode = GetNodeByPrefix(outerPath);
                    if (currNode != null)
                    {
                        queue.Enqueue(currNode);
                    }
            }

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                foreach (var content in node.Contents)
                {
                    //Debug.Log((content as CharacterItem).CharacterId + " " + node);
                    if (condition == null)
                        yield return content;
                    else if (condition(content))
                        yield return content;
                }

                if (node.IsLeaf)
                    continue;
                foreach (var subNode in node.Nodes)
                {
                    queue.Enqueue(subNode);
                }
            }
        }
    }
}
