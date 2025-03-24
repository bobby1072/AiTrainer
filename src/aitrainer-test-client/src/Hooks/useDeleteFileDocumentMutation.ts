import { useMutation, useQueryClient } from "react-query";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";
import { QueryKeys } from "../Constants";
import { useEffect } from "react";

export const useDeleteFileDocumentMutation = () => {
  const { user } = useAuthentication();
  const queryClient = useQueryClient();
  const mutationResults = useMutation<string, Error, { id: string }>(
    ({ id }) => {
      if (!user) throw new Error("User is not authenticated");
      return AiTrainerWebClient.DeleteFileDocument(user.access_token, id);
    }
  );

  useEffect(() => {
    if (mutationResults.data) {
      queryClient.refetchQueries(QueryKeys.GetTopLayerOfFile);
    }
  }, [mutationResults.data, queryClient]);

  return { ...mutationResults };
};
