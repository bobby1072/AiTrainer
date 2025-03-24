export type FileCollectionSaveInput = {
  id?: string | null;
  parentId?: string | null;
  collectionName: string;
  collectionDescription?: string | null;
  dateCreated?: string | null;
  dateModified?: string | null;
};
