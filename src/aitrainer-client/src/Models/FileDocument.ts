import { FileDocumentMetaData } from "./FileDocumentMetaData";

export type FileDocument = {
  id?: string | null;
  collectionId?: string | null;
  fileDescription?: string | null;
  userId: string;
  fileType: number;
  fileName: string;
  fileData: string;
  dateCreated: string;
  metaData?: FileDocumentMetaData | null;
};

export type FileDocumentPartial = Omit<FileDocument, "fileData">;
