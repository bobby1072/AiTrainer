import { FileCollection } from "./FileCollection";
import { FileDocumentPartial } from "./FileDocument";

export type FlatFileDocumentPartialCollection = {
  fileCollections: FileCollection[];
  fileDocuments: FileDocumentPartial[];
};
