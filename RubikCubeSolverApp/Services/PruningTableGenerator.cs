using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

using RubikCubeSolverApp.Enums;
using RubikCubeSolverApp.Extensions;
using RubikCubeSolverApp.Models;

namespace RubikCubeSolverApp.Services
{
    public static class PruningTableGenerator
    {
        private static readonly OperationType[] G0_Operations = new OperationType[] 
        { 
            OperationType.F, OperationType.FI, OperationType.F2, 
            OperationType.B, OperationType.BI, OperationType.B2,
            OperationType.U, OperationType.UI, OperationType.U2,
            OperationType.D, OperationType.DI, OperationType.D2,
            OperationType.L, OperationType.LI, OperationType.L2,
            OperationType.R, OperationType.RI, OperationType.R2,
        };

        private static readonly OperationType[] G1_Operations = new OperationType[]
        {
            OperationType.F2,
            OperationType.B2,
            OperationType.U, OperationType.UI, OperationType.U2,
            OperationType.D, OperationType.DI, OperationType.D2,
            OperationType.L, OperationType.LI, OperationType.L2,
            OperationType.R, OperationType.RI, OperationType.R2,
        };

        private static readonly OperationType[] G2_Operations = new OperationType[]
        {
            OperationType.F2,
            OperationType.B2,
            OperationType.U, OperationType.UI, OperationType.U2,
            OperationType.D, OperationType.DI, OperationType.D2,
            OperationType.L2,
            OperationType.R2,
        };

        private static readonly OperationType[] G3_Operations = new OperationType[]
        {
            OperationType.F2,
            OperationType.B2,
            OperationType.U2,
            OperationType.D2,
            OperationType.L2,
            OperationType.R2
        };

        private static readonly string G0_Mask = string.Join("", new string[] 
        {
            "XoXoXoXoX", "XXXXXXXXX", "XXXoXoXXX", "XXXXXXXXX", "XXXoXoXXX", "XoXoXoXoX"
        });

        private static readonly string G1_Mask = string.Join("", new string[]
        {
            "xxxxXxxxx", "XXXXXXXXX", "XXXyXyXXX", "XXXXXXXXX", "XXXyXyXXX", "xxxxXxxxx"
        });

        private static readonly string G2_Mask = string.Join("", new string[]
        {
            "0X0XXX0X0", "1111X1111", "2222X2222", "3131X1313", "4242X2424", "5X5XXX5X5"
        });

        private static readonly string G3_Mask = string.Join("", new string[]
        {
            "000000000", "111111111", "222222222", "333333333", "444444444", "555555555"
        });

        private static void G_DFS(
            RubikCube2 cube, 
            IEnumerable<OperationType> operations,
            OperationType[] availableOperations, 
            Dictionary<RubikCube2, IList<OperationType>> dict,
            int depth,
            int maxDepth,
            Stopwatch stopwatch,
            ref int i)
        {
            if (depth >= maxDepth)
            {
                return;
            }

            ++i;

            foreach (var operation in availableOperations)
            {
                List<OperationType> newOperations = operations.Prepend(operation.GetOpposite()).ToList();
                var newCube = new RubikCube2(cube);
                newCube.MakeOperation(operation);

                dict.TryAdd(newCube, newOperations);

                G_DFS(cube, operations, availableOperations, dict, depth + 1, maxDepth, stopwatch, ref i);
            }
        }

        private static void G_IDDFS(RubikCube2 cube,
            IEnumerable<OperationType> operations,
            OperationType[] availableOperations,
            Dictionary<RubikCube2, IList<OperationType>> dict,
            int maxDepth)
        {
            File.WriteAllText("G_DFS_Progress.txt", "");

            int i = 0;
            Stopwatch stopwatch = new();
            stopwatch.Start();

            using PeriodicTimer periodicTimer = new(TimeSpan.FromSeconds(5));
            Task.Run(async () =>
            {
                while (await periodicTimer.WaitForNextTickAsync())
                {
                    double max = (1 - Math.Pow(availableOperations.Length, maxDepth + 1)) / (1 - availableOperations.Length);
                    double progress = i / max;

                    File.AppendAllText("G_DFS_Progress.txt",
                        $"[{DateTime.Now}] Progress: {progress * 100:0.0}%, " +
                        $"elapsed: {stopwatch.Elapsed.TotalSeconds:0}s, " +
                        $"remaining: {stopwatch.Elapsed.TotalSeconds * (1 / progress - 1):0}s\n");
                }
            });

            for (int depth = 1; depth <= maxDepth; ++depth)
            {
                G_DFS(cube, operations, availableOperations, dict, 0, depth, stopwatch, ref i);
            }
        }

        public static Dictionary<RubikCube2, IList<OperationType>> G0_IDDFS()
        {
            RubikCube2 startCube = new(G1_Mask);
            IList<OperationType> startOperations = Array.Empty<OperationType>();

            Dictionary<RubikCube2, IList<OperationType>> dict = new()
            {
                { startCube, startOperations }
            };

            Stopwatch stopwatch = new();
            stopwatch.Start();

            G_IDDFS(startCube, startOperations, G0_Operations, dict, 6);

            File.WriteAllText("G0_IDDFS.txt", $"{stopwatch.Elapsed.TotalSeconds}s\n");
            File.AppendAllLines("G0_IDDFS.txt", dict.Select(p => $"{p.Key} {string.Join(", ", p.Value)}"));

            return dict;
        }

        private static Dictionary<RubikCube2, IList<OperationType>> G(string mask, OperationType[] availableOperations)
        {
            RubikCube2 startCube = new(mask);

            IList<OperationType> startOperations = Array.Empty<OperationType>();

            Dictionary<RubikCube2, IList<OperationType>> solutions = new()
            {
                { startCube, startOperations }
            };

            Queue<(RubikCube2, IList<OperationType>)> queue = new();

            queue.Enqueue((startCube, startOperations));

            while (queue.Count > 0)
            {
                (RubikCube2 cube, IList<OperationType> operations) = queue.Dequeue();

                foreach (OperationType operation in availableOperations)
                {
                    List<OperationType> newOperations = operations.Prepend(operation.GetOpposite()).ToList();
                    RubikCube2 newCube = new(cube);
                    newCube.MakeOperation(operation);

                    if (!solutions.ContainsKey(newCube))
                    {
                        solutions.Add(newCube, newOperations);
                        queue.Enqueue((newCube, newOperations));
                    }
                }
            }

            return solutions;
        }

        public static Dictionary<RubikCube2, IList<OperationType>> G0()
        {
            return G(G0_Mask, G0_Operations);
        }

        public static Dictionary<RubikCube2, IList<OperationType>> G1()
        {
            return G(G1_Mask, G1_Operations);
        }

        public static Dictionary<RubikCube2, IList<OperationType>> G2()
        {
            return G(G2_Mask, G2_Operations);
        }

        public static Dictionary<RubikCube2, IList<OperationType>> G3()
        {
            return G(G3_Mask, G3_Operations);
        }
    }
}
