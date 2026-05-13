# Il tuo collega più produttivo scrive test inutili

Slide e progetto demo del talk su **mutation testing** presentato a DevRomagna 2026.

> Coverage al 90%, test tutti verdi, code review approvata. Tre settimane dopo, il cliente chiama: i numeri sono sbagliati da giorni.
>
> Il codice lo aveva generato un LLM. I test pure. Il problema non era il codice — era che nessun test stava verificando qualcosa di utile. La coverage contava righe eseguite, non difetti trovati.

## Cosa c'è qui

| Cartella / file | Contenuto |
|---|---|
| `slides.pdf` | Le slide del talk (formato 16:9). |
| `before/` | Demo .NET con suite di test "deboli": 10 verdi, coverage 93%, mutation score **65%**. |
| `after/` | Stessa demo con test rinforzati: 22 verdi, mutation score **92%**. |
| `saved-reports/` | Report HTML pre-generati (Stryker before/after + coverage). Apribili senza Docker. |
| `Makefile` | Comandi orchestratori per build, test e mutation testing. |

## Requisiti

- Docker + Docker Compose (la demo gira in container per non richiedere `dotnet` locale)

Per esplorare solo i report pre-generati basta un browser: apri `saved-reports/stryker-before/mutation-report.html`.

## Quick start

```bash
make build          # build immagini Docker (before + after)
make test           # 10 test verdi sulla before
make stryker        # mutation testing sulla before — score ~65%, ~30 sec
make report         # apre il report HTML appena generato

make test-after     # 22 test verdi sulla after
make stryker-after  # mutation testing sulla after — score ~92%
make report-after   # apre il report after
```

Oppure, senza eseguire nulla, apri direttamente i report salvati:

```bash
make open-stryker-saved        # Stryker before (65%)
make open-stryker-after-saved  # Stryker after (92%)
make open-coverage-saved       # Coverage before (93%)
```

`make help` per la lista completa.

## La domanda del talk in due tabelle

**Before — la suite "verde ma cieca"**

| Metrica | Valore |
|---|---|
| Test | 10 |
| Code coverage | 93% (linee), 80% (branch) |
| Mutation score | **65%** |
| Mutanti sopravvissuti | 12 |

**After — la suite rinforzata**

| Metrica | Valore |
|---|---|
| Test | 22 |
| Mutation score | **92%** |
| Mutanti sopravvissuti | 3 |

La coverage ti dice **quanto codice esegui**. Il mutation score ti dice **quanto bene lo verifichi**.

## Provare Stryker sul tuo progetto

```bash
dotnet tool install -g dotnet-stryker
dotnet stryker
```

Equivalenti per altri stack:
- JavaScript / TypeScript → [Stryker Mutator](https://stryker-mutator.io/)
- Java → [PIT](https://pitest.org/)
- Python → [mutmut](https://github.com/boxed/mutmut) o [cosmic-ray](https://github.com/sixty-north/cosmic-ray)

## Licenza

[MIT](LICENSE) — puoi prendere il codice, le slide, le idee e riusarle come vuoi.

## Contatti

- [montelli.dev](https://montelli.dev)
- [github.com/monte97](https://github.com/monte97)
- francesco@montelli.dev
