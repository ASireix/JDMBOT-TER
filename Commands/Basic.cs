using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace BotJDM.Commands
{
    public class Basic : BaseCommandModule
    {
        [Command("test")]
        public async Task TestCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Coucou");
        }

        [Command("hello")]
        public async Task HelloCommand(CommandContext ctx)
        {
            await ctx.RespondAsync("Hello! How can I assist you today?");
        }
    }
}
