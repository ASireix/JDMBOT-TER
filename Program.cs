using BotJDM.Commands;
using BotJDM.Config;
using BotJDM.Database.Services;
using BotJDM.Database;
using BotJDM.SlashCommands;
using BotJDM.SlashCommands.Tests;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace BotJDM
{
    public sealed class Program
    {
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }

        static async Task Main(string[] args)
        {
            // ✅ 使用 bin 目录中的 botdata.db
            var dbPath = Path.Combine(AppContext.BaseDirectory, "botdata.db");

            Console.WriteLine("📂 当前运行目录: " + AppContext.BaseDirectory);
            Console.WriteLine("💾 正在使用数据库路径: " + dbPath);
            Console.WriteLine("📊 数据库是否存在: " + File.Exists(dbPath));
            Console.WriteLine("🔐 是否可写: " + (File.Exists(dbPath) && !new FileInfo(dbPath).IsReadOnly));

            var services = new ServiceCollection();

            services.AddDbContext<BotDBContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            services.AddScoped<UserService>();
            services.AddScoped<RelationService>();
            services.AddScoped<NodeService>();

            var serviceProvider = services.BuildServiceProvider();

            var botConfig = new BotConfig();
            await botConfig.ReadJSON();

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

            var slashCommandsConfig = Client.UseSlashCommands(new SlashCommandsConfiguration
            {
                Services = serviceProvider
            });

            try
            {
                slashCommandsConfig.RegisterCommands<SlashCommandsAPI>();
                slashCommandsConfig.RegisterCommands<SlashCommandsBasicConv>();
                slashCommandsConfig.RegisterCommands<SlashConversationCommands>();
                slashCommandsConfig.RegisterCommands<SlashCommandAsk>();
                slashCommandsConfig.RegisterCommands<SlashCommandProvide>();
                slashCommandsConfig.RegisterCommands<SlashCommandRate>();
                slashCommandsConfig.RegisterCommands<SlashCommandInfo>();
                slashCommandsConfig.RegisterCommands<SlashCommandAdventure>();
                slashCommandsConfig.RegisterCommands<SlashCommandAutoProvide>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ SlashCommand 注册异常: " + ex.Message);
            }

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
