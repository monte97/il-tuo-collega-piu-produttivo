using MutationTestingDemo.Exceptions;
using MutationTestingDemo.Interfaces;
using MutationTestingDemo.Models;

namespace MutationTestingDemo.Services;

public class OrderService
{
    private readonly IDiscountService _discountService;
    private readonly IShippingService _shippingService;

    public OrderService(IDiscountService discountService, IShippingService shippingService)
    {
        _discountService = discountService;
        _shippingService = shippingService;
    }

    public OrderResult ProcessOrder(Order order)
    {
        if (order.Items == null || order.Items.Count == 0)
        {
            throw new InvalidOrderException("L'ordine deve contenere almeno un prodotto.");
        }

        // Calcola subtotale
        decimal subtotal = order.Items.Sum(item => item.UnitPrice * item.Quantity);

        // Applica sconto
        decimal discountAmount = _discountService.CalculateDiscount(order.Customer, subtotal);

        // Calcola spedizione
        var (shippingCost, shippingType) = _shippingService.CalculateShipping(order.Customer, subtotal);

        // Total = subtotale - sconto + spedizione
        decimal total = subtotal - discountAmount + shippingCost;

        // Conferma l'ordine se il totale e' valido
        var status = total > 0 ? OrderStatus.Confirmed : OrderStatus.RequiresReview;

        return new OrderResult
        {
            Subtotal = subtotal,
            DiscountAmount = discountAmount,
            ShippingCost = shippingCost,
            ShippingType = shippingType,
            Total = total,
            Status = status
        };
    }
}
