import { FaissStore } from "@langchain/community/vectorstores/faiss";
import FaissStoreFactory from "./FaissStoreFactory";
import TestDocuments from "./TestDocuments";

export default class TestStoreService {
  private readonly _storeFactory;
  public Store?: FaissStore = undefined;
  public constructor(storeFactory: FaissStoreFactory) {
    this._storeFactory = storeFactory;
  }
  public async BuildStoreAndLoadDocuments() {
    this.Store = this._storeFactory.CreateFaissStore();
    await this.Store!.addDocuments(TestDocuments.Documents, {
      ids: Object.keys(TestDocuments.Documents),
    });
    return;
  }
}
