using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RubikCubeSolverApp.Enums;

namespace RubikCubeSolverApp.Models
{
    public class Edge
    {
        public required Piece FirstPiece { get; set; }

        public required Piece SecondPiece { get; set; }

        public bool HasColorTypes(ColorType first, ColorType second)
        {
            return (FirstPiece.ColorType == first && SecondPiece.ColorType == second) ||
                   (FirstPiece.ColorType == second && SecondPiece.ColorType == first);
        }

        public bool ColorTypesEqual(ColorType first, ColorType second)
        {
            return FirstPiece.ColorType == first && SecondPiece.ColorType == second;
        }

        public bool HasColorType(ColorType color)
        {
            return FirstPiece.ColorType == color || SecondPiece.ColorType == color;
        }
    }
}