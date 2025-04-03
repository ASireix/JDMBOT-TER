// SlashCommandRate.cs
using BotJDM.APIRequest;
using BotJDM.Database.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.Interactivity.Extensions;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace BotJDM.SlashCommands
{
    public class SlashCommandRate : ApplicationCommandModule
    {
        private readonly NodeService _nodeService;
        private readonly RelationService _relationService;
        private readonly UserService _userService;

        public SlashCommandRate(NodeService nodeService, RelationService relationService, UserService userService)
        {
            _nodeService = nodeService;
            _relationService = relationService;
            _userService = userService;
        }

        [SlashCommand("t-evaluer", "Engage une conversation avec le bot qui vous demandera de vérifier si les relations existent")]
        public async Task RateRelation(InteractionContext ctx)
        {
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
        }
    }
}