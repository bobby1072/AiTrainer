enum AppSettingsKeys {
  OpenAiApiKey = "OPENAI_API_KEY",
  ApiKey = "AiTrainerCore.ApiKey",
  DocumentChunkerChunkSize = "DocumentChunker.ChunkSize",
  DocumentChunkerChunkOverlap = "DocumentChunker.ChunkOverlap",
  ExpressPort = "ExpressPort",
}

type AppSettings = {
  [K in keyof typeof AppSettingsKeys]: string;
};

export default abstract class AppSettingsProvider {
  public static readonly AllAppSettings: AppSettings = Object.entries(
    AppSettingsKeys
  ).reduce(
    (acc, [key, val]) => ({
      ...acc,
      [key]: AppSettingsProvider.TryGetValue(val as any),
    }),
    {}
  ) as AppSettings;
  private static readonly _appSettingsJson: Record<
    string,
    any
  > = require("./../data/expressappsettings.json");
  private static readonly _appSettingsDevJson: Record<
    string,
    any
  > = require("./../data/expressappsettings.dev.json");
  private static readonly _isForceProduction: boolean =
    AppSettingsProvider._appSettingsJson["UseProd"];

  private static TryGetValue(key: AppSettingsKeys): string | undefined | null {
    try {
      const [devResult, prodResult] = [
        AppSettingsProvider.FindVal(
          key.toString(),
          AppSettingsProvider._appSettingsDevJson
        ),
        AppSettingsProvider.FindVal(
          key.toString(),
          AppSettingsProvider._appSettingsJson
        ),
      ];
      return AppSettingsProvider._isForceProduction
        ? prodResult?.toString()
        : process.env.NODE_ENV === "development"
        ? devResult?.toString() || prodResult?.toString()
        : prodResult?.toString();
    } catch {
      return undefined;
    }
  }
  private static FindVal(
    key: string,
    jsonDoc: Record<string, any>
  ): string | null | undefined {
    const keys = key.split(".");
    let result: any = jsonDoc;
    for (const k of keys) {
      try {
        if (result[k] !== undefined) {
          result = result[k];
        }
      } catch {}
    }

    return result.toString();
  }
}
