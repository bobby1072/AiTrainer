import axios, { AxiosError, AxiosInstance, AxiosResponse } from "axios";
import AppSettingsProvider from "./AppSettingsProvider";
import AppSettingsKeys from "./AppSettingsKeys";
import { AiTrainerWebOutcome } from "../Models/AiTrainerWebOutcome";
import Constants from "../Constants";
import { ClientSettingsConfiguration } from "../Models/ClientSettingsConfiguration";

export default abstract class AiTrainerWebClient {
  private static readonly baseUrl = AppSettingsProvider.TryGetValue(
    AppSettingsKeys.AiTrainerWebEndpoint
  );
  private static readonly _httpClient: AxiosInstance = axios.create({
    baseURL: AiTrainerWebClient.baseUrl ?? "http://localhost:5007",
  });
  public static async GetClientConfiguration(): Promise<ClientSettingsConfiguration> {
    const response = await AiTrainerWebClient._httpClient
      .get<AiTrainerWebOutcome<ClientSettingsConfiguration>>(
        "Api/ClientConfiguration"
      )
      .catch(AiTrainerWebClient.HandleError)
      .then(AiTrainerWebClient.HandleThen);
    if (!response) {
      throw new Error(Constants.ErrorMessages.InternalServerError);
    }

    return response;
  }
  private static HandleError(e: any): PromiseLike<never> {
    if (e instanceof AxiosError) {
      const axiosError = e as AxiosError;
      const responseException = axiosError.response
        ?.data as AiTrainerWebOutcome<null>;
      throw new Error(
        responseException?.exceptionMessage &&
        responseException?.exceptionMessage?.length > 0
          ? responseException?.exceptionMessage
          : Constants.ErrorMessages.InternalServerError
      );
    } else {
      throw new Error(Constants.ErrorMessages.InternalServerError);
    }
  }
  private static HandleThen<T>(
    response: AxiosResponse<AiTrainerWebOutcome<T>, any>
  ): T | null | undefined {
    if (response.data.isSuccess === false) {
      throw new Error(
        response.data?.exceptionMessage &&
        response.data?.exceptionMessage?.length > 0
          ? response.data?.exceptionMessage
          : Constants.ErrorMessages.InternalServerError
      );
    }

    return response.data.data;
  }
}
