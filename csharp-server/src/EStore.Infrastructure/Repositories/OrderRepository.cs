using EStore.Domain.Entities;
using EStore.Application.Abstractions;
using EStore.Application.Dtos;
using EStore.Application.Exceptions;
using EStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EStore.Infrastructure.Repositories;

internal sealed class OrderRepository(AppDbContext db) : IOrderRepository
{
    public async Task<OrderDto> CreateAsync(
        string customer, IReadOnlyList<CreateOrderItemRequest> items, CancellationToken ct)
    {
        // without Commit() calls, Dispose do the rollbck: if something breaks, nothing happens on DB
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var ids = items.Select(i => i.ProductId).ToArray();
        var prices = await db.Products.AsNoTracking()
            .Where( p => ids.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Price, ct);

        foreach (var item in items)
            if(!prices.ContainsKey(item.ProductId))
                throw new NotFoundException($"Product not found: {item.ProductId}");

        // atomic decreasing stocks, always by ascending productID (keeps back the deadlock)
        foreach (var item in items.OrderBy(i => i.ProductId))
        {
            var affected = await db.Products
                .Where(p => p.Id == item.ProductId && p.Stock >= item.Quantity)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Stock, p => p.Stock - item.Quantity), ct);

            if (affected == 0)
                throw new ConflictException($"Unsufficient stock for product: {item.ProductId}");
        }

        // writing order and items ( SaveChanges sends INSERTs like one batch)

        var order = new Order
        {
            Customer = customer,
            Total = items.Sum(i => prices[i.ProductId] * i.Quantity),
            Items = items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = prices[i.ProductId]
            }).ToList()
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync(ct); // id and CreatedAt are returning from base
        await tx.CommitAsync(ct);

        return ToDto(order);
    }

    public async Task<OrderDto?> GetByIdAsync(long id, CancellationToken ct)
    {
        var order = await db.Orders.AsNoTracking()
        .Include(o => o.Items.OrderBy(i => i.Id))
        .FirstOrDefaultAsync(o => o.Id == id, ct);

        return order is null ? null : ToDto(order);
    }

    private static OrderDto ToDto(Order o) => new(
        o.Id, o.Customer, o.Total, o.CreatedAt, 
        o.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity, i.UnitPrice)).ToList()
    );

}


/*
[FIRST TIME DOING POST METHOD]

naive decreasing stock in C# instead in base:
var p = await var p = await db.Products.FindAsync(id);   // read stock = 1
p.Stock -= 1;                                            // calculate in C#
await db.SaveChangesAsync();                             // write 0

that is a lost update scenario where two paralel reqs reads and writes 0 

so EF do the calculations via SQL then theres no concurency and bad behaviour
and when first one commits, Psql evaluates again stock >= @q

[TRANSACTIONS]

- when we explicitly do the BeginTransaction, it garantues Dispose without Commit is rollback
- ExecuteUpdateAsync executes instantly, without SaveChanges and change tracker 

**Double transaction calling is neccesary for escaping deadlock and concurency problem, it holds 'session' for whole transaction**
this is only edit for achievement - didnt know that I must push the commit with Co-authored
*/
