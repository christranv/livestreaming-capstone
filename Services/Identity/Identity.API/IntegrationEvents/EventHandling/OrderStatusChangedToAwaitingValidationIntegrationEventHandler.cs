﻿// using Identity.API.IntegrationEvents;
//
// namespace Microsoft.eShopOnContainers.Services.Catalog.API.IntegrationEvents.EventHandling
// {
//     using BuildingBlocks.EventBus.Abstractions;
//     using BuildingBlocks.EventBus.Events;
//     using Infrastructure;
//     using IntegrationEvents.Events;
//     using Microsoft.Extensions.Logging;
//     using Serilog.Context;
//     using System.Collections.Generic;
//     using System.Linq;
//     using System.Threading.Tasks;
//
//     public class OrderStatusChangedToAwaitingValidationIntegrationEventHandler : 
//         IIntegrationEventHandler<OrderStatusChangedToAwaitingValidationIntegrationEvent>
//     {
//         private readonly CatalogContext _catalogContext;
//         private readonly IIdentityIntegrationEventService _identityIntegrationEventService;
//         private readonly ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> _logger;
//
//         public OrderStatusChangedToAwaitingValidationIntegrationEventHandler(
//             CatalogContext catalogContext,
//             IIdentityIntegrationEventService identityIntegrationEventService,
//             ILogger<OrderStatusChangedToAwaitingValidationIntegrationEventHandler> logger)
//         {
//             _catalogContext = catalogContext;
//             _identityIntegrationEventService = identityIntegrationEventService;
//             _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
//         }
//
//         public async Task Handle(OrderStatusChangedToAwaitingValidationIntegrationEvent @event)
//         {
//             using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
//             {
//                 _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
//
//                 var confirmedOrderStockItems = new List<ConfirmedOrderStockItem>();
//
//                 foreach (var orderStockItem in @event.OrderStockItems)
//                 {
//                     var catalogItem = _catalogContext.CatalogItems.Find(orderStockItem.ProductId);
//                     var hasStock = catalogItem.AvailableStock >= orderStockItem.Units;
//                     var confirmedOrderStockItem = new ConfirmedOrderStockItem(catalogItem.Id, hasStock);
//
//                     confirmedOrderStockItems.Add(confirmedOrderStockItem);
//                 }
//
//                 var confirmedIntegrationEvent = confirmedOrderStockItems.Any(c => !c.HasStock)
//                     ? (IntegrationEvent)new OrderStockRejectedIntegrationEvent(@event.OrderId, confirmedOrderStockItems)
//                     : new OrderStockConfirmedIntegrationEvent(@event.OrderId);
//
//                 await _identityIntegrationEventService.SaveEventAndCatalogContextChangesAsync(confirmedIntegrationEvent);
//                 await _identityIntegrationEventService.PublishThroughEventBusAsync(confirmedIntegrationEvent);
//
//             }
//         }
//     }
// }