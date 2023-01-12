using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RubikCubeSolverApp.Enums;

namespace RubikCubeSolverApp.Models
{
    public class Face
    {
        public const int PieceCount = 9;

        public required FaceType Type { get; set; }

        public IList<Piece> Pieces { get; } = new List<Piece>();

        public event Action<Face, Piece>? PieceChanged;

        public Piece TopLeftPiece => GetPiece(PieceType.TopLeft);

        public Piece TopPiece => GetPiece(PieceType.Top);

        public Piece TopRightPiece => GetPiece(PieceType.TopRight);

        public Piece LeftPiece => GetPiece(PieceType.Left);

        public Piece MiddlePiece => GetPiece(PieceType.Middle);

        public Piece RightPiece => GetPiece(PieceType.Right);

        public Piece BottomLeftPiece => GetPiece(PieceType.BottomLeft);

        public Piece BottomPiece => GetPiece(PieceType.Bottom);

        public Piece BottomRightPiece => GetPiece(PieceType.BottomRight);

        public Piece[] EdgePieces => new[] { TopPiece, RightPiece, BottomPiece, LeftPiece };

        public Piece[] CornerPieces => new[] { TopLeftPiece, TopRightPiece, BottomRightPiece, BottomLeftPiece };

        public Face()
        {
            foreach (int n in Enumerable.Range(0, PieceCount))
            {
                Pieces.Add(new() 
                { 
                    ColorType = (ColorType)Type, 
                    Type = (PieceType)n 
                });
            }
        }

        public IEnumerable<ColorType> GetXLayerColorTypes(XLayer layer) =>
            GetXLayer(layer).Select(e => e.ColorType).ToArray();

        public IEnumerable<ColorType> GetYLayerColorTypes(YLayer layer) =>
            GetYLayer(layer).Select(e => e.ColorType).ToArray();

        public IEnumerable<ColorType> GetZLayerColorTypes(ZLayer layer) =>
            GetZLayer(layer).Select(e => e.ColorType).ToArray();

        public void SetXLayerColorTypes(XLayer layer, IEnumerable<ColorType> colorTypes)
        {
            IEnumerable<Piece> e = GetXLayer(layer);

            foreach (int i in Enumerable.Range(0, 3))
            {
                SetValue(e.ElementAt(i), colorTypes.ElementAt(i));
            }
        }

        public void SetYLayerColorTypes(YLayer layer, IEnumerable<ColorType> colorTypes)
        {
            IEnumerable<Piece> e = GetYLayer(layer);

            foreach (int i in Enumerable.Range(0, 3))
            {
                SetValue(e.ElementAt(i), colorTypes.ElementAt(i));
            }
        }

        public void SetZLayerColorTypes(ZLayer layer, IEnumerable<ColorType> colorTypes)
        {
            IEnumerable<Piece> e = GetZLayer(layer);

            foreach (int i in Enumerable.Range(0, 3))
            {
                SetValue(e.ElementAt(i), colorTypes.ElementAt(i));
            }
        }

        public IEnumerable<Piece> GetXLayer(XLayer layer)
        {
            int i = (int)layer;

            return Type switch
            {
                FaceType.Top => new[] { Pieces[6 + i], Pieces[3 + i], Pieces[i] },
                FaceType.Back => new[] { Pieces[2 - i], Pieces[5 - i], Pieces[8 - i] },
                FaceType.Bottom => new[] { Pieces[6 + i], Pieces[3 + i], Pieces[i] },
                FaceType.Front => new[] { Pieces[6 + i], Pieces[3 + i], Pieces[i] },
                FaceType.Left or FaceType.Right => throw new InvalidOperationException(),
                _ => throw new NotImplementedException()
            };
        }

        public IEnumerable<Piece> GetYLayer(YLayer layer)
        {
            int i = (int)layer;

            return Type switch
            {
                FaceType.Front or FaceType.Left or FaceType.Back or FaceType.Right => new[] { Pieces[3 * i], Pieces[3 * i + 1], Pieces[3 * i + 2] },
                FaceType.Top or FaceType.Bottom => throw new InvalidOperationException(),
                _ => throw new NotImplementedException()
            };
        }

        public IEnumerable<Piece> GetZLayer(ZLayer layer)
        {
            int i = (int)layer;

            return Type switch
            {
                FaceType.Top => new[] { Pieces[6 - 3 * i], Pieces[7 - 3 * i], Pieces[8 - 3 * i] },
                FaceType.Right => new[] { Pieces[i], Pieces[3 + i], Pieces[6 + i] },
                FaceType.Bottom => new[] { Pieces[2 + 3 * i], Pieces[1 + 3 * i], Pieces[3 * i] },
                FaceType.Left => new[] { Pieces[8 - i], Pieces[5 - i], Pieces[2 - i] },
                FaceType.Front or FaceType.Back => throw new InvalidOperationException(),
                _ => throw new NotImplementedException()
            };
        }

        private Piece GetPiece(PieceType piece)
        {
            return Pieces[(int)piece];
        }

        private ColorType GetValue(PieceType piece)
        {
            return GetPiece(piece).ColorType;
        }

        private void SetValue(Piece piece, ColorType value)
        {
            piece.ColorType = value;
            PieceChanged?.Invoke(this, piece);
        }

        private void SetValue(PieceType pieceType, ColorType value)
        {
            SetValue(Pieces[(int)pieceType], value);
        }

        public void TurnClockwise()
        {
            ColorType topLeft = GetValue(PieceType.TopLeft);
            ColorType top = GetValue(PieceType.Top);
            ColorType topRight = GetValue(PieceType.TopRight);
            ColorType left = GetValue(PieceType.Left);
            ColorType right = GetValue(PieceType.Right);
            ColorType bottomLeft = GetValue(PieceType.BottomLeft);
            ColorType bottom = GetValue(PieceType.Bottom);
            ColorType bottomRight = GetValue(PieceType.BottomRight);

            SetValue(PieceType.TopLeft, bottomLeft);
            SetValue(PieceType.Top, left);
            SetValue(PieceType.TopRight, topLeft);
            SetValue(PieceType.Left, bottom);
            SetValue(PieceType.Right, top);
            SetValue(PieceType.BottomLeft, bottomRight);
            SetValue(PieceType.Bottom, right);
            SetValue(PieceType.BottomRight, topRight);
        }

        public void TurnCounterClockwise()
        {
            ColorType topLeft = GetValue(PieceType.TopLeft);
            ColorType top = GetValue(PieceType.Top);
            ColorType topRight = GetValue(PieceType.TopRight);
            ColorType middleLeft = GetValue(PieceType.Left);
            ColorType middleRight = GetValue(PieceType.Right);
            ColorType bottomLeft = GetValue(PieceType.BottomLeft);
            ColorType bottom = GetValue(PieceType.Bottom);
            ColorType bottomRight = GetValue(PieceType.BottomRight);

            SetValue(PieceType.TopLeft, topRight);
            SetValue(PieceType.Top, middleRight);
            SetValue(PieceType.TopRight, bottomRight);
            SetValue(PieceType.Left, top);
            SetValue(PieceType.Right, bottom);
            SetValue(PieceType.BottomLeft, topLeft);
            SetValue(PieceType.Bottom, middleLeft);
            SetValue(PieceType.BottomRight, bottomLeft);
        }

        public void Reset()
        {
            foreach (Piece piece in Pieces)
            {
                SetValue(piece, (ColorType)Type);
            }
        }

        public void Print()
        {
            Debug.WriteLine(
                $"{TopLeftPiece.ColorType} {TopPiece.ColorType} {TopRightPiece.ColorType}\n" +
                $"{LeftPiece.ColorType} {MiddlePiece.ColorType} {RightPiece.ColorType}\n" +
                $"{BottomLeftPiece.ColorType} {BottomPiece.ColorType} {BottomRightPiece.ColorType}");
        }
    }
}