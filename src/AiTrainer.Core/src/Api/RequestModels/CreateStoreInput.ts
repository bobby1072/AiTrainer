import { z } from "zod";

export const CreateStoreInputSchema = z.object({
  documents: z
    .array(
      z.object({
        pageContent: z.string().min(1),
        metadata: z.record(z.string(), z.string()),
      })
    )
    .min(2),
});

export type CreateStoreInput = z.infer<typeof CreateStoreInputSchema>;
