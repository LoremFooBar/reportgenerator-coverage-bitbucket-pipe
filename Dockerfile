FROM mcr.microsoft.com/dotnet/sdk:5.0 as build

ENV PRROJECT_NAME "DotNet.CodeCoverage.BitbucketPipe"

WORKDIR /source

COPY src/$PRROJECT_NAME/$PRROJECT_NAME.csproj .

RUN dotnet restore

COPY src/$PRROJECT_NAME/. ./

RUN dotnet publish -c release -o /app


FROM mcr.microsoft.com/dotnet/sdk:5.0

LABEL maintainer="@amit_e"

WORKDIR /app

COPY --from=build /app .

# add dotnet global tools to PATH
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet tool install -g dotnet-reportgenerator-globaltool

ENTRYPOINT ["dotnet", "/app/DotNet.CodeCoverage.BitbucketPipe.dll"]
