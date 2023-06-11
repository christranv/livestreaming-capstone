using System.Threading.Tasks;
using Stream.Domain.Seedwork;

namespace Stream.Domain.AggregatesModel.StreamSessionAggregate
{
    //This is just the RepositoryContracts or Interface defined at the Domain Layer
    //as requisite for the Stream Aggregate
    public interface IStreamSessionRepository : IRepository<StreamSession>
    {
        StreamSession Add(StreamSession streamSession);
        void Update(StreamSession streamSession);
        Task<StreamSession> GetAsync(string streamSessionId);
        Task<StreamSession> GetStartedStreamSessionByStreamerIdAsync(string streamerId);
        Task<StreamSession> GetLatestStreamSessionByStreamerIdAsync(string streamerId);
        Task<StreamSessionCategory> GetCategoryById(string categoryGuid);
    }
}
