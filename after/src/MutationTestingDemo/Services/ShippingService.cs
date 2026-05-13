using MutationTestingDemo.Interfaces;
using MutationTestingDemo.Models;

namespace MutationTestingDemo.Services;

public class ShippingService : IShippingService
{
    public (decimal Cost, ShippingType Type) CalculateShipping(Customer customer, decimal subtotal)
    {
        // VIP: spedizione gratuita
        if (customer.IsVip)
        {
            return (0m, ShippingType.Free);
        }

        // Ordine > 500 EUR: gratuita
        if (subtotal > 500m)
        {
            return (0m, ShippingType.Free);
        }

        // Ordine > 100 EUR: Express (7.50 EUR)
        if (subtotal > 100m)
        {
            return (7.50m, ShippingType.Express);
        }

        // Default: Standard (5.00 EUR)
        return (5.00m, ShippingType.Standard);
    }
}
