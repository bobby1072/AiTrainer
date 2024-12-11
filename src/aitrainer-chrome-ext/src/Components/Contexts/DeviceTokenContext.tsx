import { createContext, useContext } from "react";
import { SolicitedDeviceToken } from "../../Models/SolicitedDeviceToken";
import { useGetDeviceToken } from "../Hooks/IssueTokenMutation";
import { Loading } from "../Common/Loading";
import { ErrorComponent } from "../Common/ErrorComponent";

export const DeviceTokenContext = createContext<
  SolicitedDeviceToken | undefined
>(undefined);

export const useDeviceTokenContext = () => {
  const value = useContext(DeviceTokenContext);
  if (!value) throw new Error("DeviceTokenContext has not been registered");
  return value;
};

export const DeviceTokenContextProvider: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {
  const { isLoading, data, errorMessage } = useGetDeviceToken();
  if (isLoading) return <Loading fullScreen />;
  else if (errorMessage || !data)
    return <ErrorComponent errorMessage={errorMessage} fullScreen />;

  return (
    <DeviceTokenContext.Provider value={data}>
      {children}
    </DeviceTokenContext.Provider>
  );
};
