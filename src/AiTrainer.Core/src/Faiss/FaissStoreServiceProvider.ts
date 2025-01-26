import { FaissStore } from "@langchain/community/vectorstores/faiss";
import { Document } from "@langchain/core/documents";
import Guid from "../Utils/Guid";
import AppSettingsProvider from "../Utils/AppSettingsProvider";
import { AppSettingsKeys } from "../Utils/AppSettingsKeys";
import { access, mkdir, unlink, writeFile } from "fs";
import path from "path";
import { FaissEmbeddings } from "./FaissEmbeddings";

process.env.OPENAI_API_KEY = AppSettingsProvider.TryGetValue(
  AppSettingsKeys.OpenAiApiKey
)!;

export default abstract class FaissStoreServiceProvider {
  private static readonly _directoryToSaveTo: string = "./filestore/";
  public static CreateFaissStore(): FaissStore {
    const vectorStore = new FaissStore(FaissEmbeddings, {});
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
  public static async GetSaveItemsFromStore(store: FaissStore) {
    const docStore = store.docstore;

    const jsonDocStore = docStore._docs;
    const indexFile = store.index;
  }
  public static async SaveStoreToFile(store: FaissStore): Promise<string> {
    const filePath = FaissStoreServiceProvider.CreateNewFilePath();

    await store.save(filePath);

    return filePath;
  }
  public async SaveRawStoreToFile(
    jsonObject: any,
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

    const filePath = FaissStoreServiceProvider.CreateNewFilePath();

    const jsonFilePath = path.join(filePath, "docstore.json");
    const indexFilePath = path.join(filePath, "faiss.index");

    const jsonWriteJob = async () =>
      writeFile(jsonFilePath, JSON.stringify(jsonObject), (ex) => {
        if (ex) {
          throw ex;
        }
      });

    const indexWriteJob = async () =>
      writeFile(indexFilePath, indexFile, (ex) => {
        if (ex) {
          throw ex;
        }
      });

    await Promise.all([jsonWriteJob(), indexWriteJob()]);

    return filePath;
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
  private static async LoadFaissStoreFromFile(
    filePath: string
  ): Promise<FaissStore> {
    const vectorStore = await FaissStore.load(filePath, FaissEmbeddings);
    return vectorStore;
  }
  private static CreateNewFilePath(): string {
    return path.join(
      FaissStoreServiceProvider._directoryToSaveTo,
      Guid.NewGuidString()
    );
  }
}

FaissStoreServiceProvider.LoadFaissStoreFromFileAndRemoveFile;
