using System;

namespace Event.API.ViewModel
{
    public class SubEventFollowerViewModel
    {
        public string UserId { get; }

        public SubEventFollowerViewModel(string userId)
        {
            UserId = userId;
        }
    }
}