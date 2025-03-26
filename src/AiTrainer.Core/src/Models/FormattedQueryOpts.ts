export type FormattedChatQueryOpts = {
  systemPromptMessage: string;
  humanPromptMessage: string;
  extraInput?: Record<string, string> | null;
};
