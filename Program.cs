using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.EventArgs;
using System;
using System.Threading.Tasks;
using BotJDM.Config;

namespace DiscordBotTemplateNet7
{
    public sealed class Program
    {
        public static DiscordClient Client { get; private set; }
        public static SlashCommandsExtension SlashCommands { get; private set; }

        static async Task Main(string[] args)
        {
            var botConfig = new BotConfig();
            await botConfig.ReadJSON();

            // 初始化数据库
            await MySqlDatabaseHelper.InitializeDatabase();

            // 配置 Discord 客户端
            var config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,    // 启用所有意图
                Token = botConfig.Token,         // 从配置读取 Token
                TokenType = TokenType.Bot,       // 设置为 Bot 类型
            };

            Client = new DiscordClient(config);
            
            // 使用 SlashCommands
            SlashCommands = Client.UseSlashCommands();

            // 注册 SlashCommand 类
            SlashCommands.RegisterCommands<SlashCommandsAPI>();

            // 监听 Bot 准备就绪事件
            Client.Ready += OnClientReady;

            Console.WriteLine("Commands registered successfully!");

            Console.WriteLine("==============================");
            Console.WriteLine("NET 7.0 C# Discord Bot");
            Console.WriteLine("Made by samjesus8");
            Console.WriteLine("==============================");

            // 连接 Discord
            await Client.ConnectAsync();

            // 阻止程序退出
            await Task.Delay(-1);
        }

        // Bot 启动完成后的事件处理
        private static Task OnClientReady(DiscordClient sender, ReadyEventArgs args)
        {
            Console.WriteLine("Bot is now connected and ready!");
            return Task.CompletedTask;
        }
    }
}