using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotJDM.Utils
{
    public static class DiscordCustomMessages
    {
        public async static Task UnimplementedCommand(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            var embed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Yellow,
                Title = "Attention",
                Description = "Cette commande n'est pas encore implémenter"
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
    }
}
