FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY bin/Debug/netcoreapp2.1/publish /app
ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000

ENTRYPOINT /bin/bash -c "dotnet Web.dll"