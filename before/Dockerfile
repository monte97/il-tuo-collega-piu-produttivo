FROM mcr.microsoft.com/dotnet/sdk:8.0

RUN dotnet tool install -g dotnet-stryker \
 && dotnet tool install -g dotnet-reportgenerator-globaltool
ENV PATH="$PATH:/root/.dotnet/tools"

WORKDIR /app

COPY MutationTestingDemo.sln ./
COPY src/MutationTestingDemo/MutationTestingDemo.csproj src/MutationTestingDemo/
COPY tests/MutationTestingDemo.Tests/MutationTestingDemo.Tests.csproj tests/MutationTestingDemo.Tests/

RUN dotnet restore

COPY . .

RUN dotnet build -c Debug

ENTRYPOINT ["bash"]
