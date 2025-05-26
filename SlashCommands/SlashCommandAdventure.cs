using BotJDM.Database.Services;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Threading.Tasks;
using BotJDM.APIRequest.Models;
using BotJDM.Utils;
using DSharpPlus;

namespace BotJDM.SlashCommands
{
    public class SlashCommandAdventure : ApplicationCommandModule
    {
        private static readonly ConversationManager _manager = new();
        private static readonly Random _rng = new();
        private static readonly float baitProbility = 0.5f;
        private static readonly float lieProbility = 0.2f;

        [SlashCommand("start", "Commence une conversation avec le bot.")]
        public async Task Start(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            var state = _manager.GetOrCreateConversation(ctx.Channel.Id, ctx.User.Id);
            var greeting = ConversationManager.GetRandomPhrase("greetings");

            state.conversationStateName = ConversationStateNames.Idle;
            state.lastQuestion = null;
            state.LastInteraction = DateTime.UtcNow;

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(greeting));
        }

        [SlashCommand("stop", "Arrête la conversation.")]
        public async Task Stop(InteractionContext ctx)
        {
            await ctx.DeferAsync();
            _manager.StopConversation(ctx.Channel.Id);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Conversation terminée."));
        }

        public static async Task HandleMessageAsync(DiscordMessage message)
        {
            if (message.Author.IsBot) return;

            if (!_manager.IsUserInConversation(message.Channel.Id, message.Author.Id))
                return;

            var state = _manager.GetOrCreateConversation(message.Channel.Id, message.Author.Id);
            state.LastInteraction = DateTime.UtcNow;
            
            var repType = _manager.GetResponseTypeFromPhrase(message.Content);
            var res = await PhraseAnalyzer.GetMeaningFromPhrase(message.Content);
            (Relation,bool,bool)? relationFound;
            await message.RespondAsync($"The type of phrase you made is : {repType.ToString()}");
            AskedRelation? askedRelation;
            switch (state.conversationStateName)
            {
                case ConversationStateNames.Idle:
                    switch (repType)
                    {
                        case ResponseType.Question:
                            await RespondToQuestion(message, res, state);
                            state.conversationStateName = ConversationStateNames.AttenteFeedback;
                            break;
                        case ResponseType.Affirmation:
                            await RespondToAffirmation(message, res, state);
                            break;
                        case ResponseType.FeedbackPositif:
                            await message.RespondAsync(ConversationManager.GetRandomPhrase("question_error"));
                            break;
                        case ResponseType.FeedbackNegatif:
                            await message.RespondAsync(ConversationManager.GetRandomPhrase("question_error"));
                            break;
                        case ResponseType.DemandeQuestion:
                            //Cherche une question et lui pose
                            state.conversationStateName = ConversationStateNames.AttenteFeedback;
                            break;
                    }

                    break;
                case ConversationStateNames.AttenteReponse:
                    switch (repType)
                    {
                        case ResponseType.Question:
                            await message.RespondAsync(ConversationManager.GetRandomPhrase("rudeResponses"));
                            await RespondToQuestion(message, res, state);
                            break;
                        case ResponseType.Affirmation:
                            await message.RespondAsync(ConversationManager.GetRandomPhrase("rudeResponses"));
                            await RespondToAffirmation(message, res, state);
                            break;
                        case ResponseType.FeedbackPositif:
                            askedRelation = state.lastRelationAsked;
                            if (askedRelation == null)
                            {
                                await message.RespondAsync(ConversationManager.GetRandomPhrase("question_error"));
                            }
                            else
                            {
                                await JDMHelper.UpdateUserTrustFactor(message.Author.Id, message.Author.Username, askedRelation.Value.isCorrect);
                                await message.RespondAsync(ConversationManager.GetRandomPhrase(askedRelation.Value.isCorrect ? "affirmative_keywords" : "negative_keywords"));
                            }
                            
                            state.conversationStateName = ConversationStateNames.Idle;

                            break;
                        case ResponseType.FeedbackNegatif:
                            askedRelation = state.lastRelationAsked;
                            if (askedRelation == null)
                            {
                                await message.RespondAsync(ConversationManager.GetRandomPhrase("question_error"));
                            }
                            else
                            {
                                await JDMHelper.UpdateUserTrustFactor(message.Author.Id, message.Author.Username, !askedRelation.Value.isCorrect);
                                await message.RespondAsync(ConversationManager.GetRandomPhrase(!askedRelation.Value.isCorrect ? "affirmative_keywords" : "negative_keywords"));
                            }
                            
                            break;
                        case ResponseType.DemandeQuestion:
                            //Cherche une question et lui pose
                            state.conversationStateName = ConversationStateNames.AttenteFeedback;
                            break;
                    }
                    break;

                case ConversationStateNames.AttenteFeedback:
                    switch (repType)
                    {
                        case ResponseType.Question:
                            await message.RespondAsync(ConversationManager.GetRandomPhrase("rudeResponses"));
                            await RespondToQuestion(message, res, state);
                            break;
                        case ResponseType.Affirmation:
                            await message.RespondAsync(ConversationManager.GetRandomPhrase("rudeResponses"));
                            await RespondToAffirmation(message, res, state);
                            break;
                        case ResponseType.FeedbackPositif:
                            askedRelation = state.lastRelationAsked;
                            if (askedRelation == null)
                            {
                                await message.RespondAsync(ConversationManager.GetRandomPhrase("question_error"));
                            }
                            else
                            {
                                await JDMHelper.UpdateRelationInDatabase(askedRelation.Value.relation,askedRelation.Value.node1, askedRelation.Value.node2, 
                                    message.Author.Id, message.Author.Username, true);
                                await message.RespondAsync(ConversationManager.GetRandomPhrase("response_thanks") + $"({askedRelation.Value.relation})");
                            }
                            break;
                        case ResponseType.FeedbackNegatif:
                            askedRelation = state.lastRelationAsked;
                            if (askedRelation == null)
                            {
                                await message.RespondAsync(ConversationManager.GetRandomPhrase("question_error"));
                            }
                            else
                            {
                                await JDMHelper.UpdateRelationInDatabase(askedRelation.Value.relation,askedRelation.Value.node1, askedRelation.Value.node2, 
                                    message.Author.Id, message.Author.Username, false);
                                await message.RespondAsync(ConversationManager.GetRandomPhrase("response_thanks") + $"({askedRelation.Value.relation})");
                            }
                            break;
                        case ResponseType.DemandeQuestion:
                            //Cherche une question et lui pose
                            state.conversationStateName = ConversationStateNames.AttenteFeedback;
                            break;
                    }
                    break;
            }
        }

        private static async Task RespondToQuestion(DiscordMessage message, (string subject, string relation, string target)? res, ConversationState state)
        {
            if (res == null)
            {
                await message.RespondAsync(ConversationManager.GetRandomPhrase("question_error"));
            }
            else
            {
                state.oldTheme = res.Value.subject;
                var relationFound = await JDMHelper.FindRelationFromTo(res.Value.subject, res.Value.target,
                    res.Value.relation);
                                
                await message.RespondAsync(relationFound == null
                    ? ConversationManager.GetRandomPhrase("negative_keywords")
                    : ConversationManager.GetRandomPhrase("affirmative_keywords") + $"({res.Value.relation})");
            }
        }
        
        private static async Task RespondToAffirmation(DiscordMessage message, (string subject, string relation, string target)? res, ConversationState state)
        {
            if (res == null)
            {
                await message.RespondAsync(ConversationManager.GetRandomPhrase("question_error"));
            }
            else
            {
                state.oldTheme = res.Value.subject;
                var relationFound = await JDMHelper.FindRelationFromTo(res.Value.subject, res.Value.target,
                    res.Value.relation);

                if (relationFound == null)
                {
                    await JDMHelper.UpdateRelationInDatabase(res.Value.relation,res.Value.subject, res.Value.target, 
                        message.Author.Id, message.Author.Username);
                    await message.RespondAsync(ConversationManager.GetRandomPhrase("response_thanks") + $"({res.Value.relation})");
                }
                else
                {
                    await message.RespondAsync(ConversationManager.GetRandomPhrase("affirmative_keywords") + $"({res.Value.relation})");
                }
            }
        }
    }
}
