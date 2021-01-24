using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace qt_benchmark.QuadTree.Services.v2
{
    public class QuadTreeNode<T> where T : Agent
    {
        public readonly byte Index;

        public string FullPath;

        public QuadTreeNode<T>[] Nodes { get; private set; }
        private Square _bounds;
        private readonly HashSet<T> _contents;

        public Square Bounds => _bounds;
        public bool IsEmpty => _contents.Count == 0;
        public bool IsLeaf => Nodes == null || Nodes.Length == 0;

        public HashSet<T> Contents => _contents;

        public const float MinVertexSize = 10f;

        public QuadTreeNode<T> this[char index]
        {
            get
            {
                int i = int.Parse($"{index}");
                return Nodes[i];
            }
        }

        public QuadTreeNode(byte index, Square bounds)
        {
            Index = index;

            _bounds = bounds;

            _contents = new HashSet<T>();
        }

        public void Insert(T item, string path)
        {
            var position = item.position;

            // Should not happen
            if (!_bounds.Contains(position))
                return;

            if (IsLeaf && CanBranch())
                CreateSubNodes(path);

            if (!IsLeaf)
            {
                var subNode = Nodes.First(node => node.Bounds.Contains(position));
                //if (!string.IsNullOrEmpty(FullPath) || path == "0")
                path += subNode.Index;
                //Debug.Log(path + " " + subNode.Bounds + " " + subNode.Bounds.Contains(position));
                subNode.Insert(item, path);
                return;
            }
            else
            {
                //Debug.Log(FullPath);
                _contents.Add(item);
                item.SetPath(path);
            }
        }

        public void Remove(T item)
        {
            _contents.Remove(item);
        }

        private bool CanBranch()
        {
            var x = _bounds.Vertex / 2 > MinVertexSize;
            return x;
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
                Nodes[i] = new QuadTreeNode<T>(i, new Square(subNodesStarts[i], subNodeVertex))
                {
                    FullPath = path + i,
                };
            }
        }

        public override string ToString()
        {
            return $"" +
                $"{FullPath} " +
                $"{Bounds.X}, {Bounds.Y}";
        }
    }
}
