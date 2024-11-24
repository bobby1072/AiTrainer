import React, { ReactNode } from "react";
import { AuthenticationContextProvider } from "./Components/Contexts/AuthenticationContext";
import { useClientSettingsConfigurationContext } from "./Components/Contexts/ClientSettingsConfigurationContext";

const { protocol, host } = window.location;

export const App: React.FC<{ children: ReactNode }> = ({ children }) => {
  const { authorityClientId, authorityHost, scope } =
    useClientSettingsConfigurationContext();

  return (
    <AuthenticationContextProvider
      clientRootHost={`${protocol}//${host}`}
      settings={{
        authority: authorityHost,
        client_id: authorityClientId,
        scope,
      }}
    >
      {children}
    </AuthenticationContextProvider>
  );
};
