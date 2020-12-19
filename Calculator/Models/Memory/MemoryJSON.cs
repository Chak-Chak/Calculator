using System.Collections.ObjectModel;
using System.IO;
using Calculator.Interfaces;
using Newtonsoft.Json;

namespace Calculator.Models.Memory
{
    public class MemoryJSON : IMemory
    {
        public ObservableCollection<double> MemoryColl { get; }
        private string path;
        public MemoryJSON(string jsonPath = "memory.json")
        {
            path = jsonPath;
            if (File.Exists(path) == false)
            {
                FileStream stream = File.Create(path);
                stream.Close();
                MemoryColl = new ObservableCollection<double>();
                return;
            }
            var lastTimeExpressions = File.ReadAllText(path);
            MemoryColl = JsonConvert.DeserializeObject<ObservableCollection<double>>(lastTimeExpressions);
            MemoryColl = MemoryColl ?? new ObservableCollection<double>();
        }
        public void Add(double value)
        {
            MemoryColl.Insert(0, value);
            SaveToJson();
        }

        public void Increase(int index, double value)
        {
            MemoryColl[index] += value;
            SaveToJson();
        }

        public void Decrease(int index, double value)
        {
            MemoryColl[index] -= value;
            SaveToJson();
        }

        public void Delete(int index)
        {
            MemoryColl.RemoveAt(index);
            SaveToJson();
        }

        public void Clear()
        {
            if (MemoryColl.Count > 0)
            {
                MemoryColl.Clear();
                SaveToJson();
            }
        }

        public void SaveToJson()
        {
            if (File.Exists(path) == false)
            {
                File.Create(path);
            }
            var data = JsonConvert.SerializeObject(MemoryColl);
            File.WriteAllText(path, data);
        }
    }
}
