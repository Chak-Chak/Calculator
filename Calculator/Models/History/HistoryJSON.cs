using System.Collections.ObjectModel;
using System.IO;
using Calculator.Interfaces;
using Newtonsoft.Json;

namespace Calculator.Models.History
{
    public class HistoryJSON : IHistory
    {
        public ObservableCollection<Expression> HistoryColl { get; }
        private string path;
        public HistoryJSON(string jsonPath = "history.json")
        {
            path = jsonPath;
            if (File.Exists(path) == false)
            {
                FileStream stream = File.Create(path);
                stream.Close();
                HistoryColl = new ObservableCollection<Expression>();
                return;
            }
            var lastTimeExpressions = File.ReadAllText(path);
            HistoryColl = JsonConvert.DeserializeObject<ObservableCollection<Expression>>(lastTimeExpressions);
            HistoryColl = HistoryColl ?? new ObservableCollection<Expression>();
        }
        public void Add(Expression expression)
        {
            HistoryColl.Add(expression);
            SaveToJson();
        }

        public void Delete(int index)
        {
            HistoryColl.RemoveAt(index);
            SaveToJson();
        }

        public void Clear()
        {
            if (HistoryColl.Count > 0)
            {
                HistoryColl.Clear();
                SaveToJson();
            }
        }

        public void SaveToJson()
        {
            if (File.Exists(path) == false)
            {
                File.Create(path);
            }
            var data = JsonConvert.SerializeObject(HistoryColl);
            File.WriteAllText(path, data);
        }
    }
}
