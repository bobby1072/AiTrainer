import { HubConnection, HubConnectionState } from "@microsoft/signalr";
import { useQuery } from "react-query";
import { QueryKeys } from "../Constants";

export const useConnectToSignalR = (hubConnection: HubConnection) => {
  const query = useQuery<void, Error>(QueryKeys.ConnectToSignalR, async () => {
    if (hubConnection.state === HubConnectionState.Disconnected) {
      await hubConnection.start();
    }
  });

  return { ...query };
};
