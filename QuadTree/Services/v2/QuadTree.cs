using System;
using System.Collections.Generic;
using System.Text;

namespace qt_benchmark.QuadTree.Services.v2
{
    public class QuadTree: IQuadTreeService
    {
        private QuadTreeNode<Agent> _root;
        private readonly Square bounds;

        public QuadTree(Square bounds)
        {

            this.bounds = bounds;
            Reset();
        }

        public void Reset()
        {
            _root = new QuadTreeNode<Agent>(0, bounds);
        }

        public QuadTreeNode<Agent> GetNodeByPrefix(string query)
        {
            var node = _root;
            for (int i = 0; i < query.Length; i++)
            {
                char index = query[i];
                node = node[index];
            }

            return node;
        }


        public void Initialize()
        {
        }

        public void Update()
        {
        }

        public void Query(Agent agent, float radius, Dictionary<int, Agent> allAgents, HashSet<Agent> buffer)
        {
            var quadPath = agent.nodeId;
            foreach (var nodeItem in InnerQuery(quadPath))
            {
                buffer.Add(new Agent());
            }
        }

        public void Insert(Agent item)
        {
            _root.Insert(item, "");
            item.OnMove = Update;
        }

        public void Update(Agent item)
        {
            var pathBuilder = new StringBuilder();
            pathBuilder.Append(item.nodeId);

            var itemPosition = item.position;

            var node = GetNodeByPrefix(item.nodeId);
            
            node.Remove(item);
            
            while (!GetNodeByPrefix(pathBuilder.ToString()).Bounds.Contains(itemPosition))
            {
                pathBuilder.Remove(pathBuilder.Length - 1, 1);
            }

            var ancestor = GetNodeByPrefix(pathBuilder.ToString());
            ancestor.Insert(item, pathBuilder.ToString());
        }

        private IEnumerable<Agent> InnerQuery(string quadId, Func<Agent, bool> condition = null, int anscestorLevel = 0)
        {
            var startAncestorPath = quadId.Substring(0, quadId.Length - anscestorLevel);
            var startNode = GetNodeByPrefix(startAncestorPath);

            var queue = new Queue<QuadTreeNode<Agent>>();
            queue.Enqueue(startNode);

            foreach (var surroundingNodeId in QuaternaryUtils.GetSurroundingNodes(startAncestorPath))
            {
                try
                {
                    var surroundingNode = GetNodeByPrefix(surroundingNodeId);
                    //Debug.LogWarning(surroundingNodeId);
                    queue.Enqueue(surroundingNode);
                }
                catch (Exception e)
                {
                    continue;
                }
            }

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                foreach (var content in node.Contents)
                {
                    if (condition != null)
                    {
                        if (condition(content))
                        {
                            yield return content;
                        }
                    }
                    else
                    {
                        //Debug.Log((content as CharacterItem).CharacterId + " " + node);
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
                try
                {
                    queue.Enqueue(GetNodeByPrefix(outerPath));
                }
                catch
                {
                    continue;
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
