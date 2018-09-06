#!/bin/bash
DIR=`dirname $0`
source $DIR/dotnet-include.sh

dotnet restore $DIR/../LatamPartnerBootcampSFApp/src/LatamPartnerBootcampSFApp/CustomerProfileService/CustomerProfileService.csproj -s https://api.nuget.org/v3/index.json
dotnet build $DIR/../LatamPartnerBootcampSFApp/src/LatamPartnerBootcampSFApp/CustomerProfileService/CustomerProfileService.csproj -v normal

cd `dirname $DIR/../LatamPartnerBootcampSFApp/src/LatamPartnerBootcampSFApp/CustomerProfileService/CustomerProfileService.csproj`
dotnet publish -o ../../../../LatamPartnerBootcampSFApp/LatamPartnerBootcampSFApp/CustomerProfileServicePkg/Code
cd -
