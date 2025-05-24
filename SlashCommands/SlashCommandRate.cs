// SlashCommandRate.cs
using BotJDM.APIRequest;
using BotJDM.Database.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Extensions;
using System.Linq;
using System.Threading.Tasks;
using System;
using BotJDM.APIRequest.Models;

namespace BotJDM.SlashCommands
{
    public class SlashCommandRate : ApplicationCommandModule
    {
        private readonly NodeService _nodeService;
        private readonly RelationService _relationService;
        private readonly UserService _userService;
        private int maxApiNodes = 40000;

        public SlashCommandRate(NodeService nodeService, RelationService relationService, UserService userService)
        {
            _nodeService = nodeService;
            _relationService = relationService;
            _userService = userService;
        }

        [SlashCommand("t-evaluer", "Engage une conversation avec le bot qui vous demandera de vérifier si les relations existent")]
        public async Task RateRelation(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            bool continuer = true;

            while (continuer)
            {
                continuer = await RunEvaluationRound(ctx);
            }
            /*
            try
            {
                Console.WriteLine("[SlashCommand] /t-evaluer triggered by user: " + ctx.User.Username);
                await ctx.DeferAsync();

                var allNodes = await _nodeService.GetAllNodesAsync();
                Console.WriteLine($"[Info] Total nodes fetched: {allNodes.Count}");

                if (!allNodes.Any())
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Aucun noeud disponible."));
                    return;
                }

                var random = new Random();
                var randomNode = allNodes[random.Next(allNodes.Count)];
                Console.WriteLine($"[Info] Random node selected: {randomNode.Name} (ID: {randomNode.Id})");

                var relations = await _relationService.GetRelationsFromAsync(randomNode.Id);
                bool isReversed = false;

                if (!relations.Any())
                {
                    Console.WriteLine($"[Info] No outgoing relations from node {randomNode.Name}, trying as target (node2)...");
                    relations = await _relationService.GetRelationsToAsync(randomNode.Id);
                    isReversed = true;
                }

                if (!relations.Any())
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Aucune relation trouvée autour de {randomNode.Name}."));
                    return;
                }

                var selectedRelation = relations[random.Next(relations.Count)];
                var node1 = await _nodeService.GetNodeByIdAsync(selectedRelation.Node1);
                var node2 = await _nodeService.GetNodeByIdAsync(selectedRelation.Node2);

                if (node1 == null || node2 == null)
                {
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Un des noeuds liés est introuvable."));
                    return;
                }

                var relationName = await JDMApiHttpClient.GetRelationNameFromId(selectedRelation.Type);
                string question = $"**{node1.Name} → {relationName} → {node2.Name}**\nEst-ce que cette relation existe ? (oui / non)";

                var embed = new DiscordEmbedBuilder
                {
                    Title = "Évaluation de relation",
                    Description = question,
                    Color = DiscordColor.Azure
                };

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));

                var interactivity = ctx.Client.GetInteractivity();
                Console.WriteLine("[Await] Waiting for user input (oui/non)...");
                var response = await interactivity.WaitForMessageAsync(
                    m => m.Author.Id == ctx.User.Id &&
                        (m.Content.ToLower() == "oui" || m.Content.ToLower() == "non"),
                    TimeSpan.FromSeconds(20));

                if (response.TimedOut)
                {
                    Console.WriteLine("[Timeout] User did not respond in time.");
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("⏳ Temps expiré. Veuillez réessayer."));
                    return;
                }

                bool userSaidYes = response.Result.Content.ToLower() == "oui";
                bool isCorrect = userSaidYes; 

                Console.WriteLine("[Trust] Updating TrustFactor...");
                await _userService.AddUserAsync(ctx.User.Id, ctx.User.Username);
                var trust = await _userService.GetTrustFactorAsync(ctx.User.Id) ?? 0;
                trust += isCorrect ? 5 : -5;
                await _userService.UpdateTrustFactorAsync(ctx.User.Id, trust);

                var feedback = new DiscordEmbedBuilder()
                {
                    Title = isCorrect ? "Bonne réponse!" : "Mauvaise réponse...",
                    Description = $"Ton TrustFactor est maintenant : {trust}",
                    Color = isCorrect ? DiscordColor.Green : DiscordColor.Red
                };

                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(feedback));
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Error] " + ex);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Une erreur est survenue. Veuillez réessayer."));
            }
            */
        }

        private async Task<bool> RunEvaluationRound(InteractionContext ctx)
        {
            var random = new Random();

            bool shouldBeTrue = random.Next(2) == 0;

            //bool useApi = random.Next(2) == 0;
            bool useApi = true;
            string node1 = null;
            string node2 = null;
            string relationName = null;

            var embed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green
            };

            if (useApi)
            {
                Node? rdmNode = await JDMApiHttpClient.GetNodeById(random.Next(0, maxApiNodes));
                RelationRet? res = await JDMApiHttpClient.GetRelationsFrom(rdmNode.name);
                Relation rel = res.relations[random.Next(0, res.relations.Count)];
                Node n1 = await JDMApiHttpClient.GetNodeById(rel.node1);
                Node n2 = await JDMApiHttpClient.GetNodeById(rel.node2);
                node1 = n1.name;
                node2 = n2.name;
                if (shouldBeTrue)
                {
                    relationName = await JDMApiHttpClient.GetRelationNameFromId(rel.type);
                }
                else
                {
                    List<RelationType>? types = await JDMApiHttpClient.GetRelationTypes();
                    int relationId = GetRandomExcluding(types.Count, rel.type, random);
                    relationName = await JDMApiHttpClient.GetRelationNameFromId(relationId);
                }
            }
            else
            {
                if (shouldBeTrue)
                {

                }
                else
                {

                }
            }

            embed.Title = "Question";
            embed.Description = $"Est-ce qu'une relation **{relationName}** entre **{node1}** et **{node2}** existe ? (oui / non)";
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));

            var interactivity = ctx.Client.GetInteractivity();
            var response = await interactivity.WaitForMessageAsync(
                m => m.Author.Id == ctx.User.Id &&
                     (m.Content.ToLower().Contains("oui") || m.Content.ToLower().Contains("non")),
                TimeSpan.FromSeconds(20)
            );

            if (response.TimedOut)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("⏳ Temps écoulé. Essaie encore !"));
                return false;
            }

            bool userSaidTrue = response.Result.Content.ToLower().Contains("oui");
            bool isCorrect = userSaidTrue == shouldBeTrue;

            await _userService.AddUserAsync(ctx.User.Id, ctx.User.Username);
            var trust = await _userService.GetTrustFactorAsync(ctx.User.Id) ?? 0;
            int delta = isCorrect ? CalculateTrustGain(trust) : -CalculateTrustPenalty(trust);
            int newTrust = Math.Clamp(trust + delta, -100, 100);
            await _userService.UpdateTrustFactorAsync(ctx.User.Id, newTrust);

            var feedback = new DiscordEmbedBuilder
            {
                Title = isCorrect ? "✅ Bonne réponse !" : "❌ Mauvaise réponse...",
                Description = $"🎯 Ton TrustFactor est maintenant : **{newTrust}**",
                Color = isCorrect ? DiscordColor.Green : DiscordColor.Red
            };

            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(feedback));

            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("🔁 Tu veux rejouer ? (oui / non)"));

            var retry = await interactivity.WaitForMessageAsync(
                m => m.Author.Id == ctx.User.Id &&
                     (m.Content.ToLower().Contains("oui") || m.Content.ToLower().Contains("non")),
                TimeSpan.FromSeconds(15)
            );

            if (!retry.TimedOut && retry.Result.Content.ToLower().Contains("oui"))
                return true;

            return false;
        }
        private int CalculateTrustGain(int trust)
        {
            if (trust >= 90) return 1;
            if (trust >= 70) return 2;
            if (trust >= 30) return 3;
            return 5;
        }

        private int CalculateTrustPenalty(int trust)
        {
            if (trust <= -90) return 1;
            if (trust <= -70) return 2;
            if (trust <= -30) return 3;
            return 5;
        }

        int GetRandomExcluding(int max, int excluded, Random random)
        {
            int result;
            result = random.Next(max);
            while (result == excluded)
            {
                result = random.Next(max);
            }

            return result;
        }

    }

}