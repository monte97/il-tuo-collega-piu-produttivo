# Demo — Before

Punto di partenza della demo live. 10 test, tutti verdi, coverage al 93%.

I test sembrano corretti ma non lo sono: due mutazioni sulla stessa riga di `OrderService` sopravvivono perché i dati di test sono troppo semplici.

## Prerequisiti

- Docker

## Setup

```bash
make build
```

## Comandi

```bash
make test            # Esegue i 10 test — tutti verdi
make stryker         # Mutation testing — score atteso ~65%
make coverage        # Genera report HTML code coverage
make report          # Apri report Stryker nel browser
make report-coverage # Apri report coverage nel browser
make all             # Build + mutation testing + coverage in un colpo solo
make help            # Lista completa dei comandi
```

### Report coverage

`make coverage` usa `dotnet test --collect:'XPlat Code Coverage'` per produrre un file XML in formato Cobertura, poi lo converte in HTML tramite [ReportGenerator](https://github.com/danielpalme/ReportGenerator) (tool di terze parti, de facto standard nell'ecosistema .NET — non incluso nel SDK).

## Dominio

Un piccolo sistema di elaborazione ordini con tre servizi e regole di business esplicite.

### Modelli

**`Order`** — l'input del sistema: un `Customer` e una lista di `OrderItem`.

**`Customer`**
| Campo | Tipo | Significato |
|---|---|---|
| `Name` | string | Nome del cliente |
| `IsVip` | bool | Cliente VIP |
| `LoyaltyYears` | int | Anni di fedeltà |

**`OrderItem`**
| Campo | Tipo | Significato |
|---|---|---|
| `ProductName` | string | Nome prodotto |
| `UnitPrice` | decimal | Prezzo unitario |
| `Quantity` | int | Quantità |

**`OrderResult`** — l'output del sistema:
| Campo | Tipo | Significato |
|---|---|---|
| `Subtotal` | decimal | Somma di `UnitPrice * Quantity` per ogni item |
| `DiscountAmount` | decimal | Sconto applicato in euro |
| `ShippingCost` | decimal | Costo di spedizione |
| `ShippingType` | enum | `Standard` / `Express` / `Free` |
| `Total` | decimal | `Subtotal - DiscountAmount + ShippingCost` |
| `Status` | enum | `Confirmed` se totale > 0, altrimenti `RequiresReview` |

### Regole di business

**Sconto** (`DiscountService`):
- Cliente VIP → +15%
- Subtotale > 200€ → +5%
- Fedeltà ≥ 3 anni → +10%
- Cap massimo al 25%

**Spedizione** (`ShippingService`):
- Cliente VIP → gratuita
- Subtotale > 500€ → gratuita
- Subtotale > 100€ → Express (7,50€)
- Altrimenti → Standard (5,00€)

**Validazione** (`OrderService`): un ordine senza item solleva `InvalidOrderException`.

### Struttura del codice

```
src/MutationTestingDemo/
├── Models/          # Order, OrderItem, Customer, OrderResult, OrderStatus, ShippingType
├── Interfaces/      # IDiscountService, IShippingService
├── Exceptions/      # InvalidOrderException
└── Services/
    ├── OrderService.cs      # Orchestratore: calcola subtotale, applica sconto e spedizione, determina status
    ├── DiscountService.cs   # Implementa le regole di sconto
    └── ShippingService.cs   # Implementa le regole di spedizione

tests/MutationTestingDemo.Tests/
├── OrderServiceTests.cs
├── DiscountServiceTests.cs
└── ShippingServiceTests.cs
```

## Cosa mostra

- [`src/MutationTestingDemo/Services/OrderService.cs:26`](src/MutationTestingDemo/Services/OrderService.cs#L26): `Sum()` → `Max()` non viene rilevato (test ha 1 solo item)
- [`src/MutationTestingDemo/Services/OrderService.cs:26`](src/MutationTestingDemo/Services/OrderService.cs#L26): `* Quantity` → `/ Quantity` non viene rilevato (Quantity = 1)
- [`src/MutationTestingDemo/Services/DiscountService.cs`](src/MutationTestingDemo/Services/DiscountService.cs): boundary `>= 3 anni` → `> 3 anni` non viene rilevato (LoyaltyYears = 5)
- [`tests/MutationTestingDemo.Tests/OrderServiceTests.cs`](tests/MutationTestingDemo.Tests/OrderServiceTests.cs): status dell'ordine mai verificato nei test
