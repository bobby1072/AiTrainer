import { useMutation } from "react-query";
import { useAuthentication } from "../Components/Contexts/AuthenticationContext";
import { SharedFileCollectionMember } from "../Models/SharedFileCollectionMember";
import { SharedFileCollectionMemberSaveInput } from "../Models/SharedFileCollectionMemberSaveInput";
import AiTrainerWebClient from "../Utils/AiTrainerWebClient";

export const useShareFileCollectionWithMembersMutation = () => {
  const { user } = useAuthentication();
  const mutationResults = useMutation<
    SharedFileCollectionMember[],
    Error,
    { fileColInput: SharedFileCollectionMemberSaveInput }
  >(({ fileColInput: fileCol }) => {
    if (!user) throw new Error("User is not authenticated");
    return AiTrainerWebClient.ShareFileCollection(fileCol, user.access_token);
  });

  return { ...mutationResults };
};
