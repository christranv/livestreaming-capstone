using System.Collections.Generic;
using System.Runtime.Serialization;
using MediatR;

namespace Stream.API.Application.Commands
{
    public class UpdateStreamSessionsViewCountCommand : IRequest<bool>
    {
        [DataMember] public Dictionary<string, int> ViewCountPerStreamSession { get; private set; }

        public UpdateStreamSessionsViewCountCommand(Dictionary<string, int> viewCountPerStreamSession)
        {
            ViewCountPerStreamSession = viewCountPerStreamSession;
        }
    }
}