import { useMutation } from "react-query";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";
import { ChatFormattedQueryInput } from "../Models/ChatFormattedQueryInput";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";

export const useChatQueryMutation = <T>() => {
  const { user } = useAuthentication();

  const mutationResults = useMutation<
    string,
    Error,
    ChatFormattedQueryInput<T>
  >((input) => {
    if (!user) throw new Error("User is not authenticated");
    return AiTrainerWebClient.ChatQuery(input, user.access_token);
  });

  return { ...mutationResults };
};
