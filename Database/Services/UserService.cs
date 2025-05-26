using BotJDM.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotJDM.Utils.TrustFactor;

namespace BotJDM.Database.Services
{
    public class UserService
    {
        private readonly BotDBContext _db;

        public UserService(BotDBContext db)
        {
            _db = db;
        }

        public async Task<UserEntity?> GetUser(ulong discordId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.DiscordUserId == discordId);
        }

        public async Task<UserEntity> AddUserAsync(ulong discordId, string username)
        {
            var user = await GetUser(discordId);
            if (user == null)
            {
                user = new UserEntity
                {
                    DiscordUserId = discordId,
                    Username = username,
                    TrustFactor = 0
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }

            return user;
        }

        public async Task UpdateTrustFactorAsync(ulong discordId, float newTrust)
        {
            var user = await AddUserAsync(discordId, "");
            user.TrustFactor = Math.Clamp(newTrust, TrustFactorManager.minTrust, TrustFactorManager.maxTrust);
            await _db.SaveChangesAsync();
        }

        public async Task<float?> GetTrustFactorAsync(ulong discordId)
        {
            var user = await _db.Users.FindAsync(discordId);
            return user?.TrustFactor;
        }
    }
}
