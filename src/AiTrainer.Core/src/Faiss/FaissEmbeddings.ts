import { OpenAIEmbeddings } from "@langchain/openai";
import AppSettingsProvider from "../Utils/AppSettingsProvider";

process.env.OPENAI_API_KEY = AppSettingsProvider.AllAppSettings.OpenAiApiKey;
export const FaissEmbeddings = new OpenAIEmbeddings({
  model: "text-embedding-3-small",
});
