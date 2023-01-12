using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RubikCubeSolverApp.Models;
using RubikCubeSolverApp.Services;

namespace RubikCubeSolverAppTests
{
    public class RubikCubeSolverTests
    {
        [Fact]
        public void SolveFirstPhaseTest()
        {
            foreach (int _ in Enumerable.Range(0, 1000))
            {
                RubikCube cube = new();

                cube.Randomize();

                RubikCubeSolver.SolveFirstPhase(cube);

                bool finished = RubikCubeSolver.IsFirstPhaseFinished(cube);

                Assert.True(finished);
            }
        }

        [Fact]
        public void SolveSecondPhaseTest()
        {
            foreach (int _ in Enumerable.Range(0, 1000))
            {
                RubikCube cube = new();

                cube.Randomize();

                RubikCubeSolver.SolveFirstPhase(cube);
                RubikCubeSolver.SolveSecondPhase(cube);

                bool finished = RubikCubeSolver.IsSecondPhaseFinished(cube);

                Assert.True(finished);
            }
        }

        [Fact]
        public void SolveThirdPhaseTest()
        {
            foreach (int _ in Enumerable.Range(0, 1000))
            {
                RubikCube cube = new();

                cube.Randomize();

                RubikCubeSolver.SolveFirstPhase(cube);
                RubikCubeSolver.SolveSecondPhase(cube);
                RubikCubeSolver.SolveThirdPhase(cube);

                bool finished = RubikCubeSolver.IsThirdPhaseFinished(cube);

                Assert.True(finished);
            }
        }

        [Fact]
        public void SolveFourthPhaseTest()
        {
            foreach (int _ in Enumerable.Range(0, 1000))
            {
                RubikCube cube = new();

                cube.Randomize();

                RubikCubeSolver.SolveFirstPhase(cube);
                RubikCubeSolver.SolveSecondPhase(cube);
                RubikCubeSolver.SolveThirdPhase(cube);
                RubikCubeSolver.SolveFourthPhase(cube);

                bool finished = RubikCubeSolver.IsFourthPhaseFinished(cube);

                Assert.True(finished);
            }
        }

        [Fact]
        public void SolveFifthPhaseTest()
        {
            foreach (int _ in Enumerable.Range(0, 1000))
            {
                RubikCube cube = new();

                cube.Randomize();

                RubikCubeSolver.SolveFirstPhase(cube);
                RubikCubeSolver.SolveSecondPhase(cube);
                RubikCubeSolver.SolveThirdPhase(cube);
                RubikCubeSolver.SolveFourthPhase(cube);
                RubikCubeSolver.SolveFifthPhase(cube);

                bool finished = RubikCubeSolver.IsFifthPhaseFinished(cube);

                Assert.True(finished);
            }
        }

        [Fact]
        public void SolveSixthPhaseTest()
        {
            foreach (int _ in Enumerable.Range(0, 1000))
            {
                RubikCube cube = new();

                cube.Randomize();

                RubikCubeSolver.SolveFirstPhase(cube);
                RubikCubeSolver.SolveSecondPhase(cube);
                RubikCubeSolver.SolveThirdPhase(cube);
                RubikCubeSolver.SolveFourthPhase(cube);
                RubikCubeSolver.SolveFifthPhase(cube);
                RubikCubeSolver.SolveSixthPhase(cube);

                bool finished = RubikCubeSolver.IsSixthPhaseFinished(cube);

                Assert.True(finished);
            }
        }

        [Fact]
        public void SolveTest()
        {
            int opCount = 0;
            int testCount = 1000;

            foreach (int _ in Enumerable.Range(0, testCount))
            {
                RubikCube cube = new();

                cube.Randomize();

                var operations = new RubikCubeSolver().Solve(cube);

                opCount += operations.Count;

                bool finished = cube.IsSolved();

                Assert.True(finished);
            }

            Debug.WriteLine($"Avg operation count: {opCount / testCount}");
        }

    }
}
