using System.Collections.Generic;

namespace qt_benchmark
{
	class Program
	{
		static void Main(string[] args)
		{
			var test = new Test(new Basic());
			test.Start();
		}
	}

	class Basic : IQuadTreeService
	{
		public void Initialize()
		{
		}

		public Agent[] Query(float x, float y, float radius, Dictionary<int, Agent> allAgents)
		{
			return null;
		}

		public void Update()
		{
		}
	}

	interface IQuadTreeService
	{
		void Initialize();
		void Update();
		Agent[] Query(float x, float y, float radius, Dictionary<int, Agent> allAgents);
	}

	class Agent
	{
		int id;
		float radius;
		float x;
		float y;
	}

	class Test
	{
		private readonly Dictionary<int, Agent> agents = new Dictionary<int, Agent>();
		private readonly IQuadTreeService qt;

		public Test(IQuadTreeService qt)
		{
			this.qt = qt;
		}
		public void Start() 
		{
			qt.Initialize();
		}

		public void Update()
		{
			//Move stuff
			qt.Update();
			//query for each agent
		}
	}
}
