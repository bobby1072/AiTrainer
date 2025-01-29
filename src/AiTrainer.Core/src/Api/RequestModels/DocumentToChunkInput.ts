import { z } from "zod";

export const DocumentToChunkInputSchema = z.object({
  documentText: z.string().min(1),
});

export type DocumentToChunkInput = z.infer<typeof DocumentToChunkInputSchema>;
