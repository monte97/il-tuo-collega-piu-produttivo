namespace MutationTestingDemo.Models;

public class OrderResult
{
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public ShippingType ShippingType { get; set; }
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; }
}
