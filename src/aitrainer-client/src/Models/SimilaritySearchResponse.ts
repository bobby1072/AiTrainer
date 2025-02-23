export type SimilaritySearchResponse = {
  items: SimilaritySearchResponseItem[];
};
export type SimilaritySearchResponseItem = {
  pageContent: string;
  metadata: Record<string, string>;
};
