import { OpenAIEmbeddings } from "@langchain/openai";
import { FaissStore } from "@langchain/community/vectorstores/faiss";
import AppSettingsProvider from "../Utils/AppSettingsProvider";
import { AppSettingsKeys } from "../Utils/AppSettingsKeys";

process.env.OPENAI_API_KEY = AppSettingsProvider.TryGetValue(
  AppSettingsKeys.OpenAiApiKey
)!;

export default abstract class FaissStoreFactory {
  private static readonly _embeddings = new OpenAIEmbeddings({
    model: "text-embedding-3-small",
  });
  public static CreateFaissStore(): FaissStore {
    const vectorStore = new FaissStore(FaissStoreFactory._embeddings, {});
    return vectorStore;
  }

  public static async LoadFaissStoreFromFile(
    filePath: string
  ): Promise<FaissStore> {
    const vectorStore = await FaissStore.load(
      filePath,
      FaissStoreFactory._embeddings
    );
    return vectorStore;
  }
}
