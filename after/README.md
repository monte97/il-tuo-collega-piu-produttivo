# Demo — After

Versione migliorata dopo la sessione di mutation testing. 22 test, score ~92%.

I test sono stati riscritti per uccidere i mutanti survived: dati multi-item, assertion sullo Status, boundary a `LoyaltyYears = 3`.

## Prerequisiti

- Docker

## Setup

```bash
make build
```

## Comandi

```bash
make test            # Esegue i 22 test — tutti verdi
make stryker         # Mutation testing — score atteso ~92%
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

## Cosa è cambiato rispetto a before

- `OrderServiceTests`: ordini con più item e `Quantity > 1` (uccide `Sum→Max` e `*→/`)
- `OrderServiceTests`: assertion su `result.Status` e `result.ShippingType`
- `DiscountServiceTests`: boundary test a `LoyaltyYears = 3` e `LoyaltyYears = 2`
- `DiscountServiceTests`: boundary test a subtotale 200€ (esatto e appena sopra)
- `DiscountServiceTests`: test cap sconto al 25%
- `ShippingServiceTests`: boundary test a 500 e 100
- `ShippingServiceTests`: test dedicato per Express shipping
