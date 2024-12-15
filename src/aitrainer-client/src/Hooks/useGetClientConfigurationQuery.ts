import { useQuery } from "react-query";
import { QueryKeys } from "../Constants";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";
import { ClientSettingsConfiguration } from "../Models/ClientSettingsConfiguration";

export const useGetClientConfigurationQuery = () => {
  const query = useQuery<ClientSettingsConfiguration, Error>(
    QueryKeys.GetClientConfiguration,
    AiTrainerWebClient.GetClientConfiguration,
    {
      retry: (count) => count < 3,
    }
  );

  return { ...query };
};
