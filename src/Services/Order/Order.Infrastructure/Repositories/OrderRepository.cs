using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Application.Orders.Abstractions;
using Order.Contracts.Events;
using Order.Domain.Orders;
using Order.Infrastructure.Persistence;

namespace Order.Infrastructure.Repositories;

public sealed class OrderRepository(AppDbContext db, IIdentityClient identityClient, IProductClient productClient) : IOrderRepository
{
    public async Task AddAsync(OrderModel order, CancellationToken ct)
    {
        using var transaction = await db.Database.BeginTransactionAsync(ct);

        foreach (var item in order.Items)
        {
            // try
            // {
            //     //Via mensageria
            //     var endpoint = await bus.GetSendEndpoint(new Uri("queue:order-created-queuev2"));
            //     var @event = new OrderUpdateStockIntegrationEvent(
            //         item.ProductId,
            //         item.Quantity
            //     );
            //     await endpoint.Send(@event, ct);
            // }
            // catch (Exception ex)
            // {
            //     await transaction.RollbackAsync(ct);
            //     throw new InvalidOperationException(ex.Message, ex);
            // }

            // Via client

            var productModel = await productClient.GetByIdAsync(item.ProductId, ct);
            item.UnitPrice = productModel?.Price ?? 0;
            
            var stockUpdated = await productClient.DecreaseStockAsync(item.ProductId, item.Quantity, ct);
            if (stockUpdated) continue;
            await transaction.RollbackAsync(ct);
            throw new InvalidOperationException("Failed to update inventory. Order canceled.");
        }

        order.TotalAmount = order.Items.Sum(x => x.UnitPrice * x.Quantity);
        await db.Orders.AddAsync(order, ct);
        
        await transaction.CommitAsync(ct);
    }

    public async Task<OrderModel> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var model = await db.Orders.Include((i) => i.Items).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        
        if(model == null)
            throw new Exception("Order not found");
        
        var userModel = await identityClient.GetByIdAsync(model.UserId, ct);
        if (userModel != null) model.SellerName = userModel.Name;
        
        foreach (var item in model.Items)
        {
            var productModel = await productClient.GetByIdAsync(item.ProductId, ct);
            if (productModel != null)
            {
                item.Name = productModel.Name;
            }
        }
        
        return model;
    }

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);

    public async Task<List<OrderModel>> ListAsync(CancellationToken ct)
    {
       var listModel = await db.Orders.Include((i) => i.Items).AsNoTracking().ToListAsync(cancellationToken: ct);

       foreach (var order in listModel)
       {
           var userModel = await identityClient.GetByIdAsync(order.UserId, ct);
           if (userModel != null) order.SellerName = userModel.Name;
           foreach (var item in order.Items)
           {
               var productModel = await productClient.GetByIdAsync(item.ProductId, ct);
               if (productModel != null)
               {
                   item.Name = productModel.Name;
               }
           }
       }
       
       return listModel;
    }
}
