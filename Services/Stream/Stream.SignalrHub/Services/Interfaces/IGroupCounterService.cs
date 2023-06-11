using System.Collections.Generic;

namespace Stream.SignalrHub.Services.Interfaces
{
    public interface IGroupCounterService
    {
        public long GetGroupUserCount(string groupId);
        public long NewUserConnected(string groupId);
        public long UserDisconnected(string groupId);
        public Dictionary<string, int> GetAllGroupsUserCount();
    }
}