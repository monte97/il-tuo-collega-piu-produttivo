using MutationTestingDemo.Models;

namespace MutationTestingDemo.Interfaces;

public interface IShippingService
{
    (decimal Cost, ShippingType Type) CalculateShipping(Customer customer, decimal subtotal);
}
