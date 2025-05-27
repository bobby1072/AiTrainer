export type SharedFileCollectionMemberSaveInput = {
  collectionId: string;
  membersToShareTo: SharedFileCollectionSingleMemberSaveInput[];
};

export type SharedFileCollectionSingleMemberSaveInput = {
  userId: string;
  canViewDocuments: boolean;
  canDownloadDocuments: boolean;
  canCreateDocuments: boolean;
  canRemoveDocuments: boolean;
};
