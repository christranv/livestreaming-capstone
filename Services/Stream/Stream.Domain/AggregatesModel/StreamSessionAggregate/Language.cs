using System;

namespace Stream.Domain.AggregatesModel.StreamSessionAggregate
{
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using SeedWork;

    public class Language
        : Enumeration
    {
        public static Language Eng = new Language(1, nameof(Eng).ToLowerInvariant());
        public static Language Vie = new Language(2, nameof(Vie).ToLowerInvariant());

        public Language(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<Language> List() =>
            new[] {Eng, Vie};

        public static Language FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new StreamDomainException(
                    $"Possible values for Language: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static Language From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new StreamDomainException(
                    $"Possible values for Language: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}