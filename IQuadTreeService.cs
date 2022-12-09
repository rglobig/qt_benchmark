using System.Collections.Generic;

namespace qt_benchmark
{
    interface IQuadTreeService
    {
        void Reset();
        void Initialize();
        void Update();
        void Query(Agent agent, float radius, Dictionary<int, Agent> allAgents, HashSet<Agent> buffer);
        void Insert(Agent agent);
    }
}
