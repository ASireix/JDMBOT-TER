using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using System;
using System.Threading.Tasks;
using BotJDM.Config;

namespace BotJDM
{
    public sealed class Program
    {
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        public static SlashCommandsExtension SlashCommands { get; private set; }

        static async Task Main(string[] args)
        {
            var botConfig = new BotConfig();
            await botConfig.ReadJSON();

            await MySqlDatabaseHelper.InitializeDatabase();

            var config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                //Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMembers | DiscordIntents.GuildPresences,
                Token = botConfig.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
                Intents = DiscordIntents.All,   
                Token = botConfig.Token,        
                TokenType = TokenType.Bot,     
            };

            Client = new DiscordClient(config);
            
            SlashCommands = Client.UseSlashCommands();

            SlashCommands.RegisterCommands<SlashCommandsAPI>();

            Client.Ready += OnClientReady;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { botConfig.Prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<Basic>();
            Commands.RegisterCommands<ConversationCommands>();

            SlashCommands = Client.UseSlashCommands();
            SlashCommands.RegisterCommands<SlashConversationCommands>();

            await ConversationCommands.InitializeKnowledgeBase();
            await SlashConversationCommands.InitializeKnowledgeBase();
            
            Console.WriteLine("============================== \n" +
                              "Les commandes de Rigbot sont utilisables. \n" +
                              "==============================");
            Console.WriteLine("Commands registered successfully!");

            Console.WriteLine("==============================");
            Console.WriteLine("NET 7.0 C# Discord Bot");
            Console.WriteLine("Made by samjesus8");
            Console.WriteLine("==============================");

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs args)
        {
            Console.WriteLine("Bot is now connected and ready!");
            return Task.CompletedTask;
        }
    }
}
