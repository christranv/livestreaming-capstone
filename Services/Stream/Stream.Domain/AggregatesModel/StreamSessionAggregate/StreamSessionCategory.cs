using Stream.Domain.Seedwork;

namespace Stream.Domain.AggregatesModel.StreamSessionAggregate
{
    public class StreamSessionCategory
        : Entity, IAggregateRoot
    {
        public string CategoryGuid { get; }
        public string Name { get; }

        public StreamSessionCategory(string categoryGuid, string name)
        {
            CategoryGuid = categoryGuid;
            Name = name;
        }
    }
}