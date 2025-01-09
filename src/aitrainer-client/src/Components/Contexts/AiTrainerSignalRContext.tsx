import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { createContext, useEffect, useState } from "react";
import AppSettingsProvider from "../../Utils/AppSettingsProvider";
import { AppSettingsKeys } from "../../Utils/AppSettingsKeys";
import { Loading } from "../Common/Loading";

const signalRConnectionBuilder = new HubConnectionBuilder().withUrl(
  `${
    AppSettingsProvider.TryGetValue(AppSettingsKeys.AiTrainerWebEndpoint) ??
    "http://localhost:5222"
  }/Api/SignalR`
);

export type AiTrainerSignalRContextType = {
  hubConnection: HubConnection;
};

export const AiTrainerSignalRContext = createContext<
  AiTrainerSignalRContextType | undefined
>(undefined);

export const AiTrainerSignalRProvider: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {
  const [hubConnection, setHubConnection] = useState<HubConnection>();

  useEffect(() => {
    setHubConnection(signalRConnectionBuilder.build());
  }, [setHubConnection]);

  if (!hubConnection) return <Loading />;
  return (
    <AiTrainerSignalRContext.Provider value={{ hubConnection }}>
      {children}
    </AiTrainerSignalRContext.Provider>
  );
};
