using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

public class SlashCommandsAPI : ApplicationCommandModule
{
    [SlashCommand("add_node", "Add a new node to the database.")]
    [SlashRequirePermissions(DSharpPlus.Permissions.Administrator)]
    public async Task AddNodeAsync(InteractionContext ctx,
        [Option("name", "The name of the node.")] string nodeName,
        [Option("type", "The type of the node.")] string nodeType,
        [Option("attributes", "The attributes of the node.")] string attributes)
    {
        try
        {
            long discordId = (long)ctx.User.Id;
            string discordUsername = ctx.User.Username;
            await MySqlDatabaseHelper.EnsureUserExists(discordId, discordUsername);

            await MySqlDatabaseHelper.AddNode(nodeName, nodeType, attributes, discordId);

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Node '{nodeName}' (Type: '{nodeType}') added successfully!"));
        }
        catch (Exception ex)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Error adding node: {ex.Message}"));
        }
    }

    [SlashCommand("get_users", "Get all registered Discord IDs and usernames.")]
    [SlashRequirePermissions(DSharpPlus.Permissions.Administrator)]
    public async Task GetUsersAsync(InteractionContext ctx)
    {
        try
        {
            long discordId = (long)ctx.User.Id;
            string discordUsername = ctx.User.Username;
            await MySqlDatabaseHelper.EnsureUserExists(discordId, discordUsername);

            List<(long discordId, string discordUsername)> users = await MySqlDatabaseHelper.GetAllUserDiscordIds();

            if (users == null || users.Count == 0)
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("No users found."));
            }
            else
            {
                string userList = string.Join("\n", users.Select(user => $"ID: {user.discordId}, Username: {user.discordUsername}"));
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                    .WithContent($"Registered Discord users:\n{userList}"));
            }
        }
        catch (Exception ex)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Error retrieving users: {ex.Message}"));
        }
    }

    [SlashCommand("add_relation", "Add a new relation to the database(between node1 and node2).")]
    [SlashRequirePermissions(DSharpPlus.Permissions.Administrator)]
    public async Task AddRelationAsync(InteractionContext ctx,
        [Option("name", "The name of the relation.")] string relationName,
        [Option("description", "The description of the relation.")] string relationDescription)
    {
        try
        {
            long discordId = (long)ctx.User.Id;
            string discordUsername = ctx.User.Username;
            await MySqlDatabaseHelper.EnsureUserExists(discordId, discordUsername);

            await MySqlDatabaseHelper.AddRelation(relationName, relationDescription, discordId);

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Relation '{relationName}' added successfully!"));
        }
        catch (Exception ex)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Error adding relation: {ex.Message}"));
        }
    }

    [SlashCommand("add_conversation", "Add a new conversation(node1 relation node2).")]
    [SlashRequirePermissions(DSharpPlus.Permissions.Administrator)]
    public async Task AddConversationAsync(InteractionContext ctx,
        [Option("node1", "The name of the first node.")] string node1Name,
        [Option("relation", "The name of the relation.")] string relationName,
        [Option("node2", "The name of the second node.")] string node2Name)
    {
        try
        {
            long discordId = (long)ctx.User.Id;
            string discordUsername = ctx.User.Username;
            await MySqlDatabaseHelper.EnsureUserExists(discordId, discordUsername);

            await MySqlDatabaseHelper.AddConversation(discordId, node1Name, relationName, node2Name);

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Conversation between '{node1Name}' and '{node2Name}' with relation '{relationName}' added successfully!"));
        }
        catch (Exception ex)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Error adding conversation: {ex.Message}"));
        }
    }

    [SlashCommand("get_nodes", "Get all nodes from the database.")]
    [SlashRequirePermissions(DSharpPlus.Permissions.Administrator)]
    public async Task GetNodesAsync(InteractionContext ctx)
    {
        try
        {
            long discordId = (long)ctx.User.Id;
            string discordUsername = ctx.User.Username;
            await MySqlDatabaseHelper.EnsureUserExists(discordId, discordUsername);
            List<(int nodeId, string nodeName, string nodeType, string attributes)> nodes = await MySqlDatabaseHelper.GetAllNodes();

            if (nodes == null || nodes.Count == 0)
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("No nodes found."));
            }
            else
            {
                string nodeList = string.Join("\n", nodes.Select(node => $"ID: {node.nodeId}, Name: {node.nodeName}, Type: {node.nodeType}, Attributes: {node.attributes}"));
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                    .WithContent($"Nodes in the database:\n{nodeList}"));
            }
        }
        catch (Exception ex)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Error retrieving nodes: {ex.Message}"));
        }
    }
    [SlashCommand("get_relations", "Get all relations from the database.")]
    [SlashRequirePermissions(DSharpPlus.Permissions.Administrator)]
    public async Task GetRelationsAsync(InteractionContext ctx)
    {
        try
        {
            long discordId = (long)ctx.User.Id;
            string discordUsername = ctx.User.Username;
            await MySqlDatabaseHelper.EnsureUserExists(discordId, discordUsername);
            List<(int relationId, string relationName, string relationDescription)> relations = await MySqlDatabaseHelper.GetAllRelations();

            if (relations == null || relations.Count == 0)
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("No relations found."));
            }
            else
            {
                string relationList = string.Join("\n", relations.Select(rel => $"ID: {rel.relationId}, Name: {rel.relationName}, Description: {rel.relationDescription}"));
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                    .WithContent($"Relations in the database:\n{relationList}"));
            }
        }
        catch (Exception ex)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Error retrieving relations: {ex.Message}"));
        }
    }

    [SlashCommand("get_conversations", "Get all conversations from the database.")]
    [SlashRequirePermissions(DSharpPlus.Permissions.Administrator)]
    public async Task GetConversationsAsync(InteractionContext ctx)
    {
        try
        {
            long discordId = (long)ctx.User.Id;
            string discordUsername = ctx.User.Username;
            await MySqlDatabaseHelper.EnsureUserExists(discordId, discordUsername);

            List<(int conversationId, string discordUsername, string node1Name, string relationName, string node2Name)> conversations = await MySqlDatabaseHelper.GetAllConversations();

            if (conversations == null || conversations.Count == 0)
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().WithContent("No conversations found."));
            }
            else
            {
                string conversationList = string.Join("\n", conversations.Select(convo => $"ID: {convo.conversationId}, Node1: {convo.node1Name}, Relation: {convo.relationName}, Node2: {convo.node2Name}"));
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                    .WithContent($"Conversations in the database:\n{conversationList}"));
            }
        }
        catch (Exception ex)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Error retrieving conversations: {ex.Message}"));
        }
    }

    [SlashCommand("delete_node", "Delete a node by name.")]
    [SlashRequirePermissions(DSharpPlus.Permissions.Administrator)]
    public async Task DeleteNodeAsync(InteractionContext ctx,
        [Option("name", "The name of the node to delete.")] string nodeName)
    {
        try
        {
            long discordId = (long)ctx.User.Id;
            string discordUsername = ctx.User.Username;
            await MySqlDatabaseHelper.EnsureUserExists(discordId, discordUsername);


            await MySqlDatabaseHelper.DeleteNodeByName(nodeName);

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Node '{nodeName}' deleted successfully!"));
        }
        catch (Exception ex)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Error deleting node: {ex.Message}"));
        }
    }

    [SlashCommand("delete_relation", "Delete a relation by name.")]
    [SlashRequirePermissions(DSharpPlus.Permissions.Administrator)]
    public async Task DeleteRelationAsync(InteractionContext ctx,
        [Option("name", "The name of the relation to delete.")] string relationName)
    {
        try
        {
            long discordId = (long)ctx.User.Id;
            string discordUsername = ctx.User.Username;
            await MySqlDatabaseHelper.EnsureUserExists(discordId, discordUsername);

            await MySqlDatabaseHelper.DeleteRelationByName(relationName);

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Relation '{relationName}' deleted successfully!"));
        }
        catch (Exception ex)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Error deleting relation: {ex.Message}"));
        }
    }


    [SlashCommand("delete_conversation", "Delete a conversation by id, node1_name, relation_name, and node2_name.")]
    [SlashRequirePermissions(DSharpPlus.Permissions.Administrator)]
    public async Task DeleteConversationAsync(InteractionContext ctx,
        [Option("node1", "The name of the first node.")] string node1Name,
        [Option("relation", "The name of the relation.")] string relationName,
        [Option("node2", "The name of the second node.")] string node2Name)
    {
        try
        {
            long discordId = (long)ctx.User.Id;
            string discordUsername = ctx.User.Username;
            await MySqlDatabaseHelper.EnsureUserExists(discordId, discordUsername);

            await MySqlDatabaseHelper.DeleteConversation(node1Name, relationName, node2Name);

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Conversation '{node1Name}''{relationName}''{node2Name}' deleted successfully!"));
        }
        catch (Exception ex)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Error deleting conversation: {ex.Message}"));
        }
    }

}
