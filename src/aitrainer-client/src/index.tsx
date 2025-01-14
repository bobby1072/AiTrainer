import React from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import { QueryClient, QueryClientProvider } from "react-query";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { UserManager } from "oidc-client-ts";
import { useAuthentication } from "./Components/Contexts/AuthenticationContext";
import { AuthenticatedRouteWrapper } from "./Components/Authentication/AuthenticatedRouteWrapper";
import { ClientSettingsConfigurationContextProvider } from "./Components/Contexts/ClientSettingsConfigurationContext";
import { SignInCallback } from "./Components/Authentication/SignInCallback";
import { LandingPage } from "./Components/Pages/LandingPage";
import { App } from "./App";
import { AuthenticatedRoutes } from "./Components/Authentication/AutheticatedRoutes";
import { SnackbarProvider } from "notistack";
import { FileCollectionLevelContextProvider } from "./Components/Contexts/FileCollectionLevelContext";
import {
  AiTrainerSignalRStartConnectionProvider,
  AiTrainerSignalRProvider,
} from "./Components/Contexts/AiTrainerSignalRContext";
const FallbackRoute: React.FC = () => {
  const { isLoggedIn } = useAuthentication();
  return isLoggedIn ? (
    <Navigate to={`/oidc-signin`} />
  ) : (
    <Navigate to={`/login`} />
  );
};

const Wrapper: React.FC<{ children: React.ReactNode }> = ({ children }) => (
  <App>{children}</App>
);

const AppRoutes = [
  {
    path: "/",
    element: (
      <Wrapper>
        <FallbackRoute />
      </Wrapper>
    ),
  },
  {
    path: "/login",
    element: (
      <Wrapper>
        <LandingPage />
      </Wrapper>
    ),
  },
  {
    path: "/Home",
    element: <Navigate to="/collection/home" />,
  },
  {
    path: "/oidc-signin",
    element: (
      <Wrapper>
        <SignInCallback />
      </Wrapper>
    ),
  },
  ...AuthenticatedRoutes?.map(({ link, component }) => ({
    path: link,
    element: (
      <Wrapper>
        <AuthenticatedRouteWrapper>
          <AiTrainerSignalRStartConnectionProvider>
            {component()}
          </AiTrainerSignalRStartConnectionProvider>
        </AuthenticatedRouteWrapper>
      </Wrapper>
    ),
  })),
];
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: () => false,
    },
  },
});
if (window.location.pathname === "/oidc-silent-renew") {
  new UserManager({} as any).signinSilentCallback().catch((error: any) => {
    console.error(error);
  });
} else {
  const container = document.getElementById("root");
  const root = createRoot(container!);

  root.render(
    <React.StrictMode>
      <QueryClientProvider client={queryClient}>
        <SnackbarProvider>
          <ClientSettingsConfigurationContextProvider>
            <FileCollectionLevelContextProvider>
              <AiTrainerSignalRProvider>
                <BrowserRouter>
                  <Routes>
                    {AppRoutes?.map((r) => (
                      <Route element={r.element} path={r.path} />
                    ))}
                  </Routes>
                </BrowserRouter>
              </AiTrainerSignalRProvider>
            </FileCollectionLevelContextProvider>
          </ClientSettingsConfigurationContextProvider>
        </SnackbarProvider>
      </QueryClientProvider>
    </React.StrictMode>
  );
}
