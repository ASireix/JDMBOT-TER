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

        public SlashCommandInfo(UserService userService)
        {
            _userService = userService;
        }

        [SlashCommand("info", "Demande au bot son trustfactor et l'état de la nouvelle BD")]
        public async Task AskInfo(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            
            await _userService.AddUserAsync(ctx.User.Id, ctx.User.Username);
            int trust = (await _userService.GetTrustFactorAsync(ctx.User.Id)) ?? 0;
            
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent($"Ton trustfactor est : {trust}"));
        }
    }

}
