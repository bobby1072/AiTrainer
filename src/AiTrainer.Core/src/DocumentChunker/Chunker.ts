import { RecursiveCharacterTextSplitter } from "@langchain/textsplitters";
import { ApplicationSettings } from "../Utils/AppSettingsProvider";
import ApiException from "../Exceptions/ApiException";
import ExceptionConstants from "../Exceptions/ExceptionConstants";
import { chunkit } from "semantic-chunking";

export default abstract class Chunker {
  private static readonly _splitter = new RecursiveCharacterTextSplitter({
    chunkSize:
      Number(ApplicationSettings.AllAppSettings.DocumentChunkerChunkSize) ||
      512,
    chunkOverlap:
      Number(ApplicationSettings.AllAppSettings.DocumentChunkerChunkOverlap) ||
      128,
  });
  public static async SemanticChunk({
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
      const chunkitInput = documentsToChunk.map((x) => ({
        document_name: x.fileDocumentId,
        document_text: x.documentText,
      }));
      const chunks = await chunkit(chunkitInput, {
        maxTokenSize:
          Number(ApplicationSettings.AllAppSettings.DocumentChunkerChunkSize) ||
          512,
      });
      const finalChunks = [
        ...new Map(
          documentsToChunk.map((doc) => [doc.fileDocumentId, doc])
        ).values(),
      ].map((x) => ({
        chunkedTexts: chunks
          .filter((y) => y.document_name == x.fileDocumentId)
          .map((y) => y.text),
        metadata: x.metadata,
        fileDocumentId: x.fileDocumentId,
      }));
      return finalChunks;
    } catch (e) {
      throw new ApiException(ExceptionConstants.ChunkerError, e as Error);
    }
  }
  public static async RecursiveChunk({
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
      const allChunks = await Promise.all(
        documentsToChunk.map((x) => Chunker._splitter.splitText(x.documentText))
      );

      return allChunks.map((x, index) => ({
        chunkedTexts: x,
        fileDocumentId: documentsToChunk[index]!.fileDocumentId,
        metadata: documentsToChunk[index]!.metadata,
      }));
    } catch (e) {
      throw new ApiException(ExceptionConstants.ChunkerError, e as Error);
    }
  }
}
