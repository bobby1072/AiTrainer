import axios, { AxiosError } from "axios";
import AppSettingsProvider from "./AppSettingsProvider";
import AppSettingsKeys from "./AppSettingsKeys";
import { AiTrainerWebOutcome } from "../Models/AiTrainerWebOutcome";
import ErrorConstants from "./ErrorConstants";
import { ClientSettingsConfiguration } from "../Models/ClientSettingsConfiguration";

export default abstract class AiTrainerWebClient {
  private static readonly _httpClient = axios.create({
    baseURL:
      AppSettingsProvider.TryGetValue(AppSettingsKeys.AiTrainerWebEndpoint) ??
      "http://localhost:5007",
  });
  private static HandleError(e: any): PromiseLike<never> {
    if (e instanceof AxiosError) {
      const axiosError = e as AxiosError;
      const responseException = axiosError.response
        ?.data as AiTrainerWebOutcome<null>;
      throw new Error(
        responseException.exceptionMessage &&
        responseException.exceptionMessage?.length > 0
          ? responseException.exceptionMessage
          : ErrorConstants.InternalServerError
      );
    } else {
      throw new Error(ErrorConstants.InternalServerError);
    }
  }
  public static async GetClientConfiguration(): Promise<
    ClientSettingsConfiguration | undefined | null
  > {
    const response = await this._httpClient
      .get("Api/ClientConfiguration")
      .catch(this.HandleError);
    const outcome =
      response.data as AiTrainerWebOutcome<ClientSettingsConfiguration>;

    return outcome.data as ClientSettingsConfiguration;
  }
}
