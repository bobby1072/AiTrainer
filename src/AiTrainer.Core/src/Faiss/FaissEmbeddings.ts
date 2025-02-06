import { OpenAIEmbeddings } from "@langchain/openai";
import { ApplicationSettings } from "../Utils/AppSettingsProvider";

process.env.OPENAI_API_KEY = ApplicationSettings.AllAppSettings.OpenAiApiKey;
export const FaissEmbeddings = new OpenAIEmbeddings({
  model: "text-embedding-3-small",
});
