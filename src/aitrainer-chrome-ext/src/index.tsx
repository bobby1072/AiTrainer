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
    element: <Navigate to="/TestHome" />,
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
        <AuthenticatedRouteWrapper>{component()}</AuthenticatedRouteWrapper>
      </Wrapper>
    ),
  })),
];
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: true,
      retry: (count) => (count >= 1 ? false : true),
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
        <ClientSettingsConfigurationContextProvider>
          <BrowserRouter>
            <Routes>
              {AppRoutes?.map((r) => (
                <Route element={r.element} path={r.path} />
              ))}
            </Routes>
          </BrowserRouter>
        </ClientSettingsConfigurationContextProvider>
      </QueryClientProvider>
    </React.StrictMode>
  );
}
