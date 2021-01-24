using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace qt_benchmark.QuadTree.Services.v2
{
    public static class QuaternaryUtils
    {
        private static Dictionary<DirectionType, DirectionType> _oppositeDirections = new Dictionary<DirectionType, DirectionType>
        {
            [DirectionType.East] = DirectionType.West,
            [DirectionType.West] = DirectionType.East,

            [DirectionType.North] = DirectionType.South,
            [DirectionType.South] = DirectionType.North,

            [DirectionType.NorthEast] = DirectionType.SouthWest,
            [DirectionType.NorthWest] = DirectionType.SouthEast,

            [DirectionType.SouthEast] = DirectionType.NorthWest,
            [DirectionType.SouthWest] = DirectionType.NorthEast,
        };

        public static bool IsSouth(char quadId) => quadId == '0' || quadId == '1';
        public static bool IsNorth(char quadId) => quadId == '2' || quadId == '3';

        public static bool IsWest(char quadId) => quadId == '0' || quadId == '2';
        public static bool IsEast(char quadId) => quadId == '1' || quadId == '3';


        public static char FlipUpDown(char quadId)
        {
            switch (quadId)
            {
                case '0':
                    return '2';
                case '1':
                    return '3';
                case '2':
                    return '0';
                case '3':
                    return '1';
            }

            throw new InvalidOperationException();
        }

        public static char FlipLeftRight(char quadId)
        {
            switch (quadId)
            {
                case '0':
                    return '1';
                case '1':
                    return '0';
                case '2':
                    return '3';
                case '3':
                    return '2';
            }

            throw new InvalidOperationException();
        }

        public static string North(string quadPath)
        {
            return Direction(quadPath, FlipUpDown, IsSouth);
        }

        public static string South(string quadPath)
        {
            return Direction(quadPath, FlipUpDown, IsNorth);
        }

        public static string West(string quadPath)
        {
            return Direction(quadPath, FlipLeftRight, IsEast);
        }

        public static string East(string quadPath)
        {
            return Direction(quadPath, FlipLeftRight, IsWest);
        }

        public static string NorthWest(string quadPath)
        {
            string north = Direction(quadPath, FlipUpDown, IsSouth);

            return Direction(north, FlipLeftRight, IsEast);
        }

        public static string SouthWest(string quadPath)
        {
            string north = Direction(quadPath, FlipUpDown, IsNorth);

            return Direction(north, FlipLeftRight, IsEast);
        }

        public static string NorthEast(string quadPath)
        {
            string north = Direction(quadPath, FlipUpDown, IsSouth);

            return Direction(north, FlipLeftRight, IsWest);
        }

        public static string SouthEast(string quadPath)
        {
            string north = Direction(quadPath, FlipUpDown, IsNorth);

            return Direction(north, FlipLeftRight, IsWest);
        }

        public static ushort GetDiffLength(string quadPath1, string quadPath2)
        {
            ushort count = 0;

            var length = Math.Min(quadPath1.Length, quadPath2.Length);
            for (int i = 0; i < length; i++)
            {
                if (quadPath1[i] != quadPath2[i])
                {
                    count = (ushort)(length - i);
                    break;
                }
            }

            count += (ushort)Math.Abs(quadPath1.Length - quadPath2.Length);

            return count;
        }

        /// <summary>
        /// WIP - unstable
        /// </summary>
        public static bool CompareAdjacentPathes(string quadPath1, string quadPath2, out DirectionType directionType)
        {
            if (quadPath1 != quadPath2)
            {
                // the same path
            }
            if (quadPath1.Contains(quadPath2))
            {
                // contained
            }
            if (quadPath1.Length != quadPath2.Length)
            {
                // can not establish an adjacent position beween the received paths
            }

            if (string.Equals(East(quadPath1), quadPath2))
            {
                directionType = DirectionType.East;
                return true;
            }

            if (string.Equals(West(quadPath1), quadPath2))
            {
                directionType = DirectionType.West;
                return true;
            }

            if (string.Equals(North(quadPath1), quadPath2))
            {
                directionType = DirectionType.North;
                return true;
            }

            if (string.Equals(South(quadPath1), quadPath2))
            {
                directionType = DirectionType.South;
                return true;
            }

            if (string.Equals(NorthEast(quadPath1), quadPath2))
            {
                directionType = DirectionType.NorthEast;
                return true;
            }
            if (string.Equals(NorthWest(quadPath1), quadPath2))
            {
                directionType = DirectionType.NorthWest;
                return true;
            }

            if (string.Equals(SouthEast(quadPath1), quadPath2))
            {
                directionType = DirectionType.SouthEast;
                return true;
            }
            if (string.Equals(SouthWest(quadPath1), quadPath2))
            {
                directionType = DirectionType.SouthWest;
                return true;
            }

            directionType = DirectionType.NotFound;
            return false;
        }

        public static bool ArePathsNeighbors(string path1, string path2, ushort ancestorLevel = 0)
        {
            if (path1 == path2)
                return true;

            var calculatedLevel = path2.Length - ancestorLevel;

            var surroundingNodes = GetSurroundingNodes(path1, ancestorLevel);
            var path2Ancestor = path2.Substring(0, calculatedLevel);

            return surroundingNodes.Any(path1NeighborPath => path1NeighborPath == path2Ancestor);
        }

        public static string[] GetSurroundingNodes(string quadId, ushort ancestorLevel = 0)
        {
            var ancestorPath = ancestorLevel > 0
                ? quadId.Substring(0, quadId.Length - ancestorLevel)
                : quadId;

            var n = North(ancestorPath);
            var s = South(ancestorPath);
            var w = West(ancestorPath);
            var e = East(ancestorPath);

            var nw = NorthWest(ancestorPath);
            var ne = NorthEast(ancestorPath);
            var sw = SouthWest(ancestorPath);
            var se = SouthEast(ancestorPath);

            return new string[] { n, s, w, e, nw, ne, sw, se };
        }

        public static IEnumerable<string> GetOuterNodePathsForPath(string quadPath, DirectionType directionType)
        {
            switch (directionType)
            {
                case DirectionType.East:
                    yield return West(quadPath);
                    yield return NorthWest(quadPath);
                    yield return SouthWest(quadPath);
                    break;
                case DirectionType.West:
                    yield return East(quadPath);
                    yield return NorthEast(quadPath);
                    yield return SouthEast(quadPath);
                    break;
                case DirectionType.North:
                    yield return South(quadPath);
                    yield return SouthWest(quadPath);
                    yield return SouthEast(quadPath);
                    break;
                case DirectionType.South:
                    yield return North(quadPath);
                    yield return NorthWest(quadPath);
                    yield return NorthEast(quadPath);
                    break;

                case DirectionType.NorthEast:
                    yield return NorthWest(quadPath);
                    yield return West(quadPath);
                    yield return South(quadPath);
                    yield return SouthWest(quadPath);
                    yield return SouthEast(quadPath);
                    break;
                case DirectionType.NorthWest:
                    yield return NorthEast(quadPath);
                    yield return East(quadPath);
                    yield return South(quadPath);
                    yield return SouthWest(quadPath);
                    yield return SouthEast(quadPath);
                    break;

                case DirectionType.SouthEast:
                    yield return SouthWest(quadPath);
                    yield return West(quadPath);
                    yield return North(quadPath);
                    yield return NorthWest(quadPath);
                    yield return NorthEast(quadPath);
                    break;
                case DirectionType.SouthWest:
                    yield return SouthEast(quadPath);
                    yield return East(quadPath);
                    yield return North(quadPath);
                    yield return NorthWest(quadPath);
                    yield return NorthEast(quadPath);
                    break;
            }
        }

        public static DirectionType GetOppositeDirection(DirectionType dir1)
        {
            return _oppositeDirections[dir1];
        }

        private static string Direction(string quadPath, Func<char, char> reverseCharFunc, Func<char, bool> oppositeCharFunc)
        {
            StringBuilder sb = new StringBuilder();
            char[] neighnour = new char[quadPath.Length];

            var cIndex = quadPath[quadPath.Length - 1];

            bool isOpposite = oppositeCharFunc(cIndex);

            neighnour[quadPath.Length - 1] = reverseCharFunc(cIndex);

            int i = quadPath.Length - 2;
            bool changedLast = false;
            if (!isOpposite)
            {

                // reduce until no longer needed
                for (; i > 0; i--)
                {
                    cIndex = quadPath[i];

                    neighnour[i] = reverseCharFunc(cIndex);

                    // means we no longer need to reduce the parent
                    if (oppositeCharFunc(cIndex))
                    {
                        changedLast = true;
                        break;
                    }
                }
            }

            // fill in missing characters
            for (int j = 0; j <= i; j++)
            {
                if (j == i && changedLast)
                {
                    continue;
                }

                neighnour[j] = quadPath[j];
            }

            for (int k = 0; k < neighnour.Length; k++)
            {
                sb.Append(neighnour[k]);
            }

            return sb.ToString();
        }

        public static string GetQuadTreePath(Vector2 position, TreeDefinitions treeDefinitions)
        {
            var pathBuilder = new StringBuilder();

            ushort currentDepth = 0;
            var bounds = new Square(new Vector2(0,0), treeDefinitions.Size);
            var currentNode = bounds;
            do
            {
                currentNode = CreateSubNodes(currentNode, position, out var index);
                pathBuilder.Append(index);
                currentDepth++;
            }
            // as long as we haven't reached the lowest possible depth of the tree
            while (currentDepth != treeDefinitions.Depth);

            return pathBuilder.ToString();
        }
        private static Square CreateSubNodes(Square bounds, Vector2 position, out char index)
        {
            var subNodeVertex = bounds.Vertex / 2;
            var tl = new Square(new Vector2(bounds.X, bounds.Y), subNodeVertex);
            if (tl.Contains(position))
            {
                index = '0';
                return tl;
            }
            var tr = new Square(new Vector2(bounds.X + subNodeVertex, bounds.Y), subNodeVertex);
            if (tr.Contains(position))
            {
                index = '1';
                return tr;
            }
            var bl = new Square(new Vector2(bounds.X, bounds.Y + subNodeVertex), subNodeVertex);
            if (bl.Contains(position))
            {
                index = '2';
                return bl;
            }
            var br =
                new Square(new Vector2(bounds.X + subNodeVertex, bounds.Y + subNodeVertex), subNodeVertex);
            if (br.Contains(position))
            {
                index = '3';
                return br;
            }

            throw new ArgumentException($"{position} is out of bound {bounds}");
        }
    }
}


public struct TreeDefinitions
{
    public float Depth;
    public float Size;

    public TreeDefinitions(float depth, float size)
    {
        Depth = depth;
        Size = size;
    }
}