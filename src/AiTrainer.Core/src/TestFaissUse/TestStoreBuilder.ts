import { FaissStore } from "@langchain/community/vectorstores/faiss";
import FaissStoreFactory from "../Faiss/FaissStoreFactory";
import TestDocuments from "./TestDocuments";
export default class TestStoreService {
  private static readonly _directoryToSaveTo: string = "./filestore/";
  public Store?: FaissStore = undefined;
  public async BuildStoreAndLoadDocuments() {
    this.Store = FaissStoreFactory.CreateFaissStore();
    await this.Store!.addDocuments(TestDocuments.Documents, {
      ids: Object.keys(TestDocuments.Documents),
    });
    return;
  }
  public async SaveStore() {
    await this.Store?.save(TestStoreService._directoryToSaveTo);
  }

  // public async LoadStoreFromFile() {
  //   this.Store = await FaissStore.load(
  //     TestStoreService._directoryToSaveTo,
  //     FaissStoreFactory.Embeddings
  //   );
  // }
}
