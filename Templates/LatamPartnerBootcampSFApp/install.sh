#!/bin/bash
cd `dirname $0`
sfctl application upload --path LatamPartnerBootcampSFApp --show-progress
sfctl application provision --application-type-build-path LatamPartnerBootcampSFApp
sfctl application create --app-name fabric:/LatamPartnerBootcampSFApp --app-type LatamPartnerBootcampSFAppType --app-version 1.0.0
cd -
