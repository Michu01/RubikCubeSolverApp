using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

using RubikCubeSolverApp.Enums;
using RubikCubeSolverApp.Models;

namespace RubikCubeSolverApp.Services
{
    public static class PruningTableFileManager
    {
        static PruningTableFileManager()
        {
            Directory.CreateDirectory("tables");
        }

        private static void Generate(string filename, Func<Dictionary<RubikCube2, IList<OperationType>>> gen)
        {
            var g = gen();
            var s = g.Select(g => g.Key.ToString() + " " + string.Join(' ', g.Value));
            File.WriteAllLines(filename, s);
        }

        private static void GenerateG0()
        {
            Generate("tables/G0.txt", PruningTableGenerator.G0);
        }

        private static void GenerateG1()
        {
            Generate("tables/G1.txt", PruningTableGenerator.G1);
        }

        private static void GenerateG2()
        {
            Generate("tables/G2.txt", PruningTableGenerator.G2);
        }

        private static void GenerateG3()
        {
            Generate("tables/G3.txt", PruningTableGenerator.G3);
        }

        public static void AssureGenerated()
        {
            if (!File.Exists("tables/G0.txt"))
            {
                GenerateG0();
            }

            if (!File.Exists("tables/G1.txt"))
            {
                GenerateG1();
            }

            if (!File.Exists("tables/G2.txt"))
            {
                GenerateG2();
            }

            if (!File.Exists("tables/G3.txt"))
            {
                GenerateG3();
            }
        }

        private static IEnumerable<(RubikCube2, IEnumerable<OperationType>)> Load(string filename)
        {
            return File
                .ReadLines(filename)
                .Select(line => line.Split(' ', 2))
                .Select(words =>
                    (new RubikCube2(words[0]),
                    words[1].Length == 0 ? Enumerable.Empty<OperationType>() : words[1]
                        .Split()
                        .Select(s => Enum.Parse<OperationType>(s))
                    )
                );
        }

        public static IEnumerable<(RubikCube2, IEnumerable<OperationType>)> LoadG0()
        {
            return Load("tables/G0.txt");
        }

        public static IEnumerable<(RubikCube2, IEnumerable<OperationType>)> LoadG1()
        {
            return Load("tables/G1.txt");
        }

        public static IEnumerable<(RubikCube2, IEnumerable<OperationType>)> LoadG2()
        {
            return Load("tables/G2.txt");
        }

        public static IEnumerable<(RubikCube2, IEnumerable<OperationType>)> LoadG3()
        {
            return Load("tables/G3.txt");
        }
    }
}
