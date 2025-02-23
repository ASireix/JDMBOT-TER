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
        [SlashCommand("?","Demande au bot si la relation 'relation' existe entre objet1 et objet2")]
        public async Task AskRelation(InteractionContext ctx, [Option("object1", "Objet 1")] string object1,
        [Option("relation", "Nom de la relation ex:r_agent-1")] string relation, [Option("object2", "Objet 2")] string object2)
        {
            //TODO
        }

        [SlashCommand("!", "Informe le bot d'une relation 'relation' entre objet1 et objet2")]
        public async Task ProvideRelation(InteractionContext ctx, [Option("object1", "Objet 1")] string object1,
        [Option("relation", "Nom de la relation ex:r_agent-1")] string relation, [Option("object2", "Objet 2")] string object2)
        {
            //TODO
        }

        [SlashCommand("=", "Engage une conversation avec le bot qui vous demandera de vérifier si les relations existes. Vous " +
            "pouvez choisir le thème de départ de la relation")]
        public async Task RateRelation(InteractionContext ctx, [Option("object1", "Objet 1")] string object1,
        [Option("relation", "Nom de la relation ex:r_agent-1")] string relation, [Option("object2", "Objet 2")] string object2)
        {
            //TODO
        }
    }
}
