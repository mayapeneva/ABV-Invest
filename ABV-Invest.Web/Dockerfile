FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["ABV-Invest.Web/ABV_Invest.Web.csproj", "ABV-Invest.Web/"]
COPY ["ABV-Invest.Models/ABV_Invest.Models.csproj", "ABV-Invest.Models/"]
COPY ["ABV-Invest.Data/ABV_Invest.Data.csproj", "ABV-Invest.Data/"]
COPY ["ABV_Invest.Services/ABV_Invest.Services.csproj", "ABV_Invest.Services/"]
COPY ["ABV_Invest.Common/ABV_Invest.Common.csproj", "ABV_Invest.Common/"]
RUN dotnet restore "ABV-Invest.Web/ABV_Invest.Web.csproj"
COPY . .
WORKDIR "/src/ABV-Invest.Web"
RUN dotnet build "ABV_Invest.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ABV_Invest.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ABV_Invest.Web.dll"]