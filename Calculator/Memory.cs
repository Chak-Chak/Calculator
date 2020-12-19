using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator.Interfaces;

namespace Calculator.Models
{
    public class Memory : IMemory
    {
        public Memory()
        {
            MemoryColl = MemoryColl ?? new ObservableCollection<double>();
        }

        public ObservableCollection<double> MemoryColl { get; }

        public void Add(double value)
        {
            MemoryColl.Insert(0, value);
        }

        public void Clear()
        {
            MemoryColl.Clear();
        }

        public void Increase(int index, double value)
        {
            MemoryColl[index] += value;
        }

        public void Decrease(int index, double value)
        {
            MemoryColl[index] -= value;
        }

        public void Delete(int index)
        {
            MemoryColl.RemoveAt(index);
        }
    }
}
