import {
  FaissLibArgs,
  FaissStore,
} from "@langchain/community/vectorstores/faiss";
import { FaissEmbeddings } from "./FaissEmbeddings";
import path from "path";
import Guid from "../Utils/Guid";
import { Document } from "@langchain/core/documents";
import { access, mkdir, rmdir, unlink, writeFile } from "fs/promises";
import { EmbeddingsInterface } from "@langchain/core/embeddings";
import { DocStore, DocStorePageInfo } from "../Models/DocStore";

export default class AiTrainerFaissStore extends FaissStore {
  private static readonly _directoryToSaveTo: string = "./filestore/";
  private constructor(embeds: EmbeddingsInterface, args: FaissLibArgs) {
    super(embeds, args);
  }
  public async LoadDocumentsIntoStore(documents: Document[]): Promise<void> {
    await this.addDocuments(documents, {
      ids: documents.map((_) => Guid.NewGuidString()),
    });
  }
  public async SaveStoreToFile(): Promise<string> {
    const filePath = AiTrainerFaissStore.CreateNewFilePath();

    await this.save(filePath);

    return filePath;
  }
  public GetSaveItemsFromStore(): {
    jsonDocStore: DocStore;
    indexFile: Buffer;
  } {
    const jsonDocStore = this.docstore._docs;
    const indexFile = this.index;

    return {
      jsonDocStore: AiTrainerFaissStore.RawJsonDocToDocStore(jsonDocStore),
      indexFile: indexFile.toBuffer(),
    };
  }
  public static async SaveRawStoreToFile(
    jsonObject: Record<string, any>,
    indexFile: string | Buffer
  ): Promise<string> {
    await AiTrainerFaissStore.TryToAccessThenCreateDirectory(
      AiTrainerFaissStore._directoryToSaveTo
    );

    const filePath = AiTrainerFaissStore.CreateNewFilePath();

    await AiTrainerFaissStore.TryToAccessThenCreateDirectory(filePath);

    const jsonFilePath = path.join(filePath, "docstore.json");
    const indexFilePath = path.join(filePath, "faiss.index");

    const jsonWriteJob = () =>
      writeFile(jsonFilePath, JSON.stringify(jsonObject));

    const indexWriteJob = async () => writeFile(indexFilePath, indexFile);

    await Promise.all([jsonWriteJob(), indexWriteJob()]);

    return filePath;
  }

  public static async LoadFaissStoreFromFileAndRemoveFile(
    filePath: string
  ): Promise<AiTrainerFaissStore> {
    const vectorStore = await AiTrainerFaissStore.LoadFaissStoreFromFile(
      filePath
    );

    await unlink(path.join(filePath, "docstore.json"));
    await unlink(path.join(filePath, "faiss.index"));
    await rmdir(filePath);

    return AiTrainerFaissStore.ToAiTrainerFaissStore(vectorStore);
  }
  public static CreateFaissStore(): AiTrainerFaissStore {
    const vectorStore = new AiTrainerFaissStore(FaissEmbeddings, {});
    return vectorStore;
  }
  private static CreateNewFilePath(): string {
    return path.join(
      AiTrainerFaissStore._directoryToSaveTo,
      Guid.NewGuidString()
    );
  }
  private static RawJsonDocToDocStore(
    rawStore: Map<string, Document<Record<string, any>>>
  ): DocStore {
    const idEnt: Record<string, string> = Array.from(rawStore.keys()).reduce(
      (acc, val, index) => ({ ...acc, [index.toString()]: val }),
      {}
    );

    return [
      Object.values(idEnt).map((x: string) => {
        const foundDoc = rawStore.get(x);
        return [
          x,
          {
            metadata: foundDoc?.metadata,
            pageContent: foundDoc?.pageContent,
          } as DocStorePageInfo,
        ];
      }),
      idEnt,
    ];
  }
  private static async LoadFaissStoreFromFile(
    filePath: string
  ): Promise<AiTrainerFaissStore> {
    const vectorStore = AiTrainerFaissStore.ToAiTrainerFaissStore(
      await AiTrainerFaissStore.load(filePath, FaissEmbeddings)
    );
    return vectorStore;
  }
  private static ToAiTrainerFaissStore(store: FaissStore) {
    return new AiTrainerFaissStore(store.embeddings, store.args);
  }
  private static async TryToAccessThenCreateDirectory(directory: string) {
    try {
      await access(directory);
    } catch {
      await mkdir(directory);
    }
  }
}
