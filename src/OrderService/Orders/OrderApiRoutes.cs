using MongoDB.Driver;
using UniqueIdGenerator.Net;

namespace OrderService.Orders;

public static class OrderApiRoutes
{
    public static WebApplication MapOrderApiRoutes(this WebApplication app)
    {
        app.MapPost("/orders/", CreateOrder);
        app.MapGet("/orders", GetOrders);
        app.MapGet("/orders/{id}", GetOrderById).WithName(nameof(GetOrderById));
        app.MapPut("/orders/{id}/cancel", CancelOrderById);

        return app;
    }

    private static async Task<IResult> CancelOrderById(ulong id, IMongoClient mongoClient)
    {
        var db = mongoClient.GetOrderCollection();

        var order = await db.Find(x => x.Id == id).FirstOrDefaultAsync();

        if(order is null)
        {
            return Results.NotFound();
        }
        order.Cancel();
        await db.ReplaceOneAsync(x => x.Id == id, order);

        return Results.NoContent();
    }

    private static async Task<Order> GetOrderById(ulong id, IMongoClient mongoClient)
    {
        var db = mongoClient.GetOrderCollection();

        return await db.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    private static async Task<IResult> CreateOrder(
        Order order, IMongoClient mongoClient, Generator idGenerator)
    {
        order = order with
        {
            Id = idGenerator.NextLong(),
            CreatedAt = DateTime.Now
        };

        var db = mongoClient.GetOrderCollection();

        await db.InsertOneAsync(order);

        return Results.CreatedAtRoute(nameof(GetOrderById), new { id = order.Id });
    }

    private static async Task<IEnumerable<Order>> GetOrders(IMongoClient mongoClient)
    {
        var db = mongoClient.GetOrderCollection();

        return await db.Find(x => true).ToListAsync();
    }
}
