param (
    [switch] $webDebug = $false,
    [switch] $coreDebug = $false
)
$ErrorActionPreference = "Stop"

if ($webDebug -eq $false) {
    docker compose -f docker-compose.yml -f docker-compose.webapi.yml up -d --build
}
else {
    docker compose up -d --build
}
if ($coreDebug -eq $false) {
    Start-Process -NoNewWindow -FilePath "cmd.exe" -ArgumentList '/c npm run start:dev --prefix src/AiTrainer.Core'
    Start-Sleep -Seconds 5 
}


npm start --prefix src/aitrainer-client
