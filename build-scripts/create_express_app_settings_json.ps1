param (
    [string] $filePath = ".\src\AiTrainer.Core\src\Data\expressappsettings.json"
)
$ErrorActionPreference = "Stop"
$data = @{
    OPENAI_API_KEY  = "OpenAI_ApiKey"
    ReleaseVersion = "1.0.0"
    DocumentChunker = @{
        ChunkSize    = 512
        ChunkOverlap = 128
    }
    ExpressPort     = 4000
    AiTrainerCore   = @{
        ApiKey = "AiTrainer_ApiKey"
    }
}

$json = $data | ConvertTo-Json -Depth 3 -Compress

$json | Out-File -FilePath $filePath -Encoding utf8

Write-Host "JSON file 'expressappsettings.json' has been created"