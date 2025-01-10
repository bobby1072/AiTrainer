import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";
import { createContext, useState } from "react";
import AppSettingsProvider from "../../Utils/AppSettingsProvider";
import { AppSettingsKeys } from "../../Utils/AppSettingsKeys";
import { useConnectToSignalR } from "../../Hooks/ConnectToSignalR";
import { Loading } from "../Common/Loading";
import { ErrorComponent } from "../Common/ErrorComponent";

const signalRConnectionBuilder = new HubConnectionBuilder()
  .withUrl(
    `${
      AppSettingsProvider.TryGetValue(AppSettingsKeys.AiTrainerWebEndpoint) ||
      "http://localhost:5222"
    }/Api/SignalR`
  )
  .configureLogging(
    process.env.NODE_ENV === "development" ? LogLevel.Debug : LogLevel.None
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
  const [hubConnection] = useState<HubConnection>(
    signalRConnectionBuilder.build()
  );
  const { isLoading, error } = useConnectToSignalR(hubConnection);

  if (isLoading) return <Loading fullScreen />;
  if (error) return <ErrorComponent fullScreen />;
  return (
    <AiTrainerSignalRContext.Provider
      value={{
        hubConnection,
        isConnected: hubConnection.state === HubConnectionState.Connected,
      }}
    >
      {children}
    </AiTrainerSignalRContext.Provider>
  );
};
