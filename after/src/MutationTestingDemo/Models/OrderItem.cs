namespace MutationTestingDemo.Models;

public class OrderItem
{
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
