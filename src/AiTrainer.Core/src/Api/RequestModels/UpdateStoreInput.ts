import { z } from "zod";
import { DocStoreSchema } from "../../Models/DocStore";
import { CreateStoreInputSchema } from "./CreateStoreInput";

export const UpdateStoreInputSchema = z.object({
  fileInput: z.any().refine((x) => {
    return x instanceof Buffer;
  }),
  jsonDocStore: DocStoreSchema,
  newDocuments: CreateStoreInputSchema,
});

export type UpdateStoreInput = z.infer<typeof UpdateStoreInputSchema>;
