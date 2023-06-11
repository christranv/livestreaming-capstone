using System.Runtime.Serialization;
using MediatR;
using Stream.API.Application.Models;

namespace Stream.API.Application.Commands
{
    public class CreateStreamSessionCommand :  IRequest<bool>
    {
       
        [DataMember]
        public SrsCallbackModel SrsCallbackCallbackModel { get; private set; }

        public CreateStreamSessionCommand(SrsCallbackModel srsCallbackModel)
        {
            SrsCallbackCallbackModel = srsCallbackModel;
        }
    }

}
