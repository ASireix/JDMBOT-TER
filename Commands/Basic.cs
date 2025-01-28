using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace BotJDM.Commands
{
    public class Basic : BaseCommandModule
    {
        [Command("test")]
        public async Task TestCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Hello");
        }
    }
}
