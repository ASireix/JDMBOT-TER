using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MySqlDatabaseHelper
{
    private static string connectionString = "server=127.0.0.1;user=root;database=bot;password=Gn598246";

    public static async Task InitializeDatabase()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                await connection.OpenAsync();
                Console.WriteLine("Database connection successful!");

                string createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS users (
                        user_id INT AUTO_INCREMENT PRIMARY KEY,
                        discord_id BIGINT NOT NULL UNIQUE,
                        discord_username VARCHAR(255) NOT NULL,  -- 添加discord_username字段
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    )";

                string createNodesTable = @"
                    CREATE TABLE IF NOT EXISTS nodes (
                        node_id INT AUTO_INCREMENT PRIMARY KEY,
                        node_name VARCHAR(255) NOT NULL,
                        node_type VARCHAR(255) NOT NULL,
                        attributes TEXT NOT NULL,
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
                    )";

                string createRelationsTable = @"
                    CREATE TABLE IF NOT EXISTS relations (
                        relation_id INT AUTO_INCREMENT PRIMARY KEY,
                        relation_name VARCHAR(255) NOT NULL,
                        relation_description TEXT NOT NULL,
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    )";

                string createConversationsTable = @"
                    CREATE TABLE IF NOT EXISTS conversations (
                        conversation_id INT AUTO_INCREMENT PRIMARY KEY,
                        discord_id BIGINT,    
                        node1_id  INT,
                        relation_id INT,
                        node2_id INT,
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        FOREIGN KEY (discord_id) REFERENCES users(discord_id),
                        FOREIGN KEY (node1_id) REFERENCES nodes(node_id),
                        FOREIGN KEY (node2_id) REFERENCES nodes(node_id),
                        FOREIGN KEY (relation_id) REFERENCES relations(relation_id)
                    )";

                await ExecuteNonQueryAsync(createUsersTable, connection);
                await ExecuteNonQueryAsync(createNodesTable, connection);
                await ExecuteNonQueryAsync(createRelationsTable, connection);
                await ExecuteNonQueryAsync(createConversationsTable, connection);

                Console.WriteLine("Tables verified/created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during database initialization: {ex.Message}");
            }
        }
    }

    private static async Task ExecuteNonQueryAsync(string query, MySqlConnection connection)
    {
        using (var command = new MySqlCommand(query, connection))
        {
            await command.ExecuteNonQueryAsync();
        }
    }

    // assuer l'utilisateur deja existe ou pas
    public static async Task EnsureUserExists(long discordId, string discordUsername)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                await connection.OpenAsync();
                string checkUserQuery = "SELECT COUNT(*) FROM users WHERE discord_id = @discord_id";
                using (var command = new MySqlCommand(checkUserQuery, connection))
                {
                    command.Parameters.AddWithValue("@discord_id", discordId);
                    int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                    if (count == 0)
                    {
                        string insertUserQuery = "INSERT INTO users (discord_id, discord_username) VALUES (@discord_id, @discord_username)";
                        using (var insertCommand = new MySqlCommand(insertUserQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@discord_id", discordId);
                            insertCommand.Parameters.AddWithValue("@discord_username", discordUsername); // 传递用户名
                            await insertCommand.ExecuteNonQueryAsync();
                            Console.WriteLine($"User {discordId} ({discordUsername}) added to database.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during EnsureUserExists: {ex.Message}");
            }
        }
    }

    // inserer les relation
    public static async Task AddRelation(string relationName, string relationDescription, long discordId)
    {
        await EnsureUserExists(discordId, "");

        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                await connection.OpenAsync();
                string query = "INSERT INTO relations (relation_name, relation_description) VALUES (@relation_name, @relation_description)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@relation_name", relationName);
                    command.Parameters.AddWithValue("@relation_description", relationDescription);
                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"Relation '{relationName}' added successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during AddRelation: {ex.Message}");
            }
        }
    }

//inserer node
    public static async Task AddNode(string nodeName, string nodeType, string attributes, long discordId)
    {
        await EnsureUserExists(discordId, ""); 

        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                await connection.OpenAsync();
                string query = "INSERT INTO nodes (node_name, node_type, attributes) VALUES (@node_name, @node_type, @attributes)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@node_name", nodeName);
                    command.Parameters.AddWithValue("@node_type", nodeType);
                    command.Parameters.AddWithValue("@attributes", attributes);
                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"Node '{nodeName}' added successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during AddNode: {ex.Message}");
            }
        }
    }

 // obtenir le ID et nom de discord de chaque utilisateur
    public static async Task<List<(long discordId, string discordUsername)>> GetAllUserDiscordIds()
    {
        List<(long discordId, string discordUsername)> userList = new List<(long, string)>();
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                await connection.OpenAsync();
                string query = "SELECT discord_id, discord_username FROM users"; 

                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            long discordId = reader.GetInt64(0); 
                            string discordUsername = reader.GetString(1); 
                            userList.Add((discordId, discordUsername));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during GetAllUserDiscordIds: {ex.Message}");
            }
        }

        return userList;
    }

//ajouter les phrase : node1 relation node 2
    public static async Task AddConversation(long discordId, string node1Name, string relationName, string node2Name)
{
    using (var connection = new MySqlConnection(connectionString))
    {
        try
        {
            await connection.OpenAsync();

            // obtenir node1_id
            string getNode1IdQuery = "SELECT node_id FROM nodes WHERE node_name = @node_name";
            long node1Id = await GetNodeIdByName(getNode1IdQuery, node1Name, connection);

            // obtenir node2_id
            string getNode2IdQuery = "SELECT node_id FROM nodes WHERE node_name = @node_name";
            long node2Id = await GetNodeIdByName(getNode2IdQuery, node2Name, connection);

            // obtenir relation_id
            string getRelationIdQuery = "SELECT relation_id FROM relations WHERE relation_name = @relation_name";
            long relationId = await GetRelationIdByName(getRelationIdQuery, relationName, connection);

            string insertConversationQuery = @"
                INSERT INTO conversations (discord_id, node1_id, relation_id, node2_id) 
                VALUES (@discord_id, @node1_id, @relation_id, @node2_id)";
            
            using (var command = new MySqlCommand(insertConversationQuery, connection))
            {
                command.Parameters.AddWithValue("@discord_id", discordId);
                command.Parameters.AddWithValue("@node1_id", node1Id);
                command.Parameters.AddWithValue("@relation_id", relationId);
                command.Parameters.AddWithValue("@node2_id", node2Id);

                await command.ExecuteNonQueryAsync();
                Console.WriteLine($"Conversation added: {discordId}, {node1Name}, {relationName}, {node2Name}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred during AddConversation: {ex.Message}");
        }
    }
}

private static async Task<long> GetNodeIdByName(string query, string nodeName, MySqlConnection connection)
{
    using (var command = new MySqlCommand(query, connection))
    {
        command.Parameters.AddWithValue("@node_name", nodeName);
        object result = await command.ExecuteScalarAsync();
        if (result != null)
        {
            return Convert.ToInt64(result);
        }
        else
        {
            throw new Exception($"Node '{nodeName}' not found.");
        }
    }
}

private static async Task<long> GetRelationIdByName(string query, string relationName, MySqlConnection connection)
{
    using (var command = new MySqlCommand(query, connection))
    {
        command.Parameters.AddWithValue("@relation_name", relationName);
        object result = await command.ExecuteScalarAsync();
        if (result != null)
        {
            return Convert.ToInt64(result);
        }
        else
        {
            throw new Exception($"Relation '{relationName}' not found.");
        }
    }
}

// obtenir tous les nodes
public static async Task<List<(int nodeId, string nodeName, string nodeType, string attributes)>> GetAllNodes()
{
    List<(int nodeId, string nodeName, string nodeType, string attributes)> nodes = new List<(int, string, string, string)>();

    using (var connection = new MySqlConnection(connectionString))
    {
        try
        {
            await connection.OpenAsync();
            string query = "SELECT node_id, node_name, node_type, attributes FROM nodes"; 

            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int nodeId = reader.GetInt32(0); 
                        string nodeName = reader.GetString(1); 
                        string nodeType = reader.GetString(2); 
                        string attributes = reader.GetString(3); 
                        nodes.Add((nodeId, nodeName, nodeType, attributes));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred during GetAllNodes: {ex.Message}");
        }
    }

    return nodes;
}

// obtenir tous les relations
public static async Task<List<(int relationId, string relationName, string relationDescription)>> GetAllRelations()
{
    List<(int relationId, string relationName, string relationDescription)> relations = new List<(int, string, string)>();

    using (var connection = new MySqlConnection(connectionString))
    {
        try
        {
            await connection.OpenAsync();
            string query = "SELECT relation_id, relation_name, relation_description FROM relations"; 

            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int relationId = reader.GetInt32(0); 
                        string relationName = reader.GetString(1); 
                        string relationDescription = reader.GetString(2); 
                        relations.Add((relationId, relationName, relationDescription));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred during GetAllRelations: {ex.Message}");
        }
    }

    return relations;
}
//obtenir tous les phrase
public static async Task<List<(int conversationId, string discordUsername, string node1Name, string relationName, string node2Name)>> GetAllConversations()
{
    List<(int conversationId, string discordUsername, string node1Name, string relationName, string node2Name)> conversations = new List<(int, string, string, string, string)>();

    using (var connection = new MySqlConnection(connectionString))
    {
        try
        {
            await connection.OpenAsync();
            string query = @"
                SELECT c.conversation_id, u.discord_username, n1.node_name AS node1_name, r.relation_name, n2.node_name AS node2_name
                FROM conversations c
                JOIN users u ON c.discord_id = u.discord_id 
                JOIN nodes n1 ON c.node1_id = n1.node_id
                JOIN relations r ON c.relation_id = r.relation_id
                JOIN nodes n2 ON c.node2_id = n2.node_id
                ORDER BY c.created_at DESC"; 

            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int conversationId = reader.GetInt32(0); 
                        string discordUsername = reader.GetString(1); 
                        string node1Name = reader.GetString(2);
                        string relationName = reader.GetString(3); 
                        string node2Name = reader.GetString(4); 

                        conversations.Add((conversationId, discordUsername, node1Name, relationName, node2Name));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred during GetAllConversations: {ex.Message}");
        }
    }

    return conversations;
}

public static async Task DeleteNodeByName(string nodeName)
{
    using (var connection = new MySqlConnection(connectionString))
    {
        try
        {
            await connection.OpenAsync();

            string checkNodeExistsQuery = "SELECT COUNT(*) FROM nodes WHERE node_name = @node_name";
            using (var checkCommand = new MySqlCommand(checkNodeExistsQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@node_name", nodeName);
                int nodeCount = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                if (nodeCount == 0)
                {
                    Console.WriteLine($"Node '{nodeName}' does not exist.");
                    return;
                }
            }

            string deleteConversationsQuery = @"
                DELETE FROM conversations 
                WHERE node1_id = (SELECT node_id FROM nodes WHERE node_name = @node_name)
                OR node2_id = (SELECT node_id FROM nodes WHERE node_name = @node_name)";
            using (var deleteConversationsCommand = new MySqlCommand(deleteConversationsQuery, connection))
            {
                deleteConversationsCommand.Parameters.AddWithValue("@node_name", nodeName);
                await deleteConversationsCommand.ExecuteNonQueryAsync();
                Console.WriteLine($"Conversations related to node '{nodeName}' have been deleted.");
            }

            string deleteNodeQuery = "DELETE FROM nodes WHERE node_name = @node_name";
            using (var deleteNodeCommand = new MySqlCommand(deleteNodeQuery, connection))
            {
                deleteNodeCommand.Parameters.AddWithValue("@node_name", nodeName);
                await deleteNodeCommand.ExecuteNonQueryAsync();
                Console.WriteLine($"Node '{nodeName}' has been deleted.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred during DeleteNodeByName: {ex.Message}");
        }
    }
}
public static async Task DeleteRelationByName(string relationName)
{
    using (var connection = new MySqlConnection(connectionString))
    {
        try
        {
            await connection.OpenAsync();

            string checkRelationExistsQuery = "SELECT COUNT(*) FROM relations WHERE relation_name = @relation_name";
            using (var checkCommand = new MySqlCommand(checkRelationExistsQuery, connection))
            {
                checkCommand.Parameters.AddWithValue("@relation_name", relationName);
                int relationCount = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());

                if (relationCount == 0)
                {
                    Console.WriteLine($"Relation '{relationName}' does not exist.");
                    return;
                }
            }

            string deleteConversationsQuery = @"
                DELETE FROM conversations 
                WHERE relation_id = (SELECT relation_id FROM relations WHERE relation_name = @relation_name)";
            using (var deleteConversationsCommand = new MySqlCommand(deleteConversationsQuery, connection))
            {
                deleteConversationsCommand.Parameters.AddWithValue("@relation_name", relationName);
                await deleteConversationsCommand.ExecuteNonQueryAsync();
                Console.WriteLine($"Conversations related to relation '{relationName}' have been deleted.");
            }

            string deleteRelationQuery = "DELETE FROM relations WHERE relation_name = @relation_name";
            using (var deleteRelationCommand = new MySqlCommand(deleteRelationQuery, connection))
            {
                deleteRelationCommand.Parameters.AddWithValue("@relation_name", relationName);
                await deleteRelationCommand.ExecuteNonQueryAsync();
                Console.WriteLine($"Relation '{relationName}' has been deleted.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred during DeleteRelationByName: {ex.Message}");
        }
    }
}

public static async Task DeleteConversation(string node1Name, string relationName, string node2Name)
{
    using (var connection = new MySqlConnection(connectionString))
    {
        try
        {
            await connection.OpenAsync();

            string getNode1IdQuery = "SELECT node_id FROM nodes WHERE node_name = @node_name";
            long node1Id = await GetNodeIdByName(getNode1IdQuery, node1Name, connection);

            string getNode2IdQuery = "SELECT node_id FROM nodes WHERE node_name = @node_name";
            long node2Id = await GetNodeIdByName(getNode2IdQuery, node2Name, connection);

            string getRelationIdQuery = "SELECT relation_id FROM relations WHERE relation_name = @relation_name";
            long relationId = await GetRelationIdByName(getRelationIdQuery, relationName, connection);

            string deleteConversationQuery = @"
                DELETE FROM conversations
                WHERE node1_id = @node1_id AND relation_id = @relation_id AND node2_id = @node2_id";
            
            using (var command = new MySqlCommand(deleteConversationQuery, connection))
            {
                command.Parameters.AddWithValue("@node1_id", node1Id);
                command.Parameters.AddWithValue("@relation_id", relationId);
                command.Parameters.AddWithValue("@node2_id", node2Id);

                await command.ExecuteNonQueryAsync();
                Console.WriteLine($"Conversation between '{node1Name}', '{relationName}', and '{node2Name}' deleted successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred during DeleteConversation: {ex.Message}");
        }
    }
}

}
