using BotJDM.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotJDM.Database.Services
{
    public class NodeService
    {
        private readonly BotDBContext _db;

        public NodeService(BotDBContext db)
        {
            _db = db;
        }

        public async Task<NodeEntity?> GetNodeByNameAsync(string name)
        {
            return await _db.Nodes.FirstOrDefaultAsync(n => n.Name == name);
        }

        public async Task<bool> RelationExistsBetweenAsync(string nodeName1, string nodeName2, int relationTypeId)
        {
            var node1 = await GetNodeByNameAsync(nodeName1);
            var node2 = await GetNodeByNameAsync(nodeName2);

            if (node1 == null || node2 == null)
                return false;

            return await _db.Relations.AnyAsync(r =>
                r.Node1 == node1.Id &&
                r.Node2 == node2.Id &&
                r.Type == relationTypeId);
        }

        public async Task<bool> CheckRelationOrTransitiveAsync(string nodeName1, string nodeName2, int relationTypeId)
        {
            var node1 = await GetNodeByNameAsync(nodeName1);
            var node2 = await GetNodeByNameAsync(nodeName2);

            if (node1 == null || node2 == null)
                return false;

            // relation directe
            bool direct = await _db.Relations.AnyAsync(r =>
                r.Node1 == node1.Id &&
                r.Node2 == node2.Id &&
                r.Type == relationTypeId);

            if (direct)
                return true;

            // relation indirecte
            var relService = new RelationService(_db);
            return await relService.ExistsTransitiveRelationAsync(node1.Id, node2.Id, relationTypeId);
        }

        public async Task<List<NodeEntity>> GetAllNodesAsync() 
        {
            return await _db.Nodes.ToListAsync();
        }

       
        public async Task<NodeEntity?> GetNodeByIdAsync(int id) 
        {
            return await _db.Nodes.FirstOrDefaultAsync(n => n.Id == id);
        }
    
        public async Task<NodeEntity> GetOrCreateNodeAsync(string name)
        {
            var node = await GetNodeByNameAsync(name);
            if (node != null)
                return node;

            var newNode = new NodeEntity
            {
                Name = name,
                Type = 0,
                W = 1,
                C = 1,
                Level = 0,
                CreationDate = DateTime.UtcNow,
                TouchDate = DateTime.UtcNow
            };

            _db.Nodes.Add(newNode);
            await _db.SaveChangesAsync();
            return newNode;
        }

    }
}
