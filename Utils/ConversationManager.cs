using System.Text.Json;
using javax.management.relation;

namespace BotJDM.Utils;

public class ConversationManager
{
    private readonly Dictionary<ulong, ConversationState> _conversations = new();
    private readonly Timer _cleanupTimer;
    private static readonly Random _rng = new Random();
    private static readonly string PhraseFilePath = Path.Combine("Utils", "phrases.json");
    private static JsonDocument? _phrases;

    public ConversationManager()
    {
        _cleanupTimer = new Timer(_ => CleanupOldConversations(), null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
        LoadPhrases();
    }

    private void LoadPhrases()
    {
        if (!File.Exists(PhraseFilePath))
            throw new FileNotFoundException("phrases.json manquant");

        _phrases = JsonDocument.Parse(File.ReadAllText(PhraseFilePath));
    }

    public static string GetRandomPhrase(string key)
    {
        if (_phrases == null)
            throw new InvalidOperationException("Phrases JSON non chargé");

        if (!_phrases.RootElement.TryGetProperty(key, out var element))
            return "[phrase manquante]";

        if (element.ValueKind == JsonValueKind.Array)
        {
            var values = element.EnumerateArray().Select(e => e.GetString()).Where(s => !string.IsNullOrEmpty(s)).ToList();
            return values.Count > 0 ? values[_rng.Next(values.Count)]! : "[vide]";
        }

        return "[format invalide]";
    }

    public ConversationState GetOrCreateConversation(ulong channelId, ulong userId)
    {
        if (!_conversations.TryGetValue(channelId, out var state))
        {
            state = new ConversationState
            {
                channelId = channelId,
                userId = userId,
                conversationStateName = ConversationStateNames.Idle,
                mode = ConversationMode.Normal,
                oldTheme = "",
                LastInteraction = DateTime.UtcNow
            };
            _conversations[channelId] = state;
        }
        else
        {
            state.LastInteraction = DateTime.UtcNow;
        }

        return state;
    }

    public void StopConversation(ulong channelId)
    {
        _conversations.Remove(channelId);
    }

    private void CleanupOldConversations()
    {
        var cutoff = DateTime.UtcNow - TimeSpan.FromMinutes(1);
        var expired = _conversations
            .Where(kvp => kvp.Value.LastInteraction < cutoff)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expired)
        {
            _conversations.Remove(key);
        }
    }

    public bool IsUserInConversation(ulong channelId, ulong userId)
    {
        return _conversations.TryGetValue(channelId, out var state)
               && state.userId == userId;
    }
    
    private List<string> GetList(string key)
    {
        if (_phrases == null)
            throw new InvalidOperationException("phrases.json non chargé");

        return _phrases.RootElement.TryGetProperty(key, out var prop) && prop.ValueKind == JsonValueKind.Array
            ? prop.EnumerateArray().Select(e => e.GetString()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList()
            : new List<string>();
    }

    public ResponseType GetResponseTypeFromPhrase(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
            return ResponseType.Autre;

        phrase = phrase.ToLower();

        bool ContientMotCle(string key)
        {
            var list = GetList(key);
            return list.Any(k => phrase.Contains(k.ToLower()));
        }

        if (phrase.Trim().EndsWith("?") || ContientMotCle("question_indicators"))
            return ResponseType.Question;

        if (ContientMotCle("demande_question_keywords"))
            return ResponseType.DemandeQuestion;

        if (ContientMotCle("affirmative_keywords"))
            return ResponseType.FeedbackPositif;

        if (ContientMotCle("negative_keywords"))
            return ResponseType.FeedbackNegatif;

        return ResponseType.Affirmation;
    }
}

