import { useInitiateUserConnectionQuery } from "../../Hooks/useInitiateUserConnectionQuery";
import { ErrorComponent } from "../Common/ErrorComponent";
import { Loading } from "../Common/Loading";

export const InitiateUserConnectionContextProvider: React.FC<{
  children: React.ReactNode;
}> = ({ children }) => {
  const { isLoading, error } = useInitiateUserConnectionQuery();

  if (isLoading) return <Loading fullScreen />;
  if (error) return <ErrorComponent fullScreen />;

  return <>{children}</>;
};
