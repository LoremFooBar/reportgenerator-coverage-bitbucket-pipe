FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine as build

ENV PRROJECT_NAME "ReportGenerator.BitbucketPipe"

WORKDIR /source

COPY src/$PRROJECT_NAME/$PRROJECT_NAME.csproj .
COPY src/$PRROJECT_NAME/packages.lock.json .

RUN dotnet restore --locked-mode

COPY src/$PRROJECT_NAME/. ./

RUN dotnet publish --no-restore -c release -o /app


FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine

LABEL maintainer="@amit_e"

WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "/app/ReportGenerator.BitbucketPipe.dll"]
