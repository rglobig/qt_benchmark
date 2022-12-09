namespace qt_benchmark
{
    interface ITest
    {
        void Start(int seed, IQuadTreeService qt, Map map, int agentAmount, float agentRadius, float agentSpeed);
        void Update();
    }
}
