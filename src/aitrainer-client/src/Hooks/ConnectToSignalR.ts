import { HubConnection, HubConnectionState } from "@microsoft/signalr";
import { useQuery } from "react-query";
import { QueryKeys } from "../Constants";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";
import { useGetSignalRHubContext } from "../Components/Contexts/AiTrainerSignalRContext";

export const useConnectToSignalR = () => {
  const { isLoggedIn } = useAuthentication();
  const { hubConnection, setHubConnection } = useGetSignalRHubContext();
  const query = useQuery<HubConnection, Error>(
    QueryKeys.ConnectToSignalR,
    async () => {
      if (
        hubConnection.state === HubConnectionState.Disconnected &&
        isLoggedIn
      ) {
        await hubConnection.start();
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
