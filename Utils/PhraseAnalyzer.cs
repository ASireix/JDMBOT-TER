using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace BotJDM.Utils
{
    public static class PhraseAnalyzer
    {
        public static async Task<(string subject, string relation, string target)?> GetMeaningFromPhrase(string phrase)
        {
            try
            {
                string resultJson = await SyntaxAnalyzer.AnalyzeWithPython(phrase);
                var tokens = JsonSerializer.Deserialize<List<Token>>(resultJson);

                foreach (var token in tokens)
                {
                    if (token.dep == "nsubj" || token.dep == "nsubj:pass")
                    {
                        string realVerb = FindTruePredicate(token.head, tokens);
                        if (!string.IsNullOrEmpty(realVerb))
                        {
                            return (token.text, "r_agent-1", realVerb);
                        }
                    }
                    else if (token.dep == "obj" || token.dep == "dobj")
                    {
                        string realVerb = FindTruePredicate(token.head, tokens);
                        if (!string.IsNullOrEmpty(realVerb))
                        {
                            return (realVerb, "r_patient", token.text);
                        }
                    }
                    else if ((token.dep == "xcomp" || token.dep == "ccomp" || token.dep == "advcl") &&
                             token.pos == "VERB")
                    {
                        string realVerb = FindTruePredicate(token.head, tokens);
                        if (!string.IsNullOrEmpty(realVerb) &&
                            realVerb != token.text && realVerb != token.lemma)
                        {
                            return (realVerb, "r_action", token.text);
                        }
                    }
                    else if (token.dep == "iobj")
                    {
                        string realVerb = FindTruePredicate(token.head, tokens);
                        if (!string.IsNullOrEmpty(realVerb))
                        {
                            return (realVerb, "r_benef", token.text);
                        }
                    }
                    else if ((token.dep.StartsWith("obl") || token.dep == "advmod") &&
                             (token.pos == "NOUN" || token.pos == "PROPN" || token.pos == "ADV"))
                    {
                        string realVerb = FindTruePredicate(token.head, tokens);
                        if (!string.IsNullOrEmpty(realVerb))
                        {
                            return (realVerb, "r_place", token.text);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PhraseAnalyzer ERROR] {ex.Message}");
            }

            return null;
        }

        private static string FindTruePredicate(string verbText, List<Token> tokens)
        {
            var current = tokens.Find(t => t.text == verbText);
            if (current == null) return verbText;

            while (current != null)
            {
                if (current.pos == "AUX" || current.dep == "aux")
                {
                    var parent = tokens.Find(t => t.text == current.head);
                    if (parent == null || parent.text == current.text) break;
                    current = parent;
                }
                else
                {
                    break;
                }
            }

            var complements = tokens.FindAll(t =>
                t.head == current.text &&
                (t.dep == "xcomp" || t.dep == "ccomp") &&
                t.pos == "VERB"
            );

            return complements.Count > 0 ? complements[0].lemma : current.lemma;
        }

        private class Token
        {
            public string text { get; set; } = "";
            public string lemma { get; set; } = "";
            public string dep { get; set; } = "";
            public string head { get; set; } = "";
            public string pos { get; set; } = "";
        }
    }
}
