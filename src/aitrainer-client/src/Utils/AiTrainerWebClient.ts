import axios, { AxiosError, AxiosInstance, AxiosResponse } from "axios";
import AppSettingsProvider from "./AppSettingsProvider";
import { AiTrainerWebOutcome } from "../Models/AiTrainerWebOutcome";
import { ClientSettingsConfiguration } from "../Models/ClientSettingsConfiguration";
import { ErrorMessages } from "../Constants";
import { AppSettingsKeys } from "./AppSettingsKeys";
import { FlatFileDocumentPartialCollection } from "../Models/FlatFileDocumentPartialCollection";
import { FileCollectionSaveInput } from "../Models/FileCollectionSaveInput";
import { FileCollection } from "../Models/FileCollection";
import { FileDocumentPartial } from "../Models/FileDocument";

export default abstract class AiTrainerWebClient {
  private static readonly _baseUrl = AppSettingsProvider.TryGetValue(
    AppSettingsKeys.AiTrainerWebEndpoint
  );
  private static readonly _httpClient: AxiosInstance = axios.create({
    baseURL: AiTrainerWebClient._baseUrl || "http://localhost:5007",
  });
  public static async GetClientConfiguration(): Promise<ClientSettingsConfiguration> {
    const response = await AiTrainerWebClient._httpClient
      .get<AiTrainerWebOutcome<ClientSettingsConfiguration>>(
        "Api/ClientConfiguration"
      )
      .catch(AiTrainerWebClient.HandleError)
      .then(AiTrainerWebClient.HandleThen);

    if (!response) {
      throw new Error(ErrorMessages.ErrorHasOccurred);
    }

    return response;
  }
  public static async SaveFileDocument(
    accessToken: string,
    uploadFormData: FormData
  ): Promise<FileDocumentPartial> {
    const response = await AiTrainerWebClient._httpClient
      .post<AiTrainerWebOutcome<FileDocumentPartial>>(
        "Api/FileDocument/Upload",
        uploadFormData,
        {
          headers: {
            Authorization: AiTrainerWebClient.FormatAccessToken(accessToken),
          },
        }
      )
      .catch(AiTrainerWebClient.HandleError)
      .then(AiTrainerWebClient.HandleThen);

    if (!response) {
      throw new Error(ErrorMessages.ErrorHasOccurred);
    }

    return response;
  }
  public static async DeleteFileCollection(
    accessToken: string,
    fileColId: string
  ): Promise<string> {
    const deletedId = await AiTrainerWebClient._httpClient
      .post<AiTrainerWebOutcome<string>>(
        `Api/FileCollection/Delete`,
        { id: fileColId },
        {
          headers: {
            Authorization: AiTrainerWebClient.FormatAccessToken(accessToken),
          },
        }
      )
      .catch(AiTrainerWebClient.HandleError)
      .then(AiTrainerWebClient.HandleThen);

    if (!deletedId) {
      throw new Error(ErrorMessages.ErrorHasOccurred);
    }

    return deletedId;
  }
  public static async DeleteFileDocument(
    accessToken: string,
    fileDocId: string
  ): Promise<string> {
    const deletedId = await AiTrainerWebClient._httpClient
      .post<AiTrainerWebOutcome<string>>(
        `Api/FileDocument/Delete`,
        { id: fileDocId },
        {
          headers: {
            Authorization: AiTrainerWebClient.FormatAccessToken(accessToken),
          },
        }
      )
      .catch(AiTrainerWebClient.HandleError)
      .then(AiTrainerWebClient.HandleThen);

    if (!deletedId) {
      throw new Error(ErrorMessages.ErrorHasOccurred);
    }

    return deletedId;
  }
  public static async DownloadFileDocument(
    accessToken: string,
    fileDocId: string
  ): Promise<Blob> {
    const response = await AiTrainerWebClient._httpClient
      .post<Blob>(
        "Api/FileDocument/Download",
        { id: fileDocId },
        {
          headers: {
            Authorization: AiTrainerWebClient.FormatAccessToken(accessToken),
          },
          responseType: "blob",
        }
      )
      .catch(AiTrainerWebClient.HandleError);

    if (!response.data) {
      throw new Error(ErrorMessages.ErrorHasOccurred);
    }

    return response.data;
  }
  public static async GetLayerOfFile(
    accessToken: string,
    parentId?: string | null
  ): Promise<FlatFileDocumentPartialCollection> {
    const response = await AiTrainerWebClient._httpClient
      .post<AiTrainerWebOutcome<FlatFileDocumentPartialCollection>>(
        "Api/FileCollection/GetOneLayer",
        { id: parentId ? parentId : null },
        {
          headers: {
            Authorization: AiTrainerWebClient.FormatAccessToken(accessToken),
          },
        }
      )
      .catch(AiTrainerWebClient.HandleError)
      .then(AiTrainerWebClient.HandleThen);

    if (!response) {
      throw new Error(ErrorMessages.ErrorHasOccurred);
    }

    return response;
  }
  public static async SaveFileCollection(
    fileInput: FileCollectionSaveInput,
    accessToken: string
  ): Promise<FileCollection> {
    const response = await AiTrainerWebClient._httpClient
      .post<AiTrainerWebOutcome<FileCollection>>(
        "Api/FileCollection/Save",
        fileInput,
        {
          headers: {
            Authorization: AiTrainerWebClient.FormatAccessToken(accessToken),
          },
        }
      )
      .catch(AiTrainerWebClient.HandleError)
      .then(AiTrainerWebClient.HandleThen);

    if (!response) {
      throw new Error(ErrorMessages.ErrorHasOccurred);
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
          : ErrorMessages.ErrorHasOccurred
      );
    } else {
      throw new Error(ErrorMessages.ErrorHasOccurred);
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
          : ErrorMessages.ErrorHasOccurred
      );
    }

    return response.data.data;
  }
  private static FormatAccessToken(accessToken: string) {
    return `Bearer ${accessToken
      .replaceAll("Bearer ", "")
      .replaceAll("bearer ", "")}`;
  }
}
