import { useMutation, useQueryClient } from "react-query";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";
import { QueryKeys } from "../Constants";
import { useEffect } from "react";

export const useHttpFileCollectionFaissSyncMutation = () => {
  const { user } = useAuthentication();
  const queryClient = useQueryClient();
  const mutationResult = useMutation<
    boolean,
    Error,
    { collectionId?: string | null }
  >((input) => {
    if (!user) throw new Error("User is not authenticated");
    return AiTrainerWebClient.SyncFaissStore(
      user.access_token,
      input.collectionId
    );
  });
  useEffect(() => {
    if (mutationResult.data) {
      queryClient.refetchQueries(QueryKeys.GetTopLayerOfFile);
    }
  }, [mutationResult.data, queryClient]);

  return { ...mutationResult };
};
