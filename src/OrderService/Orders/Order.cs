namespace OrderService.Orders;

public record class Order(
    ulong Id,
    string Customer,
    IList<OrderLineItem> lineItems,
    DateTime CreatedAt
)
{
    public string Status { get; private set; } = "Accepted";

    public void Cancel() => Status = "Cancelled";

    public void Complete() => Status = "Completed";
};

public record class OrderLineItem(
    string Product,
    int Quantity,
    decimal Price
);