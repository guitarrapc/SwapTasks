#!/usr/bin/env bash

project=SwapTasks
set -eu

CYAN='\033[0;36m'
NC='\033[0m'

__exec() {
    local cmd=${1:0}
    shift
    echo -e "${CYAN} > $cmd $@${NC}"
    $cmd $@
}

rm -r artifacts/
rm -r Example/obj/
rm -r Source/${project}/obj/

# Build
__exec dotnet build -c Release ./src/${project}
__exec dotnet pack -c Release ./src/${project}

# PropExample
__exec dotnet msbuild /nologo '/t:Build;Publish' ./Example/PropExample

# NuGetExample
__exec dotnet publish ./Example/NuGetExample -c Debug
__exec dotnet publish ./Example/NuGetExample /p:Trigger=hoge
__exec dotnet publish ./Example/NuGetExample -c Debug /p:Trigger=piyo