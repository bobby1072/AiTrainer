export default abstract class AppSettingsProvider {
  private static readonly _appSettingsJson = require("./../Data/reactappsettings.json");
  public static TryGetValue(key: string): string | undefined | null {
    try {
      const keys = key.split(".");
      let result: any = AppSettingsProvider._appSettingsJson;

      for (const k of keys) {
        if (result[k] === undefined) {
          return null;
        }
        result = result[k];
      }

      return result as string;
    } catch {
      return undefined;
    }
  }
}
