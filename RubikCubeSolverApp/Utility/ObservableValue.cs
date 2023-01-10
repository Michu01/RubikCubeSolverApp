using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace RubikCubeSolverApp.Utility
{
    internal class ObservableValue<T> : ObservableObject
        where T : struct
    {
        private T value;

        public T Value
        {
            get => value;
            set => SetProperty(ref this.value, value);
        }
    }
}
