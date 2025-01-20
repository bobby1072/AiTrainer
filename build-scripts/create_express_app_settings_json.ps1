# Define the data as a hashtable
$data = @{
    OPENAI_API_KEY  = "OpenAI_ApiKey"
    DocumentChunker = @{
        ChunkSize    = 512
        ChunkOverlap = 128
    }
    ExpressPort     = 4000
    AiTrainerCore   = @{
        ApiKey = "AiTrainer_ApiKey"
    }
}

# Convert the hashtable to JSON
$json = $data | ConvertTo-Json -Depth 3 -Compress

# Save the JSON to a file
$json | Out-File -FilePath "./../src/AiTrainer.Core/src/Data/expressappsettings.json" -Encoding utf8

# Output to console for confirmation
Write-Host "JSON file 'expressappsettings.json' has been created."