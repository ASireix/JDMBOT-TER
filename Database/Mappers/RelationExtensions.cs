using BotJDM.APIRequest.Models;
using BotJDM.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotJDM.Database.Mappers
{
    public static class RelationExtensions
    {
        public static RelationEntity ToEntity(this Relation relation, int probability = 0)
        {
            return new RelationEntity
            {
                Id = relation.id,
                Node1 = relation.node1,
                Node2 = relation.node2,
                Type = relation.type,
                W = relation.w,
                C = relation.c,
                InfoId = relation.infoId,
                CreationDate = relation.creationDate,
                TouchDate = relation.touchDate,
                Nw = relation.nw,
                Probability = probability
            };
        }

        public static Relation ToRelation(this RelationEntity entity)
        {
            return new Relation
            {
                id = entity.Id,
                node1 = entity.Node1,
                node2 = entity.Node2,
                type = entity.Type,
                w = entity.W,
                c = entity.C,
                infoId = entity.InfoId,
                creationDate = entity.CreationDate,
                touchDate = entity.TouchDate,
                nw = entity.Nw
            };
        }
    }
}
