#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/TestService1/TestService1.csproj", "Services/TestService1/"]
RUN dotnet restore "Services/TestService1/TestService1.csproj"
COPY . .
WORKDIR "/src/Services/TestService1"
RUN dotnet build "TestService1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestService1.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestService1.dll"]