using BotJDM.Database.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotJDM.SlashCommands
{
    public class SlashCommandAdventure : ApplicationCommandModule
    {
        private readonly UserService _userService;
        private readonly RelationService _relationService;
        private readonly NodeService _nodeService;

        public SlashCommandAdventure(UserService userService, RelationService relationService, NodeService nodeService)
        {
            _nodeService = nodeService;
            _userService = userService;
            _relationService = relationService;
        }

        [SlashCommand("discussion","Commence une discussion avec le bot de gestion de connaissance")]
        public async Task StartDiscussion(InteractionContext ctx)
        {
            await ctx.DeferAsync();
        }


    }
}
