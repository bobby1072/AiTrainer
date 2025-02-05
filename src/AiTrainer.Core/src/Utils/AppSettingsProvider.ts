import { AppSettings, AppSettingsKeys } from "./AppSettingsKeys";

type JsonDoc = {
  [key: string]: any;
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
  private static readonly _appSettingsJson: JsonDoc = require("./../data/expressappsettings.json");
  private static readonly _appSettingsDevJson: JsonDoc = require("./../data/expressappsettings.dev.json");
  private static readonly _isForceProduction: boolean =
    AppSettingsProvider._appSettingsJson["UseProd"];

  private static TryGetValue(key: AppSettingsKeys): string | undefined | null {
    try {
      const devResult = AppSettingsProvider.FindVal(
        key.toString(),
        AppSettingsProvider._appSettingsDevJson
      );

      const prodResult = AppSettingsProvider.FindVal(
        key.toString(),
        AppSettingsProvider._appSettingsJson
      );

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
    jsonDoc: JsonDoc
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
