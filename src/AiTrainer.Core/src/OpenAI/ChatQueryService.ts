import { AIMessageChunk } from "@langchain/core/messages";
import { Chat } from "./Chat";
import { ChatPromptTemplate } from "@langchain/core/prompts";
import { FormattedChatQueryOpts } from "../Models/FormattedQueryOpts";

export default abstract class ChatQueryService {
  public static async SimpleQuery(input: string): Promise<AIMessageChunk> {
    const result = await Chat.invoke(input);
    return result;
  }

  public static async FormattedQuery({
    humanPromptMessage,
    systemPromptMessage,
    extraInput = {},
  }: FormattedChatQueryOpts): Promise<AIMessageChunk> {
    const prompt = ChatPromptTemplate.fromMessages([
      ["system", systemPromptMessage],
      ["human", "{input}"],
    ]);

    const pipe = prompt.pipe(Chat);

    const result = await pipe.invoke({
      ...extraInput,
      input: humanPromptMessage,
    });

    return result;
  }
}
