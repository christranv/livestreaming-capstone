using System;

namespace Stream.Domain.AggregatesModel.StreamSessionAggregate
{
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using SeedWork;

    public class StreamSessionStatus
        : Enumeration
    {
        public static StreamSessionStatus Created = new StreamSessionStatus(2, nameof(Created).ToLowerInvariant());
        public static StreamSessionStatus Published = new StreamSessionStatus(3, nameof(Published).ToLowerInvariant());
        public static StreamSessionStatus Banned = new StreamSessionStatus(4, nameof(Banned).ToLowerInvariant());
        public static StreamSessionStatus Finished = new StreamSessionStatus(5, nameof(Finished).ToLowerInvariant());

        public StreamSessionStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<StreamSessionStatus> List() =>
            new[] {Created, Published, Banned, Finished};

        public static StreamSessionStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new StreamDomainException(
                    $"Possible values for StreamStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static StreamSessionStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new StreamDomainException(
                    $"Possible values for StreamStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}