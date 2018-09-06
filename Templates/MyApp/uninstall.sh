#!/bin/bash


sfctl application delete --application-id MyApp
sfctl application unprovision --application-type-name MyAppType --application-type-version 1.0.0
sfctl store delete --content-path MyApp