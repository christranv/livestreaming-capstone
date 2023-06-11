using System.Runtime.Serialization;
using MediatR;
using Stream.API.Application.Models;

namespace Stream.API.Application.Commands
{
    public class SetFinishedStreamSessionStatusCommand : IRequest<bool>
    {
        [DataMember] public string StreamerIdentityGuid { get; private set; }
        [DataMember] public string LiveToken { get; private set; }

        public SetFinishedStreamSessionStatusCommand(string streamerIdentityGuid = "", string liveToken = "")
        {
            StreamerIdentityGuid = streamerIdentityGuid;
            LiveToken = liveToken;
        }
    }
}