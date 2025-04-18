import { z } from "zod";
import { ZodGuid } from "../../Utils/ZodGuid";

export const DocumentToChunkInputSchema = z.object({
  documentsToChunk: z
    .array(
      z.object({
        documentText: z.string().min(1),
        fileDocumentId: ZodGuid(),
        metadata: z
          .record(z.string().min(1), z.string().nullable().optional())
          .optional()
          .nullable(),
      })
    )
    .min(1),
});

export type DocumentToChunkInput = z.infer<typeof DocumentToChunkInputSchema>;
