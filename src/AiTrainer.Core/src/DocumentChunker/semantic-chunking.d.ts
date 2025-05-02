declare module "semantic-chunking" {
  export function chunkit(
    input: {
      document_name: string | null | undefined;
      document_text: string;
    }[],
    chunkitOpts?: {
      logging?: boolean;
      maxTokenSize?: number;
      similarityThreshold?: number;
      dynamicThresholdLowerBound?: number;
      dynamicThresholdUpperBound?: number;
      numSimilaritySentencesLookahead?: number;
      combineChunks?: boolean;
      combineChunksSimilarityThreshold?: number;
      onnxEmbeddingModel?: string;
      dtype?: "fp32" | "fp16" | "q8" | "q4";
      localModelPath?: string;
      modelCacheDir?: string;
      returnEmbedding?: boolean;
      returnTokenLength?: boolean;
      chunkPrefix?: string;
      excludeChunkPrefixInResults?: boolean;
    }
  ): Promise<
    {
      document_id: number;
      document_name: string;
      number_of_chunks: number;
      chunk_number: number;
      model_name: string;
      dtype: string;
      text: string;
      token_length: number;
    }[]
  >;
}
