export type FileDocument = {
  id?: string | null;
  collectionId?: string | null;
  userId: string;
  fileType: number;
  fileName: string;
  fileData: string;
  dateCreated: string;
};

export type FileDocumentPartial = Omit<FileDocument, "fileData">;
