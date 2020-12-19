using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Interfaces
{
    interface IMemory
    {
        ObservableCollection<double> MemoryColl { get; }

        void Add(double value);
        void Delete(int index);
        void Increase(int index, double value);
        void Decrease(int index, double value);
        void Clear();
    }
}
