﻿param([switch]$pack)

$ErrorActionPreference = "Stop"

function step($command) {
    write-host ([Environment]::NewLine + $command.ToString().Trim()) -fore CYAN
    & $command
    if ($lastexitcode -ne 0) { throw $lastexitcode }
}

$fixie = "src/artifacts/bin/Fixie.Console/release/Fixie.Console.dll"

if (test-path artifacts) { remove-item artifacts -Recurse }

step { dotnet clean src -c Release --nologo -v minimal }
step { dotnet build src -c Release --nologo }
step { dotnet $fixie *.Tests -c Release --no-build }

if ($pack) {
    step { dotnet pack src/Fixie -o artifacts -c Release --no-build --nologo }
    step { dotnet pack src/Fixie.Console -o artifacts -c Release --no-build --nologo }
    step { dotnet pack src/Fixie.TestAdapter -o artifacts -c Release --no-build --nologo }
}