using System;
using System.Collections.Generic;
using System.Numerics;

namespace qt_benchmark
{
    class PrimeTest : ITest
    {
        private Dictionary<int, Agent> agents;
        private IQuadTreeService qt;
        private Map map;
        private Random random;
        private HashSet<Agent> buffer;
        private HashSet<int> primeProduct;
        private readonly Vector2 Up = new Vector2(0, 1);
        private readonly Vector2 Right = new Vector2(1, 0);
        private readonly Vector2 Left = new Vector2(-1, 0);
        private readonly Vector2 Down = new Vector2(0, -1);

        Vector2 RandomDirection()
        {
            var x = random.Next(-100, 101);
            var y = random.Next(-100, 101);
            return Vector2.Normalize(new Vector2(x, y));
        }

        public void Reset(int seed, IQuadTreeService qt, Map map, int agentAmount, float agentRadius, float agentSpeed)
        {
            random = new Random(seed);
            agents = new Dictionary<int, Agent>(agentAmount);
            buffer = new HashSet<Agent>(32);
            primeProduct = new HashSet<int>(agentAmount);
            this.qt = qt;
            this.map = map;

            qt.Reset();

            for (int i = 0; i < agentAmount; i++)
            {
                var agent = new Agent
                {
                    id = i,
                    prime = Primes.numbers[i],
                    radius = agentRadius,
                    position = new Vector2(random.Next(0, map.sizeX), random.Next(0, map.sizeY)),
                    move = RandomDirection(),
                    speed = agentSpeed
                };
                agents.Add(i, agent);
                qt.Insert(agent);
            }

            qt.Initialize();
        }

        public void Update(out int actualChecks, out int totalChecks)
        {
            foreach (var agent in agents)
            {
                agent.Value.Move();
            }

            qt.Update();

            CheckCollision(out actualChecks, out totalChecks);
        }

        private void CheckCollision(out int actualChecks, out int totalChecks)
        {
            buffer.Clear();
            primeProduct.Clear();

            totalChecks = 0;
            actualChecks = 0;

            foreach (var kvp in agents)
            {
                var agent = kvp.Value;
                qt.Query(agent, agent.radius, agents, buffer);

                foreach (var other in buffer)
                {
                    if (other == agent) continue;
                    totalChecks++;
                    var product = agent.prime * other.prime;
                    if (primeProduct.Contains(product)) continue;
                    var distance = Vector2.DistanceSquared(agent.position, other.position);
                    var radius = (agent.radius + other.radius) * (agent.radius + other.radius);
                    if (distance <= radius)
                    {
                        var direction = agent.position - other.position;
                        direction = Vector2.Normalize(direction);
                        agent.move = direction;

                        var otherDirection = other.position - agent.position;
                        otherDirection = Vector2.Normalize(otherDirection);
                        other.move = otherDirection;
                    }
                    primeProduct.Add(product);
                    actualChecks++;
                }

                CheckMapBounds(agent);
            }
        }

        private void CheckMapBounds(Agent agent)
        {
            var position = agent.position;
            if (position.X <= 0)
            {
                position.X = 0;
                agent.move = Vector2.Reflect(agent.move, Right);
            }
            if (position.Y <= 0)
            {
                position.Y = 0;
                agent.move = Vector2.Reflect(agent.move, Up);
            }
            if (position.X >= map.sizeX - 1)
            {
                position.X = map.sizeX - 1;
                agent.move = Vector2.Reflect(agent.move, Left);
            }
            if (position.Y >= map.sizeY - 1)
            {
                position.Y = map.sizeY - 1;
                agent.move = Vector2.Reflect(agent.move, Down);
            }
            agent.position = position;
        }

        public void Draw()
        {
            for (int row = 0; row < map.sizeY; row++)
            {
                var line = string.Empty;

                for (int i = 0; i < map.sizeX * 2; i++)
                {
                    line += ' ';
                }

                foreach (var agent in agents)
                {
                    int y = Convert.ToInt32(Math.Round(agent.Value.position.Y));
                    if (row == y)
                    {
                        int x = Convert.ToInt32(Math.Round(agent.Value.position.X));
                        var chars = line.ToCharArray();
                        chars[x * 2] = 'o';
                        line = new string(chars);
                    }
                }
                Console.WriteLine(line);
            }
        }
    }
}
