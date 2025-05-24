using BotJDM.Database.Entities;
using BotJDM.Database.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace BotJDM.SlashCommands
{
    public class SlashCommandAutoProvide : ApplicationCommandModule
    {
        private readonly NodeService _nodeService;
        private readonly RelationService _relationService;

        public SlashCommandAutoProvide(NodeService nodeService, RelationService relationService)
        {
            _nodeService = nodeService;
            _relationService = relationService;
        }

        [SlashCommand("r-auto", "Analyse une phrase et enregistre automatiquement une ou plusieurs relations")]
        public async Task AutoProvideRelation(InteractionContext ctx,
            [Option("phrase", "Phrase complète à analyser")] string phrase)
        {
            await ctx.DeferAsync();
            var embed = new DiscordEmbedBuilder();

            try
            {
                string resultJson = await SyntaxAnalyzer.AnalyzeWithPython(phrase);
                var tokens = JsonSerializer.Deserialize<List<Token>>(resultJson);
                var triples = new List<(string subject, string relation, string target)>();

                // Debug log
                Console.WriteLine("=== Tokens Analysis ===");
                foreach (var token in tokens)
                {
                    Console.WriteLine($"Text: {token.text}, Lemma: {token.lemma}, Dep: {token.dep}, Head: {token.head}, Pos: {token.pos}");
                }

                foreach (var token in tokens)
                {
                    // sujet (agent)
                    if (token.dep == "nsubj" || token.dep == "nsubj:pass")
                    {
                        string realVerb = FindTruePredicate(token.head, tokens);
                        if (!string.IsNullOrEmpty(realVerb))
                        {
                            triples.Add((token.text, "r_agent-1", realVerb));
                        }
                    }
                    // object (patient)
                    else if (token.dep == "obj" || token.dep == "dobj")
                    {
                        string realVerb = FindTruePredicate(token.head, tokens);
                        if (!string.IsNullOrEmpty(realVerb))
                        {
                            triples.Add((realVerb, "r_patient", token.text));
                        }
                    }
                    //Complément de verbe / clause d'action( xcomp / ccomp / advcl)
                    else if ((token.dep == "xcomp" || token.dep == "ccomp" || token.dep == "advcl") &&
                             token.pos == "VERB")
                    {
                        string realVerb = FindTruePredicate(token.head, tokens);
                        if (!string.IsNullOrEmpty(realVerb) &&
                            realVerb != token.text && realVerb != token.lemma)
                        {
                            triples.Add((realVerb, "r_action", token.text));
                        }
                    }
                    // Objet indirect (beneficiaire)
                    else if (token.dep == "iobj")
                    {
                        string realVerb = FindTruePredicate(token.head, tokens);
                        if (!string.IsNullOrEmpty(realVerb))
                        {
                            triples.Add((realVerb, "r_benef", token.text));
                        }
                    }
                    // Adverbial (lieu/tepms) obl:arg, obl:loc, obl:mod...
                    else if ((token.dep.StartsWith("obl") || token.dep == "advmod") &&
                             (token.pos == "NOUN" || token.pos == "PROPN" || token.pos == "ADV"))
                    {
                        string realVerb = FindTruePredicate(token.head, tokens);
                        if (!string.IsNullOrEmpty(realVerb))
                        {
                            triples.Add((realVerb, "r_place", token.text));
                        }
                    }
                }

                if (triples.Count == 0)
                {
                    embed.Color = DiscordColor.Orange;
                    embed.Title = "Analyse incomplète";
                    embed.Description = "Aucun triplet exploitable trouvé dans la phrase.";
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
                    return;
                }

                var messages = new List<string>();
                foreach (var (subject, relation, target) in triples)
                {
                    var node1 = await _nodeService.GetOrCreateNodeAsync(subject);
                    var node2 = await _nodeService.GetOrCreateNodeAsync(target);
                    var relationId = await APIRequest.JDMApiHttpClient.GetRelationIdFromName(relation);

                    bool exists = await _relationService.RelationExistsAsync(node1.Id, node2.Id, relationId);
                    if (exists)
                    {
                        messages.Add($"⚠️ Relation **{relation}** entre **{subject}** et **{target}** déjà existante.");
                    }
                    else
                    {
                        var newRelation = new RelationEntity
                        {
                            Node1 = node1.Id,
                            Node2 = node2.Id,
                            Type = relationId,
                            CreationDate = DateTime.UtcNow,
                            TouchDate = DateTime.UtcNow,
                            W = 1,
                            C = 1,
                            Nw = 1,
                            Probability = 50,
                            InfoId = 0
                        };
                        await _relationService.AddRelationAsync(newRelation);
                        messages.Add($"✅ Relation **{relation}** entre **{subject}** et **{target}** enregistrée !");
                    }
                }

                embed.Color = DiscordColor.Azure;
                embed.Title = "Résultat de l’analyse";
                embed.Description = string.Join("\n", messages);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
            }
            catch (Exception ex)
            {
                embed.Color = DiscordColor.Red;
                embed.Title = "Erreur interne";
                embed.Description = $"Erreur: {ex.Message}";
                Console.WriteLine($"[EXCEPTION] {ex}");
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
            }
        }

        /// <summary>
        /// 查找谓语动词（处理助动词、补语等）
        /// </summary>
        private string FindTruePredicate(string verbText, List<Token> tokens)
        {
            var current = tokens.Find(t => t.text == verbText);
            if (current == null) return verbText;

            // （skip AUX）
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

            // （peut → utiliser）
            var complements = tokens.FindAll(t =>
                t.head == current.text &&
                (t.dep == "xcomp" || t.dep == "ccomp") &&
                t.pos == "VERB"
            );

            return complements.Count > 0 ? complements[0].lemma : current.lemma;
        }

        public class Token
        {
            public string text { get; set; } = "";
            public string lemma { get; set; } = "";
            public string dep { get; set; } = "";
            public string head { get; set; } = "";
            public string pos { get; set; } = "";
        }
    }
}
