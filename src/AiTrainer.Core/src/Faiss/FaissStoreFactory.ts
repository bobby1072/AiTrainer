import { FaissStore } from "@langchain/community/vectorstores/faiss";
import AppSettingsProvider from "../Utils/AppSettingsProvider";
import { AppSettingsKeys } from "../Utils/AppSettingsKeys";
import { FaissEmbeddings } from "./FaissEmbeddings";

process.env.OPENAI_API_KEY = AppSettingsProvider.TryGetValue(
  AppSettingsKeys.OpenAiApiKey
)!;

export default abstract class FaissStoreFactory {
  public static CreateFaissStore(): FaissStore {
    const vectorStore = new FaissStore(FaissEmbeddings, {});
    return vectorStore;
  }

  public static async LoadFaissStoreFromFile(
    filePath: string
  ): Promise<FaissStore> {
    const vectorStore = await FaissStore.load(filePath, FaissEmbeddings);
    return vectorStore;
  }
}
