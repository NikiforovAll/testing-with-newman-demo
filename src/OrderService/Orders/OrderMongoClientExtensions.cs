using MongoDB.Driver;

namespace OrderService.Orders;

public static class OrderMongoClientExtensions
{
    private const string OrderDatabaseName = "orders-db";
    private const string OrderCollectionName = "orders";

    public static IMongoCollection<Order> GetOrderCollection(this IMongoClient mongoClient) =>
        mongoClient.GetDatabase(OrderDatabaseName).GetCollection<Order>(OrderCollectionName);
}
