import { OpenAIEmbeddings } from "@langchain/openai";
import { AppSettingsKeys } from "../Utils/AppSettingsKeys";
import AppSettingsProvider from "../Utils/AppSettingsProvider";

process.env.OPENAI_API_KEY = AppSettingsProvider.TryGetValue(
  AppSettingsKeys.OpenAiApiKey
)!;

export const FaissEmbeddings = new OpenAIEmbeddings({
  model: "text-embedding-3-small",
});
