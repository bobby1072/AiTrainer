import { useMutation, useQueryClient } from "react-query";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";
import { useEffect } from "react";
import { QueryKeys } from "../Constants";

export const useDeleteFileCollectionMutation = () => {
  const { user } = useAuthentication();
  const queryClient = useQueryClient();
  const mutationResults = useMutation<string, Error, { id: string }>(
    ({ id }) => {
      if (!user) throw new Error("User is not authenticated");
      return AiTrainerWebClient.DeleteFileCollection(user.access_token, id);
    }
  );

  useEffect(() => {
    if (mutationResults.data) {
      queryClient.refetchQueries(QueryKeys.GetTopLayerOfFile);
    }
  }, [mutationResults.data, queryClient]);

  return { ...mutationResults };
};
