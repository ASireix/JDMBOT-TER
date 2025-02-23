using BotJDM.Utils;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotJDM.SlashCommands
{
    public class SlashCommandsMain : ApplicationCommandModule
    {
        [SlashCommand("q-demande","Demande au bot si la relation 'relation' existe entre objet1 et objet2")]
        public async Task AskRelation(InteractionContext ctx, [Option("object1", "Objet 1")] string object1,
        [Option("relation", "Nom de la relation ex:r_agent-1")] string relation, [Option("object2", "Objet 2")] string object2)
        {
            //TODO
            await DiscordCustomMessages.UnimplementedCommand(ctx);
        }

        [SlashCommand("r-informer", "Informe le bot d'une relation 'relation' entre objet1 et objet2")]
        public async Task ProvideRelation(InteractionContext ctx, [Option("object1", "Objet 1")] string object1,
        [Option("relation", "Nom de la relation ex:r_agent-1")] string relation, [Option("object2", "Objet 2")] string object2)
        {
            //TODO
            await DiscordCustomMessages.UnimplementedCommand(ctx);
        }

        [SlashCommand("t-evaluer", "Engage une conversation avec le bot qui vous demandera de vérifier si les relations existes")]
        public async Task RateRelation(InteractionContext ctx, [Option("object1", "Objet 1")] string object1,
        [Option("relation", "Nom de la relation ex:r_agent-1")] string relation, [Option("object2", "Objet 2")] string object2)
        {
            //TODO
            await DiscordCustomMessages.UnimplementedCommand(ctx);
        }
    }
}
