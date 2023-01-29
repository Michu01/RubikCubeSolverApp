using System;

using RubikCubeSolverApp.Enums;

namespace RubikCubeSolverApp.Extensions
{
    public static class OperationTypeExtensions
    {
        public static OperationType GetOpposite(this OperationType operation) => operation switch
        {
            OperationType.F => OperationType.FI,
            OperationType.F2 => OperationType.F2,
            OperationType.FI => OperationType.F,
            OperationType.U => OperationType.UI,
            OperationType.U2 => OperationType.U2,
            OperationType.UI => OperationType.U,
            OperationType.D => OperationType.DI,
            OperationType.D2 => OperationType.D2,
            OperationType.DI => OperationType.D,
            OperationType.S => OperationType.SI,
            OperationType.SI => OperationType.S,
            OperationType.L => OperationType.LI,
            OperationType.L2 => OperationType.L2,
            OperationType.LI => OperationType.L,
            OperationType.R => OperationType.RI,
            OperationType.R2 => OperationType.R2,
            OperationType.RI => OperationType.R,
            OperationType.M => OperationType.MI,
            OperationType.MI => OperationType.M,
            OperationType.E => OperationType.EI,
            OperationType.EI => OperationType.E,
            OperationType.B => OperationType.BI,
            OperationType.B2 => OperationType.B2,
            OperationType.BI => OperationType.B,
            OperationType.X => OperationType.XI,
            OperationType.XI => OperationType.X,
            OperationType.Y => OperationType.YI,
            OperationType.YI => OperationType.Y,
            OperationType.Z => OperationType.ZI,
            OperationType.ZI => OperationType.Z,
            _ => throw new NotImplementedException()
        };

        public static OperationType GetHalfTurn(this OperationType operation) => operation switch
        {
            OperationType.F => OperationType.F2,
            OperationType.FI => OperationType.F2,
            OperationType.B => OperationType.B2,
            OperationType.BI => OperationType.B2,
            OperationType.U => OperationType.U2,
            OperationType.UI => OperationType.U2,
            OperationType.D => OperationType.D2,
            OperationType.DI => OperationType.D2,
            OperationType.L => OperationType.L2,
            OperationType.LI => OperationType.L2,
            OperationType.R => OperationType.R2,
            OperationType.RI => OperationType.R2,
            _ => throw new NotImplementedException()
        };
    }
}
