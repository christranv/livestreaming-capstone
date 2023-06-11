using System.Collections.Generic;
using System.Linq;
using Stream.SignalrHub.Services.Interfaces;

namespace Stream.SignalrHub.Services
{
    namespace Stream.SignalrHub.Services.Interfaces
    {
        public class GroupCounterService : IGroupCounterService
        {
            private readonly Dictionary<string, int> _connectionIdsPerGroup = new();

            public long GetGroupUserCount(string groupId)
            {
                return _connectionIdsPerGroup[groupId];
            }

            public long NewUserConnected(string groupId)
            {
                var _ = _connectionIdsPerGroup.ContainsKey(groupId)
                    ? _connectionIdsPerGroup[groupId]++
                    : _connectionIdsPerGroup[groupId] = 1;
                return _connectionIdsPerGroup[groupId];
            }

            public long UserDisconnected(string groupId)
            {
                _connectionIdsPerGroup[groupId]--;
                return _connectionIdsPerGroup[groupId];
            }

            public Dictionary<string, int> GetAllGroupsUserCount()
            {
                return _connectionIdsPerGroup;
            }
        }
    }
}