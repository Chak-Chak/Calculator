using System.Collections.ObjectModel;
using Calculator.Interfaces;

namespace Calculator.Models.History
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

        public void Delete(int index)
        {
            HistoryColl.RemoveAt(index);
        }

        public void Clear()
        {
            if (HistoryColl.Count > 0) HistoryColl.Clear();
        }
    }
}