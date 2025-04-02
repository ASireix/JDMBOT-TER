using BotJDM.APIRequest.Models;
using BotJDM.APIRequest;
using BotJDM.Utils;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotJDM.Database.Services;

namespace BotJDM.SlashCommands
{
    public class SlashCommandAsk : ApplicationCommandModule
    {
        private readonly NodeService _nodeService;

        public SlashCommandAsk(NodeService nodeService)
        {
            _nodeService = nodeService;
        }

        [SlashCommand("q-demande", "Demande au bot si la relation 'relation' existe entre objet1 et objet2")]
        public async Task AskRelation(
            InteractionContext ctx,
            [Option("object1", "Objet 1")] string object1,
            [Option("relation", "Nom de la relation ex:r_agent-1")] string relation,
            [Option("object2", "Objet 2")] string object2)
        {
            await ctx.DeferAsync();

            var embed = new DiscordEmbedBuilder();

            var relationId = await JDMApiHttpClient.GetRelationIdFromName(relation);
            if (relationId == -1)
            {
                embed.Color = DiscordColor.Red;
                embed.Title = "Erreur";
                embed.Description = "Relation inconnue.";
            }
            else
            {
                var relationReferences = await JDMApiHttpClient.GetRelationsFromTo(object1, object2, [relationId]);

                if (relationReferences?.relations.Count > 0)
                {
                    embed.Color = DiscordColor.Green;
                    embed.Title = "Réponse API JDM";
                    embed.Description = $"La relation {relation} existe entre {object1} et {object2}.";
                }
                else
                {
                    bool foundInDb = await _nodeService.CheckRelationOrTransitiveAsync(object1, object2, relationId);

                    embed.Color = foundInDb ? DiscordColor.Orange : DiscordColor.Red;
                    embed.Title = "Réponse Base Locale";

                    embed.Description = foundInDb
                        ? $"Relation {relation} trouvée en local entre **{object1}** et **{object2}**."
                        : $"Aucune relation {relation} trouvée entre **{object1}** et **{object2}**.";
                }
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
    }
}
