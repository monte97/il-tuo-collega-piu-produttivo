using MutationTestingDemo.Interfaces;
using MutationTestingDemo.Models;

namespace MutationTestingDemo.Services;

public class DiscountService : IDiscountService
{
    public decimal CalculateDiscount(Customer customer, decimal subtotal)
    {
        decimal discountPercentage = 0m;

        // VIP: +15% sconto
        if (customer.IsVip)
        {
            discountPercentage += 0.15m;
        }

        // Ordine > 200 EUR: +5% sconto
        if (subtotal > 200m)
        {
            discountPercentage += 0.05m;
        }

        // Fedelta' >= 3 anni: +10% sconto
        if (customer.LoyaltyYears >= 3)
        {
            discountPercentage += 0.10m;
        }

        // Cap al 25%
        if (discountPercentage > 0.25m)
        {
            discountPercentage = 0.25m;
        }

        return subtotal * discountPercentage;
    }
}
