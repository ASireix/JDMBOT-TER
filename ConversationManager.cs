using System;
using System.Threading.Tasks;

namespace DiscordBotTemplateNet7
{
    public class ConversationManager
    {
        // 假设这是将对话记录存入数据库的方法
        public static async Task RecordConversation(int userId, string conversationText)
        {
            // 调用 MySqlDatabaseHelper 的 AddConversation 方法
            await MySqlDatabaseHelper.AddConversation(userId, conversationText);
        }

        // 处理节点之间的关系，并插入数据库
        public static async Task CreateNodeRelation(int nodeId1, int nodeId2, string relationType, decimal weight)
        {
            // 调用 MySqlDatabaseHelper 的 AnalyzeAndInsertNodeOrRelation 方法
            await MySqlDatabaseHelper.AnalyzeAndInsertNodeOrRelation(nodeId1, nodeId2, relationType, weight);
        }
    }
}