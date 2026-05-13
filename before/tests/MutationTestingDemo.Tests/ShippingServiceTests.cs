using MutationTestingDemo.Models;
using MutationTestingDemo.Services;

namespace MutationTestingDemo.Tests;

// DEMO: tre test, tutti verdi. Coprono VIP, ordine grande e ordine piccolo.
// Ma mancano i boundary: 500 EUR (soglia gratuita) e 100 EUR (soglia Express).
// Stryker muta ">" in ">=" su entrambe le soglie — i test non se ne accorgono.

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
    public void LargeOrder_ShouldGetFreeShipping()
    {
        // PROBLEMA: 600m è ben oltre la soglia di 500.
        // Un mutante "> 500" → ">= 500" o "> 600" non cambia l'esito di questo test.
        var customer = new Customer { Name = "Luigi Verdi", IsVip = false };

        var (cost, type) = _sut.CalculateShipping(customer, 600m);

        Assert.Equal(0m, cost);
        Assert.Equal(ShippingType.Free, type);
    }

    [Fact]
    public void SmallOrder_ShouldGetStandardShipping()
    {
        // PROBLEMA: 50m è lontano dalla soglia di 100 (Express).
        // Non copre il caso Express né il boundary a 100.
        var customer = new Customer { Name = "Anna Bianchi", IsVip = false };

        var (cost, type) = _sut.CalculateShipping(customer, 50m);

        Assert.Equal(5.00m, cost);
        Assert.Equal(ShippingType.Standard, type);
    }
}
