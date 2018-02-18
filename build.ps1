#!/usr/bin/env powershell
$ErrorActionPreference = 'Stop'
$project = "SwapTasks"

function exec($_cmd) {
    write-host " > $_cmd $args" -ForegroundColor cyan
    & $_cmd @args
    if ($LASTEXITCODE -ne 0) {
        throw 'Command failed'
    }
}

Remove-Item artifacts/ -Recurse -ErrorAction Ignore
Remove-Item src/$project/obj/ -Recurse -ErrorAction Ignore
Remove-Item Example/obj/ -Recurse -ErrorAction Ignore

# Build
exec dotnet build -c Release ./src/$project
exec dotnet pack -c Release ./src/$project

# PropExample
exec dotnet msbuild /nologo '/t:Build;Publish' ./Example/PropExample

# NuGetExample
exec dotnet publish ./Example/NuGetExample -c Debug
exec dotnet publish ./Example/NuGetExample /p:Trigger=hoge
exec dotnet publish ./Example/NuGetExample -c Debug /p:Trigger=piyo