import { FaissStore } from "@langchain/community/vectorstores/faiss";
import { Document } from "@langchain/core/documents";
import Guid from "../Utils/Guid";
import { OpenAIEmbeddings } from "@langchain/openai";
import AppSettingsProvider from "../Utils/AppSettingsProvider";
import { AppSettingsKeys } from "../Utils/AppSettingsKeys";
import { access, mkdir, unlink, writeFile } from "fs";
import path from "path";

process.env.OPENAI_API_KEY = AppSettingsProvider.TryGetValue(
  AppSettingsKeys.OpenAiApiKey
)!;

export default abstract class FaissStoreServiceProvider {
  private static readonly _directoryToSaveTo: string = "./filestore/";
  private static readonly _embeddings = new OpenAIEmbeddings({
    model: "text-embedding-3-small",
  });

  public static CreateFaissStore(): FaissStore {
    const vectorStore = new FaissStore(
      FaissStoreServiceProvider._embeddings,
      {}
    );
    return vectorStore;
  }

  public static async LoadFaissStoreFromFile(
    filePath: string
  ): Promise<FaissStore> {
    const vectorStore = await FaissStore.load(
      filePath,
      FaissStoreServiceProvider._embeddings
    );
    return vectorStore;
  }
  public static async LoadFaissStoreFromFileAndRemoveFile(
    filePath: string
  ): Promise<FaissStore> {
    const vectorStore = await FaissStoreServiceProvider.LoadFaissStoreFromFile(
      filePath
    );

    unlink(filePath, (ex) => {
      if (ex) {
        throw ex;
      }
    });

    return vectorStore;
  }

  public static async SaveStoreToFile(store: FaissStore): Promise<string> {
    const filePath = `${
      FaissStoreServiceProvider._directoryToSaveTo
    }${Guid.NewGuidString()}`;

    await store.save(filePath);

    return filePath;
  }
  //Need to refactor this to save the raw store to file
  public async SaveRawStoreToFile(
    jsonObject: Record<string, any>,
    indexFile: string | Buffer
  ): Promise<string> {
    try {
      await access(FaissStoreServiceProvider._directoryToSaveTo, (ex) => {
        if (ex) {
          throw ex;
        }
      });
    } catch {
      await mkdir(FaissStoreServiceProvider._directoryToSaveTo, (ex) => {
        if (ex) {
          throw ex;
        }
      });
    }

    const jsonFilePath = path.join(
      FaissStoreServiceProvider._directoryToSaveTo,
      "data.json"
    );
    const indexFilePath = path.join(
      FaissStoreServiceProvider._directoryToSaveTo,
      "index.file"
    );

    const jsonWriteJob = writeFile(
      jsonFilePath,
      JSON.stringify(jsonObject),
      (ex) => {
        if (ex) {
          throw ex;
        }
      }
    );

    const indexWriteJob = writeFile(indexFilePath, indexFile, (ex) => {
      if (ex) {
        throw ex;
      }
    });

    await Promise.all([jsonWriteJob, indexWriteJob]);

    throw new Error();
  }

  public static async LoadDocumentsIntoStore(
    store: FaissStore,
    documents: Document[]
  ): Promise<void> {
    await store.addDocuments(documents, {
      ids: documents.map((_) => Guid.NewGuidString()),
    });
  }

  public static async SimSearchStore(
    store: FaissStore,
    prompt: string,
    numberOfDocsToReturn: number
  ): Promise<Document[]> {
    const searchResults = await store.similaritySearch(
      prompt,
      numberOfDocsToReturn
    );

    return searchResults;
  }
}
