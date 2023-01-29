using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RubikCubeSolverApp.Enums;

namespace RubikCubeSolverApp.Models
{
    public class Corner
    {
        public required Piece FirstPiece { get; set; }

        public required Piece SecondPiece { get; set; }

        public required Piece ThirdPiece { get; set; }

        public bool HasColorTypes(ColorType first, ColorType second, ColorType third)
        {
            return (FirstPiece.ColorType == first && SecondPiece.ColorType == second && ThirdPiece.ColorType == third) ||
                   (FirstPiece.ColorType == first && SecondPiece.ColorType == third && ThirdPiece.ColorType == second) ||
                   (FirstPiece.ColorType == second && SecondPiece.ColorType == first && ThirdPiece.ColorType == third) ||
                   (FirstPiece.ColorType == second && SecondPiece.ColorType == third && ThirdPiece.ColorType == first) ||
                   (FirstPiece.ColorType == third && SecondPiece.ColorType == first && ThirdPiece.ColorType == second) ||
                   (FirstPiece.ColorType == third && SecondPiece.ColorType == second && ThirdPiece.ColorType == first);
        }

        public bool ColorTypesEqual(ColorType first, ColorType second, ColorType third)
        {
            return FirstPiece.ColorType == first && SecondPiece.ColorType == second && ThirdPiece.ColorType == third;
        }

        public bool HasColorType(ColorType color)
        {
            return FirstPiece.ColorType == color || SecondPiece.ColorType == color || ThirdPiece.ColorType == color;
        }
    }
}
