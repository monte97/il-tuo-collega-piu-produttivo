using MutationTestingDemo.Models;
using MutationTestingDemo.Services;

namespace MutationTestingDemo.Tests;

// DEMO: i test dello sconto sembrano completi — coprono VIP, fedeltà, ordine grande.
// Ma nessuno testa il boundary esatto: la riga 25 di DiscountService usa ">= 3".
// Stryker la muta in "> 3": tutti i test usano LoyaltyYears = 5, quindi sopravvive.

public class DiscountServiceTests
{
    private readonly DiscountService _sut = new();

    [Fact]
    public void VipCustomer_ShouldGetVipDiscount()
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
    public void LoyalCustomer_ShouldGetLoyaltyDiscount()
    {
        // PROBLEMA: LoyaltyYears = 5 non testa il boundary.
        // Con ">= 3" → "> 3", LoyaltyYears = 5 supera entrambe le condizioni.
        // Serve un test con LoyaltyYears = 3 per uccidere il mutante.
        var customer = new Customer { Name = "Anna Bianchi", IsVip = false, LoyaltyYears = 5 };

        var result = _sut.CalculateDiscount(customer, 150m);

        // Fedelta' 10% di 150 = 15
        Assert.Equal(15m, result);
    }

    [Fact]
    public void RegularCustomer_ShouldGetNoDiscount()
    {
        var customer = new Customer { Name = "Giulia Neri", IsVip = false, LoyaltyYears = 0 };

        var result = _sut.CalculateDiscount(customer, 100m);

        Assert.Equal(0m, result);
    }
}
