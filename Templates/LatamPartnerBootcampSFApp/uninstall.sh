#!/bin/bash


sfctl application delete --application-id myapp
sfctl application unprovision --application-type-name myappType --application-type-version 1.0.0
sfctl store delete --content-path myapp