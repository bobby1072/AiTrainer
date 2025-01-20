import { AppSettingsKeys } from "./AppSettingsKeys";

export default abstract class AppSettingsProvider {
  private static readonly _appSettingsJson = require("./../data/expressappsettings.json");
  public static TryGetValue(key: AppSettingsKeys): string | undefined | null {
    try {
      const keys = key.split(".");
      if (keys.length < 1) {
        return undefined;
      }
      let result: any = this._appSettingsJson;
      for (const k of keys) {
        if (result[k] === undefined) {
          return undefined;
        }
        result = result[k];
      }

      return result.toString() as string;
    } catch {
      return undefined;
    }
  }
}
