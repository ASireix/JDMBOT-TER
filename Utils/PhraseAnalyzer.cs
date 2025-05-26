using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BotJDM.Utils
{
    public static class PhraseAnalyzer
    {
        private static readonly HashSet<string> AllowedFallbackRelations = new()
        {
            "r_agent-1", "r_patient", "r_action", "r_benef", "r_lieu", "r_instr", "r_instr-1", "r_isa"
        };

        private static readonly HashSet<string> WeakVerbs = new()
        {
            "avoir", "être", "faire", "y", "il", "exister"
        };

        public static async Task<(string subject, string relation, string target)?> GetMeaningFromPhrase(string phrase)
        {
            try
            {
                var relationTypes = JDMHelper.LoadRelationTypes();
                var relationNames = new HashSet<string>(relationTypes.Select(r => r.name));

                // 1. Relation explicite dans la phrase
                var words = phrase.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < words.Length - 2; i++)
                {
                    string candidate = words[i + 1];
                    if (relationNames.Contains(candidate))
                    {
                        Console.WriteLine($"[Explicit] {words[i]} {candidate} {words[i + 2]}");
                        return (words[i], candidate, words[i + 2]);
                    }
                }

                // 2. Fallback NLP
                string resultJson = await SyntaxAnalyzer.AnalyzeWithPython(phrase);
                var tokens = JsonSerializer.Deserialize<List<Token>>(resultJson);

                if (tokens == null || tokens.Count == 0) return null;

                // 2.1 Copule avec "être" -> r_isa
                var attrToken = tokens.FirstOrDefault(t => t.dep == "attr");
                if (attrToken != null)
                {
                    var subjectToken = tokens.FirstOrDefault(t => t.head == attrToken.head && (t.dep == "nsubj" || t.dep == "nsubj:pass"));
                    if (subjectToken != null)
                    {
                        LogFallback("r_isa", subjectToken.text, attrToken.text);
                        return (subjectToken.text, "r_isa", attrToken.text);
                    }
                }

                // 2.2 "il y a X en Y" ou "X en Y" -> r_lieu
                var possibleLieu = tokens.FirstOrDefault(t => t.dep.StartsWith("obl") && (t.pos == "PROPN" || t.pos == "NOUN"));
                if (possibleLieu != null)
                {
                    var governor = tokens.FirstOrDefault(t => t.text == possibleLieu.head && !WeakVerbs.Contains(t.lemma));
                    if (governor != null && governor.pos != "VERB") // Ne pas extraire si verbe conjugué
                    {
                        LogFallback("r_lieu", governor.text, possibleLieu.text);
                        return (governor.text, "r_lieu", possibleLieu.text);
                    }
                }

                // 2.3 Fallback sur les dépendances classiques
                foreach (var token in tokens)
                {
                    string realVerb = FindTruePredicate(token.head, tokens);
                    var verbToken = tokens.FirstOrDefault(t => t.text == realVerb);

                    // Exclure si verbe conjugué (indicatif, subjonctif, etc.)
                    if (verbToken != null && verbToken.pos == "VERB" && char.IsLower(verbToken.text[0]))
                    {
                        continue;
                    }

                    if ((token.dep == "nsubj" || token.dep == "nsubj:pass") && verbToken != null)
                    {
                        LogFallback("r_agent-1", token.text, realVerb);
                        return (token.text, "r_agent-1", realVerb);
                    }

                    if ((token.dep == "obj" || token.dep == "dobj") && verbToken != null)
                    {
                        LogFallback("r_patient", realVerb, token.text);
                        return (realVerb, "r_patient", token.text);
                    }

                    if ((token.dep == "xcomp" || token.dep == "ccomp" || token.dep == "advcl") &&
                        token.pos == "VERB" && verbToken != null &&
                        realVerb != token.text && realVerb != token.lemma)
                    {
                        LogFallback("r_action", realVerb, token.text);
                        return (realVerb, "r_action", token.text);
                    }

                    if (token.dep == "iobj" && verbToken != null)
                    {
                        LogFallback("r_benef", realVerb, token.text);
                        return (realVerb, "r_benef", token.text);
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

            var visited = new HashSet<string>();
            while (current != null && !visited.Contains(current.text))
            {
                visited.Add(current.text);
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
                t.pos == "VERB");

            return complements.Count > 0 ? complements[0].text : current.text;
        }

        private static void LogFallback(string relation, string subject, string target)
        {
            if (AllowedFallbackRelations.Contains(relation))
            {
                Console.WriteLine($"[Fallback] {subject} {relation} {target}");
            }
            else
            {
                Console.WriteLine($"[Fallback SKIPPED] {subject} ? {target} : {relation} non autorisée");
            }
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
