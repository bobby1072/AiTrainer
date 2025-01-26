export type FileDocumentMetaData = {
  documentId: string;
  title?: string | null;
  author?: string | null;
  subject?: string | null;
  keywords?: string | null;
  creator?: string | null;
  producer?: string | null;
  creationDate?: string | null;
  modifiedDate?: string | null;
  numberOfPages?: number | null;
  isEncrypted?: boolean | null;
  extraData?: { [key: string]: string | undefined | null } | null;
};
