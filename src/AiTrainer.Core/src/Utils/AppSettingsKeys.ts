export enum AppSettingsKeys {
  OpenAiApiKey = "OPENAI_API_KEY",
  ApiKey = "AiTrainerCore.ApiKey",
  DocumentChunkerChunkSize = "DocumentChunker.ChunkSize",
  DocumentChunkerChunkOverlap = "DocumentChunker.ChunkOverlap",
  ExpressPort = "ExpressPort",
}
export type AppSettings = {
  [K in keyof typeof AppSettingsKeys]: string;
};
