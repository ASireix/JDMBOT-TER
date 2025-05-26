using BotJDM.Database.Entities;
using BotJDM.Utils.TrustFactor;
using Microsoft.EntityFrameworkCore;
using BotJDM.APIRequest;
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
        private readonly int probaThreshold = 50;

        public RelationService(BotDBContext db)
        {
            _db = db;
        }

        #region Probability

        public async Task<int?> GetProbabilityAsync(int relationId)
        {
            var relation = await _db.Relations
                .FirstOrDefaultAsync(r => r.Id == relationId);

            return relation?.Probability;
        }

        public async Task SetProbabilityAsync(int relationId, int probability)
        {
            var relation = await _db.Relations
                .FirstOrDefaultAsync(r => r.Id == relationId);

            if (relation != null)
            {
                relation.Probability = Math.Clamp(probability, 0, 100);
                await _db.SaveChangesAsync();
            }
        }

        #endregion
        
        public async Task<bool> RelationExistsAsync(int node1, int node2, int type)
        {
            return await _db.Relations.AnyAsync(r => r.Node1 == node1 && r.Node2 == node2 && r.Type == type);
        }
        
        public async Task<RelationEntity?> GetRelFromTo(int node1, int node2, int type)
        {
            return await _db.Relations.FirstOrDefaultAsync(r => r.Node1 == node1 && r.Node2 == node2 && r.Type == type);
        }

        public async Task<RelationEntity> AddRelationAsync(RelationEntity relation)
        {
            var existing = await _db.Relations.FirstOrDefaultAsync(r =>
                r.Node1 == relation.Node1 &&
                r.Node2 == relation.Node2 &&
                r.Type == relation.Type);

            if (existing != null)
            {
                return existing; // relation déjà existante
            }

            var entry = await _db.Relations.AddAsync(relation);
            await _db.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<int> CountAsync()
        {
            return await _db.Relations.CountAsync();
        }
        
        /// <summary>
        /// Renvoie les X premières relations
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public async Task<List<RelationEntity>> GetFirstRelationsAsync(int amount)
        {
            return await _db.Relations
                .OrderBy(r => r.Id)
                .Where(r => r.Probability >= probaThreshold)
                .Take(amount)
                .ToListAsync();
        }

        public async Task<List<RelationEntity>> GetRelationsFromAsync(int nodeId)
        {
            return await _db.Relations
                .Where(r => r.Node1 == nodeId)
                .ToListAsync();
        }

        public async Task<List<RelationEntity>> GetRelationsToAsync(int nodeId)
        {
            return await _db.Relations.Where(r => r.Node2 == nodeId).ToListAsync();
        }

        private async Task<RelationEntity?> ExistsTransitiveRelationAsync(int startNodeId, int endNodeId, int relationType,
            int maxDepth = 3, int minProba = -1)
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
                    .Where(r => r.Node1 == currentNode && r.Type == relationType 
                                                       && r.Probability >= (minProba < 0 ? probaThreshold : minProba))
                    .ToListAsync();

                foreach (var rel in directRelations)
                {
                    if (rel.Node2 == endNodeId)
                        return rel;

                    toVisit.Enqueue((rel.Node2, depth + 1));
                }
            }

            return null;
        }

        public async Task<RelationEntity?> RelationExistsBetweenAsync(string nodeName1, string nodeName2, int relationTypeId, int minProba = -1)
        {
            var node1 = await _db.Nodes.FirstOrDefaultAsync(n => n.Name == nodeName1);
            var node2 = await _db.Nodes.FirstOrDefaultAsync(n => n.Name == nodeName2);

            if (node1 == null || node2 == null)
                return null;

            return await _db.Relations.FirstOrDefaultAsync(r =>
                r.Node1 == node1.Id &&
                r.Node2 == node2.Id &&
                r.Type == relationTypeId &&
                r.Probability >= (minProba < 0 ? probaThreshold : minProba));
        }

        public async Task<RelationEntity?> CheckRelationOrTransitiveAsync(string nodeName1, string nodeName2, int relationTypeId, int maxDepth = 3, int minProba = -1)
        {
            var node1 = await _db.Nodes.FirstOrDefaultAsync(n => n.Name == nodeName1);
            var node2 = await _db.Nodes.FirstOrDefaultAsync(n => n.Name == nodeName2);

            if (node1 == null || node2 == null)
                return null;

            var rel = await RelationExistsBetweenAsync(nodeName1, nodeName2, relationTypeId, minProba);
            if (rel != null)
                return rel;

            return await ExistsTransitiveRelationAsync(node1.Id, node2.Id, relationTypeId, maxDepth, minProba);
        }
    }
}