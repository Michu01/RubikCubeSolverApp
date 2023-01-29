using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Win32;

using RubikCubeSolverApp.Enums;
using RubikCubeSolverApp.Models;

namespace RubikCubeSolverApp.Services
{
    internal class RubikCubeFileManager : IRubikCubeFileManager
    {
        public void Load(RubikCube rubikCube)
        {
            var openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Filter = "Rubik cube|*.rc"
            };
            
            if (openFileDialog.ShowDialog() is not bool success || !success)
            {
                return;
            }

            string text = File.ReadAllText(openFileDialog.FileName);

            if (text.Length != 54)
            {
                throw new Exception("Invalid data");
            }

            for (int i = 0; i != RubikCube.FaceCount; i++)
            {
                for (int j = 0; j != Face.PieceCount; j++)
                {
                    int value = text[i * Face.PieceCount + j] - '0';

                    rubikCube.Set(i, j, (ColorType)value);
                }
            }
        }

        public void Save(RubikCube rubikCube)
        {
            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "Rubik cube (*.rc)|*.rc",
                InitialDirectory = Directory.GetCurrentDirectory()
            };

            if (saveFileDialog.ShowDialog() is not bool success || !success)
            {
                return;
            }

            File.WriteAllText(saveFileDialog.FileName, rubikCube.ToString());
        }
    }
}
