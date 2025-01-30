using DSharpPlus.SlashCommands;
using DSharpPlus.Entities;
using System.Threading.Tasks;

public class SlashCommandsAPI : ApplicationCommandModule
{
    // 添加节点的 Slash 命令
    [SlashCommand("addnode", "Add a new node to the database.")]
    public async Task AddNodeAsync(InteractionContext ctx, 
        [Option("name", "The name of the node.")] string nodeName,
        [Option("type", "The type of the node.")] string nodeType,
        [Option("attributes", "The attributes of the node.")] string attributes)
    {
        try
        {
            // 假设这是数据库插入方法，你可以根据实际的 MySqlDatabaseHelper 实现来调整
            await MySqlDatabaseHelper.InsertNode(nodeName, nodeType, attributes);
            
            // 发送成功消息
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent($"Node '{nodeName}' of type '{nodeType}' with attributes '{attributes}' added successfully!"));
        }
        catch (Exception ex)
        {
            // 错误处理
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent($"Error adding node: {ex.Message}"));
        }
    }

    // 获取所有节点的 Slash 命令
    [SlashCommand("getnodes", "Get all nodes from the database.")]
    public async Task GetNodesAsync(InteractionContext ctx)
    {
        try
        {
            // 获取节点的方法
            var nodes = await MySqlDatabaseHelper.GetNodes();

            if (nodes == null || nodes.Count == 0)
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("No nodes found."));
            }
            else
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("Nodes in the database:\n" + string.Join("\n", nodes)));
            }
        }
        catch (Exception ex)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent($"Error retrieving nodes: {ex.Message}"));
        }
    }
}