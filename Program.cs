using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using qt_benchmark.QuadTree.Services.v2;

namespace qt_benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new Test(
                seed: 1337,
                //qt: new Basic(),
                qt: new QuadTree.Services.v2.QuadTree(new Square(new Vector2(0, 0), 30)),
                map: new Map { sizeX = 30, sizeY = 30 },
                agentAmount: 200,
                agentRadius: 0.5f,
                agentSpeed: 1
                );

            test.Start();

            var ticks = 100;

            var total = 0L;
            var average = 0L;
            var highest = 0L;
            var lowest = long.MaxValue;

            var watch = new Stopwatch();

            for (int i = 0; i < ticks; i++)
            {
                watch.Start();
                test.Update();
                watch.Stop();

                var current = watch.ElapsedMilliseconds;
                total += current;
                average = total / (i + 1);
                if (current > highest) highest = current;
                if (current < lowest) lowest = current;

                Console.Clear();
                test.Draw();
                watch.Reset();
                Thread.Sleep(10);
            }

            Console.WriteLine($"Average: {average}ms / Highest: {highest}ms / Lowest: {lowest}ms");
            Console.ReadLine();
        }
    }

    class Basic : IQuadTreeService
    {
        public void Initialize() { }
        public void Query(Agent queriedAgent, float radius, Dictionary<int, Agent> allAgents, HashSet<Agent> buffer)
        {
            Vector2 position = queriedAgent.position;
            for (int i = 0; i < allAgents.Count; i++)
            {
                var agent = allAgents[i];
                var distanceSquare = (agent.radius + radius) * (agent.radius + radius);
                if (Vector2.DistanceSquared(position, agent.position) <= distanceSquare)
                {
                    buffer.Add(agent);
                }
            }
        }

        public void Insert(Agent agent)
        {
        }

        public void Update() { }
    }

    interface IQuadTreeService
    {
        void Initialize();
        void Update();
        void Query(Agent agent, float radius, Dictionary<int, Agent> allAgents, HashSet<Agent> buffer);
        void Insert(Agent agent);
    }

    class Map
    {
        public int sizeX;
        public int sizeY;
    }

    class Test
    {
        private readonly Dictionary<int, Agent> agents;
        private readonly IQuadTreeService qt;
        private readonly Map map;
        private readonly Random random;
        private readonly HashSet<Agent> buffer;
        private readonly Vector2 Up = new Vector2(0, 1);
        private readonly Vector2 Right = new Vector2(1, 0);
        private readonly Vector2 Left = new Vector2(-1, 0);
        private readonly Vector2 Down = new Vector2(0, -1);

        public Test(int seed, IQuadTreeService qt, Map map, int agentAmount, float agentRadius, float agentSpeed)
        {
            random = new Random(seed);
            agents = new Dictionary<int, Agent>(agentAmount);
            buffer = new HashSet<Agent>(32);
            this.qt = qt;
            this.map = map;

            for (int i = 0; i < agentAmount; i++)
            {
                var agent = new Agent
                {
                    id = i,
                    radius = agentRadius,
                    position = new Vector2(random.Next(0, map.sizeX), random.Next(0, map.sizeY)),
                    move = Vector2.Normalize(RandomDirection()),
                    speed = agentSpeed
                };
                agents.Add(i, agent);
                qt.Insert(agent);
            }
        }

        Vector2 RandomDirection()
        {
            var x = random.Next(-100, 101);
            var y = random.Next(-100, 101);
            return new Vector2(x, y);
        }

        public void Start()
        {
            qt.Initialize();
        }

        public void Update()
        {
            foreach (var agent in agents)
            {
                agent.Value.Move();
            }

            qt.Update();

            buffer.Clear();

            foreach (var agent in agents)
            {
                qt.Query(agent.Value, agent.Value.radius, agents, buffer);

                foreach (var other in buffer)
                {
                    if (agent.Value != other)
                    {
                        agent.Value.move = -agent.Value.move;
                    }
                }
            }

            foreach (var agent in agents)
            {
                var position = agent.Value.position;

                if (position.X <= 0)
                {
                    position.X = 0;
                    agent.Value.move = Vector2.Reflect(agent.Value.move, Right);
                }
                if (position.Y <= 0)
                {
                    position.Y = 0;
                    agent.Value.move = Vector2.Reflect(agent.Value.move, Up);
                }
                if (position.X >= map.sizeX - 1)
                {
                    position.X = map.sizeX - 1;
                    agent.Value.move = Vector2.Reflect(agent.Value.move, Left);
                }
                if (position.Y >= map.sizeY - 1)
                {
                    position.Y = map.sizeY - 1;
                    agent.Value.move = Vector2.Reflect(agent.Value.move, Down);
                }

                agent.Value.position = position;
            }
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
