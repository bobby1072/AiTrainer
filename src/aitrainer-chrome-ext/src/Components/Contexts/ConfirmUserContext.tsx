import { createContext, useContext } from "react";
import { SolicitedDeviceToken } from "../../Models/SolicitedDeviceToken";
import { User } from "../../Models/User";
import { useConfirmUser } from "../Hooks/ConfirmUser";
import { useGetDeviceToken } from "../Hooks/GetDeviceToken";
import { ErrorComponent } from "../Common/ErrorComponent";
import { PageBase } from "../Common/PageBase";
import { SaveUserForm } from "../User/SaveUserForm";

export const ConfirmUserContext = createContext<User | undefined>(undefined);

export const useConfirmUserContext = () => {
  const value = useContext(ConfirmUserContext);
  if (!value) throw new Error("DeviceTokenContext has not been registered");
  return value;
};

export const ConfirmUserContextProvider: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {
  const {
    isLoading,
    error,
    confirmUser,
    reset: resetMutation,
    data,
  } = useConfirmUser({
    onSuccess: () => {
      setDeviceToken({ ...deviceToken, inUse: true } as SolicitedDeviceToken);
    },
  });
  const { setValue: setDeviceToken, data: deviceToken } = useGetDeviceToken();
  if (!deviceToken)
    return <ErrorComponent errorMessage="Device token not found" fullScreen />;
  else if (!deviceToken.inUse)
    return (
      <PageBase>
        <SaveUserForm
          handleSubmit={(vals) => {
            resetMutation();
            confirmUser(vals);
          }}
          isLoading={isLoading}
          error={error}
          user={data}
        />
      </PageBase>
    );
  return (
    <ConfirmUserContext.Provider value={data}>
      {children}
    </ConfirmUserContext.Provider>
  );
};
