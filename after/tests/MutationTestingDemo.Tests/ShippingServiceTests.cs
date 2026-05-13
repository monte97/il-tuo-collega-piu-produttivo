using MutationTestingDemo.Models;
using MutationTestingDemo.Services;

namespace MutationTestingDemo.Tests;

public class ShippingServiceTests
{
    private readonly ShippingService _sut = new();

    [Fact]
    public void VipCustomer_ShouldGetFreeShipping()
    {
        var customer = new Customer { Name = "Mario Rossi", IsVip = true };

        var (cost, type) = _sut.CalculateShipping(customer, 50m);

        Assert.Equal(0m, cost);
        Assert.Equal(ShippingType.Free, type);
    }

    [Fact]
    public void LargeOrder_Over500_ShouldGetFreeShipping()
    {
        var customer = new Customer { Name = "Luigi Verdi", IsVip = false };

        var (cost, type) = _sut.CalculateShipping(customer, 600m);

        Assert.Equal(0m, cost);
        Assert.Equal(ShippingType.Free, type);
    }

    [Fact]
    public void MediumOrder_Over100_ShouldGetExpress()
    {
        var customer = new Customer { Name = "Anna Bianchi", IsVip = false };

        var (cost, type) = _sut.CalculateShipping(customer, 150m);

        Assert.Equal(7.50m, cost);
        Assert.Equal(ShippingType.Express, type);
    }

    [Fact]
    public void SmallOrder_ShouldGetStandard()
    {
        var customer = new Customer { Name = "Giulia Neri", IsVip = false };

        var (cost, type) = _sut.CalculateShipping(customer, 50m);

        Assert.Equal(5.00m, cost);
        Assert.Equal(ShippingType.Standard, type);
    }

    // --- Boundary tests ---

    [Fact]
    public void Boundary_ExactlyAt500_ShouldNotGetFreeShipping()
    {
        var customer = new Customer { Name = "Marco Gialli", IsVip = false };

        var (cost, type) = _sut.CalculateShipping(customer, 500m);

        Assert.Equal(7.50m, cost);
        Assert.Equal(ShippingType.Express, type);
    }

    [Fact]
    public void Boundary_ExactlyAt100_ShouldGetStandard()
    {
        var customer = new Customer { Name = "Marco Gialli", IsVip = false };

        var (cost, type) = _sut.CalculateShipping(customer, 100m);

        Assert.Equal(5.00m, cost);
        Assert.Equal(ShippingType.Standard, type);
    }
}
