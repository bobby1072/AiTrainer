import { Document } from "@langchain/core/documents";

export default abstract class TestDocuments {
  public static readonly Documents: Document[] = [
    {
      pageContent: "The powerhouse of the cell is the mitochondria",
      metadata: { source: "https://example.com" },
    },
    {
      pageContent: "Buildings are made out of brick",
      metadata: { source: "https://example.com" },
    },
    {
      pageContent: "Mitochondria are made out of lipids",
      metadata: { source: "https://example.com" },
    },
    {
      pageContent: "The 2024 Olympics are in Paris",
      metadata: { source: "https://example.com" },
    },
  ];
}
