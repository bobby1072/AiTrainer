param (
    [string] $filePath = ".\src\aitrainer-test-client\src\Data\reactappsettings.json"
)
$ErrorActionPreference = "Stop"
$data = @{
  AiTrainerWebEndpoint = "http://localhost:5222"
}


$json = $data | ConvertTo-Json -Depth 3 -Compress

$json | Out-File -FilePath $filePath -Encoding utf8

Write-Host "JSON file 'reactappsettings.json' has been created"