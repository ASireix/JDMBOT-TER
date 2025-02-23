using BotJDM.Utils;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace BotJDM.Commands
{
    public class ConversationCommands : BaseCommandModule
    {
        private static KnowledgeBase knowledgeBase = new KnowledgeBase();
        private static string lastQuestion = string.Empty;

        public static async Task InitializeKnowledgeBase()
        {
            await knowledgeBase.LoadKnowledge();
        }

        [Command("ask")]
        public async Task AskCommand(CommandContext ctx, [RemainingText] string question)
        {
            if (question.Trim().StartsWith("Pourquoi", System.StringComparison.OrdinalIgnoreCase))
            {
                await ctx.RespondAsync("Utilisez la commande !why pour cette question.");
                return;
            }

            lastQuestion = question;
            string answer = knowledgeBase.GetKnowledge(question);
            await ctx.RespondAsync(answer);
        }

        [Command("teach")]
        public async Task TeachCommand(CommandContext ctx, [RemainingText] string input)
        {
            var parts = input.Split('?');
            if (parts.Length < 2)
            {
                await ctx.RespondAsync("Format incorrect. Utilisez: !teach question ? réponse");
                return;
            }

            string question = parts[0].Trim() + "?";
            string answer = parts[1].Trim();

            knowledgeBase.AddKnowledge(question, answer);
            await ctx.RespondAsync("Merci pour ces informations.");
        }

        [Command("why")]
        public async Task WhyCommand(CommandContext ctx, [RemainingText] string question)
        {
            if (!question.Trim().StartsWith("Pourquoi", System.StringComparison.OrdinalIgnoreCase))
            {
                await ctx.RespondAsync("Utilisez la commande !ask pour cette question.");
                return;
            }

            lastQuestion = question;
            string answer = knowledgeBase.GetKnowledge(question);
            if (answer != "Je ne sais pas.")
            {
                await ctx.RespondAsync($"Parce que {answer}");
            }
            else
            {
                await ctx.RespondAsync(answer);
            }
        }

        [Command("respond")]
        public async Task RespondCommand(CommandContext ctx, [RemainingText] string response)
        {
            if (!string.IsNullOrEmpty(lastQuestion))
            {
                knowledgeBase.AddKnowledge(lastQuestion, response);
                await ctx.RespondAsync("Merci pour ces informations.");
                lastQuestion = string.Empty;
            }
            else
            {
                await ctx.RespondAsync("Je n'ai pas de question en attente.");
            }
        }
    }
}