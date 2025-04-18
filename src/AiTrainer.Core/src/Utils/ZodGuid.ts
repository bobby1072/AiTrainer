import { z } from "zod";
import Guid from "./Guid";

export const ZodGuid = () => z.string().transform((x) => Guid.Parse(x));
