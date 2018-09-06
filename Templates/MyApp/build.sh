#!/bin/bash
DIR=`dirname $0`
source $DIR/dotnet-include.sh

dotnet restore $DIR/../MyApp/src/MyApp/MyStatelessService/MyStatelessService.csproj -s https://api.nuget.org/v3/index.json
dotnet build $DIR/../MyApp/src/MyApp/MyStatelessService/MyStatelessService.csproj -v normal

cd `dirname $DIR/../MyApp/src/MyApp/MyStatelessService/MyStatelessService.csproj`
dotnet publish -o ../../../../MyApp/MyApp/MyStatelessServicePkg/Code
cd -
