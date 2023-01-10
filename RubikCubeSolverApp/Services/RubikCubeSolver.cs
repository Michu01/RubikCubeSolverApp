using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Windows.Media;
using RubikCubeSolverApp.Models;
using RubikCubeSolverApp.Enums;

namespace RubikCubeSolverApp.Services
{
    public class RubikCubeSolver : IRubikCubeSolver
    {
        public bool IsFirstPhaseFinished(RubikCube cube)
        {
            ColorType topColor = cube.TopFace.MiddlePiece.ColorType;

            bool topOk = cube.TopFace.TopPiece.ColorType == topColor && cube.BackFace.TopPiece.ColorType == cube.BackFace.MiddlePiece.ColorType;
            bool rightOk = cube.TopFace.RightPiece.ColorType == topColor && cube.RightFace.TopPiece.ColorType == cube.RightFace.MiddlePiece.ColorType;
            bool bottomOk = cube.TopFace.BottomPiece.ColorType == topColor && cube.FrontFace.TopPiece.ColorType == cube.FrontFace.MiddlePiece.ColorType;
            bool leftOk = cube.TopFace.LeftPiece.ColorType == topColor && cube.LeftFace.TopPiece.ColorType == cube.LeftFace.MiddlePiece.ColorType;

            return topOk && rightOk && bottomOk && leftOk;
        }

        private static bool IsTopDamaged(RubikCube cube, ColorType topColor, bool topOk, bool rightOk, bool bottomOk, bool leftOk)
        {
            return topOk && cube.TopFace.TopPiece.ColorType != topColor ||
                rightOk && cube.TopFace.RightPiece.ColorType != topColor ||
                bottomOk && cube.TopFace.BottomPiece.ColorType != topColor ||
                leftOk && cube.TopFace.LeftPiece.ColorType != topColor;
        }

        public void FirstPhase(RubikCube cube, AutoResetEvent? resumeEvent = null)
        {
            while (!IsFirstPhaseFinished(cube))
            {
                ColorType topColor = cube.TopFace.MiddlePiece.ColorType;
                ColorType frontColor = cube.FrontFace.MiddlePiece.ColorType;

                //Top bottom
                if (cube.TopFace.BottomPiece.ColorType == topColor &&
                    cube.FrontFace.TopPiece.ColorType == frontColor)
                {
                    cube.Y();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Top left
                if (cube.TopFace.LeftPiece.ColorType == topColor &&
                    cube.LeftFace.TopPiece.ColorType == frontColor)
                {
                    cube.L();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Top right
                if (cube.TopFace.RightPiece.ColorType == topColor &&
                    cube.RightFace.TopPiece.ColorType == frontColor)
                {
                    cube.RI();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Top top
                if (cube.TopFace.TopPiece.ColorType == topColor &&
                    cube.BackFace.TopPiece.ColorType == frontColor)
                {
                    cube.BI();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Left top
                if (cube.LeftFace.TopPiece.ColorType == topColor &&
                    cube.TopFace.LeftPiece.ColorType == frontColor)
                {
                    cube.L();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Left bottom
                if (cube.LeftFace.BottomPiece.ColorType == topColor &&
                    cube.BottomFace.LeftPiece.ColorType == frontColor)
                {
                    cube.D();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Left left
                if (cube.LeftFace.LeftPiece.ColorType == topColor &&
                    cube.BackFace.RightPiece.ColorType == frontColor)
                {
                    cube.B();
                    resumeEvent?.WaitOne();
                    cube.DI();
                    resumeEvent?.WaitOne();
                    cube.BI();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Left right
                if (cube.LeftFace.RightPiece.ColorType == topColor &&
                    cube.FrontFace.LeftPiece.ColorType == frontColor)
                {
                    cube.F();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Right top
                if (cube.RightFace.TopPiece.ColorType == topColor &&
                    cube.TopFace.RightPiece.ColorType == frontColor)
                {
                    cube.RI();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Right bottom
                if (cube.RightFace.BottomPiece.ColorType == topColor &&
                    cube.BottomFace.RightPiece.ColorType == frontColor)
                {
                    cube.DI();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Right left
                if (cube.RightFace.LeftPiece.ColorType == topColor &&
                    cube.FrontFace.RightPiece.ColorType == frontColor)
                {
                    cube.FI();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Right right
                if (cube.RightFace.RightPiece.ColorType == topColor &&
                    cube.BackFace.LeftPiece.ColorType == frontColor)
                {
                    cube.BI();
                    resumeEvent?.WaitOne();
                    cube.DI();
                    resumeEvent?.WaitOne();
                    cube.B();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Front top
                if (cube.FrontFace.TopPiece.ColorType == topColor &&
                    cube.TopFace.BottomPiece.ColorType == frontColor)
                {
                    cube.F();
                    resumeEvent?.WaitOne();
                    cube.UI();
                    resumeEvent?.WaitOne();
                    cube.R();
                    resumeEvent?.WaitOne();
                    cube.U();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Front bottom
                if (cube.FrontFace.BottomPiece.ColorType == topColor &&
                    cube.BottomFace.TopPiece.ColorType == frontColor)
                {
                    cube.FI();
                    resumeEvent?.WaitOne();
                    cube.UI();
                    resumeEvent?.WaitOne();
                    cube.R();
                    resumeEvent?.WaitOne();
                    cube.U();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Front left
                if (cube.FrontFace.LeftPiece.ColorType == topColor &&
                    cube.LeftFace.RightPiece.ColorType == frontColor)
                {
                    cube.U();
                    resumeEvent?.WaitOne();
                    cube.LI();
                    resumeEvent?.WaitOne();
                    cube.UI();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Front right
                if (cube.FrontFace.RightPiece.ColorType == topColor &&
                    cube.RightFace.LeftPiece.ColorType == frontColor)
                {
                    cube.UI();
                    resumeEvent?.WaitOne();
                    cube.R();
                    resumeEvent?.WaitOne();
                    cube.U();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Back left
                if (cube.BackFace.LeftPiece.ColorType == topColor &&
                    cube.RightFace.RightPiece.ColorType == frontColor)
                {
                    cube.UI();
                    resumeEvent?.WaitOne();
                    cube.RI();
                    resumeEvent?.WaitOne();
                    cube.U();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //!!!
                //Back right
                if (cube.BackFace.RightPiece.ColorType == topColor &&
                    cube.LeftFace.LeftPiece.ColorType == frontColor)
                {
                    cube.U();
                    resumeEvent?.WaitOne();
                    cube.L();
                    resumeEvent?.WaitOne();
                    cube.UI();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Back top
                if (cube.BackFace.TopPiece.ColorType == topColor &&
                    cube.TopFace.TopPiece.ColorType == frontColor)
                {
                    cube.BI();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Back bottom
                if (cube.BackFace.BottomPiece.ColorType == topColor &&
                    cube.BottomFace.BottomPiece.ColorType == frontColor)
                {
                    cube.DI();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Bottom top
                if (cube.BottomFace.TopPiece.ColorType == topColor &&
                    cube.FrontFace.BottomPiece.ColorType == frontColor)
                {
                    cube.FI();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Bottom bottom
                if (cube.BottomFace.BottomPiece.ColorType == topColor &&
                    cube.BackFace.BottomPiece.ColorType == frontColor)
                {
                    cube.DI();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Bottom left
                if (cube.BottomFace.LeftPiece.ColorType == topColor &&
                    cube.LeftFace.BottomPiece.ColorType == frontColor)
                {
                    cube.D();
                    resumeEvent?.WaitOne();
                    continue;
                }

                //Bottom right
                if (cube.BottomFace.RightPiece.ColorType == topColor &&
                    cube.RightFace.BottomPiece.ColorType == frontColor)
                {
                    cube.DI();
                    resumeEvent?.WaitOne();
                    continue;
                }

                throw new NotImplementedException();
            }
        }

        public void Solve(RubikCube cube, AutoResetEvent? resumeEvent = null)
        {
            FirstPhase(cube, resumeEvent);
        }
    }
}
