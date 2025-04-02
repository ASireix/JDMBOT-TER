using BotJDM.Database.Entities;
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
    }
}
