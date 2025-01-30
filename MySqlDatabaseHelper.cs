using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MySqlDatabaseHelper
{
    private static string connectionString = "server=127.0.0.1;user=root;database=bot;password=Gn598246";

//initialiser database
    public static async Task InitializeDatabase()
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                await connection.OpenAsync();
                Console.WriteLine("Database connection successful!");

                string createNodesTable = @"
                    CREATE TABLE IF NOT EXISTS nodes (
                        node_id INT AUTO_INCREMENT PRIMARY KEY,
                        node_name VARCHAR(255) NOT NULL,
                        node_type VARCHAR(255) NOT NULL,
                        attributes TEXT NOT NULL,
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
                    )";

                using (var command = new MySqlCommand(createNodesTable, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }

                Console.WriteLine("Tables verified/created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during database initialization: {ex.Message}");
            }
        }
    }

    // inserer node
    public static async Task InsertNode(string nodeName, string nodeType, string attributes)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                await connection.OpenAsync();
                string query = "INSERT INTO nodes (node_name, node_type, attributes) VALUES (@node_name, @node_type, @attributes)";

                using (var command = new MySqlCommand(query, connection))
                {
                    // 将每个参数的值添加到查询中
                    command.Parameters.AddWithValue("@node_name", nodeName);
                    command.Parameters.AddWithValue("@node_type", nodeType);
                    command.Parameters.AddWithValue("@attributes", attributes);

                    // 执行插入操作
                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"Node '{nodeName}' inserted successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during InsertNode: {ex.Message}");
            }
        }
    }

    // gener tous les nodes（GetNodes）
    public static async Task<List<string>> GetNodes()
    {
        var nodes = new List<string>();

        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                await connection.OpenAsync();
                string query = "SELECT node_name FROM nodes";

                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                         while (await reader.ReadAsync())
                    {
                        // 获取列索引并确保没有空值
                        int ordinal = reader.GetOrdinal("node_name");

                        // 检查是否为空值，若为空则跳过
                        if (!reader.IsDBNull(ordinal))
                        {
                            // 使用 GetString() 获取列值，确保返回的是字符串
                            nodes.Add(reader.GetString(ordinal));
                        }
                    }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during GetNodes: {ex.Message}");
            }
        }

        return nodes;
    }

    // 插入对话数据（AddConversation）
    public static async Task AddConversation(int userId, string conversationText)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                await connection.OpenAsync();
                string query = "INSERT INTO conversations (user_id, conversation_text, created_at) VALUES (@user_id, @conversation_text, NOW())";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@user_id", userId);
                    command.Parameters.AddWithValue("@conversation_text", conversationText);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during AddConversation: {ex.Message}");
            }
        }
    }

    // 分析并插入节点或关系（AnalyzeAndInsertNodeOrRelation）
    public static async Task AnalyzeAndInsertNodeOrRelation(int nodeId1, int nodeId2, string relationType, decimal weight)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            try
            {
                await connection.OpenAsync();
                string query = "INSERT INTO relationships (node_id_1, node_id_2, relation_type, weight, status) VALUES (@node_id_1, @node_id_2, @relation_type, @weight, 'active')";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@node_id_1", nodeId1);
                    command.Parameters.AddWithValue("@node_id_2", nodeId2);
                    command.Parameters.AddWithValue("@relation_type", relationType);
                    command.Parameters.AddWithValue("@weight", weight);

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during AnalyzeAndInsertNodeOrRelation: {ex.Message}");
            }
        }
    }
}