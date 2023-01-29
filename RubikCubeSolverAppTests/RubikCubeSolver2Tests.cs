using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RubikCubeSolverApp.Models;
using RubikCubeSolverApp.Services;

namespace RubikCubeSolverAppTests
{
    public class RubikCubeSolver2Tests
    {
        [Fact]
        public void SolveTest()
        {
            RubikCube2 cube = new();

            cube.Randomize();

            RubikCubeSolver2.Solve(cube);

            Assert.True(cube.IsSolved());
        }
    }
}
