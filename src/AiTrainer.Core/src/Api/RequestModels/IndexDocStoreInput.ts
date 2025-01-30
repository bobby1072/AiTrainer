import { z } from "zod";
import { DocStoreSchema } from "../../Models/DocStore";
import { CreateStoreInputSchema } from "./CreateStoreInput";

export const IndexDocStoreInputSchema = z.object({
  fileInput: z
    .any()
    .refine((x) => typeof x === "string" || x instanceof Buffer),
  docStore: DocStoreSchema,
  newDocuments: CreateStoreInputSchema,
});

export type IndexDocStoreInput = z.infer<typeof IndexDocStoreInputSchema>;
