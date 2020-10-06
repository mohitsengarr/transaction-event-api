#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TransactionEventApi/TransactionEventApi.csproj", "TransactionEventApi/"]
COPY ["TransactionEventApi.Business/TransactionEventApi.Business.csproj", "TransactionEventApi.Business/"]
COPY ["TransactionEventApi.Common/TransactionEventApi.Common.csproj", "TransactionEventApi.Common/"]
RUN dotnet restore "TransactionEventApi/TransactionEventApi.csproj"
COPY . .
WORKDIR "/src/TransactionEventApi"
RUN dotnet build "TransactionEventApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TransactionEventApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransactionEventApi.dll"]
