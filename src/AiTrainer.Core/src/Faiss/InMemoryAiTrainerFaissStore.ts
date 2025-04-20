import {
  FaissLibArgs,
  FaissStore,
} from "@langchain/community/vectorstores/faiss";
import { FaissEmbeddings } from "./FaissEmbeddings";
import path from "path";
import Guid from "../Utils/Guid";
import { Document } from "@langchain/core/documents";
import { EmbeddingsInterface } from "@langchain/core/embeddings";
import { DocStore, DocStorePageInfo } from "../Models/DocStore";
import { fs } from "memfs";
const in_memory_js_1 = require("./in_memory.ts");
export default class InMemoryAiTrainerFaissStore extends FaissStore {
  private static readonly _directoryToSaveTo: string = "/filestore/";
  private constructor(embeds: EmbeddingsInterface, args: FaissLibArgs) {
    super(embeds, args);
  }
  public async RemoveDocumentsFromStore(documentIds: string[]) {
    await this.delete({ ids: documentIds });
  }
  public async LoadDocumentsIntoStore(documents: Document[]): Promise<void> {
    await this.addDocuments(documents, {
      ids: documents.map((_) => Guid.NewGuidString()),
    });
  }
  public GetSaveItemsFromStore(): {
    jsonDocStore: DocStore;
    indexFile: Buffer;
  } {
    const jsonDocStore = this.docstore._docs;
    const indexFile = this.index;

    return {
      jsonDocStore:
        InMemoryAiTrainerFaissStore.RawJsonDocToDocStore(jsonDocStore),
      indexFile: indexFile.toBuffer(),
    };
  }
  public static async LoadFromInMemObjects(
    jsonDocStore: DocStore,
    rawIndex: Buffer
  ) {
    const [docstoreFiles, mapping] = jsonDocStore;
    const { IndexFlatL2 } = await this.importFaiss();
    const index = IndexFlatL2.fromBuffer(rawIndex);
    const docstore = new in_memory_js_1.SynchronousInMemoryDocstore(
      new Map(docstoreFiles)
    );
    return new InMemoryAiTrainerFaissStore(FaissEmbeddings, {
      docstore,
      index,
      mapping,
    });
  }

  public static async load(directory: string, embeddings: EmbeddingsInterface) {
    const [docstoreFiles, mapping] = JSON.parse(
      fs.readFileSync(path.join(directory, "docstore.json"), "utf8").toString()
    );

    const { IndexFlatL2 } = await this.importFaiss();

    const readIndex = fs.readFileSync(path.join(directory, "faiss.index"), {
      encoding: undefined,
    });
    const index = IndexFlatL2.fromBuffer(readIndex as Buffer);

    const docstore = new in_memory_js_1.SynchronousInMemoryDocstore(
      new Map(docstoreFiles)
    );
    return new InMemoryAiTrainerFaissStore(embeddings, {
      docstore,
      index,
      mapping,
    });
  }
  public static async SaveRawStoreToFile(
    jsonObject: Record<string, any>,
    indexFile: string | Buffer
  ): Promise<string> {
    await InMemoryAiTrainerFaissStore.TryToAccessThenCreateDirectory(
      InMemoryAiTrainerFaissStore._directoryToSaveTo
    );

    const filePath = InMemoryAiTrainerFaissStore.CreateNewFilePath();

    await InMemoryAiTrainerFaissStore.TryToAccessThenCreateDirectory(filePath);

    const jsonFilePath = path.join(filePath, "docstore.json");
    const indexFilePath = path.join(filePath, "faiss.index");

    fs.writeFileSync(jsonFilePath, JSON.stringify(jsonObject));
    fs.writeFileSync(indexFilePath, indexFile);

    return filePath;
  }

  public static async LoadFaissStoreFromFileAndRemoveFile(
    filePath: string
  ): Promise<InMemoryAiTrainerFaissStore> {
    const vectorStore =
      await InMemoryAiTrainerFaissStore.LoadFaissStoreFromFile(filePath);

    fs.unlinkSync(path.join(filePath, "docstore.json"));
    fs.unlinkSync(path.join(filePath, "faiss.index"));
    fs.rmdirSync(filePath);

    return vectorStore;
  }
  public static CreateFaissStore(): InMemoryAiTrainerFaissStore {
    const vectorStore = new InMemoryAiTrainerFaissStore(FaissEmbeddings, {});
    return vectorStore;
  }
  private static CreateNewFilePath(): string {
    return path.join(
      InMemoryAiTrainerFaissStore._directoryToSaveTo,
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
  ): Promise<InMemoryAiTrainerFaissStore> {
    const vectorStore = await InMemoryAiTrainerFaissStore.load(
      filePath,
      FaissEmbeddings
    );
    return vectorStore;
  }
  private static async TryToAccessThenCreateDirectory(directory: string) {
    try {
      fs.accessSync(directory);
    } catch {
      fs.mkdirSync(directory);
    }
  }
}
