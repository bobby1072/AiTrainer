export type SingleDocumentChunk = {
  id: string;
  pageContent: string;
  metadata?: Record<string, string> | null;
  fileDocumentId: string;
};
