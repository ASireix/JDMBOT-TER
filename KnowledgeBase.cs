using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace BotJDM
{
    public class KnowledgeBase
    {
        private Dictionary<string, string> knowledge = new Dictionary<string, string>();
        private const string KnowledgeFilePath = "knowledge.json";

        public void AddKnowledge(string question, string answer)
        {
            knowledge[question.ToLower()] = answer;
            SaveKnowledge();
        }

        public string GetKnowledge(string question)
        {
            return knowledge.ContainsKey(question.ToLower()) ? knowledge[question.ToLower()] : "Je ne sais pas.";
        }

        public bool HasKnowledge(string question)
        {
            return knowledge.ContainsKey(question.ToLower());
        }

        public async Task LoadKnowledge()
        {
            if (File.Exists(KnowledgeFilePath))
            {
                string json = await File.ReadAllTextAsync(KnowledgeFilePath);
                var entries = JsonSerializer.Deserialize<List<KeyValuePair<string, string>>>(json);
                if (entries != null)
                {
                    foreach (var entry in entries)
                    {
                        knowledge[entry.Key] = entry.Value;
                    }
                }
            }
        }

        private void SaveKnowledge()
        {
            var entries = new List<KeyValuePair<string, string>>(knowledge);
            string json = JsonSerializer.Serialize(entries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(KnowledgeFilePath, json);
        }
    }
}