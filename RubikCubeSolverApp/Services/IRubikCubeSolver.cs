using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using RubikCubeSolverApp.Enums;
using RubikCubeSolverApp.Models;

namespace RubikCubeSolverApp.Services
{
    internal interface IRubikCubeSolver
    {
        List<OperationType> Solve(RubikCube cube, AutoResetEvent? resumeEvent = null);
    }
}
