using MutationTestingDemo.Models;
using MutationTestingDemo.Services;

namespace MutationTestingDemo.Tests;

// DEMO: questo file è il punto di partenza della demo live.
// I test sono verdi, la coverage è al 93% — tutto sembra ok.
// Spoiler: non lo è.

public class OrderServiceTests
{
    private readonly OrderService _sut = new(new DiscountService(), new ShippingService());

    // Un cliente VIP con un solo prodotto da 80€:
    // - sconto VIP 15% → 15% di 80 = 12€
    // - spedizione gratuita perché VIP
    // - totale = 80 - 12 + 0 = 68€
    [Fact]
    public void VipOrder_ShouldApplyDiscountAndFreeShipping()
    {
        var order = new Order
        {
            Customer = new Customer { Name = "Mario Rossi", IsVip = true, LoyaltyYears = 1 },
            Items = new List<OrderItem>
            {
                new() { ProductName = "Tastiera meccanica", UnitPrice = 80m, Quantity = 1 }
            }
        };

        var result = _sut.ProcessOrder(order);

        Assert.Equal(80m, result.Subtotal);
        Assert.Equal(12m, result.DiscountAmount);
        Assert.Equal(68m, result.Total);
    }

    // Un cliente normale, senza fedeltà, ordine da 10€:
    // - nessuna regola di sconto applicabile
    // - spedizione Standard a 5€ (ordine < 100€)
    // - totale = 10 - 0 + 5 = 15€
    [Fact]
    public void RegularCustomer_SmallOrder_ShouldHaveNoDiscount()
    {
        var order = new Order
        {
            Customer = new Customer { Name = "Luigi Verdi", IsVip = false, LoyaltyYears = 0 },
            Items = new List<OrderItem>
            {
                new() { ProductName = "Cavo USB-C", UnitPrice = 10m, Quantity = 1 }
            }
        };

        var result = _sut.ProcessOrder(order);

        Assert.Equal(10m, result.Subtotal);
        Assert.Equal(0m, result.DiscountAmount);
        Assert.Equal(15m, result.Total);
    }

    // Un ordine senza prodotti non è valido:
    // OrderService deve sollevare InvalidOrderException
    [Fact]
    public void EmptyOrder_ShouldThrow()
    {
        var order = new Order
        {
            Customer = new Customer { Name = "Giulia Neri" },
            Items = new List<OrderItem>()
        };

        Assert.Throws<MutationTestingDemo.Exceptions.InvalidOrderException>(
            () => _sut.ProcessOrder(order));
    }
}
