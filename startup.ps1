param (
    [switch] $debug = $false,
    [switch] $webDebug = $false
)
$ErrorActionPreference = "Stop"

if ($webDebug -eq $true) {
    docker compose -f docker-compose.yml -f docker-compose.coreapi.yml up -d --build
}
elseif ($debug -eq $false) {
    docker compose -f docker-compose.yml -f docker-compose.webapi.yml -f docker-compose.coreapi.yml up -d --build
}
else {
    docker compose up -d --build
}


npm start --prefix src/aitrainer-test-client
