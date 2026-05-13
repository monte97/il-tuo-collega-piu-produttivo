.PHONY: build test test-order test-discount test-shipping stryker coverage report report-coverage all clean help

USER_ID := $(shell id -u)
GROUP_ID := $(shell id -g)

build: ## Build Docker image
	docker compose build

test: ## Esegue i 10 test — tutti verdi
	docker compose run --rm demo -c "dotnet test --verbosity normal"

test-order: ## Esegue solo OrderServiceTests
	docker compose run --rm demo -c "dotnet test --filter 'FullyQualifiedName~OrderServiceTests' --verbosity normal"

test-discount: ## Esegue solo DiscountServiceTests
	docker compose run --rm demo -c "dotnet test --filter 'FullyQualifiedName~DiscountServiceTests' --verbosity normal"

test-shipping: ## Esegue solo ShippingServiceTests
	docker compose run --rm demo -c "dotnet test --filter 'FullyQualifiedName~ShippingServiceTests' --verbosity normal"

stryker: ## Mutation testing — score atteso ~65%
	mkdir -p reports
	docker compose run --rm demo -c "\
		cd tests/MutationTestingDemo.Tests && \
		dotnet stryker -f ../../stryker-config.json -O ../../reports && \
		chown -R $(USER_ID):$(GROUP_ID) ../../reports"

coverage: ## Genera report HTML di code coverage
	mkdir -p reports/coverage
	docker compose run --rm demo -c "\
		dotnet test \
			--collect:'XPlat Code Coverage' \
			--results-directory /app/reports/coverage/raw \
			--verbosity normal && \
		reportgenerator \
			-reports:/app/reports/coverage/raw/*/coverage.cobertura.xml \
			-targetdir:/app/reports/coverage/html \
			-reporttypes:Html && \
		chown -R $(USER_ID):$(GROUP_ID) /app/reports/coverage"

all: build stryker coverage ## Build + mutation testing + code coverage in un colpo solo

report: ## Apri report HTML Stryker
	@if [ -f reports/reports/mutation-report.html ]; then \
		xdg-open reports/reports/mutation-report.html; \
	else echo "Nessun report. Esegui prima: make stryker"; fi

report-coverage: ## Apri report HTML coverage
	@if [ -f reports/coverage/html/index.html ]; then \
		xdg-open reports/coverage/html/index.html; \
	else echo "Nessun report. Esegui prima: make coverage"; fi

clean: ## Rimuovi report e immagini Docker
	docker compose down --rmi local -v 2>/dev/null || true
	rm -rf reports/

help: ## Mostra questo help
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-16s\033[0m %s\n", $$1, $$2}'
