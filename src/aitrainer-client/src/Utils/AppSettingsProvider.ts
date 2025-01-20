import { AppSettingsKeys } from "./AppSettingsKeys";

export default abstract class AppSettingsProvider {
  private static readonly _appSettingsJson = require("./../Data/reactappsettings.json");
  public static TryGetValue(key: AppSettingsKeys): string | undefined | null {
    try {
      const keys = key.split(".");
      if (keys.length < 1) {
        return undefined;
      }
      let result: any = AppSettingsProvider._appSettingsJson;
      for (const k of keys) {
        if (result[k] === undefined) {
          return undefined;
        }
        result = result[k];
      }

      return result as string;
    } catch {
      return undefined;
    }
  }
}
