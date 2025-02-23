using BotJDM.APIRequest;
using BotJDM.APIRequest.Models;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Text;

namespace BotJDM.SlashCommands.Tests;

public class SlashCommandsAPI : ApplicationCommandModule
{
    [SlashCommand("get-node-by-id", "Return node informations from id")]
    public async Task GetNodeId(InteractionContext ctx, [Option("id", "Node id")] long id)
    {
        await ctx.DeferAsync();

        var embed = new DiscordEmbedBuilder()
        {
            Color = DiscordColor.Green
        };

        Node? node = await JDMApiHttpClient.GetNodeById((int)id);
        if (node == null)
        {
            embed.Title = "Node is emty";
            embed.Color = DiscordColor.Red;
        }
        else
        {
            string rep = $"id={node.id}\n" +
                $"name={node.name}\n" +
                $"type={node.type}\n";
            embed.Title = rep;
        }

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }

    [SlashCommand("get-node-by-name", "Return node informations from name")]
    public async Task GetNodeName(InteractionContext ctx, [Option("name", "Node name")] string _name)
    {
        await ctx.DeferAsync();

        var embed = new DiscordEmbedBuilder()
        {
            Color = DiscordColor.Green
        };

        Node? node = await JDMApiHttpClient.GetNodeByName(_name);
        if (node == null)
        {
            embed.Title = "Node is emty";
            embed.Color = DiscordColor.Red;
        }
        else
        {
            string rep = $"id={node.id}\n" +
                $"name={node.name}\n" +
                $"type={node.type}\n";
            embed.Title = rep;
        }

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }

    [SlashCommand("get-node-refinement", "WIP - Return node refinement")]
    public async Task GetNodeRefinement(InteractionContext ctx, [Option("name", "Node name")] string _name)
    {
        await ctx.DeferAsync();

        var embed = new DiscordEmbedBuilder()
        {
            Color = DiscordColor.Green
        };

        Node? node = await JDMApiHttpClient.GetNodeRefinement(_name);
        if (node == null)
        {
            embed.Title = "Node is emty";
            embed.Color = DiscordColor.Red;
        }
        else
        {
            string rep = $"id={node.id}\n" +
                $"name={node.name}\n" +
                $"type={node.type}\n";
            embed.Title = rep;
        }

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }

    [SlashCommand("get-node-types", "Return all node types")]
    public async Task GetNodeTypes(InteractionContext ctx)
    {
        await ctx.DeferAsync();

        var embed = new DiscordEmbedBuilder()
        {
            Color = DiscordColor.Green
        };

        List<NodeType>? nodeTypes = await JDMApiHttpClient.GetNodeTypes();
        if (nodeTypes == null || nodeTypes.Count == 0)
        {
            embed.Title = "List is empty or null";
            embed.Color = DiscordColor.Red;
        }
        else
        {
            string rep = string.Join(",", nodeTypes);
            StringBuilder sb = new StringBuilder();
            foreach (var item in nodeTypes)
            {
                sb.AppendLine(item.name);
            }
            embed.Title = "List of node types";
            embed.Description = sb.ToString();
        }

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }

    [SlashCommand("get-relations-from", "Return all id of relations from a node")]
    public async Task GetRelationFrom(InteractionContext ctx, [Option("name", "Node name")] string nodeName)
    {
        await ctx.DeferAsync();

        var embed = new DiscordEmbedBuilder()
        {
            Color = DiscordColor.Green
        };

        RelationRet? relation = await JDMApiHttpClient.GetRelationsFrom(nodeName);
        if (relation == null)
        {
            embed.Title = "Relation is null";
            embed.Color = DiscordColor.Red;
        }
        else
        {

            embed.Title = $"Relations from {nodeName}";

            string rep = string.Join(",", relation.relations);
            StringBuilder sb = new StringBuilder();
            foreach (var item in relation.relations)
            {
                sb.AppendLine("" + item.type);
            }
            embed.Description = sb.ToString();
        }

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }

    [SlashCommand("get-relations-from-to", "Return all id of relations from node 1 to node 2")]
    public async Task GetRelationFromTo(InteractionContext ctx, [Option("node1", "Node 1 name")] string node1Name,
        [Option("node2", "Node 2 name")] string node2Name)
    {
        await ctx.DeferAsync();

        var embed = new DiscordEmbedBuilder()
        {
            Color = DiscordColor.Green
        };

        RelationRet? relation = await JDMApiHttpClient.GetRelationsFromTo(node1Name, node2Name);
        if (relation == null)
        {
            embed.Title = "Relation is null";
            embed.Color = DiscordColor.Red;
        }
        else
        {
            embed.Title = $"Relations from {node1Name} to {node2Name}";

            string rep = string.Join(",", relation.relations);
            List<int> ids = relation.relations.Select(r => r.type).ToList();
            List<string> relNames = await JDMApiHttpClient.GetRelationNamesFromIds(ids);

            StringBuilder sb = new StringBuilder();
            foreach (var item in relNames)
            {
                sb.AppendLine("nom = " + item);
            }
            embed.Description = sb.ToString();
        }

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }

    [SlashCommand("get-relations-to", "Return all id of relations from node 1 to node 2")]
    public async Task GetRelationTo(InteractionContext ctx, [Option("node", "Node name")] string nodeName)
    {
        await ctx.DeferAsync();

        var embed = new DiscordEmbedBuilder()
        {
            Color = DiscordColor.Green
        };

        RelationRet? relation = await JDMApiHttpClient.GetRelationsTo(nodeName);
        if (relation == null)
        {
            embed.Title = "Relation is null";
            embed.Color = DiscordColor.Red;
        }
        else
        {
            embed.Title = $"Relations to {nodeName}";

            string rep = string.Join(",", relation.relations);
            List<int> ids = relation.relations.Select(r => r.type).ToList();
            List<string> relNames = await JDMApiHttpClient.GetRelationNamesFromIds(ids);
            StringBuilder sb = new StringBuilder();
            foreach (var item in relNames)
            {
                sb.AppendLine("nom = " + item);
            }
            embed.Description = sb.ToString();
        }

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }

    [SlashCommand("get-relation-types", "Return all relation types")]
    public async Task GetRelationTypes(InteractionContext ctx)
    {
        await ctx.DeferAsync();

        var embed = new DiscordEmbedBuilder()
        {
            Color = DiscordColor.Green
        };

        List<RelationType>? relationTypes = await JDMApiHttpClient.GetRelationTypes();
        if (relationTypes == null || relationTypes.Count == 0)
        {
            embed.Title = "List is empty or null";
            embed.Color = DiscordColor.Red;
        }
        else
        {
            string rep = string.Join(",", relationTypes);

            StringBuilder sb = new StringBuilder();
            foreach (var item in relationTypes)
            {
                sb.AppendLine("id = " + item.id + ", " + item.name);
            }
            embed.Title = "List of relations types";
            embed.Description = sb.ToString();
        }

        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }

}