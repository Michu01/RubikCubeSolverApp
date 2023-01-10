using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;

using RubikCubeSolverApp.Enums;
using RubikCubeSolverApp.Models;
using RubikCubeSolverApp.Services;
using RubikCubeSolverApp.Utility;

namespace RubikCubeSolverApp.ViewModels
{
    internal class MainViewModel : IDisposable
    {
        private readonly RubikCube rubikCube = new();

        private readonly IRubikCubeSolver rubikSolver = new RubikCubeSolver();

        private readonly AutoResetEvent resumeEvent = new(false);

        private Task? stepSolve;

        public ObservableValue<ColorType>[] ColorTypes { get; } = new ObservableValue<ColorType>[54];

        public IRelayCommand ResetCommand { get; }

        public IRelayCommand RandomCommand { get; }

        public IRelayCommand UndoCommand { get; }

        public IRelayCommand SolveCommand { get; }

        public IRelayCommand StepCommand { get; }

        public IRelayCommand UCommand { get; }

        public IRelayCommand UICommand { get; }

        public IRelayCommand ECommand { get; }

        public IRelayCommand EICommand { get; }

        public IRelayCommand DCommand { get; }

        public IRelayCommand DICommand { get; }

        public IRelayCommand FCommand { get; }

        public IRelayCommand FICommand { get; }

        public IRelayCommand SCommand { get; }

        public IRelayCommand SICommand { get; }

        public IRelayCommand BCommand { get; }

        public IRelayCommand BICommand { get; }

        public IRelayCommand LCommand { get; }

        public IRelayCommand LICommand { get; }

        public IRelayCommand MCommand { get; }

        public IRelayCommand MICommand { get; }

        public IRelayCommand RCommand { get; }

        public IRelayCommand RICommand { get; }

        public IRelayCommand XCommand { get; }

        public IRelayCommand XICommand { get; }

        public IRelayCommand YCommand { get; }

        public IRelayCommand YICommand { get; }

        public IRelayCommand ZCommand { get; }

        public IRelayCommand ZICommand { get; }

        public MainViewModel()
        {
            for (int m = 0; m < RubikCube.FaceCount; ++m)
            {
                for (int n = 0; n < Face.PieceCount; ++n)
                {
                    ObservableValue<ColorType> observableValue = new() { Value = (ColorType)m };

                    observableValue.PropertyChanged += CreateHandler(m, n, observableValue);

                    ColorTypes[m * Face.PieceCount + n] = observableValue;
                }
            }

            rubikCube.FacePieceChanged += RubikCube_ColorTypeChanged;
            rubikCube.FaceChanged += RubikCube_FaceChanged;

            RandomCommand = new RelayCommand(rubikCube.Randomize);
            ResetCommand = new RelayCommand(rubikCube.Reset);
            UndoCommand = new RelayCommand(rubikCube.Undo);
            SolveCommand = new RelayCommand(() => rubikSolver.Solve(rubikCube));
            StepCommand = new RelayCommand(() =>
            {
                if (stepSolve?.IsCompleted ?? false)
                {
                    stepSolve.Dispose();
                    stepSolve = null;
                }

                if (stepSolve is null)
                {
                    stepSolve = Task.Factory.StartNew(() => rubikSolver.Solve(rubikCube, resumeEvent), TaskCreationOptions.LongRunning);
                }
                else
                {
                    resumeEvent.Set();
                }
            });

            UCommand = new RelayCommand(rubikCube.U);
            UICommand = new RelayCommand(rubikCube.UI);
            ECommand = new RelayCommand(rubikCube.E);
            EICommand = new RelayCommand(rubikCube.EI);
            DCommand = new RelayCommand(rubikCube.D);
            DICommand = new RelayCommand(rubikCube.DI);

            FCommand = new RelayCommand(rubikCube.F);
            FICommand = new RelayCommand(rubikCube.FI);
            SCommand = new RelayCommand(rubikCube.S);
            SICommand = new RelayCommand(rubikCube.SI);
            BCommand = new RelayCommand(rubikCube.B);
            BICommand = new RelayCommand(rubikCube.BI);

            LCommand = new RelayCommand(rubikCube.L);
            LICommand = new RelayCommand(rubikCube.LI);
            MCommand = new RelayCommand(rubikCube.M);
            MICommand = new RelayCommand(rubikCube.MI);
            RCommand = new RelayCommand(rubikCube.R);
            RICommand = new RelayCommand(rubikCube.RI);

            XCommand = new RelayCommand(rubikCube.X);
            XICommand = new RelayCommand(rubikCube.XI);
            YCommand = new RelayCommand(rubikCube.Y);
            YICommand = new RelayCommand(rubikCube.YI);
            ZCommand = new RelayCommand(rubikCube.Z);
            ZICommand = new RelayCommand(rubikCube.ZI);

            Task.Run(DebugFunction);
        }

        private async Task DebugFunction()
        {
            using PeriodicTimer periodicTimer = new(TimeSpan.FromSeconds(3));

            while (await periodicTimer.WaitForNextTickAsync())
            {

            }
        }

        private void RubikCube_FaceChanged(Face face)
        {
            foreach (Piece piece in face.Pieces)
            {
                RubikCube_ColorTypeChanged(face, piece);
            }
        }

        private PropertyChangedEventHandler CreateHandler(int m, int n, ObservableValue<ColorType> observableValue)
        {
            return (s, e) => rubikCube.Set(m, n, observableValue.Value);
        }

        private void RubikCube_ColorTypeChanged(Face face, Piece piece)
        {
            ColorTypes[(int)face.Type * Face.PieceCount + (int)piece.Type].Value = piece.ColorType;
        }

        public void Dispose()
        {
            resumeEvent?.Dispose();
        }
    }
}
