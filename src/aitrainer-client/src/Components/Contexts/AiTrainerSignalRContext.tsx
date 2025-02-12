import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";
import { createContext, useContext, useEffect, useState } from "react";
import { ApplicationSettings } from "../../Utils/AppSettingsProvider";
import { useConnectToSignalR } from "../../Hooks/useConnectToSignalR";
import { Loading } from "../Common/Loading";
import { ErrorComponent } from "../Common/ErrorComponent";

export const signalRConnectionBuilderFunc = (): HubConnectionBuilder => {
  return new HubConnectionBuilder()
    .withAutomaticReconnect()
    .configureLogging(
      process.env.NODE_ENV === "development" ? LogLevel.Debug : LogLevel.None
    );
};

export type AiTrainerSignalRContextType = {
  hubConnection: HubConnection;
  setHubConnection: (hubConnection: HubConnection) => void;
  isConnected: boolean;
  fullyAuthenticated: boolean;
  setFullyAuthenticated: (newVal: boolean) => void;
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
  const [isConnected, setIsConnected] = useState<boolean>(false);
  const [fullyAuthenticated, setFullyAuthenticated] = useState<boolean>(false);
  useEffect(() => {
    if (!hubConnection) return;

    hubConnection.onreconnected(() => setIsConnected(true));
    hubConnection.onclose(() => setIsConnected(false));

    // const handleEvent = (
    //   eventName: string,
    //   callback: (...args: any[]) => void
    // ) => {
    //   hubConnection.on(eventName, callback);
    // };

    // Register connection state change listeners

    // Example: Listening to a custom SignalR event
    // handleEvent("CustomEventName", (data) => {
    //   console.log("Received custom event:", data);
    // });

    // Start the connection if disconnected
    // if (hubConnection.state === HubConnectionState.Disconnected) {
    //   hubConnection
    //     .start()
    //     .then(() => {
    //       handleConnectionStateChange();
    //     });
    // }

    // return () => {
    //   // Cleanup: Unregister events and stop connection on unmount
    //   hubConnection.off("CustomEventName");
    //   hubConnection
    //     .stop()
    //     .catch((err) => console.error("SignalR Disconnect Error:", err));
    // };
  }, [hubConnection]);
  return (
    <AiTrainerSignalRContext.Provider
      value={{
        hubConnection,
        setHubConnection,
        isConnected,
        fullyAuthenticated,
        setFullyAuthenticated,
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
