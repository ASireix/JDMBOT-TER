using BotJDM.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotJDM.Database.Services
{
    public class UserService
    {
        private readonly BotDBContext _db;

        public UserService(BotDBContext db)
        {
            _db = db;
        }

        public async Task<bool> UserExistsAsync(ulong discordId)
        {
            return await _db.Users.AnyAsync(u => u.DiscordUserId == discordId);
        }

        public async Task AddUserAsync(ulong discordId, string username)
        {
            if (!await UserExistsAsync(discordId))
            {
                var user = new UserEntity
                {
                    DiscordUserId = discordId,
                    Username = username,
                    TrustFactor = 0
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpdateTrustFactorAsync(ulong discordId, int newTrust)
        {
            var user = await _db.Users.FindAsync(discordId);
            if (user is not null)
            {
                user.TrustFactor = Math.Clamp(newTrust, -100, 100);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<int?> GetTrustFactorAsync(ulong discordId)
        {
            var user = await _db.Users.FindAsync(discordId);
            return user?.TrustFactor;
        }
    }
}
