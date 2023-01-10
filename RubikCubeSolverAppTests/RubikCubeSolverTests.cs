using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RubikCubeSolverApp.Models;
using RubikCubeSolverApp.Services;

namespace RubikCubeSolverAppTests
{
    public class RubikCubeSolverTests
    {
        private readonly RubikCubeSolver rubikCubeSolver = new();

        [Fact]
        public void SolveFirstPhaseTest()
        {
            foreach (int _ in Enumerable.Range(0, 1000))
            {
                RubikCube cube = new();

                cube.Randomize();

                rubikCubeSolver.FirstPhase(cube);

                bool finished = rubikCubeSolver.IsFirstPhaseFinished(cube);

                Assert.True(finished);
            }
        }
    }
}
