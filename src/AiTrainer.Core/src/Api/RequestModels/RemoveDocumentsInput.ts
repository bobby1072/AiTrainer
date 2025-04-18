import { z } from "zod";
import { DocStoreSchema } from "../../Models/DocStore";
import { ZodGuid } from "../../Utils/ZodGuid";

export const RemoveDocumentsInputSchema = z.object({
  fileInput: z.any().refine((x) => {
    return x instanceof Buffer;
  }),
  jsonDocStore: DocStoreSchema,
  documentIdsToRemove: z.array(ZodGuid()).min(1),
});

export type RemoveDocumentsInput = z.infer<typeof RemoveDocumentsInputSchema>;
