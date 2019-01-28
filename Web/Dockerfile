FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY bin/Debug/netcoreapp2.1/publish /app
EXPOSE 8080

CMD ASPNETCORE_URLS=http://*:$PORT dotnet Web.dll