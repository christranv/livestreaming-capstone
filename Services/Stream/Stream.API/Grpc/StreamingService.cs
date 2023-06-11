using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Grpc.Core;
// using Google.Protobuf.Collections;
using System.Collections.Generic;
using Stream.API.Application.Commands;
using Stream.API.Application.Models;
using Team5.BuildingBlocks.MessageBroker.EventBus.Extensions;

// namespace GrpcStreaming
// {
//     public class StreamingService : StreamingGrpc.StreamingGrpcBase
//     {
//         private readonly IMediator _mediator;
//         private readonly ILogger<StreamingService> _logger;
//
//         public StreamingService(IMediator mediator, ILogger<StreamingService> logger)
//         {
//             _mediator = mediator;
//             _logger = logger;
//         }
//
//     }
// }
