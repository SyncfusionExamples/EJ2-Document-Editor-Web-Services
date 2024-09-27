#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat



FROM mcr.microsoft.com/dotnet/sdk:2.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/*.csproj ./

RUN dotnet restore

# Copy everything else and build
COPY . ./

# RUN dotnet 
RUN cd src && dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:2.1
WORKDIR /app
COPY --from=build-env /app/src/out .


EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "EJ2APIServices.dll"]
