using GigHub.Repositories;

namespace GigHub.Persistence {
    public interface IUnitOfWork {
        IGigRepository Gigs { get; }

        void Complete();
    }
}