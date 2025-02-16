export type SimilaritySearchResponse = {
  items: SimilaritySearchResponseItems[];
};
type SimilaritySearchResponseItems = {
  pageContent: string;
  metadata: Record<string, string>;
};
