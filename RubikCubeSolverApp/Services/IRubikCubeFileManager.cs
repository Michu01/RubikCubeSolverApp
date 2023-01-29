using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RubikCubeSolverApp.Models;

namespace RubikCubeSolverApp.Services
{
    internal interface IRubikCubeFileManager
    {
        void Save(RubikCube rubikCube);

        void Load(RubikCube rubikCube);
    }
}
