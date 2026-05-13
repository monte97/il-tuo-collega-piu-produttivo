using MutationTestingDemo.Exceptions;
using MutationTestingDemo.Models;
using MutationTestingDemo.Services;

namespace MutationTestingDemo.Tests;

public class OrderServiceTests
{
    private readonly OrderService _sut = new(new DiscountService(), new ShippingService());

    [Fact]
    public void VipOrder_ShouldCalculateCorrectTotals()
    {
        var order = new Order
        {
            Customer = new Customer { Name = "Mario Rossi", IsVip = true, LoyaltyYears = 1 },
            Items = new List<OrderItem>
            {
                new() { ProductName = "Tastiera meccanica", UnitPrice = 80m, Quantity = 2 },
                new() { ProductName = "Mouse wireless", UnitPrice = 40m, Quantity = 1 }
            }
        };

        var result = _sut.ProcessOrder(order);

        // Subtotale: 80*2 + 40*1 = 200 (multi-item killa Sum->Max)
        Assert.Equal(200m, result.Subtotal);
        // VIP 15% di 200 = 30
        Assert.Equal(30m, result.DiscountAmount);
        // VIP -> spedizione gratuita
        Assert.Equal(0m, result.ShippingCost);
        Assert.Equal(ShippingType.Free, result.ShippingType);
        // Totale: 200 - 30 + 0 = 170
        Assert.Equal(170m, result.Total);
        // Status confermato
        Assert.Equal(OrderStatus.Confirmed, result.Status);
    }

    [Fact]
    public void MultipleItems_ShouldSumCorrectly()
    {
        var order = new Order
        {
            Customer = new Customer { Name = "Luigi Verdi", IsVip = false, LoyaltyYears = 0 },
            Items = new List<OrderItem>
            {
                new() { ProductName = "Cavo USB-C", UnitPrice = 10m, Quantity = 3 },
                new() { ProductName = "Mouse wireless", UnitPrice = 40m, Quantity = 2 }
            }
        };

        var result = _sut.ProcessOrder(order);

        // Subtotale: 10*3 + 40*2 = 110 (Quantity > 1 killa * -> /)
        Assert.Equal(110m, result.Subtotal);
    }

    [Fact]
    public void NonVipSmallOrder_ShouldHaveStandardShipping()
    {
        var order = new Order
        {
            Customer = new Customer { Name = "Luigi Verdi", IsVip = false, LoyaltyYears = 0 },
            Items = new List<OrderItem>
            {
                new() { ProductName = "Cavo USB-C", UnitPrice = 10m, Quantity = 3 }
            }
        };

        var result = _sut.ProcessOrder(order);

        // Subtotale 30, nessuno sconto, Standard 5.00
        Assert.Equal(30m, result.Subtotal);
        Assert.Equal(0m, result.DiscountAmount);
        Assert.Equal(5.00m, result.ShippingCost);
        Assert.Equal(ShippingType.Standard, result.ShippingType);
        // Totale: 30 - 0 + 5 = 35 (killa +/- sulla spedizione)
        Assert.Equal(35m, result.Total);
        Assert.Equal(OrderStatus.Confirmed, result.Status);
    }

    [Fact]
    public void EmptyOrder_ShouldThrowWithMessage()
    {
        var order = new Order
        {
            Customer = new Customer { Name = "Giulia Neri" },
            Items = new List<OrderItem>()
        };

        var ex = Assert.Throws<InvalidOrderException>(() => _sut.ProcessOrder(order));
        Assert.Contains("almeno un prodotto", ex.Message);
    }

    [Fact]
    public void NullItems_ShouldThrow()
    {
        var order = new Order
        {
            Customer = new Customer { Name = "Giulia Neri" },
            Items = null!
        };

        Assert.Throws<InvalidOrderException>(() => _sut.ProcessOrder(order));
    }

    [Fact]
    public void MediumOrder_ShouldGetExpressShipping()
    {
        var order = new Order
        {
            Customer = new Customer { Name = "Paolo Viola", IsVip = false, LoyaltyYears = 0 },
            Items = new List<OrderItem>
            {
                new() { ProductName = "Tastiera meccanica", UnitPrice = 75m, Quantity = 2 }
            }
        };

        var result = _sut.ProcessOrder(order);

        Assert.Equal(150m, result.Subtotal);
        Assert.Equal(0m, result.DiscountAmount);
        Assert.Equal(7.50m, result.ShippingCost);
        Assert.Equal(ShippingType.Express, result.ShippingType);
        Assert.Equal(157.50m, result.Total);
        Assert.Equal(OrderStatus.Confirmed, result.Status);
    }
}
