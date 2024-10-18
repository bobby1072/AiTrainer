import { RecursiveCharacterTextSplitter } from "@langchain/textsplitters";
import AppSettingsProvider from "../Utils/AppSettingsProvider";
import ApiException from "../Exceptions/ApiException";
import ExceptionConstants from "../Exceptions/ExceptionConstants";

const splitter = new RecursiveCharacterTextSplitter({
  chunkSize:
    AppSettingsProvider.TryGetValue<number>("DocumentChunker.ChunkSize") || 512,
  chunkOverlap:
    AppSettingsProvider.TryGetValue<number>("DocumentChunker.ChunkOverlap") ||
    128,
});

export default abstract class Chunker {
  public static Chunk(text: string): Promise<string[]> {
    try {
      return splitter.splitText(text);
    } catch (e) {
      throw new ApiException(ExceptionConstants.ChunkerError, e as Error);
    }
  }
}
