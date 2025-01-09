import { HubConnection } from "@microsoft/signalr";
import { QueryKey, useQuery, UseQueryOptions } from "react-query";
import { QueryKeys } from "../Constants";

export const useConnectToSignalR = (
  hubConnection: HubConnection,
  options?: Partial<
    | Omit<
        UseQueryOptions<boolean, Error, boolean, QueryKey>,
        "queryKey" | "queryFn"
      >
    | undefined
  >
) => {
  const query = useQuery<boolean, Error>(
    QueryKeys.ConnectToSignalR,
    async () => {
      await hubConnection.start();
      return true;
    },
    {
      ...(!!options && options),
    }
  );

  return { ...query };
};
