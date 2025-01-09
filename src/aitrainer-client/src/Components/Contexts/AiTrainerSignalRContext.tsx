import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { createContext, useState } from "react";
import AppSettingsProvider from "../../Utils/AppSettingsProvider";
import { AppSettingsKeys } from "../../Utils/AppSettingsKeys";
import { useConnectToSignalR } from "../../Hooks/ConnectToSignalR";
import { Loading } from "../Common/Loading";
import { ErrorComponent } from "../Common/ErrorComponent";

const signalRConnectionBuilder = new HubConnectionBuilder().withUrl(
  `${
    AppSettingsProvider.TryGetValue(AppSettingsKeys.AiTrainerWebEndpoint) ??
    "http://localhost:5222"
  }/Api/SignalR`
);

export type AiTrainerSignalRContextType = {
  hubConnection: HubConnection;
  isConnected: boolean;
};

export const AiTrainerSignalRContext = createContext<
  AiTrainerSignalRContextType | undefined
>(undefined);

export const AiTrainerSignalRProvider: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {
  const [hubConnection, setHubConnection] = useState<HubConnection>(
    signalRConnectionBuilder.build()
  );
  const [isConnected, setIsConnected] = useState<boolean>(false);
  const { isLoading, error } = useConnectToSignalR(hubConnection, {
    onSuccess: (data) => {
      setIsConnected(data);
    },
  });

  if (isLoading) return <Loading fullScreen />;
  if (error) return <ErrorComponent />;
  return (
    <AiTrainerSignalRContext.Provider value={{ hubConnection, isConnected }}>
      {children}
    </AiTrainerSignalRContext.Provider>
  );
};
