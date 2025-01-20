import { OpenAIEmbeddings } from "@langchain/openai";

export const FaissEmbeddings = new OpenAIEmbeddings({
  model: "text-embedding-3-small",
});
