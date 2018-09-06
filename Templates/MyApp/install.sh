#!/bin/bash
cd `dirname $0`
sfctl application upload --path MyApp --show-progress
sfctl application provision --application-type-build-path MyApp
sfctl application create --app-name fabric:/MyApp --app-type MyAppType --app-version 1.0.0
cd -
