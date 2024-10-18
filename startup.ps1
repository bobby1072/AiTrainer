param (
    [switch] $debugweb = $false
)
$ErrorActionPreference = "Stop"

if($debugweb -eq $false)
{
    docker compose -f docker-compose.yml -f docker-compose.webapi.yml up -d --build
} else {
    docker compose up -d --build
}