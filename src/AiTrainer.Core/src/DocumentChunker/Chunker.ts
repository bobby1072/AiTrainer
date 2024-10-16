import { RecursiveCharacterTextSplitter } from "@langchain/textsplitters";
import AppSettingsProvider from "../Utils/AppSettingsProvider";

const splitter = new RecursiveCharacterTextSplitter({
  chunkSize:
    AppSettingsProvider.TryGetValue<number>("DocumentChunker.ChunkSize") || 512,
  chunkOverlap:
    AppSettingsProvider.TryGetValue<number>("DocumentChunker.ChunkOverlap") ||
    128,
});

export default abstract class Chunker {
  public static Chunk(text: string): Promise<string[]> {
    return splitter.splitText(text);
  }
}
