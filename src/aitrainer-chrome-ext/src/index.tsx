import React from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import { QueryClient, QueryClientProvider } from "react-query";
import { MemoryRouter, Navigate, Route, Routes } from "react-router-dom";
import { Routes as AppRoutes } from "./Components/Common/Routes";
import { DeviceTokenContextProvider } from "./Components/Contexts/DeviceTokenContext";
import { ConfirmUserContextProvider } from "./Components/Contexts/ConfirmUserContext";

const AppComps = [
  {
    path: "/",
    element: <Navigate to="/TestHome" />,
  },
  ...AppRoutes?.map(({ link, component }) => ({
    path: link,
    element: component(),
  })),
];
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus:
        process.env.NODE_ENV === "development" ? true : false,
      retry: (count) => (count >= 2 ? false : true),
    },
  },
});
const container = document.getElementById("root");
const root = createRoot(container!);

root.render(
  <React.StrictMode>
    <QueryClientProvider client={queryClient}>
      <DeviceTokenContextProvider>
        <ConfirmUserContextProvider>
          <MemoryRouter>
            <Routes>
              {AppComps?.map((r) => (
                <Route element={r.element} path={r.path} />
              ))}
            </Routes>
          </MemoryRouter>
        </ConfirmUserContextProvider>
      </DeviceTokenContextProvider>
    </QueryClientProvider>
  </React.StrictMode>
);
