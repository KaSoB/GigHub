using GigHub.Models;
using GigHub.Repositories;

namespace GigHub.Persistence {
    public class UnitOfWork : IUnitOfWork {
        private readonly ApplicationDbContext _context;

        public IGigRepository Gigs { get; private set; }

        public UnitOfWork(ApplicationDbContext context) {
            _context = context;
            Gigs = new GigRepository(_context);
        }

        public void Complete() {
            _context.SaveChanges();
        }
    }
}