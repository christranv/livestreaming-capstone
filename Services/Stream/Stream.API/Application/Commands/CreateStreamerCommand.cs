using System.Runtime.Serialization;
using MediatR;

namespace Stream.API.Application.Commands
{
    public class CreateStreamerCommand : IRequest<bool>
    {
        [DataMember]
        public string IdentityGuid { get; private set; }
        public CreateStreamerCommand(string identityGuid)
        {
            IdentityGuid = identityGuid;
        }
    }
}
