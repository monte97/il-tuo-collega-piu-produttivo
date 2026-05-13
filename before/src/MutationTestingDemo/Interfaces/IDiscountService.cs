using MutationTestingDemo.Models;

namespace MutationTestingDemo.Interfaces;

public interface IDiscountService
{
    decimal CalculateDiscount(Customer customer, decimal subtotal);
}
