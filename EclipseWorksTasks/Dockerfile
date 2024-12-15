# Etapa 1: Compilação
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

EXPOSE 8080

# Copiar o arquivo de projeto e o restante do código-fonte
COPY EclipseWorksTasks.csproj ./
COPY . ./

# Restaurar as dependências do projeto
RUN dotnet restore

# Defina a variável de ambiente ASPNETCORE_URLS
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Publicar a aplicação no modo Release
RUN dotnet publish -c Release -o /app/published

# Etapa 2: Execução
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiar os binários compilados da etapa de build para a imagem final
COPY --from=build /app/published .

# Comando de entrada para iniciar a aplicação
ENTRYPOINT ["dotnet", "EclipseWorksTasks.dll"]
