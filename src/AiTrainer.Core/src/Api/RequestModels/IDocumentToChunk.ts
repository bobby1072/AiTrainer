import { z } from "zod";

export const DocumentToChunkSchema = z.object({
  documentText: z.string(),
});

export type IDocumentToChunk = z.infer<typeof DocumentToChunkSchema>;
