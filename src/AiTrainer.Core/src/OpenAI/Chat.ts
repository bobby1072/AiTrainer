import { ChatOpenAI } from "@langchain/openai";
import { ApplicationSettings } from "../Utils/AppSettingsProvider";

process.env.OPENAI_API_KEY = ApplicationSettings.AllAppSettings.OpenAiApiKey;
export const Chat = new ChatOpenAI({
  model: "gpt-4o",
  temperature: 0,
  maxTokens: undefined,
  timeout: 210,
  maxRetries: 2,
  apiKey: ApplicationSettings.AllAppSettings.OpenAiApiKey,
  openAIApiKey: ApplicationSettings.AllAppSettings.OpenAiApiKey,
});
