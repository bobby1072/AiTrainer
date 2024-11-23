import { useQuery } from "react-query";
import Constants from "../Constants";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";
import { ClientSettingsConfiguration } from "../Models/ClientSettingsConfiguration";

export const useGetClientConfigurationQuery = () => {
  const query = useQuery<ClientSettingsConfiguration, Error>(
    Constants.QueryKeys.GetClientConfiguration,
    AiTrainerWebClient.GetClientConfiguration
  );

  return { ...query };
};
