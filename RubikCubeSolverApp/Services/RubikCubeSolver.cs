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
using System.Diagnostics;

namespace RubikCubeSolverApp.Services
{
    public class RubikCubeSolver : IRubikCubeSolver
    {
        public static bool IsFirstPhaseFinished(RubikCube cube)
        {
            foreach (Piece edgePiece in cube.TopFace.EdgePieces)
            {
                if (edgePiece.ColorType != cube.TopFace.MiddlePiece.ColorType)
                {
                    return false;
                }
            }

            foreach (Face yAxisFace in cube.YAxisFaces)
            {
                if (yAxisFace.TopPiece.ColorType != yAxisFace.MiddlePiece.ColorType)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsSecondPhaseFinished(RubikCube cube)
        {
            if (!IsFirstPhaseFinished(cube))
            {
                return false;
            }

            foreach (Piece cornerPiece in cube.TopFace.CornerPieces)
            {
                if (cornerPiece.ColorType != cube.TopFace.MiddlePiece.ColorType)
                {
                    return false;
                }
            }

            foreach (Face yAxisFace in cube.YAxisFaces)
            {
                if (yAxisFace.TopLeftPiece.ColorType != yAxisFace.TopPiece.ColorType ||
                    yAxisFace.TopRightPiece.ColorType != yAxisFace.TopPiece.ColorType)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsThirdPhaseFinished(RubikCube cube)
        {
            if (cube.YAxisFaces
                .Any(face => face
                .GetYLayerColorTypes(YLayer.Bottom)
                .Concat(face.GetYLayerColorTypes(YLayer.Middle))
                .Any(c => c != face.MiddlePiece.ColorType)))
            {
                return false;
            }

            if (cube.BottomFace.Pieces.Any(p => p.ColorType != cube.BottomMiddle.ColorType))
            {
                return false;
            }

            return true;
        }

        public static bool IsFourthPhaseFinished(RubikCube cube)
        {
            if (!IsThirdPhaseFinished(cube))
            {
                return false;
            }

            if (cube.TopFace.EdgePieces.Any(edge => edge.ColorType != cube.TopMiddle.ColorType))
            {
                return false;
            }

            return true;
        }

        public static bool IsFifthPhaseFinished(RubikCube cube)
        {
            if (!IsFourthPhaseFinished(cube))
            {
                return false;
            }

            return cube.YAxisFaces.All(face => face.TopPiece.ColorType == face.MiddlePiece.ColorType);
        }

        public static bool IsSixthPhaseFinished(RubikCube cube)
        {
            if (!IsFifthPhaseFinished(cube))
            {
                return false;
            }

            if (!cube.FrontTopLeftCorner.HasColorTypes(cube.FrontMiddle.ColorType, cube.TopMiddle.ColorType, cube.LeftMiddle.ColorType))
            {
                return false;
            }

            if (!cube.FrontTopRightCorner.HasColorTypes(cube.FrontMiddle.ColorType, cube.TopMiddle.ColorType, cube.RightMiddle.ColorType))
            {
                return false;
            }

            if (!cube.BackTopLeftCorner.HasColorTypes(cube.BackMiddle.ColorType, cube.TopMiddle.ColorType, cube.LeftMiddle.ColorType))
            {
                return false;
            }

            if (!cube.BackTopRightCorner.HasColorTypes(cube.BackMiddle.ColorType, cube.TopMiddle.ColorType, cube.RightMiddle.ColorType))
            {
                return false;
            }

            return true;
        }

        public static bool IsSeventhPhaseFinished(RubikCube cube)
        {
            return cube.IsSolved();
        }

        private static void MakeOperation(RubikCube cube, OperationType type, List<OperationType> operations, AutoResetEvent? resumeEvent = null)
        {
            cube.MakeOperation(type);
            operations.Add(type);
            resumeEvent?.WaitOne();
        }

        public static List<OperationType> SolveFirstPhase(RubikCube cube, AutoResetEvent? resumeEvent = null)
        {
            List<OperationType> operations = new();

            while (!IsFirstPhaseFinished(cube))
            {
                ColorType topColor = cube.TopFace.MiddlePiece.ColorType;
                ColorType frontColor = cube.FrontFace.MiddlePiece.ColorType;

                //Top bottom
                if (cube.TopFace.BottomPiece.ColorType == topColor &&
                    cube.FrontFace.TopPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.Y, operations, resumeEvent);
                   
                    continue;
                }

                //Top left
                if (cube.TopFace.LeftPiece.ColorType == topColor &&
                    cube.LeftFace.TopPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.L, operations, resumeEvent);
                   
                    continue;
                }

                //Top right
                if (cube.TopFace.RightPiece.ColorType == topColor &&
                    cube.RightFace.TopPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.RI, operations, resumeEvent);
                   
                    continue;
                }

                //Top top
                if (cube.TopFace.TopPiece.ColorType == topColor &&
                    cube.BackFace.TopPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.BI, operations, resumeEvent);
                   
                    continue;
                }

                //Left top
                if (cube.LeftFace.TopPiece.ColorType == topColor &&
                    cube.TopFace.LeftPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.L, operations, resumeEvent);
                   
                    continue;
                }

                //Left bottom
                if (cube.LeftFace.BottomPiece.ColorType == topColor &&
                    cube.BottomFace.LeftPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.D, operations, resumeEvent);
                   
                    continue;
                }

                //Left left
                if (cube.LeftFace.LeftPiece.ColorType == topColor &&
                    cube.BackFace.RightPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.B, operations, resumeEvent);
                    MakeOperation(cube, OperationType.DI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.BI, operations, resumeEvent);
                   
                    continue;
                }

                //Left right
                if (cube.LeftFace.RightPiece.ColorType == topColor &&
                    cube.FrontFace.LeftPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.F, operations, resumeEvent);
                   
                    continue;
                }

                //Right top
                if (cube.RightFace.TopPiece.ColorType == topColor &&
                    cube.TopFace.RightPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.RI, operations, resumeEvent);
                   
                    continue;
                }

                //Right bottom
                if (cube.RightFace.BottomPiece.ColorType == topColor &&
                    cube.BottomFace.RightPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.DI, operations, resumeEvent);
                   
                    continue;
                }

                //Right left
                if (cube.RightFace.LeftPiece.ColorType == topColor &&
                    cube.FrontFace.RightPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.FI, operations, resumeEvent);
                   
                    continue;
                }

                //Right right
                if (cube.RightFace.RightPiece.ColorType == topColor &&
                    cube.BackFace.LeftPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.BI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.DI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.B, operations, resumeEvent);
                   
                    continue;
                }

                //Front top
                if (cube.FrontFace.TopPiece.ColorType == topColor &&
                    cube.TopFace.BottomPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.F, operations, resumeEvent);
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                    continue;
                }

                //Front bottom
                if (cube.FrontFace.BottomPiece.ColorType == topColor &&
                    cube.BottomFace.TopPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.FI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                    continue;
                }

                //Front left
                if (cube.FrontFace.LeftPiece.ColorType == topColor &&
                    cube.LeftFace.RightPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                    MakeOperation(cube, OperationType.LI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                   
                    continue;
                }

                //Front right
                if (cube.FrontFace.RightPiece.ColorType == topColor &&
                    cube.RightFace.LeftPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                    continue;
                }

                //Back left
                if (cube.BackFace.LeftPiece.ColorType == topColor &&
                    cube.RightFace.RightPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.RI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                    continue;
                }

                //Back right
                if (cube.BackFace.RightPiece.ColorType == topColor &&
                    cube.LeftFace.LeftPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                    MakeOperation(cube, OperationType.L, operations, resumeEvent);
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                   
                    continue;
                }

                //Back top
                if (cube.BackFace.TopPiece.ColorType == topColor &&
                    cube.TopFace.TopPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.BI, operations, resumeEvent);
                   
                    continue;
                }

                //Back bottom
                if (cube.BackFace.BottomPiece.ColorType == topColor &&
                    cube.BottomFace.BottomPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.DI, operations, resumeEvent);
                   
                    continue;
                }

                //Bottom top
                if (cube.BottomFace.TopPiece.ColorType == topColor &&
                    cube.FrontFace.BottomPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.FI, operations, resumeEvent);
                   
                    continue;
                }

                //Bottom bottom
                if (cube.BottomFace.BottomPiece.ColorType == topColor &&
                    cube.BackFace.BottomPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.DI, operations, resumeEvent);
                   
                    continue;
                }

                //Bottom left
                if (cube.BottomFace.LeftPiece.ColorType == topColor &&
                    cube.LeftFace.BottomPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.D, operations, resumeEvent);
                   
                    continue;
                }

                //Bottom right
                if (cube.BottomFace.RightPiece.ColorType == topColor &&
                    cube.RightFace.BottomPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.DI, operations, resumeEvent);
                   
                    continue;
                }

                throw new NotImplementedException();
            }

            return operations;
        }

        public static List<OperationType> SolveSecondPhase(RubikCube cube, AutoResetEvent? resumeEvent = null)
        {
            List<OperationType> operations = new();

            if (IsSecondPhaseFinished(cube))
            {
                return operations;
            }

            if (!IsFirstPhaseFinished(cube))
            {
                throw new InvalidOperationException();
            }

            while (!IsSecondPhaseFinished(cube))
            {
                ColorType topColor = cube.TopFace.MiddlePiece.ColorType;
                ColorType frontColor = cube.FrontFace.MiddlePiece.ColorType;
                ColorType rightColor = cube.RightFace.MiddlePiece.ColorType;

                if (cube.TopFace.BottomRightPiece.ColorType == topColor &&
                    cube.FrontFace.TopRightPiece.ColorType == frontColor &&
                    cube.RightFace.TopLeftPiece.ColorType == rightColor)
                {
                    MakeOperation(cube, OperationType.Y, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.FrontFace.BottomRightPiece.ColorType == frontColor &&
                    cube.RightFace.BottomLeftPiece.ColorType == topColor &&
                    cube.BottomFace.TopRightPiece.ColorType == rightColor)
                {
                    MakeOperation(cube, OperationType.RI, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.DI, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.FrontFace.BottomRightPiece.ColorType == topColor &&
                    cube.RightFace.BottomLeftPiece.ColorType == rightColor &&
                    cube.BottomFace.TopRightPiece.ColorType == frontColor)
                {
                    MakeOperation(cube, OperationType.F, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.D, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.FI, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.FrontFace.BottomRightPiece.ColorType == rightColor &&
                    cube.RightFace.BottomLeftPiece.ColorType == frontColor &&
                    cube.BottomFace.TopRightPiece.ColorType == topColor)
                {
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.DI, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.D, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.BackBottomLeftCorner.HasColorTypes(topColor, frontColor, rightColor))
                {
                    MakeOperation(cube, OperationType.D, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.FrontBottomLeftCorner.HasColorTypes(topColor, frontColor, rightColor))
                {
                    MakeOperation(cube, OperationType.D, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.BackBottomRightCorner.HasColorTypes(topColor, frontColor, rightColor))
                {
                    MakeOperation(cube, OperationType.DI, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.FrontTopRightCorner.HasColorTypes(topColor, frontColor, rightColor))
                {
                    MakeOperation(cube, OperationType.F, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.D, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.FI, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.DI, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.FrontTopLeftCorner.HasColorTypes(topColor, frontColor, rightColor))
                {
                    MakeOperation(cube, OperationType.L, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.D, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.LI, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.BackTopRightCorner.HasColorTypes(topColor, frontColor, rightColor))
                {
                    MakeOperation(cube, OperationType.BI, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.DI, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.B, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.BackTopLeftCorner.HasColorTypes(topColor, frontColor, rightColor))
                {
                    MakeOperation(cube, OperationType.B, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.D, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.BI, operations, resumeEvent);
                   
                    continue;
                }

                throw new NotImplementedException();
            }

            return operations;
        }

        public static List<OperationType> SolveThirdPhase(RubikCube cube, AutoResetEvent? resumeEvent = null)
        {
            List<OperationType> operations = new();

            if (IsThirdPhaseFinished(cube))
            {
                return operations;
            }

            if (!IsSecondPhaseFinished(cube))
            {
                throw new InvalidOperationException();
            }

            MakeOperation(cube, OperationType.X, operations, resumeEvent);
            MakeOperation(cube, OperationType.X, operations, resumeEvent);

            while (!IsThirdPhaseFinished(cube))
            {
                ColorType frontColor = cube.FrontMiddle.ColorType;
                ColorType rightColor = cube.RightMiddle.ColorType;
                ColorType leftColor = cube.LeftMiddle.ColorType;
                ColorType backColor = cube.BackMiddle.ColorType;

                if (cube.FrontFace.GetYLayerColorTypes(YLayer.Bottom).All(c => c == frontColor) &&
                    cube.FrontFace.GetYLayerColorTypes(YLayer.Middle).All(c => c == frontColor) &&
                    cube.LeftFace.RightPiece.ColorType == leftColor &&
                    cube.RightFace.LeftPiece.ColorType == rightColor)
                {
                    MakeOperation(cube, OperationType.Y, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.FrontTopEdge.ColorTypesEqual(frontColor, rightColor))
                {
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.RI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.FI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                    MakeOperation(cube, OperationType.F, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.FrontTopEdge.ColorTypesEqual(frontColor, leftColor))
                {
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.LI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                    MakeOperation(cube, OperationType.L, operations, resumeEvent);
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                    MakeOperation(cube, OperationType.F, operations, resumeEvent);
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.FI, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.TopRightEdge.ColorTypesEqual(rightColor, frontColor) ||
                    cube.TopRightEdge.ColorTypesEqual(leftColor, frontColor))
                {
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.TopLeftEdge.ColorTypesEqual(rightColor, frontColor) ||
                    cube.TopLeftEdge.ColorTypesEqual(leftColor, frontColor))
                {
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.BackTopEdge.ColorTypesEqual(frontColor, rightColor) ||
                    cube.BackTopEdge.ColorTypesEqual(frontColor, leftColor))
                {
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.TopLeftEdge.ColorTypesEqual(frontColor, leftColor) ||
                    cube.TopLeftEdge.ColorTypesEqual(backColor, leftColor))
                {
                    MakeOperation(cube, OperationType.YI, operations, resumeEvent);
                    continue;
                }

                if (cube.TopRightEdge.ColorTypesEqual(backColor, rightColor) ||
                    cube.TopRightEdge.ColorTypesEqual(frontColor, rightColor))
                {
                    MakeOperation(cube, OperationType.Y, operations, resumeEvent);
                    continue;
                }

                if (cube.BackTopEdge.ColorTypesEqual(backColor, leftColor) ||
                    cube.BackTopEdge.ColorTypesEqual(backColor, rightColor))
                {
                    MakeOperation(cube, OperationType.Y, operations, resumeEvent);
                    MakeOperation(cube, OperationType.Y, operations, resumeEvent);
                    continue;
                }

                if (cube.FrontRightEdge.ColorTypesEqual(rightColor, frontColor))
                {
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.RI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.FI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                    MakeOperation(cube, OperationType.F, operations, resumeEvent);
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);

                    continue;
                }

                if (!cube.FrontRightEdge.HasColorTypes(frontColor, rightColor))
                {
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.RI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.FI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                    MakeOperation(cube, OperationType.F, operations, resumeEvent);
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                    MakeOperation(cube, OperationType.Y, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.FrontRightEdge.ColorTypesEqual(frontColor, rightColor))
                {
                    MakeOperation(cube, OperationType.Y, operations, resumeEvent);
                   
                    continue;
                }

                throw new NotImplementedException();
            }

            return operations;
        }

        public static List<OperationType> SolveFourthPhase(RubikCube cube, AutoResetEvent? resumeEvent = null)
        {
            List<OperationType> operations = new();

            if (IsFourthPhaseFinished(cube))
            {
                return operations;
            }

            if (!IsThirdPhaseFinished(cube))
            {
                throw new InvalidOperationException();
            }

            ColorType topColor = cube.TopMiddle.ColorType;

            for (int n = 0; n != 3 && !IsFourthPhaseFinished(cube); ++n)
            {
                if (cube.TopFace.RightPiece.ColorType == topColor && cube.TopFace.BottomPiece.ColorType == topColor)
                {
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                }

                if (cube.TopFace.TopPiece.ColorType == topColor && cube.TopFace.RightPiece.ColorType == topColor)
                {
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                   
                }

                if (cube.TopFace.LeftPiece.ColorType == topColor && cube.TopFace.BottomPiece.ColorType == topColor)
                {
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                }

                if (cube.TopFace.LeftPiece.ColorType == topColor && cube.TopFace.TopPiece.ColorType == topColor)
                {
                    MakeOperation(cube, OperationType.F, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.RI, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.FI, operations, resumeEvent);
                   
                    continue;
                }

                MakeOperation(cube, OperationType.F, operations, resumeEvent);
               
                MakeOperation(cube, OperationType.R, operations, resumeEvent);
               
                MakeOperation(cube, OperationType.U, operations, resumeEvent);
               
                MakeOperation(cube, OperationType.RI, operations, resumeEvent);
               
                MakeOperation(cube, OperationType.UI, operations, resumeEvent);
               
                MakeOperation(cube, OperationType.FI, operations, resumeEvent);
               

                if (n != 2)
                {
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                }
            }

            if (!IsFourthPhaseFinished(cube))
            {
                throw new Exception();
            }

            return operations;
        }

        public static List<OperationType> SolveFifthPhase(RubikCube cube, AutoResetEvent? resumeEvent = null)
        {
            List<OperationType> operations = new();

            if (IsFifthPhaseFinished(cube))
            {
                return operations;
            }

            if (!IsFourthPhaseFinished(cube))
            {
                throw new InvalidOperationException();
            }

            while (!IsFifthPhaseFinished(cube))
            {
                ColorType frontColor = cube.FrontMiddle.ColorType;
                ColorType leftColor = cube.LeftMiddle.ColorType;
                ColorType rightColor = cube.RightMiddle.ColorType;

                if (cube.LeftFace.TopPiece.ColorType == rightColor && cube.RightFace.TopPiece.ColorType == leftColor)
                {
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                    continue;
                }

                if (cube.FrontFace.TopPiece.ColorType == leftColor || cube.LeftFace.TopPiece.ColorType == frontColor ||
                    (cube.FrontFace.TopPiece.ColorType == leftColor && cube.BackFace.TopPiece.ColorType == rightColor))
                {
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.RI, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.RI, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                   
                    continue;
                }

                MakeOperation(cube, OperationType.Y, operations, resumeEvent);
               
            }

            return operations;
        }

        public static List<OperationType> SolveSixthPhase(RubikCube cube, AutoResetEvent? resumeEvent = null)
        {
            List<OperationType> operations = new();

            if (IsSixthPhaseFinished(cube))
            {
                return operations;
            }

            if (!IsFifthPhaseFinished(cube))
            {
                throw new InvalidOperationException();
            }

            void sequence()
            {
                MakeOperation(cube, OperationType.U, operations, resumeEvent);
               
                MakeOperation(cube, OperationType.R, operations, resumeEvent);
               
                MakeOperation(cube, OperationType.UI, operations, resumeEvent);
               
                MakeOperation(cube, OperationType.LI, operations, resumeEvent);
               
                MakeOperation(cube, OperationType.U, operations, resumeEvent);
               
                MakeOperation(cube, OperationType.RI, operations, resumeEvent);
               
                MakeOperation(cube, OperationType.UI, operations, resumeEvent);
               
                MakeOperation(cube, OperationType.L, operations, resumeEvent);
               
            }

            while (!cube.FrontTopRightCorner.HasColorTypes(cube.FrontMiddle.ColorType, cube.TopMiddle.ColorType, cube.RightMiddle.ColorType))
            {
                if (cube.FrontTopLeftCorner.HasColorTypes(cube.FrontMiddle.ColorType, cube.TopMiddle.ColorType, cube.LeftMiddle.ColorType))
                {
                    MakeOperation(cube, OperationType.YI, operations, resumeEvent);

                    break;
                }

                if (cube.BackTopLeftCorner.HasColorTypes(cube.BackMiddle.ColorType, cube.TopMiddle.ColorType, cube.LeftMiddle.ColorType))
                {
                    MakeOperation(cube, OperationType.Y, operations, resumeEvent);
                   
                    MakeOperation(cube, OperationType.Y, operations, resumeEvent);
                   
                    break;
                }

                if (cube.BackTopRightCorner.HasColorTypes(cube.BackMiddle.ColorType, cube.TopMiddle.ColorType, cube.RightMiddle.ColorType))
                {
                    MakeOperation(cube, OperationType.Y, operations, resumeEvent);
                   
                    break;
                }

                sequence();
            }

            while (!IsSixthPhaseFinished(cube))
            {
                sequence();
            }

            return operations;
        }

        public static List<OperationType> SolveSeventhPhase(RubikCube cube, AutoResetEvent? resumeEvent = null)
        {
            List<OperationType> operations = new();

            if (IsSeventhPhaseFinished(cube))
            {
                return operations;
            }

            if (!IsSixthPhaseFinished(cube))
            {
                throw new InvalidOperationException();
            }

            ColorType topColor = cube.TopMiddle.ColorType;

            while (cube.TopFace.CornerPieces.Any(p => p.ColorType != topColor))
            {
                if (cube.TopFace.BottomRightPiece.ColorType != topColor)
                {
                    MakeOperation(cube, OperationType.RI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.DI, operations, resumeEvent);
                    MakeOperation(cube, OperationType.R, operations, resumeEvent);
                    MakeOperation(cube, OperationType.D, operations, resumeEvent);
                }
                else if (cube.TopFace.TopRightPiece.ColorType != topColor)
                {
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                }
                else if (cube.TopFace.BottomLeftPiece.ColorType != topColor)
                {
                    MakeOperation(cube, OperationType.UI, operations, resumeEvent);
                }
                else if (cube.TopFace.TopLeftPiece.ColorType != topColor)
                {
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                    MakeOperation(cube, OperationType.U, operations, resumeEvent);
                }
            }

            ColorType frontColor = cube.FrontMiddle.ColorType;

            if (cube.LeftFace.TopPiece.ColorType == frontColor)
            {
                MakeOperation(cube, OperationType.UI, operations, resumeEvent);
            } 
            else if (cube.RightFace.TopPiece.ColorType == frontColor)
            {
                MakeOperation(cube, OperationType.U, operations, resumeEvent);
            }
            else if (cube.BackFace.TopPiece.ColorType == frontColor)
            {
                MakeOperation(cube, OperationType.U, operations, resumeEvent);
                MakeOperation(cube, OperationType.U, operations, resumeEvent);
            }

            if (!cube.IsSolved())
            {
                throw new Exception();
            }

            return operations;
        }

        public List<OperationType> Solve(RubikCube cube, AutoResetEvent? resumeEvent = null)
        {
            List<OperationType> operations = new();

            var ops1 = SolveFirstPhase(cube, resumeEvent);
            var ops2 = SolveSecondPhase(cube, resumeEvent);
            var ops3 = SolveThirdPhase(cube, resumeEvent);
            var ops4 = SolveFourthPhase(cube, resumeEvent);
            var ops5 = SolveFifthPhase(cube, resumeEvent);
            var ops6 = SolveSixthPhase(cube, resumeEvent);
            var ops7 = SolveSeventhPhase(cube, resumeEvent);

            Debug.WriteLine($"Phase 1 - {ops1.Count} operations: " + string.Join(", ", ops1));
            Debug.WriteLine($"Phase 2 - {ops2.Count} operations: " + string.Join(", ", ops2));
            Debug.WriteLine($"Phase 3 - {ops3.Count} operations: " + string.Join(", ", ops3));
            Debug.WriteLine($"Phase 4 - {ops4.Count} operations: " + string.Join(", ", ops4));
            Debug.WriteLine($"Phase 5 - {ops5.Count} operations: " + string.Join(", ", ops5));
            Debug.WriteLine($"Phase 6 - {ops6.Count} operations: " + string.Join(", ", ops6));
            Debug.WriteLine($"Phase 7 - {ops7.Count} operations: " + string.Join(", ", ops7));

            operations.AddRange(ops1);
            operations.AddRange(ops2);
            operations.AddRange(ops3);
            operations.AddRange(ops4);
            operations.AddRange(ops5);
            operations.AddRange(ops6);
            operations.AddRange(ops7);

            return operations;
        }
    }
}
