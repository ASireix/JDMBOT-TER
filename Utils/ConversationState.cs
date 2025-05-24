using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotJDM.Utils
{
    public class ConversationState
    {
        public int Step { get; set; }
        public bool LastAnswerWasLie { get; set; }
        public ConversationMode Mode { get; set; }
        public ulong ChannelId { get; set; }
        public ulong UserId { get; set; }
    }

    public enum ConversationMode
    {
        [ChoiceName("Libre")]
        Libre,

        [ChoiceName("Normal")]
        Normal,

        [ChoiceName("Privé")]
        Prive
    }
}
