using BotJDM.Utils;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotJDM.SlashCommands
{
    public class SlashCommandProvide : ApplicationCommandModule
    {
        [SlashCommand("r-informer", "Informe le bot d'une relation 'relation' entre objet1 et objet2")]
        public async Task ProvideRelation(InteractionContext ctx, [Option("object1", "Objet 1")] string object1,
        [Option("relation", "Nom de la relation ex:r_agent-1")] string relation, [Option("object2", "Objet 2")] string object2)
        {
            //TODO
            await DiscordCustomMessages.UnimplementedCommand(ctx);
        }
    }
}
