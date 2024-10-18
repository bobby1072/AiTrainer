import { RecursiveCharacterTextSplitter } from "@langchain/textsplitters";
import AppSettingsProvider from "../Utils/AppSettingsProvider";
import ApiException from "../Exceptions/ApiException";
import ExceptionConstants from "../Exceptions/ExceptionConstants";

export default abstract class Chunker {
  private static readonly _splitter = new RecursiveCharacterTextSplitter({
    chunkSize:
      AppSettingsProvider.TryGetValue<number>("DocumentChunker.ChunkSize") ||
      512,
    chunkOverlap:
      AppSettingsProvider.TryGetValue<number>("DocumentChunker.ChunkOverlap") ||
      128,
  });
  public static Chunk(text: string): Promise<string[]> {
    try {
      return this._splitter.splitText(text);
    } catch (e) {
      throw new ApiException(ExceptionConstants.ChunkerError, e as Error);
    }
  }
}
