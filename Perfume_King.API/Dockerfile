FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Perfume_King.API/Perfume_King.API.csproj"
WORKDIR "/src/Perfume_King.API"
RUN dotnet publish "Perfume_King.API.csproj" -c Release -o /app/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENTRYPOINT ["dotnet", "Perfume_King.API.dll"]
