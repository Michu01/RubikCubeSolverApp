using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RubikCubeSolverApp.Enums;
using RubikCubeSolverApp.Models;

namespace RubikCubeSolverApp.Services
{
    public class RubikCubeSolver2
    {
        private static RubikCube2 MaskG0(RubikCube2 cube)
        {
            RubikCube2 copy = new(cube);

            foreach (var edge in RubikCube2.EdgeIndices)
            {
                if (cube.GetValue(edge.Item1) == cube.GetValue(FaceType.Top, PieceType.Middle) ||
                    cube.GetValue(edge.Item1) == cube.GetValue(FaceType.Bottom, PieceType.Middle))
                {
                    copy.SetValue(edge.Item1, 'o');
                    continue;
                }

                if (cube.GetValue(edge.Item2) == cube.GetValue(FaceType.Top, PieceType.Middle) ||
                    cube.GetValue(edge.Item2) == cube.GetValue(FaceType.Bottom, PieceType.Middle))
                {
                    copy.SetValue(edge.Item2, 'o');
                    continue;
                }

                if (cube.GetValue(edge.Item1) == cube.GetValue(FaceType.Front, PieceType.Middle) ||
                    cube.GetValue(edge.Item1) == cube.GetValue(FaceType.Back, PieceType.Middle))
                {
                    copy.SetValue(edge.Item1, 'o');
                    continue;
                }

                if (cube.GetValue(edge.Item2) == cube.GetValue(FaceType.Front, PieceType.Middle) ||
                    cube.GetValue(edge.Item2) == cube.GetValue(FaceType.Back, PieceType.Middle))
                {
                    copy.SetValue(edge.Item2, 'o');
                    continue;
                }
            }

            foreach (int i in Enumerable.Range(0, RubikCube2.PieceCount))
            {
                if (copy.GetValue(i) != 'o')
                {
                    copy.SetValue(i, 'X');
                }
            }

            return copy;
        }

        private static RubikCube2 MaskG1(RubikCube2 cube)
        {
            RubikCube2 copy = new(cube);

            foreach (var edge in RubikCube2.EdgeIndices)
            {
                if (cube.GetValue(edge.Item1) == cube.GetValue(FaceType.Top, PieceType.Middle) ||
                    cube.GetValue(edge.Item1) == cube.GetValue(FaceType.Bottom, PieceType.Middle))
                {
                    continue;
                }

                if (cube.GetValue(edge.Item2) == cube.GetValue(FaceType.Top, PieceType.Middle) ||
                    cube.GetValue(edge.Item2) == cube.GetValue(FaceType.Bottom, PieceType.Middle))
                {
                    continue;
                }

                if (cube.GetValue(edge.Item1) == cube.GetValue(FaceType.Front, PieceType.Middle) ||
                    cube.GetValue(edge.Item1) == cube.GetValue(FaceType.Back, PieceType.Middle))
                {
                    copy.SetValue(edge.Item1, 'y');
                    continue;
                }

                if (cube.GetValue(edge.Item2) == cube.GetValue(FaceType.Front, PieceType.Middle) ||
                    cube.GetValue(edge.Item2) == cube.GetValue(FaceType.Back, PieceType.Middle))
                {
                    copy.SetValue(edge.Item2, 'y');
                    continue;
                }
            }

            foreach (int i in Enumerable.Range(0, RubikCube2.PieceCount))
            {
                if ((i - 4) % 9 == 0)
                {
                    copy.SetValue(i, 'X');
                    continue;
                }

                if (cube.GetValue(i) == cube.GetValue(FaceType.Top, PieceType.Middle) ||
                    cube.GetValue(i) == cube.GetValue(FaceType.Bottom, PieceType.Middle))
                {
                    copy.SetValue(i, 'x');
                    continue;
                }

                if (copy.GetValue(i) != 'y')
                {
                    copy.SetValue(i, 'X');
                    continue;
                }
            }

            return copy;
        }

        private static RubikCube2 MaskG2(RubikCube2 cube)
        {
            RubikCube2 masked = new(cube);

            Dictionary<char, char> cornerMap = new()
            {
                { cube.GetValue(FaceType.Top, PieceType.Middle), '0' },
                { cube.GetValue(FaceType.Left, PieceType.Middle), '1' },
                { cube.GetValue(FaceType.Front, PieceType.Middle), '2' },
                { cube.GetValue(FaceType.Right, PieceType.Middle), '3' },
                { cube.GetValue(FaceType.Back, PieceType.Middle), '4' },
                { cube.GetValue(FaceType.Bottom, PieceType.Middle), '5' }
            };

            Dictionary<char, char> edgeMap = new()
            {
                { cube.GetValue(FaceType.Top, PieceType.Middle), 'X' },
                { cube.GetValue(FaceType.Left, PieceType.Middle), '1' },
                { cube.GetValue(FaceType.Front, PieceType.Middle), '2' },
                { cube.GetValue(FaceType.Right, PieceType.Middle), '1' },
                { cube.GetValue(FaceType.Back, PieceType.Middle), '2' },
                { cube.GetValue(FaceType.Bottom, PieceType.Middle), 'X' }
            };

            foreach (int i in Enumerable.Range(0, RubikCube2.PieceCount))
            {
                if ((i - 4) % 9 == 0) //middle
                {
                    masked.SetValue(i, 'X');
                    continue;
                }

                if (i % 9 % 2 == 1) //edge
                {
                    char c = masked.GetValue(i);
                    char cMasked = edgeMap[c];
                    masked.SetValue(i, cMasked);
                    continue;
                }

                if (i % 9 % 2 == 0) //corner
                {
                    char c = masked.GetValue(i);
                    char cMasked = cornerMap[c];
                    masked.SetValue(i, cMasked);
                    continue;
                }

                throw new NotImplementedException();
            }

            return masked;
        }

        private static RubikCube2 MaskG3(RubikCube2 cube)
        {
            RubikCube2 masked = new(cube);

            Dictionary<char, char> map = new()
            {
                { cube.GetValue(FaceType.Top, PieceType.Middle), '0' },
                { cube.GetValue(FaceType.Left, PieceType.Middle), '1' },
                { cube.GetValue(FaceType.Front, PieceType.Middle), '2' },
                { cube.GetValue(FaceType.Right, PieceType.Middle), '3' },
                { cube.GetValue(FaceType.Back, PieceType.Middle), '4' },
                { cube.GetValue(FaceType.Bottom, PieceType.Middle), '5' }
            };

            foreach (int i in Enumerable.Range(0, RubikCube2.PieceCount))
            {
                char c = cube.GetValue(i);
                char mc = map[c];
                masked.SetValue(i, mc);
            }

            return masked;
        }

        private static IEnumerable<OperationType> SolveG(RubikCube2 cube, Func<RubikCube2, RubikCube2> mask, Func<IEnumerable<(RubikCube2, IEnumerable<OperationType>)>> loadTable)
        {
            RubikCube2 masked = mask(cube);

            IEnumerable<OperationType> solution = loadTable().Single(p => masked.Equals(p.Item1)).Item2;

            foreach (OperationType operation in solution)
            {
                cube.MakeOperation(operation);
            }

            return solution;
        }

        public static IEnumerable<OperationType> SolveG0(RubikCube2 cube)
        {
            return SolveG(cube, MaskG0, PruningTableFileManager.LoadG0);
        }

        public static IEnumerable<OperationType> SolveG1(RubikCube2 cube)
        {
            return SolveG(cube, MaskG1, PruningTableFileManager.LoadG1);
        }

        public static IEnumerable<OperationType> SolveG2(RubikCube2 cube)
        {
            return SolveG(cube, MaskG2, PruningTableFileManager.LoadG2);
        }

        public static IEnumerable<OperationType> SolveG3(RubikCube2 cube)
        {
            return SolveG(cube, MaskG3, PruningTableFileManager.LoadG3);
        }

        public static IEnumerable<OperationType> Solve(RubikCube2 cube)
        {
            var opsG0 = SolveG0(cube);
            var opsG1 = SolveG1(cube);
            var opsG2 = SolveG2(cube);
            var opsG3 = SolveG3(cube);

            var ops = opsG0.Concat(opsG1).Concat(opsG2).Concat(opsG3);

            Debug.WriteLine($"G0: {opsG0.Count()} operations; {string.Join(", ", opsG0)}");
            Debug.WriteLine($"G1: {opsG1.Count()} operations; {string.Join(", ", opsG1)}");
            Debug.WriteLine($"G2: {opsG2.Count()} operations; {string.Join(", ", opsG2)}");
            Debug.WriteLine($"G3: {opsG3.Count()} operations; {string.Join(", ", opsG3)}");
            Debug.WriteLine($"Total: {ops.Count()} operations; {string.Join(", ", ops)}");

            return ops;
        }
    }
}