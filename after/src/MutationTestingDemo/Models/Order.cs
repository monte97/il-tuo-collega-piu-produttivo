namespace MutationTestingDemo.Models;

public class Order
{
    public Customer Customer { get; set; } = new();
    public List<OrderItem> Items { get; set; } = new();
}
