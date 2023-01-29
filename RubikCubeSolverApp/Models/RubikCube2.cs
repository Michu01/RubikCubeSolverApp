using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RubikCubeSolverApp.Enums;
using RubikCubeSolverApp.Extensions;

namespace RubikCubeSolverApp.Models
{
    public class RubikCube2
    {
        public const int FaceCount = 6;

        public const int FacePieceCount = 9;

        public const int PieceCount = FaceCount * FacePieceCount;

        public readonly char[] value = new char[FaceCount * FacePieceCount];

        private readonly IReadOnlyList<Action> operations;

        private readonly Stack<OperationType> operationHistory = new();

        public event Action<int, char>? PieceChanged;

        public static (int, int)[] EdgeIndices => new (int, int)[]
        {
            (1, 37), (3, 10), (5, 28), (7, 19),
            (46, 25), (48, 16), (50, 34), (52, 43),
            (14, 21), (23, 30), (32, 39), (41, 12)
        };

        public RubikCube2()
        {
            operations = new Action[]
            {
                U, U2, UI, E, EI, D, D2, DI, F, F2, FI, S, SI, B, B2, BI, L, L2, LI, M, MI, R, R2, RI
            };

            Reset();
        }

        public RubikCube2(RubikCube2 rubikCube)
        {
            operations = new Action[]
            {
                U, U2, UI, E, EI, D, D2, DI, F, F2, FI, S, SI, B, B2, BI, L, L2, LI, M, MI, R, R2, RI
            };

            value = rubikCube.value.ToArray();
        }

        public RubikCube2(string values)
        {
            operations = new Action[]
            {
                U, U2, UI, E, EI, D, D2, DI, F, F2, FI, S, SI, B, B2, BI, L, L2, LI, M, MI, R, R2, RI
            };

            value = values.ToCharArray();
        }

        public char GetValue(int index)
        {
            return value[index];
        }

        public char GetValue(FaceType face, PieceType piece)
        {
            return value[(int)face * FacePieceCount + (int)piece];
        }

        private char GetValue(FaceType face, int piece)
        {
            return value[(int)face * FacePieceCount + piece];
        }

        private char GetValue(int face, PieceType piece)
        {
            return value[face * FacePieceCount + (int)piece];
        }

        private char GetValue(int face, int piece)
        {
            return value[face * FacePieceCount + piece];
        }

        private IList<char> GetValues(FaceType face, IList<int> indices)
        {
            IList<char> values = new List<char>();

            foreach (var index in indices)
            {
                char value = GetValue(face, index);
                values.Add(value);
            }

            return values;
        }

        private void SetValues(FaceType face, IList<int> indices, IList<char> values)
        {
            foreach (int i in Enumerable.Range(0, indices.Count))
            {
                SetValue(face, indices[i], values[i]);
            }
        }

        private void SetValue(FaceType face, PieceType piece, char value)
        {
            SetValue(face, (int)piece, value);
        }

        private void SetValue(FaceType face, int piece, char value)
        {
            SetValue((int)face * FacePieceCount + piece, value);
        }

        public void SetValue(int index, char value)
        {
            this.value[index] = value;
            PieceChanged?.Invoke(index, value);
        }

        private void TurnFaceClockwise(FaceType face)
        {
            char topLeft = GetValue(face, PieceType.TopLeft);
            char top = GetValue(face, PieceType.Top);
            char topRight = GetValue(face, PieceType.TopRight);
            char left = GetValue(face, PieceType.Left);
            char right = GetValue(face, PieceType.Right);
            char bottomLeft = GetValue(face, PieceType.BottomLeft);
            char bottom = GetValue(face, PieceType.Bottom);
            char bottomRight = GetValue(face, PieceType.BottomRight);

            SetValue(face, PieceType.TopLeft, bottomLeft);
            SetValue(face, PieceType.Top, left);
            SetValue(face, PieceType.TopRight, topLeft);
            SetValue(face, PieceType.Left, bottom);
            SetValue(face, PieceType.Right, top);
            SetValue(face, PieceType.BottomLeft, bottomRight);
            SetValue(face, PieceType.Bottom, right);
            SetValue(face, PieceType.BottomRight, topRight);
        }

        private void TurnFaceCounterClockwise(FaceType face)
        {
            char topLeft = GetValue(face, PieceType.TopLeft);
            char top = GetValue(face, PieceType.Top);
            char topRight = GetValue(face, PieceType.TopRight);
            char left = GetValue(face, PieceType.Left);
            char right = GetValue(face, PieceType.Right);
            char bottomLeft = GetValue(face, PieceType.BottomLeft);
            char bottom = GetValue(face, PieceType.Bottom);
            char bottomRight = GetValue(face, PieceType.BottomRight);

            SetValue(face, PieceType.TopLeft, topRight);
            SetValue(face, PieceType.Top, right);
            SetValue(face, PieceType.TopRight, bottomRight);
            SetValue(face, PieceType.Left, top);
            SetValue(face, PieceType.Right, bottom);
            SetValue(face, PieceType.BottomLeft, topLeft);
            SetValue(face, PieceType.Bottom, left);
            SetValue(face, PieceType.BottomRight, bottomLeft);
        }

        private static IList<int> GetXLayerIndices(FaceType face, XLayer layer)
        {
            int i = (int)layer;

            return face switch
            {
                FaceType.Top or FaceType.Bottom or FaceType.Front => new[]
                {
                    6 + i,
                    3 + i,
                    i
                },
                FaceType.Back => new[]
                {
                    2 - i,
                    5 - i,
                    8 - i
                },
                FaceType.Left or FaceType.Right => throw new InvalidOperationException(),
                _ => throw new NotImplementedException()
            };
        }

        private static IList<int> GetYLayerIndices(FaceType face, YLayer layer)
        {
            int i = (int)layer;

            return face switch
            {
                FaceType.Front or FaceType.Left or FaceType.Back or FaceType.Right => new[]
                {
                    3 * i,
                    3 * i + 1,
                    3 * i + 2
                },
                FaceType.Top or FaceType.Bottom => throw new InvalidOperationException(),
                _ => throw new NotImplementedException()
            };
        }

        private static IList<int> GetZLayerIndices(FaceType face, ZLayer layer)
        {
            int i = (int)layer;

            return face switch
            {
                FaceType.Top => new[]
                {
                    6 - 3 * i,
                    7 - 3 * i,
                    8 - 3 * i
                },
                FaceType.Right => new[]
                {
                    i,
                    3 + i,
                    6 + i
                },
                FaceType.Bottom => new[]
                {
                    2 + 3 * i,
                    1 + 3 * i,
                    3 * i
                },
                FaceType.Left => new[]
                {
                    8 - i,
                    5 - i,
                    2 - i
                },
                FaceType.Front or FaceType.Back => throw new InvalidOperationException(),
                _ => throw new NotImplementedException()
            };
        }

        private IList<char> GetXLayer(FaceType face, XLayer layer)
        {
            IList<int> indices = GetXLayerIndices(face, layer);
            return GetValues(face, indices);
        }

        private IList<char> GetYLayer(FaceType face, YLayer layer)
        {
            IList<int> indices = GetYLayerIndices(face, layer);
            return GetValues(face, indices);
        }

        private IList<char> GetZLayer(FaceType face, ZLayer layer)
        {
            IList<int> indices = GetZLayerIndices(face, layer);
            return GetValues(face, indices);
        }

        private void SetXLayer(XLayer layer, FaceType face, IList<char> values)
        {
            IList<int> layerIndices = GetXLayerIndices(face, layer);

            SetValues(face, layerIndices, values);
        }

        private void SetYLayer(YLayer layer, FaceType face, IList<char> values)
        {
            IList<int> layerIndices = GetYLayerIndices(face, layer);

            SetValues(face, layerIndices, values);
        }

        private void SetZLayer(ZLayer layer, FaceType face, IList<char> values)
        {
            IList<int> layerIndices = GetZLayerIndices(face, layer);

            SetValues(face, layerIndices, values);
        }

        private void TurnXLayerClockwise(XLayer layer)
        {
            IList<char> topLayer = GetXLayer(FaceType.Top, layer);
            IList<char> backLayer = GetXLayer(FaceType.Back, layer);
            IList<char> bottomLayer = GetXLayer(FaceType.Bottom, layer);
            IList<char> frontLayer = GetXLayer(FaceType.Front, layer);

            SetXLayer(layer, FaceType.Top, frontLayer);
            SetXLayer(layer, FaceType.Back, topLayer);
            SetXLayer(layer, FaceType.Bottom, backLayer);
            SetXLayer(layer, FaceType.Front, bottomLayer);
        }

        private void TurnXLayerCounterClockwise(XLayer layer)
        {
            IList<char> topLayer = GetXLayer(FaceType.Top, layer);
            IList<char> backLayer = GetXLayer(FaceType.Back, layer);
            IList<char> bottomLayer = GetXLayer(FaceType.Bottom, layer);
            IList<char> frontLayer = GetXLayer(FaceType.Front, layer);

            SetXLayer(layer, FaceType.Top, backLayer);
            SetXLayer(layer, FaceType.Back, bottomLayer);
            SetXLayer(layer, FaceType.Bottom, frontLayer);
            SetXLayer(layer, FaceType.Front, topLayer);
        }

        private void TurnYLayerClockwise(YLayer layer)
        {
            IList<char> frontLayer = GetYLayer(FaceType.Front, layer);
            IList<char> leftLayer = GetYLayer(FaceType.Left, layer);
            IList<char> backLayer = GetYLayer(FaceType.Back, layer);
            IList<char> rightLayer = GetYLayer(FaceType.Right, layer);
           
            SetYLayer(layer, FaceType.Front, rightLayer);
            SetYLayer(layer, FaceType.Left, frontLayer);
            SetYLayer(layer, FaceType.Back, leftLayer);
            SetYLayer(layer, FaceType.Right, backLayer);
        }

        private void TurnYLayerCounterClockwise(YLayer layer)
        {
            IList<char> frontLayer = GetYLayer(FaceType.Front, layer);
            IList<char> leftLayer = GetYLayer(FaceType.Left, layer);
            IList<char> backLayer = GetYLayer(FaceType.Back, layer);
            IList<char> rightLayer = GetYLayer(FaceType.Right, layer);

            SetYLayer(layer, FaceType.Front, leftLayer);
            SetYLayer(layer, FaceType.Left, backLayer);
            SetYLayer(layer, FaceType.Back, rightLayer);
            SetYLayer(layer, FaceType.Right, frontLayer);
        }

        private void TurnZLayerClockwise(ZLayer layer)
        {
            IList<char> topLayer = GetZLayer(FaceType.Top, layer);
            IList<char> rightLayer = GetZLayer(FaceType.Right, layer);
            IList<char> bottomLayer = GetZLayer(FaceType.Bottom, layer);
            IList<char> leftLayer = GetZLayer(FaceType.Left, layer);

            SetZLayer(layer, FaceType.Top, leftLayer);
            SetZLayer(layer, FaceType.Right, topLayer);
            SetZLayer(layer, FaceType.Bottom, rightLayer);
            SetZLayer(layer, FaceType.Left, bottomLayer);
        }

        private void TurnZLayerCounterClockwise(ZLayer layer)
        {
            IList<char> topLayer = GetZLayer(FaceType.Top, layer);
            IList<char> rightLayer = GetZLayer(FaceType.Right, layer);
            IList<char> bottomLayer = GetZLayer(FaceType.Bottom, layer);
            IList<char> leftLayer = GetZLayer(FaceType.Left, layer);

            SetZLayer(layer, FaceType.Top, rightLayer);
            SetZLayer(layer, FaceType.Right, bottomLayer);
            SetZLayer(layer, FaceType.Bottom, leftLayer);
            SetZLayer(layer, FaceType.Left, topLayer);
        }

        public void Undo()
        {
            if (operationHistory.TryPop(out OperationType latest))
            {
                OperationType opposite = latest.GetOpposite();
                operations[(int)opposite]();
                operationHistory.Pop();
            }
        }

        public void Reset()
        {
            foreach (int i in Enumerable.Range(0, FaceCount))
            {
                foreach (int j in Enumerable.Range(0, FacePieceCount))
                {
                    SetValue(i * FacePieceCount + j, (char)(i + '0'));
                }
            }

            operationHistory.Clear();
        }

        public override string ToString()
        {
            return new(value);
        }

        public string ToPrettyString()
        {
            StringBuilder stringBuilder = new();

            for (int i = 0; i < 3; ++i)
            {
                stringBuilder.Append("   ");
                for (int j = 0; j < 3; ++j)
                {
                    char c = GetValue(0, i * 3 + j);
                    stringBuilder.Append(c);
                }
                stringBuilder.Append('\n');
            }

            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 12; ++j)
                {
                    char c = GetValue(j / 3 + 1, 3 * i + j % 3);
                    stringBuilder.Append(c);
                }
                stringBuilder.Append('\n');
            }

            for (int i = 0; i < 3; ++i)
            {
                stringBuilder.Append("   ");
                for (int j = 0; j < 3; ++j)
                {
                    char c = GetValue(5, i * 3 + j);
                    stringBuilder.Append(c);
                }
                stringBuilder.Append('\n');
            }

            return stringBuilder.ToString();
        }

        public override bool Equals(object? obj)
        {
            return ToString() == obj?.ToString();
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public void Randomize()
        {
            Action[] ops = new[] { F, B, L, R, U, D, M, S, E };
            Random random = new();

            foreach (int _ in Enumerable.Range(0, 20))
            {
                int rand = random.Next(ops.Length);
                ops[rand]();
            }
        }

        public void U()
        {
            TurnFaceClockwise(FaceType.Top);
            TurnYLayerClockwise(YLayer.Top);
        }

        public void U2()
        {
            TurnFaceClockwise(FaceType.Top);
            TurnYLayerClockwise(YLayer.Top);
            TurnFaceClockwise(FaceType.Top);
            TurnYLayerClockwise(YLayer.Top);
        }

        public void UI()
        {
            TurnFaceCounterClockwise(FaceType.Top);
            TurnYLayerCounterClockwise(YLayer.Top);
        }

        public void D()
        {
            TurnFaceClockwise(FaceType.Bottom);
            TurnYLayerCounterClockwise(YLayer.Bottom);
        }

        public void D2()
        {
            TurnFaceClockwise(FaceType.Bottom);
            TurnYLayerCounterClockwise(YLayer.Bottom);
            TurnFaceClockwise(FaceType.Bottom);
            TurnYLayerCounterClockwise(YLayer.Bottom);
        }

        public void DI()
        {
            TurnFaceCounterClockwise(FaceType.Bottom);
            TurnYLayerClockwise(YLayer.Bottom);
        }

        public void B()
        {
            TurnFaceClockwise(FaceType.Back);
            TurnZLayerCounterClockwise(ZLayer.Back);
        }

        public void B2()
        {
            TurnFaceClockwise(FaceType.Back);
            TurnZLayerCounterClockwise(ZLayer.Back); 
            TurnFaceClockwise(FaceType.Back);
            TurnZLayerCounterClockwise(ZLayer.Back);
        }

        public void BI()
        {
            TurnFaceCounterClockwise(FaceType.Back);
            TurnZLayerClockwise(ZLayer.Back);
        }

        public void F()
        {
            TurnFaceClockwise(FaceType.Front);
            TurnZLayerClockwise(ZLayer.Front);
        }

        public void F2()
        {
            TurnFaceClockwise(FaceType.Front);
            TurnZLayerClockwise(ZLayer.Front);
            TurnFaceClockwise(FaceType.Front);
            TurnZLayerClockwise(ZLayer.Front);
        }

        public void FI()
        {
            TurnFaceCounterClockwise(FaceType.Front);
            TurnZLayerCounterClockwise(ZLayer.Front);
        }

        public void L()
        {
            TurnFaceClockwise(FaceType.Left);
            TurnXLayerCounterClockwise(XLayer.Left);
        }

        public void L2()
        {
            TurnFaceClockwise(FaceType.Left);
            TurnXLayerCounterClockwise(XLayer.Left);
            TurnFaceClockwise(FaceType.Left);
            TurnXLayerCounterClockwise(XLayer.Left);
        }

        public void LI()
        {
            TurnFaceCounterClockwise(FaceType.Left);
            TurnXLayerClockwise(XLayer.Left);
        }

        public void R()
        {
            TurnFaceClockwise(FaceType.Right);
            TurnXLayerClockwise(XLayer.Right);
        }

        public void R2()
        {
            TurnFaceClockwise(FaceType.Right);
            TurnXLayerClockwise(XLayer.Right);
            TurnFaceClockwise(FaceType.Right);
            TurnXLayerClockwise(XLayer.Right);
        }

        public void RI()
        {
            TurnFaceCounterClockwise(FaceType.Right);
            TurnXLayerCounterClockwise(XLayer.Right);
        }

        public void E()
        {
            TurnYLayerCounterClockwise(YLayer.Middle);
        }

        public void EI()
        {
            TurnYLayerClockwise(YLayer.Middle);
        }

        public void M()
        {
            TurnXLayerCounterClockwise(XLayer.Middle);
        }

        public void MI()
        {
            TurnXLayerClockwise(XLayer.Middle);
        }

        public void S()
        {
            TurnZLayerClockwise(ZLayer.Middle);
        }

        public void SI()
        {
            TurnZLayerCounterClockwise(ZLayer.Middle);
        }

        public void X()
        {
            throw new NotImplementedException();
        }

        public void XI()
        {
            throw new NotImplementedException();
        }

        public void Y()
        {
            throw new NotImplementedException();
        }

        public void YI()
        {
            throw new NotImplementedException();
        }

        public void Z()
        {
            throw new NotImplementedException();
        }

        public void ZI()
        {
            throw new NotImplementedException();
        }

        public void MakeOperation(OperationType operationType)
        {
            operations[(int)operationType]();
        }

        public bool IsSolved()
        {
            foreach (var face in Enumerable.Range(0, FaceCount))
            {
                foreach (var piece in Enumerable.Range(0, FacePieceCount))
                {
                    if (GetValue(face, piece) != GetValue(face, PieceType.Middle))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}