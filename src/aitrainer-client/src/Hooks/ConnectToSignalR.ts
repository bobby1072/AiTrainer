import { HubConnection } from "@microsoft/signalr";
import { useQuery } from "react-query";
import { QueryKeys } from "../Constants";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";
import {
  signalRConnectionBuilderFunc,
  useGetSignalRHubContext,
} from "../Components/Contexts/AiTrainerSignalRContext";
import AppSettingsProvider from "../Utils/AppSettingsProvider";
import { AppSettingsKeys } from "../Utils/AppSettingsKeys";

export const useConnectToSignalR = () => {
  const { user } = useAuthentication();
  const { hubConnection, setHubConnection, isConnected } =
    useGetSignalRHubContext();
  const query = useQuery<HubConnection, Error>(
    QueryKeys.ConnectToSignalR,
    async () => {
      if (!isConnected && user) {
        const localHub = signalRConnectionBuilderFunc()
          .withUrl(
            `${
              AppSettingsProvider.TryGetValue(
                AppSettingsKeys.AiTrainerWebEndpoint
              ) || "http://localhost:5222"
            }/Api/SignalR`,
            {
              accessTokenFactory: () => `${user.access_token}`,
            }
          )
          .build();
        await localHub.start();
      }
      return hubConnection;
    },
    {
      onSuccess: (data) => {
        setHubConnection(data);
      },
    }
  );

  return { ...query };
};
