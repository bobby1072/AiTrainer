export type ChunkedDocument = {
  documentChunks: {
    chunkedTexts: string[];
    fileDocumentId: string;
    metadata: Record<string, string | null | undefined> | null | undefined;
  }[];
};
