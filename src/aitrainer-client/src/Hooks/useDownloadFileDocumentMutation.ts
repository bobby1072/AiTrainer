import { useMutation } from "react-query";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";

export const useDownloadFileDocumentMutation = () => {
  const { user } = useAuthentication();
  const mutationResults = useMutation<Blob, Error, { fileDocId: string }>(
    ({ fileDocId }) => {
      if (!user) throw new Error("User is not authenticated");
      return AiTrainerWebClient.DownloadFileDocument(
        user.access_token,
        fileDocId
      );
    }
  );

  return { ...mutationResults };
};
