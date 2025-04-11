import { RecursiveCharacterTextSplitter } from "@langchain/textsplitters";
import { ApplicationSettings } from "../Utils/AppSettingsProvider";
import ApiException from "../Exceptions/ApiException";
import ExceptionConstants from "../Exceptions/ExceptionConstants";

export default abstract class Chunker {
  private static readonly _splitter = new RecursiveCharacterTextSplitter({
    chunkSize:
      Number(ApplicationSettings.AllAppSettings.DocumentChunkerChunkSize) ||
      512,
    chunkOverlap:
      Number(ApplicationSettings.AllAppSettings.DocumentChunkerChunkOverlap) ||
      128,
  });
  public static async Chunk({
    documentsToChunk,
  }: {
    documentsToChunk: {
      documentText: string;
      fileDocumentId: string;
      metadata?: Record<string, string | null | undefined> | null | undefined;
    }[];
  }): Promise<
    {
      chunkedTexts: string[];
      fileDocumentId: string;
      metadata: Record<string, string | null | undefined> | null | undefined;
    }[]
  > {
    try {
      const jobs = await Promise.all(
        documentsToChunk.map((x) => Chunker._splitter.splitText(x.documentText))
      );

      return jobs.map((x, index) => ({
        chunkedTexts: x,
        fileDocumentId: documentsToChunk[index].fileDocumentId,
        metadata: documentsToChunk[index].metadata,
      }));
    } catch (e) {
      throw new ApiException(ExceptionConstants.ChunkerError, e as Error);
    }
  }
}
