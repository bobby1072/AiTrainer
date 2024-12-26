import { useMutation, useQueryClient } from "react-query";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";
import { FileCollection } from "../Models/FileCollection";
import { FileCollectionSaveInput } from "../Models/FileCollectionSaveInput";
import { useEffect } from "react";
import { QueryKeys } from "../Constants";

export const useSaveFileCollectionMutation = () => {
  const { user } = useAuthentication();
  const queryClient = useQueryClient();
  const mutationResults = useMutation<
    FileCollection,
    Error,
    { fileColInput: FileCollectionSaveInput }
  >(({ fileColInput: fileCol }) => {
    if (!user) throw new Error("User is not authenticated");
    return AiTrainerWebClient.SaveFileCollection(fileCol, user.access_token);
  });

  useEffect(() => {
    if (mutationResults.data) {
      queryClient.refetchQueries(QueryKeys.GetTopLayerOfFile);
    }
  }, [mutationResults.data, queryClient]);

  return { ...mutationResults };
};
