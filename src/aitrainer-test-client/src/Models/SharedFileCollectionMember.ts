export type SharedFileCollectionMember = {
  id: string;
  userId: string;
  collectionId: string;
  canViewDocuments: boolean;
  canDownloadDocuments: boolean;
  canCreateDocuments: boolean;
  canRemoveDocuments: boolean;
};
