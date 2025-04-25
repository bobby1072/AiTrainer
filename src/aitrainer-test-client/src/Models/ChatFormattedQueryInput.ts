export type ChatFormattedQueryInput<T> = {
  collectionId?: string | null;
  definedQueryFormatsEnum: number;
  inputJson: T;
};
