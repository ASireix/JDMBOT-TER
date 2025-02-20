using BotJDM.APIRequest;
using BotJDM.APIRequest.Models;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace BotJDM.SlashCommands;

public class SlashCommandsBasicConv : ApplicationCommandModule
{
    [SlashCommand("ask-relation", "Analyse if there is a relation between the objects")]
    public async Task AskRelation(InteractionContext ctx, [Option("object1","Objet 1")] string object1,
        [Option("relation","Nom de la relation ex:r_agent-1")] string relation,[Option("object2","Objet 2")] string object2)
    {
        await ctx.DeferAsync();
        //Obtenir l'id de la relation à partir du nom
        //Obtenir la liste des id des relations depuis les 2 mots
        //Vérifier que l'id est présent dans la liste

        var embed = new DiscordEmbedBuilder();

        var relationId = await JDMApiHttpClient.GetRelationIdFromName(relation);
        if (relationId == -1)
        {
            embed.Color = DiscordColor.Red;
            embed.Title = "Error";
            embed.Title = "Please enter a valid relation type";
        }
        else
        {
            var relationReferences = await JDMApiHttpClient.GetRelationsFromTo(object1,object2,[relationId]);
            embed.Color = DiscordColor.Green;
            embed.Title = "Entretien";
            embed.Description = relationReferences == null || relationReferences?.relations.Count == 0
                ? $"Non il n'y a pas de relation {relation} entre {object1} et {object2}"
                : $"Evidemment qu'il y a la relation {relation} entre {object1} et {object2}";
        }
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }
    
    [SlashCommand("provide-relation", "Make bot more intelligent")]
    public async Task Provide(InteractionContext ctx, [Option("object1","Objet 1")] string object1,
        [Option("object2","Objet 2")] string object2)
    {
        await ctx.DeferAsync();
    }
    
    [SlashCommand("test-user", "Return a question to answer")]
    public async Task TestUser(InteractionContext ctx)
    {
        await ctx.DeferAsync();
        
        string object1 = "chat";
        string object2 = "chien";
        
        var relationTypes = await JDMApiHttpClient.GetRelationTypes();
        string? relationType = relationTypes?[new Random().Next(relationTypes.Count)].name;
        
        var embed = new DiscordEmbedBuilder()
        {
            Color = DiscordColor.Blue,
            Title = "Question",
            Description = $"Y a-t-il une relation {relationType} entre {object1} et {object2} ?"
        };
        var message = await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        
        var thumbsupEmoji = DiscordEmoji.FromName(ctx.Client, ":thumbsup:");
        var thumbsdownEmoji = DiscordEmoji.FromName(ctx.Client, ":thumbsdown:");
        
        await message.CreateReactionAsync(thumbsupEmoji);
        await message.CreateReactionAsync(thumbsdownEmoji);
        
        var interactivity = ctx.Client.GetInteractivity();
        
        var reactionResult = await interactivity.WaitForReactionAsync(
            x => (x.Emoji == thumbsupEmoji || x.Emoji == thumbsdownEmoji),
            message,
            ctx.User,
            TimeSpan.FromSeconds(10)
        );
        RelationRet? response = null;
        if (relationType != null)
        {
            var relationId = await JDMApiHttpClient.GetRelationIdFromName(relationType);
            response = await JDMApiHttpClient.GetRelationsFromTo(object1, object2, [relationId]);
        }
        if (reactionResult.TimedOut)
        {
            await ctx.Channel.SendMessageAsync($"{ctx.User.Mention}, vous n'avez pas répondu à temps !");
        }
        else
        {
            string answer = reactionResult.Result.Emoji == thumbsupEmoji ? "oui" : "non";
            bool relationNotExist = response == null || response.relations.Count == 0;
            if ((relationNotExist && answer == "oui") || (!relationNotExist && answer == "non"))
            {
                await ctx.Channel.SendMessageAsync($"Merci pour votre réponse : {answer}, \n Mais vous avez tort");
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"Merci pour votre réponse : {answer}, \n Vous êtes un génie");
            }
        }
    }
}