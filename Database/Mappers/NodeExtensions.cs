using BotJDM.APIRequest.Models;
using BotJDM.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotJDM.Database.Mappers
{
    public static class NodeExtensions
    {
        public static NodeEntity ToEntity(this Node node)
        {
            return new NodeEntity
            {
                Id = node.id,
                Name = node.name,
                Type = node.type,
                W = node.w,
                C = node.c,
                Level = node.level,
                InfoId = node.infoId,
                CreationDate = node.creationDate,
                TouchDate = node.touchDate
            };
        }
    }
}
