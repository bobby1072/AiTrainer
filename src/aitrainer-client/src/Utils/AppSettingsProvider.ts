const appSettingsJson: Record<
  string,
  any
> = require("./../Data/reactappsettings.json");
const appSettingsDevJson: Record<
  string,
  any
> = require("./../Data/reactappsettings.dev.json");

const isForceProduction: boolean = appSettingsJson["UseProd"];

enum AppSettingsKeys {
  AiTrainerWebEndpoint = "AiTrainerWebEndpoint",
}

type AppSettings = {
  [K in keyof typeof AppSettingsKeys]: string;
};

class AppSettingsProvider {
  public readonly AllAppSettings: AppSettings = Object.entries(
    AppSettingsKeys
  ).reduce(
    (acc, [key, val]) => ({
      ...acc,
      [key]: AppSettingsProvider.TryGetValue(val as any),
    }),
    {}
  ) as AppSettings;

  private static TryGetValue(key: AppSettingsKeys): string | undefined | null {
    try {
      const [devResult, prodResult] = [
        AppSettingsProvider.FindVal(key.toString(), appSettingsDevJson),
        AppSettingsProvider.FindVal(key.toString(), appSettingsJson),
      ];
      return isForceProduction
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
    const final = result.toString();
    if (final.toLowerCase() === "[object object]") {
      return undefined;
    }
    return final;
  }
}

export const ApplicationSettings = new AppSettingsProvider();
