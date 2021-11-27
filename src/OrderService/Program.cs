using OrderService;
using OrderService.Orders;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddMessageBroker(configuration);
services.AddMongo(configuration);
services.AddIdGenerator();

var app = builder.Build();

app.MapOrderApiRoutes();

app.MapGet("/", () => new SiteMap()
    with { Home = "/" }
    with { Orders = "/orders" }
        with { GetOrder = "/orders/{id}" }
        with { CreateOrder = "/orders" }
        with { CancelOrder = "/orders/{id}/cancel" }
);
app.Run();
