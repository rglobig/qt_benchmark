using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using qt_benchmark.QuadTree.Services.v1;
using qt_benchmark.QuadTree.Services.v2;

namespace qt_benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> results = new List<string>();

            const int calcsPerService = 10;
            const int agentAmount = 500;
            const int size = 100;
            const int ticksPerCalc = 100;
            const int seed = 1337;
            const float agentRadius = 0.5f;
            const float agentSpeed = 1f;

            var services = new IQuadTreeService[]
            {
                new Basic(),
                new QuadTree.Services.v1.QuadTree(new WorldPosition(size * 0.5, size * 0.5f), new Size(size, size), agentAmount, 16, 4),
                new QuadTree.Services.v2.QuadTree(new Square(new Vector2(0, 0), size))
            };

            //dry run
            Console.WriteLine("Dry run...");
            RunTest(print: false, new SimpleTest(), services, results, calculations: 2, ticks: 10);
            RunTest(print: false, new PrimeTest(), services, results, calculations: 2, ticks: 10);
            results.Clear();
            Console.Clear();
            RunTest(print: true, new SimpleTest(), services, results, calcsPerService, ticksPerCalc);
            RunTest(print: true, new PrimeTest(), services, results, calcsPerService, ticksPerCalc);
            Console.Clear();
            results.ForEach(Console.WriteLine);

            Console.ReadLine();

            static void RunTest(bool print, ITest test, IQuadTreeService[] services, List<string> results, int calculations, int ticks)
            {
                if (print)
                {
                    var line = "=========";
                    var testFor = $"TEST FOR: {test.GetType()}";
                    Console.WriteLine(testFor);
                    results.Add(line);
                    results.Add(testFor);
                    results.Add(line);
                }
                var run = 0;
                for (int i = 0; i < services.Length; i++)
                {
                    var qt = services[i];
                    for (int j = 0; j < calculations; j++)
                    {
                        run++;

                        test.Reset(seed: seed,
                            qt: qt,
                            map: new Map { sizeX = size, sizeY = size },
                            agentAmount: agentAmount,
                            agentRadius: agentRadius,
                            agentSpeed: agentSpeed);

                        var total = 0L;
                        var average = 0L;
                        var highest = 0L;
                        var lowest = long.MaxValue;
                        if (print) Console.WriteLine($"Calc... [{run}/{calculations * services.Length}] => {qt.GetType()}");
                        var watch = new Stopwatch();

                        var averageChecks = 0f;
                        var checksInSum = 0f;

                        for (int x = 0; x < ticks; x++)
                        {
                            var actualCheck = 0;
                            var totalChecks = 0;
                            watch.Start();
                            test.Update(out actualCheck, out totalChecks);
                            watch.Stop();

                            checksInSum += (actualCheck / (float)totalChecks);
                            averageChecks = checksInSum / (x + 1);

                            var current = watch.ElapsedTicks;
                            total += current;
                            average = total / (x + 1);
                            if (current > highest) highest = current;
                            if (current < lowest) lowest = current;

                            watch.Reset();
                        }
                        var aOutput = ((double)average / TimeSpan.TicksPerMillisecond).ToString("0.000");
                        var hOutput = ((double)highest / TimeSpan.TicksPerMillisecond).ToString("0.000");
                        var lOutput = ((double)lowest / TimeSpan.TicksPerMillisecond).ToString("0.000");
                        results.Add($"{qt.GetType()} Average: {aOutput}ms / Highest: {hOutput}ms / Lowest: {lOutput}ms / Checks: {averageChecks:P2}");
                    }
                }
            }
        }
    }
}
