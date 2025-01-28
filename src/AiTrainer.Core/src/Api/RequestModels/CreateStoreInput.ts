import { z } from "zod";

export const CreateStoreInputSchema = z.object({
  documents: z.array(z.string().min(1)).min(2),
});

export type CreateStoreInput = z.infer<typeof CreateStoreInputSchema>;
