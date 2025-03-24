import { useQuery } from "react-query";
import { QueryKeys } from "../Constants";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";

export const useInitiateUserConnectionQuery = () => {
  const { user } = useAuthentication();
  const queryResults = useQuery(QueryKeys.InitiateUserConnection, () => {
    if (!user) throw new Error("User is not authenticated");
    return AiTrainerWebClient.InitiateConnection(user.access_token);
  });

  return { ...queryResults };
};
