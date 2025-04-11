import { z } from "zod";
import { DocStoreSchema } from "../../Models/DocStore";

export const RemoveDocumentsInputSchema = z.object({
  fileInput: z.any().refine((x) => {
    return x instanceof Buffer;
  }),
  jsonDocStore: DocStoreSchema,
  documentIdsToRemove: z.array(z.string().uuid()).min(1),
});

export type RemoveDocumentsInput = z.infer<typeof RemoveDocumentsInputSchema>;
