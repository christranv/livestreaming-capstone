using Stream.Domain.Seedwork;
using System.Threading.Tasks;

namespace Stream.Domain.AggregatesModel.StreamerAggregate
{
    //This is just the RepositoryContracts or Interface defined at the Domain Layer
    //as requisite for the Streamer Aggregate

    public interface IStreamerRepository : IRepository<Streamer>
    {
        Streamer Add(Streamer streamer);
        Streamer Update(Streamer streamer);
        Streamer Find(string streamerId);
        Task<Streamer> FindByAuthTokenAsync(string authToken);
        Task<Streamer> FindByIdentityGuidAsync(string identityGuid);
        Task<string> GetStreamerIdFromIdentityGuid(string identityGuid);
    }
}
