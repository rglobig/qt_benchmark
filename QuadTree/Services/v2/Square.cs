using System.Collections.Generic;
using System.Numerics;

namespace qt_benchmark.QuadTree.Services.v2
{
    public struct Square
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Vertex;

        public Square(Vector2 start, float vertex)
        {
            X = start.X;
            Y = start.Y;
            Vertex = vertex;
        }

        public IEnumerable<Vector2> GetVerticles()
        {
            yield return new Vector2(X, Y);
            yield return new Vector2(X + Vertex, Y);
            yield return new Vector2(X, Y + Vertex);
            yield return new Vector2(X + Vertex, Y + Vertex);
        }

        public bool Contains(Vector2 position)
        {
            bool inX = false;
            bool inY = false;

            var x = position.X;
            var y = position.Y;

            var bottomLeft = new Vector2(X, Y);
            var topRight = new Vector2(X + Vertex, Y + Vertex);

            if (x >= bottomLeft.X && x <= topRight.X)
                inX = true;

            if (y >= bottomLeft.Y && y <= topRight.Y)
                inY = true;

            return (inX && inY);
        }

        public override string ToString()
        {
            string square = $"0: {X},{Y}, 1: {X + Vertex},{Y}, 2: {X},{Y + Vertex}, 3: {X + Vertex},{Y + Vertex}";
            return square;
        }
    }
}