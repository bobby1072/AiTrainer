import { useMutation } from "react-query";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";
import { FileCollection } from "../Models/FileCollection";
import { FileCollectionSaveInput } from "../Models/FileCollectionSaveInput";

export const useSaveFileCollectionMutation = () => {
  const { user } = useAuthentication();
  const mutationResults = useMutation<
    FileCollection,
    Error,
    { fileColInput: FileCollectionSaveInput }
  >(({ fileColInput: fileCol }) => {
    if (!user) throw new Error("User is not authenticated");
    return AiTrainerWebClient.SaveFileCollection(fileCol, user.access_token);
  });

  return { ...mutationResults };
};
