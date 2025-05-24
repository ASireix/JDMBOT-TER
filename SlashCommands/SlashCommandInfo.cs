using BotJDM.APIRequest;
using BotJDM.Database;
using BotJDM.Database.Entities;
using BotJDM.Database.Services;
using BotJDM.Utils;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotJDM.SlashCommands
{
    public class SlashCommandInfo : ApplicationCommandModule
    {
        private readonly UserService _userService;
        private readonly RelationService _relationService;
        private readonly NodeService _nodeService;

        public SlashCommandInfo(UserService userService, RelationService relationService,NodeService nodeService)
        {
            _nodeService = nodeService;
            _userService = userService;
            _relationService = relationService;
        }

        [SlashCommand("info", "Demande au bot son trustfactor et l'état de la nouvelle BD")]
        public async Task AskInfo(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            var embed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green
            };

            await _userService.AddUserAsync(ctx.User.Id, ctx.User.Username);
            int trust = (await _userService.GetTrustFactorAsync(ctx.User.Id)) ?? 0;

            var relations = await _relationService.GetFirstRelationsAsync(5);
            var relationCount = await _relationService.CountAsync();
            embed.Title = "Information";
            int amount = relationCount > 5 ? 5 : relationCount;
            var sb = new StringBuilder($"Salut {ctx.User.Username} ! \n" +
                $"Ton Trust Factor est de {trust}.\n" +
                $"Depuis ma création, {relationCount} nouvelles relations ont été trouvé.\n" +
                $"Voici les {amount} premières : \n");
            if (relationCount > 0)
            {
                foreach (var r in relations.Take(5))
                {
                    NodeEntity n1 = await _nodeService.GetNodeByIdAsync(r.Node1);
                    NodeEntity n2 = await _nodeService.GetNodeByIdAsync(r.Node2);
                    string relType = await JDMApiHttpClient.GetRelationNameFromId(r.Type);
                    sb.AppendLine($"• Node1: {n1.Name}, Node2: {n2.Name}, Type: {relType}, Proba: {r.Probability}");
                }
            }
            embed.Description = sb.ToString();

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
    }

}
