export default abstract class AppSettingsProvider {
  private static readonly _appSettingsJson = require("./../expressappsettings.json");
  public static TryGetValue<T>(key: string): T | undefined | null {
    try {
      const keys = key.split(".");
      let result: any = this._appSettingsJson;

      for (const k of keys) {
        if (result[k] === undefined) {
          return null;
        }
        result = result[k];
      }

      return result as T;
    } catch {
      return undefined;
    }
  }
}
