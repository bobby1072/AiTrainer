import { FileCollection } from "./FileCollection";
import { FileDocumentPartial } from "./FileDocument";

export type FlatFileDocumentPartialCollection = {
  self: FileCollection;
  fileCollections: FileCollection[];
  fileDocuments: FileDocumentPartial[];
};
