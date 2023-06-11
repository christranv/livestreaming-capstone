using Stream.Domain.Seedwork;

namespace Stream.Domain.AggregatesModel.StreamSessionAggregate
{
    public class StreamSessionTag
        : Entity, IAggregateRoot
    {
        public string TagGuid { get; }
        public string Name { get; }

        public StreamSessionTag(string tagGuid, string name)
        {
            TagGuid = tagGuid;
            Name = name;
        }
    }
}