using System;

using RubikCubeSolverApp.Enums;

namespace RubikCubeSolverApp.Extensions
{
    public static class OperationTypeExtensions
    {
        public static OperationType GetOpposite(this OperationType operation) => operation switch
        {
            OperationType.F => OperationType.FI,
            OperationType.FI => OperationType.F,
            OperationType.U => OperationType.UI,
            OperationType.UI => OperationType.U,
            OperationType.D => OperationType.DI,
            OperationType.DI => OperationType.D,
            OperationType.S => OperationType.SI,
            OperationType.SI => OperationType.S,
            OperationType.L => OperationType.LI,
            OperationType.LI => OperationType.L,
            OperationType.R => OperationType.RI,
            OperationType.RI => OperationType.R,
            OperationType.M => OperationType.MI,
            OperationType.MI => OperationType.M,
            OperationType.E => OperationType.EI,
            OperationType.EI => OperationType.E,
            OperationType.B => OperationType.BI,
            OperationType.BI => OperationType.B,
            OperationType.X => OperationType.XI,
            OperationType.XI => OperationType.X,
            OperationType.Y => OperationType.YI,
            OperationType.YI => OperationType.Y,
            OperationType.Z => OperationType.ZI,
            OperationType.ZI => OperationType.Z,
            _ => throw new NotImplementedException()
        };
    }
}
