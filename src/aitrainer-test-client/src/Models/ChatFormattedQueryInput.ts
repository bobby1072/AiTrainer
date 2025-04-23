export type ChatFormattedQueryInput<T> = {
  chunkId: string;
  collectionId?: string | null;
  definedQueryFormatsEnum: number;
  inputJson: T;
};
