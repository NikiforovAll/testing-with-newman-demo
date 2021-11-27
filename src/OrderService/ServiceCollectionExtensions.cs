using MassTransit;
using MongoDB.Driver;
using System.Reflection;

namespace OrderService;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMongo(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDB");
        services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

        return services;
    }

    public static IServiceCollection AddIdGenerator(
        this IServiceCollection services)
    {
        services.AddSingleton(new UniqueIdGenerator.Net.Generator(1, new DateTime(2021, 11, 27)));

        return services;
    }

    public static IServiceCollection AddMessageBroker(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RabbitMQ");
        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetExecutingAssembly());
            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(connectionString));
                cfg.ConfigureEndpoints(context);
            });
        });
        services.AddMassTransitHostedService();

        return services;
    }


}
