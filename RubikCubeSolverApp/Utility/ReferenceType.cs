using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubikCubeSolverApp.Utility
{
    public class ReferenceType<T>
        where T : struct
    {
        public T Value { get; set; }
    }
}
