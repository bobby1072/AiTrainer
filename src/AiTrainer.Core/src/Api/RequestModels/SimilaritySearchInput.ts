import { z } from "zod";
import { DocStoreSchema } from "../../Models/DocStore";

export const SimilaritySearchInputSchema = z.object({
  fileInput: z.any().refine((x) => {
    return x instanceof Buffer;
  }),
  jsonDocStore: DocStoreSchema,
  question: z.string().min(1),
  documentsToReturn: z.number().int().positive().max(30).min(1),
});

export type SimilaritySearchInput = z.infer<typeof SimilaritySearchInputSchema>;
