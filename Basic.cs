using System.Collections.Generic;
using System.Numerics;

namespace qt_benchmark
{
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
}
