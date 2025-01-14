import { useGetSignalRHubContext } from "../Components/Contexts/AiTrainerSignalRContext";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";

export const useAuthenticateWithSignalR = () => {
  const { user } = useAuthentication();
  const { hubConnection, setHubConnection } = useGetSignalRHubContext();
};
