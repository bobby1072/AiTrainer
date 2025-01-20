import { AppSettingsKeys } from "./AppSettingsKeys";

export default abstract class AppSettingsProvider {
  private static readonly _appSettingsJson = require("./../data/expressappsettings.json");
  private static readonly _appSettingsDevJson = require("./../data/expressappsettings.dev.json");
  public static TryGetValue(key: AppSettingsKeys): string | undefined | null {
    try {
      const keys = key.split(".");
      if (keys.length < 1) {
        return undefined;
      }
      let prodResult: any = AppSettingsProvider._appSettingsJson;
      let devResult: any = AppSettingsProvider._appSettingsDevJson;
      for (const k of keys) {
        try {
          if (prodResult[k] === undefined && devResult[k] == undefined) {
            return undefined;
          }
          if (devResult[k] !== undefined) {
            devResult = devResult[k];
          }
          if (prodResult[k] !== undefined) {
            prodResult = prodResult[k];
          }
          prodResult = prodResult[k];
        } catch {}
      }

      return process.env.NODE_ENV === "development"
        ? devResult
          ? devResult
          : prodResult
        : (prodResult.toString() as string);
    } catch {
      return undefined;
    }
  }
}
