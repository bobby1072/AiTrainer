param (
    [switch] $debug = $false
)
$ErrorActionPreference = "Stop"

if ($debug -eq $false) {
    docker compose -f docker-compose.yml -f docker-compose.webapi.yml up -d --build
}
else {
    docker compose up -d --build
}


npm start --prefix src/aitrainer-client
