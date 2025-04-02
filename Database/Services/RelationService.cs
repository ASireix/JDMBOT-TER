using BotJDM.Database.Entities;
using BotJDM.Utils.TrustFactor;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotJDM.Database.Services
{
    public class RelationService
    {
        private readonly BotDBContext _db;

        public RelationService(BotDBContext db)
        {
            _db = db;
        }

        public async Task<int?> GetProbabilityAsync(int node1, int node2, int type)
        {
            var relation = await _db.Relations
                .FirstOrDefaultAsync(r => r.Node1 == node1 && r.Node2 == node2 && r.Type == type);

            return relation?.Probability;
        }

        public async Task SetProbabilityAsync(int node1, int node2, int type, int probability)
        {
            var relation = await _db.Relations
                .FirstOrDefaultAsync(r => r.Node1 == node1 && r.Node2 == node2 && r.Type == type);

            if (relation != null)
            {
                relation.Probability = Math.Clamp(probability, 0, 100);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> RelationExistsAsync(int node1, int node2, int type)
        {
            return await _db.Relations.AnyAsync(r => r.Node1 == node1 && r.Node2 == node2 && r.Type == type);
        }

        public async Task AddRelationAsync(RelationEntity relation)
        {
            if (!await RelationExistsAsync(relation.Node1, relation.Node2, relation.Type))
            {
                _db.Relations.Add(relation);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<int> CountAsync()
        {
            return await _db.Relations.CountAsync();
        }

        public async Task<List<RelationEntity>> GetFirstRelationsAsync(int amount)
        {
            return await _db.Relations
                .OrderBy(r => r.Id)
                .Where(r => r.Probability >= TrustFactorManager.threshold)
                .Take(amount)
                .ToListAsync();
        }

        public async Task<List<RelationEntity>> GetRelationsFromAsync(int nodeId)
        {
            return await _db.Relations
                .Where(r => r.Node1 == nodeId)
                .ToListAsync();
        }

        public async Task<bool> ExistsTransitiveRelationAsync(int startNodeId, int endNodeId, int relationType, int maxDepth = 3)
        {
            var visited = new HashSet<int>();
            var toVisit = new Queue<(int nodeId, int depth)>();
            toVisit.Enqueue((startNodeId, 0));

            while (toVisit.Count > 0)
            {
                var (currentNode, depth) = toVisit.Dequeue();

                if (depth > maxDepth)
                    continue;

                if (visited.Contains(currentNode))
                    continue;

                visited.Add(currentNode);

                var directRelations = await _db.Relations
                    .Where(r => r.Node1 == currentNode && r.Type == relationType)
                    .ToListAsync();

                foreach (var rel in directRelations)
                {
                    if (rel.Node2 == endNodeId)
                        return true;

                    toVisit.Enqueue((rel.Node2, depth + 1));
                }
            }

            return false;
        }

    }
}
