import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";
import { createContext, useContext, useEffect, useState } from "react";
import { ApplicationSettings } from "../../Utils/AppSettingsProvider";
import { useConnectToSignalR } from "../../Hooks/useConnectToSignalR";
import { Loading } from "../Common/Loading";
import { ErrorComponent } from "../Common/ErrorComponent";
import { useMutation } from "react-query";

export const signalRConnectionBuilderFunc = (): HubConnectionBuilder => {
  return new HubConnectionBuilder()
    .withAutomaticReconnect()
    .configureLogging(
      process.env.NODE_ENV === "development" ? LogLevel.Debug : LogLevel.None
    );
};

export type AiTrainerSignalRContextType = {
  hubConnection: HubConnection;
  isConnected: boolean;
  setHubConnection: (hubConnection: HubConnection) => void;
  disposeConnection: () => void;
};

const AiTrainerSignalRContext = createContext<
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
    signalRConnectionBuilderFunc()
      .withUrl(
        `${
          ApplicationSettings.AllAppSettings.AiTrainerWebEndpoint ||
          "http://localhost:5222"
        }/Api/SignalR`
      )
      .build()
  );
  const { mutate: disposeConnection } = useMutation(
    async () => {
      await hubConnection.stop();
      return hubConnection;
    },
    {
      onSuccess: (data) => setHubConnection(data),
    }
  );
  useEffect(() => {
    if (!hubConnection) return;
    hubConnection.onreconnected(() => {
      setHubConnection(hubConnection);
    });
    hubConnection.onclose(() => {
      setHubConnection(hubConnection);
    });
  }, [hubConnection]);
  return (
    <AiTrainerSignalRContext.Provider
      value={{
        hubConnection,
        isConnected: hubConnection.state === HubConnectionState.Connected,
        setHubConnection,
        disposeConnection,
      }}
    >
      {children}
    </AiTrainerSignalRContext.Provider>
  );
};

export const AiTrainerSignalRStartConnectionProvider: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {
  const { error, isLoading } = useConnectToSignalR();

  if (isLoading) return <Loading fullScreen />;
  if (error) return <ErrorComponent fullScreen />;

  return <>{children}</>;
};
