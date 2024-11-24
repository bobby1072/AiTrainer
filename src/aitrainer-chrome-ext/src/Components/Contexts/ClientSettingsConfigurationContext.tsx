import { createContext, useContext } from "react";
import { useGetClientConfigurationQuery } from "../../Hooks/useGetClientConfigurationQuery";
import { ClientSettingsConfiguration } from "../../Models/ClientSettingsConfiguration";
import { ErrorComponent } from "../Common/ErrorComponent";
import { Loading } from "../Common/Loading";
export const ClientSettingsConfigurationContext = createContext<
  ClientSettingsConfiguration | undefined
>(undefined);

export const useClientSettingsConfigurationContext = () => {
  const value = useContext(ClientSettingsConfigurationContext);
  if (!value)
    throw new Error(
      `${ClientSettingsConfigurationContext.displayName} has not been registered`
    );
  return value;
};

export const ClientSettingsConfigurationContextProvider: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {
  const { isLoading, error, data } = useGetClientConfigurationQuery();

  if (isLoading) return <Loading fullScreen />;
  else if (error)
    return <ErrorComponent fullScreen errorMessage={error.message} />;
  else if (!data) return <ErrorComponent fullScreen />;
  return (
    <ClientSettingsConfigurationContext.Provider value={data}>
      {children}
    </ClientSettingsConfigurationContext.Provider>
  );
};
