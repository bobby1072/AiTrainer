import { z } from "zod";

export const DocumentToChunkSchema = z.object({
  documentText: z.string().min(1),
});

export type IDocumentToChunk = z.infer<typeof DocumentToChunkSchema>;
