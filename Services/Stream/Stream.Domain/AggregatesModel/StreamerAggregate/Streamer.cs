using Stream.Domain.Seedwork;
using Stream.Domain.Utils;
using System;

namespace Stream.Domain.AggregatesModel.StreamerAggregate
{
    public class Streamer
        : Entity, IAggregateRoot
    {
        public string IdentityGuid { get; }
        public string AuthToken { get; private set; }
        public string StreamKey { get; private set; }

        public Streamer(string identityGuid)
        {
            IdentityGuid = !string.IsNullOrWhiteSpace(identityGuid)
                ? identityGuid
                : throw new ArgumentNullException(nameof(identityGuid));
            ResetStreamKey();
            ResetLiveToken();
        }

        public void ResetStreamKey()
        {
            StreamKey = KeyGenerator.GetUniqueKey(40,"live_");
        }

        public void ResetLiveToken()
        {
            AuthToken = KeyGenerator.GetUniqueKey(30);
        }
    }
}