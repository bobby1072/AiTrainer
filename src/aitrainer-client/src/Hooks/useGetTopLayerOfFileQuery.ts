import { useQuery } from "react-query";
import { QueryKeys } from "../Constants";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";
import { FlatFileDocumentPartialCollection } from "../Models/FlatFileDocumentPartialCollection";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";

export const useGetTopLayerOfFileQuery = (parentId?: string | null) => {
  const { user } = useAuthentication();
  if (!user) throw new Error("User is not authenticated");
  const queryResults = useQuery<FlatFileDocumentPartialCollection, Error>(
    QueryKeys.GetTopLayerOfFile,
    () => AiTrainerWebClient.GetLayerOfFile(user.access_token)
  );

  return { ...queryResults };
};
