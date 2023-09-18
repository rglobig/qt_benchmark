using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace qt_benchmark.QuadTree.Services.v2
{
    public class QuadTreeNode<T> where T : Agent
    {
        public byte Index { get; private set; }

        public string FullPath;

        public QuadTreeNode<T>[] Nodes { get; private set; }
        private Square _bounds;
        private QuadTreeNode<T> _parent;
        private readonly HashSet<T> _contents;

        private readonly QuadTree _tree;
        QuadTreePool<T> _pool;

        public Square Bounds => _bounds;
        public bool IsEmpty => _contents.Count == 0;
        public bool IsLeaf => Nodes == null || Nodes.Length == 0;

        public HashSet<T> Contents => _contents;

        //public const float MinVertexSize = 5f;
        //public const float MinVertexSize = 2500f;
        public const float MinVertexSize = 10;
        //public const uint MaxCapacity = 1000;
        public const uint MaxCapacity = 10;

        public QuadTreeNode<T> this[char index]
        {
            get
            {
                int i = int.Parse($"{index}");
                if (IsLeaf)
                    return null;

                return Nodes[i];
            }
        }

        /// <summary>
        /// used for pooling
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="pool"></param>
        private QuadTreeNode(QuadTree tree, QuadTreePool<T> pool)
        {
            _contents = new HashSet<T>();
            _tree = tree;
            _pool = pool;
            mergeBuffer = new HashSet<T>();
        }

        /// <summary>
        /// pooling interface
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="pool"></param>
        /// <returns></returns>
        public static QuadTreeNode<T> CreateEmpty(QuadTree tree, QuadTreePool<T> pool)
        {
            return new QuadTreeNode<T>(tree, pool);
        }

        public QuadTreeNode(byte index, Square bounds, QuadTree tree, QuadTreePool<T> pool, string fullPath)
        {
            Index = index;
            _bounds = bounds;
            _contents = new HashSet<T>();
            _tree = tree;
            _pool = pool;
            mergeBuffer = new HashSet<T>();
            FullPath = fullPath;
        }

        public void SetUp(byte index, Square bounds, QuadTreeNode<T> parent, string fullPath)
        {
            _bounds = bounds;
            _parent = parent;
            Index = index;
            FullPath = fullPath;
            Contents.Clear();
        }

        private void Split(string path)
        {
            // makes Split safe to use multiple times without side effects
            if (!IsLeaf) return;
            CreateSubNodes(path);
            foreach (var content in Contents)
            {
                var position = content.position;
                var subNode = Nodes.FirstOrDefault(node => node.Bounds.Contains(position));
                // edge case -- should never have happen
                if (subNode == null)
                {
                    return;
                }
                subNode.Insert(content, String.Concat(path, subNode.Index));
                _tree.AgentToNodeLookup[content] = subNode as QuadTreeNode<Agent>;
            }
            Contents.Clear();
        }

        public void Insert(T item, string path)
        {
            var position = item.position;

            // Should not happen
            if (!_bounds.Contains(position))
                return;

            var hasSufficientSpaceToDeepen = _bounds.Vertex / 2 > MinVertexSize;
            var shouldBranch = hasSufficientSpaceToDeepen && _contents?.Count >= MaxCapacity;
            
            // check if the node has too many items, if it does - split the node's contents into sub nodes and then add the new item to one the new sub nodes
            if (IsLeaf && shouldBranch)
            {
                Split(path);

                var subNode = Nodes.First(node => node.Bounds.Contains(position));
                path += subNode.Index;
                subNode.Insert(item, path);
                _tree.AgentToNodeLookup[item] = subNode as QuadTreeNode<Agent>;
                return;
            }

            // alraedy has children which means we should assign the content to them instead
            if (!IsLeaf)
            {
                var subNode = Nodes.First(node => node.Bounds.Contains(position));
                path += subNode.Index;
                subNode.Insert(item, path);
                _tree.AgentToNodeLookup[item] = subNode as QuadTreeNode<Agent>;
                return;
            }

            _contents.Add(item);
            _tree.AgentToNodeLookup[item] = this as QuadTreeNode<Agent>;
            item.SetPath(path);
        }

        

        private void CreateSubNodes(string path)
        {
            Nodes = new QuadTreeNode<T>[4];

            var subNodeVertex = _bounds.Vertex / 2;

            var subNodesStarts = new Vector2[4] {
                new Vector2(_bounds.X, _bounds.Y),
                new Vector2(_bounds.X + subNodeVertex, _bounds.Y),
                new Vector2(_bounds.X, _bounds.Y + subNodeVertex),
                new Vector2(_bounds.X + subNodeVertex, _bounds.Y + subNodeVertex),
            };

            for (byte i = 0; i < 4; i++)
            {
                Nodes[i] = _pool.Get(i, new Square(subNodesStarts[i], subNodeVertex), this, path + i);
            }
        }

        public override string ToString()
        {
            return $"{FullPath} At [{Bounds.X}, {Bounds.X + Bounds.Vertex}], [{Bounds.Y}, {Bounds.Y + Bounds.Vertex}]";
        }

        public void Remove(T item)
        {
            _contents.Remove(item);
            _parent?.MergeChilds();
        }

        readonly HashSet<T> mergeBuffer;

        void MergeChilds()
        {
            if (IsLeaf)
                return;

            mergeBuffer.Clear();

            GetAgentsInChildrenAll(mergeBuffer);

            if (mergeBuffer.Count <= MaxCapacity)
            {
                Contents.UnionWith(mergeBuffer);

                foreach (var item in Contents)
                {
                    _tree.AgentToNodeLookup[item] = this as QuadTreeNode<Agent>;
                }

                foreach (var node in Nodes)
                {
                    _pool.AddToPool(node);
                };

                Nodes = null;
                _parent?.MergeChilds();
            }
        }

        void GetAgentsInChildrenAll(HashSet<T> buffer)
        {
            foreach (var node in Nodes)
            {
                if (!node.IsLeaf)
                {
                    node.GetAgentsInChildrenAll(buffer);
                }
                else
                {
                    buffer.UnionWith(node.Contents);
                }
            }
        }
    }
}
