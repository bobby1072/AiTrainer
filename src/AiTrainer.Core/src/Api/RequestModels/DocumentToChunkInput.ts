import { z } from "zod";

export const DocumentToChunkInputSchema = z.object({
  documentsToChunk: z
    .array(
      z.object({
        documentText: z.string().min(1),
        fileDocumentId: z.string().uuid(),
        metadata: z
          .record(z.string().min(1), z.string().nullable().optional())
          .optional()
          .nullable(),
      })
    )
    .min(1),
});

export type DocumentToChunkInput = z.infer<typeof DocumentToChunkInputSchema>;
