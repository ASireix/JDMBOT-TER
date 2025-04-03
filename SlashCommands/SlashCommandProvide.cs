// SlashCommandProvide.cs
using BotJDM.Database.Entities;
using BotJDM.Database.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using BotJDM.APIRequest;
using System;
using System.Threading.Tasks;

namespace BotJDM.SlashCommands
{
    public class SlashCommandProvide : ApplicationCommandModule
    {
        private readonly NodeService _nodeService;
        private readonly RelationService _relationService;

        public SlashCommandProvide(NodeService nodeService, RelationService relationService)
        {
            _nodeService = nodeService;
            _relationService = relationService;
        }

        [SlashCommand("r-informer", "Informe le bot d'une relation 'relation' entre objet1 et objet2")]
        public async Task ProvideRelation(InteractionContext ctx,
            [Option("object1", "Objet 1")] string object1,
            [Option("relation", "Nom de la relation ex:r_agent-1")] string relation,
            [Option("object2", "Objet 2")] string object2)
        {
            await ctx.DeferAsync();

            var embed = new DiscordEmbedBuilder();

            try
            {
                var node1 = await _nodeService.GetOrCreateNodeAsync(object1);
                var node2 = await _nodeService.GetOrCreateNodeAsync(object2);

                var relationId = await JDMApiHttpClient.GetRelationIdFromName(relation);
                if (relationId == -1)
                {
                    embed.Color = DiscordColor.Red;
                    embed.Title = "Erreur";
                    embed.Description = $"Relation inconnue: {relation}";
                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
                    return;
                }

            
                bool alreadyExists = await _relationService.RelationExistsAsync(node1.Id, node2.Id, relationId);

                if (alreadyExists)
                {
                    embed.Color = DiscordColor.Orange;
                    embed.Title = "Déjà existante";
                    embed.Description = $"La relation **{relation}** entre **{object1}** et **{object2}** est déjà connue.";
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

                    embed.Color = DiscordColor.Green;
                    embed.Title = "Relation enregistrée";
                    embed.Description = $"Merci ! La relation **{relation}** entre **{object1}** et **{object2}** a été enregistrée.";
                }

                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Error] " + ex);
                embed.Color = DiscordColor.Red;
                embed.Title = "Erreur Interne";
                embed.Description = "Une erreur est survenue lors de l'enregistrement de la relation.";
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
            }
        }
    }
}  