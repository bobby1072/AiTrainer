import { SharedFileCollectionSingleMemberSaveInput } from "../Components/File/ShareFileCollectionModal";

export type SharedFileCollectionMemberSaveInput = {
  collectionId: string;
  membersToShareTo: SharedFileCollectionSingleMemberSaveInput[];
};
