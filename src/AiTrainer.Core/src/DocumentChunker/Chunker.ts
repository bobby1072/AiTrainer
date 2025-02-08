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
  public static async Chunk(text: string[]): Promise<string[]> {
    try {
      const jobs = await Promise.all(
        text.map((x) => Chunker._splitter.splitText(x))
      );

      return jobs.flat();
    } catch (e) {
      throw new ApiException(ExceptionConstants.ChunkerError, e as Error);
    }
  }
}
