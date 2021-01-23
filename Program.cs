using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace qt_benchmark
{
	class Program
	{
		static void Main(string[] args)
		{
			var test = new Test(
				seed: 1337,
				qt: new Basic(),
				map: new Map { sizeX = 30, sizeY = 30 },
				agentAmount: 200,
				agentRadius: 0.5f,
				agentSpeed: 1
				);

			test.Start();

			var ticks = 3000;

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

				test.Draw();
				watch.Reset();
			}

			Console.WriteLine($"Average: {average}ms / Highest: {highest}ms / Lowest: {lowest}ms");
			Console.ReadLine();
		}
	}

	class Basic : IQuadTreeService
	{
		public void Initialize() { }

		public void Query(Vector2 position, float radius, Dictionary<int, Agent> allAgents, HashSet<Agent> buffer)
		{
			buffer.Clear();

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

		public void Update() { }
	}

	interface IQuadTreeService
	{
		void Initialize();
		void Update();
		void Query(Vector2 position, float radius, Dictionary<int, Agent> allAgents, HashSet<Agent> buffer);
	}

	class Agent
	{
		public int id;
		public float radius;

		public Vector2 position;

		public Vector2 move;

		public float speed;

		public void Move() => position += move * speed;
	}

	class Map
	{
		public int sizeX;
		public int sizeY;
	}

	class Test
	{
		private readonly Dictionary<int, Agent> agents = new Dictionary<int, Agent>();
		private readonly IQuadTreeService qt;
		private readonly Map map;
		private readonly Random random;

		public Test(int seed, IQuadTreeService qt, Map map, int agentAmount, float agentRadius, float agentSpeed)
		{
			random = new Random(seed);
			this.qt = qt;
			this.map = map;

			for (int i = 0; i < agentAmount; i++)
			{
				agents.Add(i, new Agent
				{
					id = i,
					radius = agentRadius,
					position = new Vector2(random.Next(0, map.sizeX), random.Next(0, map.sizeY)),
					move = Vector2.Normalize(new Vector2(random.Next(), random.Next())),
					speed = agentSpeed
				});
			}
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

			foreach (var agent in agents)
			{

			}
		}

		public void Draw()
		{

		}
	}
}
