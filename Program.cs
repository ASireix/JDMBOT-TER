using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.EventArgs;
using System;
using System.Threading.Tasks;
using BotJDM.Config;

namespace BotJDM
{
    public sealed class Program
    {
        public static DiscordClient Client { get; private set; }
        public static SlashCommandsExtension SlashCommands { get; private set; }

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
            };

            Client = new DiscordClient(config);
            
            SlashCommands = Client.UseSlashCommands();

            SlashCommands.RegisterCommands<SlashCommandsAPI>();

            Client.Ready += OnClientReady;

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
