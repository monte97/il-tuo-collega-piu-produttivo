.PHONY: help build test stryker stryker-after report report-after coverage open-stryker-saved open-stryker-after-saved open-coverage-saved clean

USER_ID := $(shell id -u)
GROUP_ID := $(shell id -g)

# === Build ===
build: ## Build Docker images (before + after)
	docker compose -f before/docker-compose.yml build
	docker compose -f after/docker-compose.yml build

# === Test ===
test: ## Esegue i 10 test before — tutti verdi
	docker compose -f before/docker-compose.yml run --rm demo -c "dotnet test --verbosity normal"

test-after: ## Esegue i 22 test after — tutti verdi
	docker compose -f after/docker-compose.yml run --rm demo -c "dotnet test --verbosity normal"

# === Mutation testing ===
stryker: ## Mutation testing su before — score atteso ~65%
	mkdir -p before/reports
	docker compose -f before/docker-compose.yml run --rm demo -c "cd tests/MutationTestingDemo.Tests && dotnet stryker -f ../../stryker-config.json -O ../../reports && chown -R $(USER_ID):$(GROUP_ID) ../../reports"

stryker-after: ## Mutation testing su after — score atteso ~92%
	mkdir -p after/reports
	docker compose -f after/docker-compose.yml run --rm demo -c "cd tests/MutationTestingDemo.Tests && dotnet stryker -f ../../stryker-config.json -O ../../reports && chown -R $(USER_ID):$(GROUP_ID) ../../reports"

# === Report (live) ===
report: ## Apri report Stryker before appena generato
	@if [ -f before/reports/reports/mutation-report.html ]; then xdg-open before/reports/reports/mutation-report.html; else echo "Nessun report. Esegui prima: make stryker"; fi

report-after: ## Apri report Stryker after appena generato
	@if [ -f after/reports/reports/mutation-report.html ]; then xdg-open after/reports/reports/mutation-report.html; else echo "Nessun report. Esegui prima: make stryker-after"; fi

# === Report salvati (saved-reports/) ===
open-stryker-saved: ## Apri report Stryker before SALVATO (65%)
	xdg-open saved-reports/stryker-before/mutation-report.html

open-stryker-after-saved: ## Apri report Stryker after SALVATO (92%)
	xdg-open saved-reports/stryker-after/mutation-report.html

open-coverage-saved: ## Apri coverage SALVATO (93%)
	xdg-open saved-reports/coverage-before/index.html

# === Cleanup ===
clean: ## Rimuovi report generati live (i saved-reports restano)
	rm -rf before/reports/ after/reports/

help: ## Mostra questo help
	@grep -E '^[a-zA-Z0-9_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-26s\033[0m %s\n", $$1, $$2}'
