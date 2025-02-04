import { SimilaritySearchInput } from "../Api/RequestModels/SimilaritySearchInput";
import { UpdateStoreInput } from "../Api/RequestModels/UpdateStoreInput";
import AiTrainerFaissStore from "./AiTrainerFaissStore";

export default abstract class AiTrainerFaissStoreApiService {
  public static async SimSearch(data: SimilaritySearchInput) {
    const faissStoreFilePath = await AiTrainerFaissStore.SaveRawStoreToFile(
      data.jsonDocStore,
      data.fileInput
    );

    const faissStore =
      await AiTrainerFaissStore.LoadFaissStoreFromFileAndRemoveFile(
        faissStoreFilePath
      );

    const result = await faissStore.similaritySearch(
      data.question,
      data.documentsToReturn
    );

    return result;
  }
  public static async CreateStore(documents: string[]) {
    const faissStore = AiTrainerFaissStore.CreateFaissStore();

    await faissStore.LoadDocumentsIntoStore(
      documents.map((x) => ({
        pageContent: x,
        metadata: {},
      }))
    );

    const storeItems = faissStore.GetSaveItemsFromStore();
    return {
      jsonDocStore: storeItems.jsonDocStore,
      indexFile: storeItems.indexFile.toString("base64"),
    };
  }

  public static async UpdateStore(data: UpdateStoreInput) {
    const faissStoreFilePath = await AiTrainerFaissStore.SaveRawStoreToFile(
      data.jsonDocStore,
      data.fileInput
    );

    const faissStore =
      await AiTrainerFaissStore.LoadFaissStoreFromFileAndRemoveFile(
        faissStoreFilePath
      );

    await faissStore.LoadDocumentsIntoStore(
      data.newDocuments.documents.map((x) => ({
        pageContent: x,
        metadata: {},
      }))
    );

    const storeItems = faissStore.GetSaveItemsFromStore();
    return {
      jsonDocStore: storeItems.jsonDocStore,
      indexFile: storeItems.indexFile.toString("base64"),
    };
  }
}
