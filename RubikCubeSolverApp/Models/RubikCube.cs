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

        public event Action<Face>? FaceChanged;

        private readonly Action[] operations;

        private readonly Face[] faces = new Face[FaceCount];

        private readonly Stack<OperationType> operationHistory = new();

        public Face TopFace => GetFace(FaceType.Top);

        public Face BottomFace => GetFace(FaceType.Bottom);

        public Face LeftFace => GetFace(FaceType.Left);

        public Face RightFace => GetFace(FaceType.Right);

        public Face FrontFace => GetFace(FaceType.Front);

        public Face BackFace => GetFace(FaceType.Back);

        public RubikCube()
        {
            foreach (int n in Enumerable.Range(0, FaceCount))
            {
                faces[n] = new()
                {
                    Type = (FaceType)n
                };

                faces[n].PieceChanged += RubikCube_PieceChanged;
            }

            operations = new Action[]
            {
                U, UI, E, EI, D, DI, F, FI, S, SI, B, BI, L, LI, M, MI, R, RI, X, XI, Y, YI, Z, ZI
            };

            Reset();
        }

        private void RubikCube_PieceChanged(Face face, Piece piece)
        {
            FacePieceChanged?.Invoke(face, piece);
        }

        private void SetFace(FaceType faceType, Face face)
        {
            face.Type = faceType;
            faces[(int)faceType] = face;
            FaceChanged?.Invoke(face);
        }

        private Face GetFace(FaceType faceType)
        {
            return faces[(int)faceType];
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
            foreach (Face face in faces)
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

        public void RI()
        {
            RightFace.TurnCounterClockwise();
            TurnXLayerCounterClockwise(XLayer.Right);
            operationHistory.Push(OperationType.RI);
        }

        public void X()
        {
            Face top = TopFace;
            Face back = BackFace;
            Face bottom = BottomFace;
            Face front = FrontFace;

            SetFace(FaceType.Top, front);
            SetFace(FaceType.Back, top);
            SetFace(FaceType.Bottom, back);
            SetFace(FaceType.Front, bottom);

            LeftFace.TurnCounterClockwise();
            RightFace.TurnClockwise();

            operationHistory.Push(OperationType.X);
        }

        public void XI()
        {
            Face top = TopFace;
            Face back = BackFace;
            Face bottom = BottomFace;
            Face front = FrontFace;

            SetFace(FaceType.Top, back);
            SetFace(FaceType.Back, bottom);
            SetFace(FaceType.Bottom, front);
            SetFace(FaceType.Front, top);

            LeftFace.TurnClockwise();
            RightFace.TurnCounterClockwise();

            operationHistory.Push(OperationType.XI);
        }

        public void Y()
        {
            Face left = LeftFace;
            Face back = BackFace;
            Face right = RightFace;
            Face front = FrontFace;

            SetFace(FaceType.Left, front);
            SetFace(FaceType.Back, left);
            SetFace(FaceType.Right, back);
            SetFace(FaceType.Front, right);

            TopFace.TurnClockwise();
            BottomFace.TurnCounterClockwise();

            operationHistory.Push(OperationType.Y);
        }

        public void YI()
        {
            Face left = LeftFace;
            Face back = BackFace;
            Face right = RightFace;
            Face front = FrontFace;

            SetFace(FaceType.Left, back);
            SetFace(FaceType.Back, right);
            SetFace(FaceType.Right, front);
            SetFace(FaceType.Front, left);

            operationHistory.Push(OperationType.YI);
        }

        public void Z()
        {
            Face left = LeftFace;
            Face top = TopFace;
            Face right = RightFace;
            Face bottom = BottomFace;

            SetFace(FaceType.Left, bottom);
            SetFace(FaceType.Top, left);
            SetFace(FaceType.Right, top);
            SetFace(FaceType.Bottom, right);

            FrontFace.TurnClockwise();
            BackFace.TurnCounterClockwise();

            operationHistory.Push(OperationType.Z);
        }

        public void ZI()
        {
            Face left = LeftFace;
            Face top = TopFace;
            Face right = RightFace;
            Face bottom = BottomFace;

            SetFace(FaceType.Left, top);
            SetFace(FaceType.Top, right);
            SetFace(FaceType.Right, bottom);
            SetFace(FaceType.Bottom, left);

            FrontFace.TurnCounterClockwise();
            BackFace.TurnClockwise();

            operationHistory.Push(OperationType.ZI);
        }

        public void Print()
        {
            foreach (Face face in faces)
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
            faces[face].Pieces[piece].ColorType = colorType;
        }
    }
}
