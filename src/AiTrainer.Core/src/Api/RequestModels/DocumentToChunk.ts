import { z } from "zod";

export const DocumentToChunkSchema = z.object({
  documentText: z.string().min(1),
});

export type DocumentToChunk = z.infer<typeof DocumentToChunkSchema>;
