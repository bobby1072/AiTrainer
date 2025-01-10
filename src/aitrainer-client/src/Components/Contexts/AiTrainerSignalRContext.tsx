import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";
import { createContext, useContext, useState } from "react";
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
  setHubConnection: (hubConnection: HubConnection) => void;
  isConnected: boolean;
};

export const AiTrainerSignalRContext = createContext<
  AiTrainerSignalRContextType | undefined
>(undefined);

export const useGetSignalRHubContext = () => {
  const context = useContext(AiTrainerSignalRContext);
  if (!context) {
    throw new Error(
      "useGetSignalRHubContext must be used within a AiTrainerSignalRProvider"
    );
  }
  return context;
};

export const AiTrainerSignalRProvider: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {
  const [hubConnection, setHubConnection] = useState<HubConnection>(
    signalRConnectionBuilder.build()
  );
  return (
    <AiTrainerSignalRContext.Provider
      value={{
        hubConnection,
        setHubConnection,
        isConnected: hubConnection.state === HubConnectionState.Connected,
      }}
    >
      {children}
    </AiTrainerSignalRContext.Provider>
  );
};

export const AiTrainerSignalRAuthenticatedProvider: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {
  const { error, isLoading } = useConnectToSignalR();

  if (isLoading) return <Loading fullScreen />;
  if (error) return <ErrorComponent fullScreen />;

  return <>{children}</>;
};
