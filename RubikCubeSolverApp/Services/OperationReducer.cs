using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RubikCubeSolverApp.Enums;
using RubikCubeSolverApp.Extensions;

namespace RubikCubeSolverApp.Services
{
    public static class OperationReducer
    {
        public static List<OperationType> ReduceOperations(IList<OperationType> operations)
        {
            List<OperationType> reduced = new();

            for (int i = 0; i != operations.Count;)
            {
                if (i + 2 < operations.Count && operations[i + 1] == operations[i] && operations[i + 2] == operations[i])
                {
                    reduced.Add(operations[i].GetOpposite());
                    i += 3;
                }
                else if (i + 1 < operations.Count && operations[i + 1] == operations[i])
                {
                    reduced.Add(operations[i].GetHalfTurn());
                    i += 2;
                }
                else
                {
                    reduced.Add(operations[i]);
                    ++i;
                }
            }

            return reduced;
        }
    }
}
