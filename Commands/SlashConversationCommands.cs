using DSharpPlus.SlashCommands;
using System.Threading.Tasks;

namespace BotJDM.Commands
{
    public class SlashConversationCommands : ApplicationCommandModule
    {
        private static KnowledgeBase knowledgeBase = new KnowledgeBase();
        private static string lastQuestion = string.Empty;

        public static async Task InitializeKnowledgeBase()
        {
            await knowledgeBase.LoadKnowledge();
        }

        [SlashCommand("ask", "Pose une question au bot")]
        public async Task AskCommand(InteractionContext ctx, [Option("question", "La question à poser")] string question)
        {
            if (question.Trim().StartsWith("Pourquoi", System.StringComparison.OrdinalIgnoreCase))
            {
                await ctx.CreateResponseAsync("Utilisez la commande /why pour cette question.");
                return;
            }

            lastQuestion = question;
            string answer = knowledgeBase.GetKnowledge(question);
            await ctx.CreateResponseAsync(answer);
        }

        [SlashCommand("teach", "Enseigne une nouvelle information au bot")]
        public async Task TeachCommand(InteractionContext ctx, [Option("question", "La question")] string question, [Option("answer", "La réponse")] string answer)
        {
            knowledgeBase.AddKnowledge(question, answer);
            await ctx.CreateResponseAsync("Merci pour ces informations.");
        }

        [SlashCommand("why", "Pose une question de type 'Pourquoi' au bot")]
        public async Task WhyCommand(InteractionContext ctx, [Option("question", "La question à poser")] string question)
        {
            if (!question.Trim().StartsWith("Pourquoi", System.StringComparison.OrdinalIgnoreCase))
            {
                await ctx.CreateResponseAsync("Utilisez la commande /ask pour cette question.");
                return;
            }

            lastQuestion = question;
            string answer = knowledgeBase.GetKnowledge(question);
            if (answer != "Je ne sais pas.")
            {
                await ctx.CreateResponseAsync($"Parce que {answer}");
            }
            else
            {
                await ctx.CreateResponseAsync(answer);
            }
        }

        [SlashCommand("respond", "Répond à la dernière question posée")]
        public async Task RespondCommand(InteractionContext ctx, [Option("response", "La réponse à la question")] string response)
        {
            if (!string.IsNullOrEmpty(lastQuestion))
            {
                knowledgeBase.AddKnowledge(lastQuestion, response);
                await ctx.CreateResponseAsync("Merci pour ces informations.");
                lastQuestion = string.Empty;
            }
            else
            {
                await ctx.CreateResponseAsync("Je n'ai pas de question en attente.");
            }
        }
    }
}