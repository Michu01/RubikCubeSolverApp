using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RubikCubeSolverApp.Enums;

namespace RubikCubeSolverApp.Models
{
    public class Piece
    {
        public required PieceType Type { get; init; }

        public required ColorType ColorType { get; set; }
    }
}
