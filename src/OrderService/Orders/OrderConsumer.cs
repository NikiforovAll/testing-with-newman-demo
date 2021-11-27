namespace OrderService.Orders;

using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using MongoDB.Driver;

public record class OrderPaid(ulong OrderId);

public class OrderConsumer : IConsumer<OrderPaid>
{
    private readonly IMongoClient mongoClient;

    public OrderConsumer(IMongoClient mongoClient) => this.mongoClient = mongoClient;

    public async Task Consume(ConsumeContext<OrderPaid> context)
    {
        var db = mongoClient.GetOrderCollection();
        var id = context.Message.OrderId;

        var order = await db.Find(x => x.Id == id).FirstOrDefaultAsync();

        _ = order ?? throw new InvalidOperationException();

        order.Complete();

        await db.ReplaceOneAsync(x => x.Id == id, order);
    }
}

public class OrderConsumerDefinition
    : ConsumerDefinition<OrderConsumer>
{
    public OrderConsumerDefinition() => EndpointName = "order-paid";

    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<OrderConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseRawJsonSerializer();
    }
}