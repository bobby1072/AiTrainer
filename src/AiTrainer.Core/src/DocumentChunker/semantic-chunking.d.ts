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
    } = {}
  ): any;
}
