using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RubikCubeSolverApp.Enums;

namespace RubikCubeSolverApp.Models
{
    public class Middle
    {
        public required Piece Piece { get; set; }

        public ColorType ColorType => Piece.ColorType;
    }
}
