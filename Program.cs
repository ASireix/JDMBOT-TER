using BotJDM.Commands;
using BotJDM.Config;
using BotJDM.SlashCommands;
using BotJDM.SlashCommands.Tests;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace BotJDM
{
    public sealed class Program
    {
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        static async Task Main(string[] args)
        {
            var botConfig = new BotConfig();
            await botConfig.ReadJSON();

            await MySqlDatabaseHelper.InitializeDatabase();

            var config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = botConfig.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            Client = new DiscordClient(config);

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(3)
            });

            Client.Ready += OnClientReady;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { botConfig.Prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false
            };
            var slashCommandsConfig = Client.UseSlashCommands();

            try
            {
                slashCommandsConfig.RegisterCommands<SlashCommandsAPI>();
                slashCommandsConfig.RegisterCommands<SlashCommandsBasicConv>();
                slashCommandsConfig.RegisterCommands<SlashConversationCommands>();
                slashCommandsConfig.RegisterCommands<SlashCommandsMain>();
            }
            catch (Exception ex) { }
            

            Commands = Client.UseCommandsNext(commandsConfig);

            Commands.RegisterCommands<Basic>();
            Commands.RegisterCommands<ConversationCommands>();

            await ConversationCommands.InitializeKnowledgeBase();
            await SlashConversationCommands.InitializeKnowledgeBase();
            
            Console.WriteLine("============================== \n" +
                              "NET 9.0 C# Discord Bot \n" +
                              "Made by samjesus8 \n" +
                              "==============================");

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}