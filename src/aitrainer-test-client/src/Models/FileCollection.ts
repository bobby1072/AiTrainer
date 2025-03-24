import { FileDocument } from "./FileDocument";

export type FileCollection = {
  id?: string | null;
  userId: string;
  collectionName: string;
  collectionDescription?: string | null;
  dateCreated: string;
  dateModified: string;
  parentId?: string | null;
  documents?: FileDocument[] | null;
};
