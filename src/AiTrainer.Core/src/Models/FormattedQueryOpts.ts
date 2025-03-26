import { z } from "zod";

export const FormattedChatQueryOptsSchema = z.object({
  systemPromptMessage: z.string().min(1).max(5000),
  humanPromptMessage: z.string().min(1).max(10000),
  extraInput: z
    .record(z.string(), z.string().min(1).max(10000))
    .optional()
    .nullable(),
});

export type FormattedChatQueryOpts = z.infer<
  typeof FormattedChatQueryOptsSchema
>;
