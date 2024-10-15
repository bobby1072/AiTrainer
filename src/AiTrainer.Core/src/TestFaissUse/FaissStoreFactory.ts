import { OpenAIEmbeddings } from "@langchain/openai";
import { FaissStore } from "@langchain/community/vectorstores/faiss";

process.env.OPENAI_API_KEY =
  "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";

export default class FaissStoreFactory {
  public Embeddings: OpenAIEmbeddings = new OpenAIEmbeddings({
    model: "text-embedding-3-small",
  });
  public CreateFaissStore(): FaissStore {
    const vectorStore = new FaissStore(this.Embeddings, {});
    return vectorStore;
  }
}
