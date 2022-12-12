namespace qt_benchmark
{
    interface ITest
    {
        void Reset(int seed, IQuadTreeService qt, Map map, int agentAmount, float agentRadius, float agentSpeed);
        void Update(out int actualChecks, out int totalChecks, out int bufferSize);
    }
}
