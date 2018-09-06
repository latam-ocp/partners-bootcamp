#!/bin/bash
cd `dirname $0`
sfctl application upload --path LatamPartnerBootcampSFApp --show-progress
sfctl application provision --application-type-build-path LatamPartnerBootcampSFApp
sfctl application upgrade --app-id fabric:/LatamPartnerBootcampSFApp --app-version $1 --parameters "{}" --mode Monitored
cd -