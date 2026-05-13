using MutationTestingDemo.Models;
using MutationTestingDemo.Services;

namespace MutationTestingDemo.Tests;

public class DiscountServiceTests
{
    private readonly DiscountService _sut = new();

    [Fact]
    public void VipCustomer_ShouldGet15PercentDiscount()
    {
        var customer = new Customer { Name = "Mario Rossi", IsVip = true, LoyaltyYears = 1 };

        var result = _sut.CalculateDiscount(customer, 150m);

        // VIP 15% di 150 = 22.50
        Assert.Equal(22.50m, result);
    }

    [Fact]
    public void VipWithLargeOrder_ShouldGetCombinedDiscount()
    {
        var customer = new Customer { Name = "Mario Rossi", IsVip = true, LoyaltyYears = 1 };

        var result = _sut.CalculateDiscount(customer, 300m);

        // VIP 15% + ordine > 200 5% = 20% di 300 = 60
        Assert.Equal(60m, result);
    }

    [Fact]
    public void LargeOrder_NonVip_ShouldGet5PercentDiscount()
    {
        var customer = new Customer { Name = "Luigi Verdi", IsVip = false, LoyaltyYears = 0 };

        var result = _sut.CalculateDiscount(customer, 250m);

        // Solo ordine > 200: 5% di 250 = 12.50
        Assert.Equal(12.50m, result);
    }

    [Fact]
    public void LoyalCustomer_ShouldGetLoyaltyDiscount()
    {
        var customer = new Customer { Name = "Anna Bianchi", IsVip = false, LoyaltyYears = 5 };

        var result = _sut.CalculateDiscount(customer, 150m);

        Assert.Equal(15m, result);
    }

    [Fact]
    public void RegularCustomer_ShouldGetNoDiscount()
    {
        var customer = new Customer { Name = "Giulia Neri", IsVip = false, LoyaltyYears = 0 };

        var result = _sut.CalculateDiscount(customer, 100m);

        Assert.Equal(0m, result);
    }

    // --- Boundary tests ---

    [Fact]
    public void Boundary_ExactlyAt200_ShouldNotGetOrderDiscount()
    {
        var customer = new Customer { Name = "Marco Gialli", IsVip = false, LoyaltyYears = 0 };

        var result = _sut.CalculateDiscount(customer, 200m);

        Assert.Equal(0m, result);
    }

    [Fact]
    public void Boundary_JustAbove200_ShouldGetOrderDiscount()
    {
        var customer = new Customer { Name = "Marco Gialli", IsVip = false, LoyaltyYears = 0 };

        var result = _sut.CalculateDiscount(customer, 200.01m);

        Assert.Equal(200.01m * 0.05m, result);
    }

    [Fact]
    public void Boundary_Exactly3YearsLoyalty_ShouldGetDiscount()
    {
        var customer = new Customer { Name = "Sara Blu", IsVip = false, LoyaltyYears = 3 };

        var result = _sut.CalculateDiscount(customer, 100m);

        Assert.Equal(10m, result);
    }

    [Fact]
    public void Boundary_2YearsLoyalty_ShouldNotGetDiscount()
    {
        var customer = new Customer { Name = "Sara Blu", IsVip = false, LoyaltyYears = 2 };

        var result = _sut.CalculateDiscount(customer, 100m);

        Assert.Equal(0m, result);
    }

    [Fact]
    public void Cap_AllDiscountsCombined_ShouldNotExceed25Percent()
    {
        // VIP (15%) + ordine > 200 (5%) + fedelta' >= 3 (10%) = 30%, cappato a 25%
        var customer = new Customer { Name = "Top Customer", IsVip = true, LoyaltyYears = 5 };

        var result = _sut.CalculateDiscount(customer, 400m);

        // 25% di 400 = 100 (non 30% = 120)
        Assert.Equal(100m, result);
    }
}
