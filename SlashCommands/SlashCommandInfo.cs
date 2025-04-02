using BotJDM.Database;
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

        public SlashCommandInfo(UserService userService, RelationService relationService)
        {
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
            var sb = new StringBuilder($"Salut {ctx.User.Username} ! \n" +
                $"Ton Trust Factor est de {trust}.\n" +
                $"Depuis ma création, {relationCount} nouvelles relations ont été trouvé.\n" +
                $"Voici les 5 premières : ");
            if (relationCount > 0)
            {
                foreach (var r in relations)
                {
                    sb.AppendLine($"• Node1: {r.Node1}, Node2: {r.Node2}, Type: {r.Type}, Proba: {r.Probability}");
                }
            }
            embed.Description = sb.ToString();

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
    }

}
