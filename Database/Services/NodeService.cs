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

        public async Task<NodeEntity> UpdateNode(NodeEntity entity)
        {
            NodeEntity? existingNode = null;

            if (entity.Id >= 0)
            {
                // Si un ID est donné, on cherche cet ID
                existingNode = await _db.Nodes.FirstOrDefaultAsync(n => n.Id == entity.Id);
            }

            if (existingNode == null)
            {
                // Insertion (ID manuel ou généré)
                entity.CreationDate = DateTime.UtcNow;
                entity.TouchDate = DateTime.UtcNow;

                var entry = await _db.Nodes.AddAsync(entity);
                await _db.SaveChangesAsync();
                return entry.Entity;
            }

            // Mise à jour
            entity.CreationDate = existingNode.CreationDate;
            entity.TouchDate = DateTime.UtcNow;

            _db.Entry(existingNode).CurrentValues.SetValues(entity);
            await _db.SaveChangesAsync();
            return existingNode;
        }



        public async Task<NodeEntity?> GetNodeByNameAsync(string name)
        {
            return await _db.Nodes.FirstOrDefaultAsync(n => n.Name == name);
        }

        public async Task<NodeEntity?> GetNodeByIdAsync(int id)
        {
            return await _db.Nodes.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<List<NodeEntity>> GetAllNodesAsync()
        {
            return await _db.Nodes.ToListAsync();
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