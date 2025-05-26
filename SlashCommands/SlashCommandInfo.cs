using BotJDM.APIRequest;
using BotJDM.Database;
using BotJDM.Database.Entities;
using BotJDM.Database.Services;
using BotJDM.Utils;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotJDM.APIRequest.Models;
using DSharpPlus;
using DSharpPlus.Interactivity.Extensions;

namespace BotJDM.SlashCommands
{
    public class SlashCommandInfo : ApplicationCommandModule
    {
        private readonly UserService _userService;
        private readonly RelationService _relationService;
        private readonly NodeService _nodeService;

        public SlashCommandInfo(UserService userService, RelationService relationService,NodeService nodeService)
        {
            _nodeService = nodeService;
            _userService = userService;
            _relationService = relationService;
        }

        [SlashCommand("info", "Demande au bot son trustfactor et l'état de la nouvelle BD")]
        public async Task AskInfo(InteractionContext ctx)
        {
            Console.WriteLine("Registered SlashCommandInfo");
            await ctx.DeferAsync();

            var embed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green
            };

            await _userService.AddUserAsync(ctx.User.Id, ctx.User.Username);
            float trust = (await _userService.GetTrustFactorAsync(ctx.User.Id)) ?? 0;

            var relations = await _relationService.GetFirstRelationsAsync(5);
            var relationCount = await _relationService.CountAsync();
            embed.Title = "Information";
            int amount = relationCount > 5 ? 5 : relationCount;
            var sb = new StringBuilder($"Salut {ctx.User.Username} ! \n" +
                $"Ton Trust Factor est de {trust}.\n" +
                $"Depuis ma création, {relationCount} nouvelles relations ont été trouvé.\n" +
                $"Voici les {amount} premières : \n");
            if (relationCount > 0)
            {
                foreach (var r in relations.Take(5))
                {
                    NodeEntity n1 = await _nodeService.GetNodeByIdAsync(r.Node1);
                    NodeEntity n2 = await _nodeService.GetNodeByIdAsync(r.Node2);
                    string relType = await JDMApiHttpClient.GetRelationNameFromId(r.Type);
                    sb.AppendLine($"• Node1: {n1.Name}, Node2: {n2.Name}, Type: {relType}, Proba: {r.Probability}");
                }
            }
            embed.Description = sb.ToString();

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }

        [SlashCommand("liste-types", "Donne la liste des types de relations possibles")]
        public async Task ListeTypes(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            var items = JDMHelper.LoadRelationTypes(); // ta désérialisation JSON
            int page = 0;
            int itemsPerPage = 20;
            int lastPage = (int)Math.Ceiling(items.Count / (double)itemsPerPage) - 1;

            var builder = new DiscordWebhookBuilder()
                .AddEmbed(BuildPage(items, page, itemsPerPage))
                .AddComponents(
                    new DiscordButtonComponent(ButtonStyle.Primary, "first", "⏮️", false),
                    new DiscordButtonComponent(ButtonStyle.Primary, "prev", "◀️", false),
                    new DiscordButtonComponent(ButtonStyle.Primary, "next", "▶️", false),
                    new DiscordButtonComponent(ButtonStyle.Primary, "last", "⏭️", false)
                );

            var message = await ctx.EditResponseAsync(builder);

            var interactivity = ctx.Client.GetInteractivity();
            while (true)
            {
                var result = await interactivity.WaitForButtonAsync(message, ctx.User, TimeSpan.FromMinutes(2));
                if (result.TimedOut) break;

                switch (result.Result.Id)
                {
                    case "first":
                        page = 0;
                        break;
                    case "prev":
                        page = Math.Max(page - 1, 0);
                        break;
                    case "next":
                        page = Math.Min(page + 1, lastPage);
                        break;
                    case "last":
                        page = lastPage;
                        break;
                }

                await result.Result.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                    .AddEmbed(BuildPage(items, page, itemsPerPage))
                    .AddComponents(
                        new DiscordButtonComponent(ButtonStyle.Primary, "first", "⏮️", page == 0),
                        new DiscordButtonComponent(ButtonStyle.Primary, "prev", "◀️", page == 0),
                        new DiscordButtonComponent(ButtonStyle.Primary, "next", "▶️", page == lastPage),
                        new DiscordButtonComponent(ButtonStyle.Primary, "last", "⏭️", page == lastPage)
                    ));
            }
        }

        [SlashCommand("analyse", "Check if text recognition works")]
        public async Task Analyse(InteractionContext ctx, [Option("phrase", "Phrase à analyser")] string phrase)
        {
            await ctx.DeferAsync();
            
            var res = await PhraseAnalyzer.GetMeaningFromPhrase(phrase);
            if (res == null) res = new ValueTuple<string, string, string>("", "", "");
            var n1 = res.Value.subject;
            var n2 = res.Value.target;
            var rel = res.Value.relation;
            var embed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Red,
                Description = $"You tried to get the relation {rel} from {n1} to {n2}"
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
        private DiscordEmbed BuildPage(List<RelationType> items, int pageIndex, int itemsPerPage = 20)
        {
            int totalPages = (int)Math.Ceiling(items.Count / (double)itemsPerPage);
            int startIndex = pageIndex * itemsPerPage;
            var pageItems = items.Skip(startIndex).Take(itemsPerPage).ToList();

            var description = string.Join("\n", pageItems.Select((item, i) =>
                $"{startIndex + i + 1} - {item.name} : {item.gpName}"));

            return new DiscordEmbedBuilder()
                .WithTitle($"Relations JDM – Page {pageIndex + 1}/{totalPages}")
                .WithDescription(description)
                .WithColor(DiscordColor.Azure)
                .Build();
        }

    }

}
