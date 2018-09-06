#!/bin/bash
cd `dirname $0`
sfctl application upload --path MyApp --show-progress
sfctl application provision --application-type-build-path MyApp
sfctl application upgrade --app-id fabric:/MyApp --app-version $1 --parameters "{}" --mode Monitored
cd -