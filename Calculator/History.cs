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
    public class History : IHistory
    {
        public History()
        {
            HistoryColl = HistoryColl ?? new ObservableCollection<Expression>();
        }

        public ObservableCollection<Expression> HistoryColl { get; }

        public void Add(Expression expression)
        {
            HistoryColl.Insert(0, expression);
        }

        public void Clear()
        {
            HistoryColl.Clear();
        }

        public void Delete(int index)
        {
            HistoryColl.RemoveAt(index);
        }
    }
}