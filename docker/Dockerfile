FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS build
WORKDIR /SchoolManagementAPI
EXPOSE 80

#COPY ["SchoolManagementAPI.csproj", "SchoolManagementAPI"]
COPY SchoolManagementAPI/*.sln .
RUN dotnet restore --use-current-runtime

COPY SchoolManagementAPI/. .
RUN dotnet publish -c Release -o /SchoolManagementAPI --use-current-runtime --self-contained false --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /SchoolManagementAPI
COPY --from=build /SchoolManagementAPI .
ENTRYPOINT ["dotnet", "SchoolManagementAPI.dll"]