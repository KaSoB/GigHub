using System;
using GigHub.Models;

namespace GigHub.Repositories {
    public class FollowRepository {
        private readonly ApplicationDbContext _context;

        public FollowRepository(ApplicationDbContext context) {
            _context = context;
        }

        internal object GetFollowing(string userId, string artistId) {
            throw new NotImplementedException();
        }
    }
}