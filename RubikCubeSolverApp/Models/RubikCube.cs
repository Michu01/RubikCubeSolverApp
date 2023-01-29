using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RubikCubeSolverApp.Enums;
using RubikCubeSolverApp.Extensions;

namespace RubikCubeSolverApp.Models
{
    public class RubikCube
    {
        public const int FaceCount = 6;

        public event Action<Face, Piece>? FacePieceChanged;

        private readonly Action[] operations;

        private readonly Stack<OperationType> operationHistory = new();

        public Face[] Faces { get; } = new Face[FaceCount];

        public IEnumerable<Piece> Pieces => Faces.Select(f => f.Pieces).SelectMany(p => p);

        public IEnumerable<Piece> EdgePieces => Faces.Select(f => f.EdgePieces).SelectMany(p => p);

        public IEnumerable<Piece> CornerPieces => Faces.Select(f => f.CornerPieces).SelectMany(p => p);

        public Face TopFace => GetFace(FaceType.Top);

        public Face BottomFace => GetFace(FaceType.Bottom);

        public Face LeftFace => GetFace(FaceType.Left);

        public Face RightFace => GetFace(FaceType.Right);

        public Face FrontFace => GetFace(FaceType.Front);

        public Face BackFace => GetFace(FaceType.Back);

        public Face[] XAxisFaces => new[] { TopFace, BackFace, BottomFace, FrontFace };

        public Face[] YAxisFaces => new[] { FrontFace, LeftFace, BackFace, RightFace };

        public Face[] ZAxisFaces => new[] { TopFace, RightFace, BottomFace, LeftFace };

        public Middle FrontMiddle => new() { Piece = FrontFace.MiddlePiece };

        public Middle BackMiddle => new() { Piece = BackFace.MiddlePiece };

        public Middle TopMiddle => new() { Piece = TopFace.MiddlePiece };

        public Middle BottomMiddle => new() { Piece = BottomFace.MiddlePiece };

        public Middle LeftMiddle => new() { Piece = LeftFace.MiddlePiece };

        public Middle RightMiddle => new() { Piece = RightFace.MiddlePiece };

        public Edge FrontLeftEdge => new() { FirstPiece = FrontFace.LeftPiece, SecondPiece = LeftFace.RightPiece };

        public Edge FrontRightEdge => new() { FirstPiece = FrontFace.RightPiece, SecondPiece = RightFace.LeftPiece };

        public Edge FrontTopEdge => new() { FirstPiece = FrontFace.TopPiece, SecondPiece = TopFace.BottomPiece };

        public Edge FrontBottomEdge => new() { FirstPiece = FrontFace.BottomPiece, SecondPiece = BottomFace.TopPiece };

        public Edge BackLeftEdge => new() { FirstPiece = BackFace.RightPiece, SecondPiece = LeftFace.LeftPiece };

        public Edge BackRightEdge => new() { FirstPiece = BackFace.LeftPiece, SecondPiece = RightFace.RightPiece };

        public Edge BackTopEdge => new() { FirstPiece = BackFace.TopPiece, SecondPiece = TopFace.TopPiece };

        public Edge BackBottomEdge => new() { FirstPiece = BackFace.BottomPiece, SecondPiece = BottomFace.BottomPiece };

        public Edge TopLeftEdge => new() { FirstPiece = TopFace.LeftPiece, SecondPiece = LeftFace.TopPiece };

        public Edge TopRightEdge => new() { FirstPiece = TopFace.RightPiece, SecondPiece = RightFace.TopPiece };

        public Edge BottomLeftEdge => new() { FirstPiece = BottomFace.LeftPiece, SecondPiece = LeftFace.BottomPiece };

        public Edge BottomRightEdge => new() { FirstPiece = BottomFace.RightPiece, SecondPiece = RightFace.BottomPiece };

        public Corner FrontTopLeftCorner => new() { FirstPiece = FrontFace.TopLeftPiece, SecondPiece = TopFace.BottomLeftPiece, ThirdPiece = LeftFace.TopRightPiece };

        public Corner FrontTopRightCorner => new() { FirstPiece = FrontFace.TopRightPiece, SecondPiece = TopFace.BottomRightPiece, ThirdPiece = RightFace.TopLeftPiece };

        public Corner FrontBottomLeftCorner => new() { FirstPiece = FrontFace.BottomLeftPiece, SecondPiece = BottomFace.TopLeftPiece, ThirdPiece = LeftFace.BottomRightPiece };

        public Corner FrontBottomRightCorner => new() { FirstPiece = FrontFace.BottomRightPiece, SecondPiece = BottomFace.TopRightPiece, ThirdPiece = RightFace.BottomLeftPiece };

        public Corner BackTopLeftCorner => new() { FirstPiece = BackFace.TopRightPiece, SecondPiece = TopFace.TopLeftPiece, ThirdPiece = LeftFace.TopLeftPiece };

        public Corner BackTopRightCorner => new() { FirstPiece = BackFace.TopLeftPiece, SecondPiece = TopFace.TopRightPiece, ThirdPiece = RightFace.TopRightPiece };

        public Corner BackBottomLeftCorner => new() { FirstPiece = BackFace.BottomRightPiece, SecondPiece = BottomFace.BottomLeftPiece, ThirdPiece = LeftFace.BottomLeftPiece };

        public Corner BackBottomRightCorner => new() { FirstPiece = BackFace.BottomLeftPiece, SecondPiece = BottomFace.BottomRightPiece, ThirdPiece = RightFace.BottomRightPiece };

        public RubikCube()
        {
            foreach (int n in Enumerable.Range(0, FaceCount))
            {
                Faces[n] = new()
                {
                    Type = (FaceType)n
                };

                Faces[n].PieceChanged += RubikCube_PieceChanged;
            }

            operations = new Action[]
            {
                U, U2, UI, E, EI, D, D2, DI, F, F2, FI, S, SI, B, B2, BI, L, L2, LI, M, MI, R, R2, RI, X, XI, Y, YI, Z, ZI
            };

            Reset();
        }

        public RubikCube(string s)
            : this()
        {
            for (int i = 0; i != FaceCount; ++i)
            {
                for (int j = 0; j != Face.PieceCount; ++j)
                {
                    Faces[i].Pieces[j].ColorType = (ColorType)(s[i * Face.PieceCount + j] - '0');
                }
            }
        }

        public RubikCube(IList<string> faces)
            : this()
        {
            for (int i = 0; i != FaceCount; ++i)
            {
                for (int j = 0; j != Face.PieceCount; ++j)
                {
                    Faces[i].Pieces[j].ColorType = (ColorType)(faces[i][j] - '0');
                }
            }
        }

        public RubikCube(RubikCube cube)
            : this()
        {
            foreach (int i in Enumerable.Range(0, FaceCount))
            {
                foreach (int j in Enumerable.Range(0, Face.PieceCount))
                {
                    Faces[i].Pieces[j].ColorType = cube.Faces[i].Pieces[j].ColorType;
                }
            }
        }

        private void RubikCube_PieceChanged(Face face, Piece piece)
        {
            FacePieceChanged?.Invoke(face, piece);
        }

        private Face GetFace(FaceType faceType)
        {
            return Faces[(int)faceType];
        }

        private void TurnXLayerClockwise(XLayer layer)
        {
            IEnumerable<ColorType> topLayer = TopFace.GetXLayerColorTypes(layer);
            IEnumerable<ColorType> backLayer = BackFace.GetXLayerColorTypes(layer);
            IEnumerable<ColorType> bottomLayer = BottomFace.GetXLayerColorTypes(layer);
            IEnumerable<ColorType> frontLayer = FrontFace.GetXLayerColorTypes(layer);

            TopFace.SetXLayerColorTypes(layer, frontLayer);
            BackFace.SetXLayerColorTypes(layer, topLayer);
            BottomFace.SetXLayerColorTypes(layer, backLayer);
            FrontFace.SetXLayerColorTypes(layer, bottomLayer);
        }

        private void TurnXLayerCounterClockwise(XLayer layer)
        {
            IEnumerable<ColorType> topLayer = TopFace.GetXLayerColorTypes(layer);
            IEnumerable<ColorType> backLayer = BackFace.GetXLayerColorTypes(layer);
            IEnumerable<ColorType> bottomLayer = BottomFace.GetXLayerColorTypes(layer);
            IEnumerable<ColorType> frontLayer = FrontFace.GetXLayerColorTypes(layer);

            TopFace.SetXLayerColorTypes(layer, backLayer);
            BackFace.SetXLayerColorTypes(layer, bottomLayer);
            BottomFace.SetXLayerColorTypes(layer, frontLayer);
            FrontFace.SetXLayerColorTypes(layer, topLayer);
        }

        private void TurnYLayerClockwise(YLayer layer)
        {
            IEnumerable<ColorType> faceLayer = FrontFace.GetYLayerColorTypes(layer);
            IEnumerable<ColorType> leftLayer = LeftFace.GetYLayerColorTypes(layer);
            IEnumerable<ColorType> backLayer = BackFace.GetYLayerColorTypes(layer);
            IEnumerable<ColorType> rightLayer = RightFace.GetYLayerColorTypes(layer);

            FrontFace.SetYLayerColorTypes(layer, rightLayer);
            LeftFace.SetYLayerColorTypes(layer, faceLayer);
            BackFace.SetYLayerColorTypes(layer, leftLayer);
            RightFace.SetYLayerColorTypes(layer, backLayer);
        }

        private void TurnYLayerCounterClockwise(YLayer layer)
        {
            IEnumerable<ColorType> faceLayer = FrontFace.GetYLayerColorTypes(layer);
            IEnumerable<ColorType> leftLayer = LeftFace.GetYLayerColorTypes(layer);
            IEnumerable<ColorType> backLayer = BackFace.GetYLayerColorTypes(layer);
            IEnumerable<ColorType> rightLayer = RightFace.GetYLayerColorTypes(layer);

            FrontFace.SetYLayerColorTypes(layer, leftLayer);
            LeftFace.SetYLayerColorTypes(layer, backLayer);
            BackFace.SetYLayerColorTypes(layer, rightLayer);
            RightFace.SetYLayerColorTypes(layer, faceLayer);
        }

        private void TurnZLayerClockwise(ZLayer layer)
        {
            IEnumerable<ColorType> topLayer = TopFace.GetZLayerColorTypes(layer);
            IEnumerable<ColorType> rightLayer = RightFace.GetZLayerColorTypes(layer);
            IEnumerable<ColorType> bottomLayer = BottomFace.GetZLayerColorTypes(layer);
            IEnumerable<ColorType> leftLayer = LeftFace.GetZLayerColorTypes(layer);

            TopFace.SetZLayerColorTypes(layer, leftLayer);
            RightFace.SetZLayerColorTypes(layer, topLayer);
            BottomFace.SetZLayerColorTypes(layer, rightLayer);
            LeftFace.SetZLayerColorTypes(layer, bottomLayer);
        }

        private void TurnZLayerCounterClockwise(ZLayer layer)
        {
            IEnumerable<ColorType> topLayer = TopFace.GetZLayerColorTypes(layer);
            IEnumerable<ColorType> rightLayer = RightFace.GetZLayerColorTypes(layer);
            IEnumerable<ColorType> bottomLayer = BottomFace.GetZLayerColorTypes(layer);
            IEnumerable<ColorType> leftLayer = LeftFace.GetZLayerColorTypes(layer);

            TopFace.SetZLayerColorTypes(layer, rightLayer);
            RightFace.SetZLayerColorTypes(layer, bottomLayer);
            BottomFace.SetZLayerColorTypes(layer, leftLayer);
            LeftFace.SetZLayerColorTypes(layer, topLayer);
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
            foreach (Face face in Faces)
            {
                face.Reset();
            }

            operationHistory.Clear();
        }

        public void U()
        {
            TopFace.TurnClockwise();
            TurnYLayerClockwise(YLayer.Top);
            operationHistory.Push(OperationType.U);
        }

        public void U2()
        {
            TopFace.TurnClockwise();
            TurnYLayerClockwise(YLayer.Top);
            TopFace.TurnClockwise();
            TurnYLayerClockwise(YLayer.Top);
            operationHistory.Push(OperationType.U2);
        }

        public void UI()
        {
            TopFace.TurnCounterClockwise();
            TurnYLayerCounterClockwise(YLayer.Top);
            operationHistory.Push(OperationType.UI);
        }

        public void E()
        {
            TurnYLayerCounterClockwise(YLayer.Middle);
            operationHistory.Push(OperationType.E);
        }

        public void EI()
        {
            TurnYLayerClockwise(YLayer.Middle);
            operationHistory.Push(OperationType.EI);
        }

        public void D()
        {
            BottomFace.TurnClockwise();
            TurnYLayerCounterClockwise(YLayer.Bottom);
            operationHistory.Push(OperationType.D);
        }

        public void D2()
        {
            BottomFace.TurnClockwise();
            TurnYLayerCounterClockwise(YLayer.Bottom);
            BottomFace.TurnClockwise();
            TurnYLayerCounterClockwise(YLayer.Bottom);
            operationHistory.Push(OperationType.D2);
        }

        public void DI()
        {
            BottomFace.TurnCounterClockwise();
            TurnYLayerClockwise(YLayer.Bottom);
            operationHistory.Push(OperationType.DI);
        }

        public void F()
        {
            GetFace(FaceType.Front).TurnClockwise();
            TurnZLayerClockwise(ZLayer.Front);
            operationHistory.Push(OperationType.F);
        }

        public void F2()
        {
            GetFace(FaceType.Front).TurnClockwise();
            TurnZLayerClockwise(ZLayer.Front);
            GetFace(FaceType.Front).TurnClockwise();
            TurnZLayerClockwise(ZLayer.Front);
            operationHistory.Push(OperationType.F2);
        }

        public void FI()
        {
            GetFace(FaceType.Front).TurnCounterClockwise();
            TurnZLayerCounterClockwise(ZLayer.Front);
            operationHistory.Push(OperationType.FI);
        }

        public void S()
        {
            TurnZLayerClockwise(ZLayer.Middle);
            operationHistory.Push(OperationType.S);
        }

        public void SI()
        {
            TurnZLayerCounterClockwise(ZLayer.Middle);
            operationHistory.Push(OperationType.SI);
        }

        public void B()
        {
            GetFace(FaceType.Back).TurnClockwise();
            TurnZLayerCounterClockwise(ZLayer.Back);
            operationHistory.Push(OperationType.B);
        }

        public void B2()
        {
            GetFace(FaceType.Back).TurnClockwise();
            TurnZLayerCounterClockwise(ZLayer.Back);
            GetFace(FaceType.Back).TurnClockwise();
            TurnZLayerCounterClockwise(ZLayer.Back);
            operationHistory.Push(OperationType.B2);
        }

        public void BI()
        {
            GetFace(FaceType.Back).TurnCounterClockwise();
            TurnZLayerClockwise(ZLayer.Back);
            operationHistory.Push(OperationType.BI);
        }

        public void L()
        {
            LeftFace.TurnClockwise();
            TurnXLayerCounterClockwise(XLayer.Left);
            operationHistory.Push(OperationType.L);
        }

        public void L2()
        {
            LeftFace.TurnClockwise();
            TurnXLayerCounterClockwise(XLayer.Left);
            LeftFace.TurnClockwise();
            TurnXLayerCounterClockwise(XLayer.Left);
            operationHistory.Push(OperationType.L2);
        }

        public void LI()
        {
            LeftFace.TurnCounterClockwise();
            TurnXLayerClockwise(XLayer.Left);
            operationHistory.Push(OperationType.LI);
        }

        public void M()
        {
            TurnXLayerCounterClockwise(XLayer.Middle);
            operationHistory.Push(OperationType.M);
        }

        public void MI()
        {
            TurnXLayerClockwise(XLayer.Middle);
            operationHistory.Push(OperationType.MI);
        }

        public void R()
        {
            RightFace.TurnClockwise();
            TurnXLayerClockwise(XLayer.Right);
            operationHistory.Push(OperationType.R);
        }

        public void R2()
        {
            RightFace.TurnClockwise();
            TurnXLayerClockwise(XLayer.Right); 
            RightFace.TurnClockwise();
            TurnXLayerClockwise(XLayer.Right);
            operationHistory.Push(OperationType.R2);
        }

        public void RI()
        {
            RightFace.TurnCounterClockwise();
            TurnXLayerCounterClockwise(XLayer.Right);
            operationHistory.Push(OperationType.RI);
        }

        public void X()
        {
            TurnXLayerClockwise(XLayer.Left);
            TurnXLayerClockwise(XLayer.Middle);
            TurnXLayerClockwise(XLayer.Right);

            LeftFace.TurnCounterClockwise();
            RightFace.TurnClockwise();

            operationHistory.Push(OperationType.X);
        }

        public void XI()
        {
            TurnXLayerCounterClockwise(XLayer.Left);
            TurnXLayerCounterClockwise(XLayer.Middle);
            TurnXLayerCounterClockwise(XLayer.Right);

            LeftFace.TurnClockwise();
            RightFace.TurnCounterClockwise();

            operationHistory.Push(OperationType.XI);
        }

        public void Y()
        {
            TurnYLayerClockwise(YLayer.Bottom);
            TurnYLayerClockwise(YLayer.Middle);
            TurnYLayerClockwise(YLayer.Top);

            TopFace.TurnClockwise();
            BottomFace.TurnCounterClockwise();

            operationHistory.Push(OperationType.Y);
        }

        public void YI()
        {
            TurnYLayerCounterClockwise(YLayer.Bottom);
            TurnYLayerCounterClockwise(YLayer.Middle);
            TurnYLayerCounterClockwise(YLayer.Top);

            TopFace.TurnCounterClockwise();
            BottomFace.TurnClockwise();

            operationHistory.Push(OperationType.YI);
        }

        public void Z()
        {
            TurnZLayerClockwise(ZLayer.Front);
            TurnZLayerClockwise(ZLayer.Middle);
            TurnZLayerClockwise(ZLayer.Back);

            FrontFace.TurnClockwise();
            BackFace.TurnCounterClockwise();

            operationHistory.Push(OperationType.Z);
        }

        public void ZI()
        {
            TurnZLayerCounterClockwise(ZLayer.Front);
            TurnZLayerCounterClockwise(ZLayer.Middle);
            TurnZLayerCounterClockwise(ZLayer.Back);

            FrontFace.TurnCounterClockwise();
            BackFace.TurnClockwise();

            operationHistory.Push(OperationType.ZI);
        }

        public void Print()
        {
            foreach (Face face in Faces)
            {
                Debug.WriteLine($"${face.Type}");
                face.Print();
            }
        }

        public void Randomize()
        {
            Action[] ops = new[] { F, B, L, R, U, D, M, S, E };
            Random random = new();

            foreach (int n in Enumerable.Range(0, 20))
            {
                int rand = random.Next(ops.Length);
                ops[rand]();
            }
        }

        public void Set(int face, int piece, ColorType colorType)
        {
            Faces[face].Pieces[piece].ColorType = colorType;
        }

        public void Update(string s)
        {
            foreach (int i in Enumerable.Range(0, FaceCount))
            {
                foreach (int j in Enumerable.Range(0, Face.PieceCount))
                {
                    ColorType colorType = (ColorType)(s[i * Face.PieceCount + j] - '0');

                    Faces[i].SetValue((PieceType)j, colorType); 
                }
            }
        }

        public bool IsSolved()
        {
            return Faces.All(face => face.Pieces.All(piece => piece.ColorType == face.MiddlePiece.ColorType));
        }

        public void MakeOperation(OperationType type)
        {
            operations[(int)type]();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new(54);

            foreach (Piece piece in Pieces)
            {
                stringBuilder.Append((byte)piece.ColorType);
            }

            return stringBuilder.ToString();
        }
    }
}
