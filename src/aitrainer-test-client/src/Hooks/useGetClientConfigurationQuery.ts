import { useQuery } from "react-query";
import { QueryKeys } from "../Constants";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";
import { ClientSettingsConfiguration } from "../Models/ClientSettingsConfiguration";

export const useGetClientConfigurationQuery = () => {
  const queryResults = useQuery<ClientSettingsConfiguration, Error>(
    QueryKeys.GetClientConfiguration,
    AiTrainerWebClient.GetClientConfiguration,
    {
      retry: (count) => count < 2,
    }
  );

  return { ...queryResults };
};
