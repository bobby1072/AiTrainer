import axios, { AxiosError, AxiosInstance, AxiosResponse } from "axios";
import AppSettingsProvider from "./AppSettingsProvider";
import { AiTrainerWebOutcome } from "../Models/AiTrainerWebOutcome";
import { ErrorMessages } from "../Constants";
import { AppSettingsKeys } from "./AppSettingsKeys";
import { SolicitedDeviceToken } from "../Models/SolicitedDeviceToken";
import { SaveUserInput } from "../Models/SaveUserInput";
import { User } from "../Models/User";

export default abstract class AiTrainerWebClient {
  private static readonly _baseUrl = AppSettingsProvider.TryGetValue(
    AppSettingsKeys.AiTrainerWebEndpoint
  );
  private static readonly _httpClient: AxiosInstance = axios.create({
    baseURL: AiTrainerWebClient._baseUrl || "http://localhost:5007",
  });
  // public static async GetClientConfiguration(): Promise<ClientSettingsConfiguration> {
  //   const response = await AiTrainerWebClient._httpClient
  //     .get<AiTrainerWebOutcome<ClientSettingsConfiguration>>(
  //       "Api/ClientConfiguration"
  //     )
  //     .catch(AiTrainerWebClient.HandleError)
  //     .then(AiTrainerWebClient.HandleThen);
  //   if (!response) {
  //     throw new Error(ErrorMessages.InternalServerError);
  //   }

  //   return response;
  // }
  public static async IssueDeviceToken(): Promise<SolicitedDeviceToken> {
    const response = await AiTrainerWebClient._httpClient
      .get<AiTrainerWebOutcome<SolicitedDeviceToken>>(
        "Api/User/IssueDeviceToken"
      )
      .catch(AiTrainerWebClient.HandleError)
      .then(AiTrainerWebClient.HandleThen);

    if (!response) {
      throw new Error(ErrorMessages.InternalServerError);
    }

    return response;
  }
  public static async ConfirmUser(userInput: SaveUserInput): Promise<User> {
    const response = await AiTrainerWebClient._httpClient
      .post<AiTrainerWebOutcome<User>>("Api/User/ConfirmUser", userInput)
      .catch(AiTrainerWebClient.HandleError)
      .then(AiTrainerWebClient.HandleThen);

    if (!response) {
      throw new Error(ErrorMessages.InternalServerError);
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
          : ErrorMessages.InternalServerError
      );
    } else {
      throw new Error(ErrorMessages.InternalServerError);
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
          : ErrorMessages.InternalServerError
      );
    }

    return response.data.data;
  }
}
