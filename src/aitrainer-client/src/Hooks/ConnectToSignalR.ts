import { HubConnection } from "@microsoft/signalr";
import { useQuery } from "react-query";
import { QueryKeys } from "../Constants";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";
import { useGetSignalRHubContext } from "../Components/Contexts/AiTrainerSignalRContext";
import { useEffect } from "react";

export const useConnectToSignalR = () => {
  const { isLoggedIn } = useAuthentication();
  const { hubConnection, setHubConnection, isConnected } =
    useGetSignalRHubContext();
  const query = useQuery<HubConnection, Error>(
    QueryKeys.ConnectToSignalR,
    async () => {
      if (!isConnected && isLoggedIn) {
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
  const { refetch } = query;

  useEffect(() => {
    if (isLoggedIn && !isConnected) {
      refetch();
    }
  }, [refetch, isConnected, isLoggedIn]);

  return { ...query };
};
