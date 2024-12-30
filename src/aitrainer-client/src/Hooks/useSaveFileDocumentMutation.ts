import { useMutation, useQueryClient } from "react-query";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";
import { FileDocumentPartial } from "../Models/FileDocument";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";
import { useEffect } from "react";
import { QueryKeys } from "../Constants";

export const useSaveFileDocumentMutation = () => {
  const { user } = useAuthentication();
  const queryClient = useQueryClient();

  const mutationResult = useMutation<
    FileDocumentPartial,
    Error,
    {
      saveInput: FormData;
    }
  >((sv) => {
    if (!user) throw new Error("User is not authenticated");
    return AiTrainerWebClient.SaveFileDocument(user.access_token, sv.saveInput);
  });

  useEffect(() => {
    if (mutationResult.data) {
      queryClient.refetchQueries(QueryKeys.GetTopLayerOfFile);
    }
  }, [mutationResult.data, queryClient]);

  return { ...mutationResult };
};
