import { useQuery } from "react-query";
import { QueryKeys } from "../Constants";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";
import { FlatFileDocumentPartialCollection } from "../Models/FlatFileDocumentPartialCollection";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";

export const useGetTopLayerOfFileQuery = (parentId?: string | null) => {
  const { user } = useAuthentication();
  const queryResults = useQuery<FlatFileDocumentPartialCollection, Error>(
    QueryKeys.GetTopLayerOfFile,
    () => {
      if (!user) throw new Error("User is not authenticated");
      return AiTrainerWebClient.GetLayerOfFile(user.access_token, parentId);
    }
  );

  return { ...queryResults };
};
