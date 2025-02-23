export type ChunkedDocument = {
  documentChunks: {
    chunkedTexts: string[];
    metadata: Record<string, string | null | undefined> | null | undefined;
  }[];
};
