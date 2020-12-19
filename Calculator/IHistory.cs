using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Interfaces
{
    interface IHistory
    {
        ObservableCollection<Expression> HistoryColl { get; }

        void Add(Expression expression);
        void Delete(int index);
        void Clear();
    }
}