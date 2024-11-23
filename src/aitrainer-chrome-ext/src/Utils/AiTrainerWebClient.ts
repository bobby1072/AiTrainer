import axios from "axios";
import AppSettingsProvider from "./AppSettingsProvider";
import AppSettingsKeys from "./AppSettingsKeys";

export default abstract class AiTrainerWebClient {
  private static readonly _httpClient = axios.create({
    baseURL:
      AppSettingsProvider.TryGetValue(AppSettingsKeys.AiTrainerWebEndpoint) ??
      "http://localhost:5007",
  });
}
