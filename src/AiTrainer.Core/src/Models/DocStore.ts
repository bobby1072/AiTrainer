import { z } from "zod";

export const DocStorePageInfoSchema = z.object({
  pageContent: z.string().nullable().optional(),
  metadata: z.record(z.string(), z.string()),
});

export const DocStoreSchema = z.tuple([
  z.array(z.tuple([z.string(), DocStorePageInfoSchema])),
  z.record(z.string(), z.string()),
]);

export type DocStorePageInfo = z.infer<typeof DocStorePageInfoSchema>;

export type DocStore = z.infer<typeof DocStoreSchema>;
