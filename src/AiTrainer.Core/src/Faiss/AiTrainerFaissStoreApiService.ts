import { RemoveDocumentsInput } from "../Api/RequestModels/RemoveDocumentsInput";
import { SimilaritySearchInput } from "../Api/RequestModels/SimilaritySearchInput";
import { UpdateStoreInput } from "../Api/RequestModels/UpdateStoreInput";
import { DocStore } from "../Models/DocStore";
import InMemoryAiTrainerFaissStore from "./InMemoryAiTrainerFaissStore";

export default abstract class AiTrainerFaissStoreApiService {
  public static async SimSearch(data: SimilaritySearchInput) {
    const faissStoreFilePath =
      await InMemoryAiTrainerFaissStore.SaveRawStoreToFile(
        data.jsonDocStore,
        data.fileInput
      );

    const faissStore =
      await InMemoryAiTrainerFaissStore.LoadFaissStoreFromFileAndRemoveFile(
        faissStoreFilePath
      );

    const result = await faissStore.similaritySearch(
      data.question,
      data.documentsToReturn
    );

    return result;
  }
  public static async CreateStore(
    documents: { pageContent: string; metadata: Record<string, string> }[]
  ) {
    const faissStore = InMemoryAiTrainerFaissStore.CreateFaissStore();

    await faissStore.LoadDocumentsIntoStore(documents);

    const storeItems = faissStore.GetSaveItemsFromStore();
    return {
      jsonDocStore: storeItems.jsonDocStore,
      indexFile: storeItems.indexFile.toString("base64"),
    };
  }

  public static async UpdateStore(data: UpdateStoreInput) {
    const faissStore = await InMemoryAiTrainerFaissStore.LoadFromInMemObjects(
      data.jsonDocStore as DocStore,
      data.fileInput
    );

    await faissStore.LoadDocumentsIntoStore(data.newDocuments.documents);

    const storeItems = faissStore.GetSaveItemsFromStore();
    return {
      jsonDocStore: storeItems.jsonDocStore,
      indexFile: storeItems.indexFile.toString("base64"),
    };
  }

  public static async RemoveDocumentsFromStore(data: RemoveDocumentsInput) {
    const faissStore = await InMemoryAiTrainerFaissStore.LoadFromInMemObjects(
      data.jsonDocStore as DocStore,
      data.fileInput
    );

    await faissStore.RemoveDocumentsFromStore(data.documentIdsToRemove);

    const storeItems = faissStore.GetSaveItemsFromStore();
    return {
      jsonDocStore: storeItems.jsonDocStore,
      indexFile: storeItems.indexFile.toString("base64"),
    };
  }
}
