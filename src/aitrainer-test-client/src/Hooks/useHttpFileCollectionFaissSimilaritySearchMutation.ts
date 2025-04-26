import { useMutation } from "react-query";
import { SingleDocumentChunk } from "../Models/SingleDocumentChunk";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";

export const useHttpFileCollectionFaissSimilaritySearchMutation = () => {
  const { user } = useAuthentication();
  const mutationResults = useMutation<
    SingleDocumentChunk[],
    Error,
    {
      question: string;
      documentsToReturn: number;
      collectionId?: string | null;
    }
  >((input) => {
    if (!user?.access_token) throw new Error("User is not authenticated");
    return AiTrainerWebClient.SimilaritySearch(input, user.access_token);
  });

  return { ...mutationResults };
};
