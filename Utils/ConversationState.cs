using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotJDM.APIRequest.Models;

namespace BotJDM.Utils
{
    public class ConversationState
    {
        public ConversationStateNames conversationStateName { get; set; }
        public string? lastQuestion { get; set; }
        public ConversationMode mode { get; set; }
        public string? oldTheme { get; set; }
        public AskedRelation? lastRelationAsked { get; set; }
        public ulong channelId { get; set; }
        public ulong userId { get; set; }
        public DateTime LastInteraction { get; set; } = DateTime.UtcNow;
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

    public enum ResponseType
    {
        Question,
        Affirmation,
        FeedbackPositif,
        FeedbackNegatif,
        DemandeQuestion,
        Autre
    }

    public enum ConversationStateNames
    {
        Idle,
        AttenteReponse,
        AttenteFeedback
    }

    public struct AskedRelation
    {
        public string relation { get; set; }
        public string node2 { get; set; }
        public string node1 { get; set; }
        public bool isExpectingAnswer { get; set; }
        public bool isCorrect { get; set; }
    }
}
