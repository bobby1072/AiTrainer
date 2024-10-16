import Chunker from "../../DocumentChunker/Chunker";
import ApiException from "../../Exceptions/ApiException";
import { DocumentToChunkSchema } from "../RequestModels/IDocumentToChunk";
import { IChunkedDocument } from "../ResponseModels/IChunkedDocument";
import BaseRoute from "./BaseRoute";
import { Request, Response } from "express";

export default class ChunkingRoute extends BaseRoute {
  private ChunkDocument() {
    return this._app.post(
      `/api/${ChunkingRoute.name}`,
      async (req: Request, res: Response) => {
        const safeParsedDocumentToChunk = DocumentToChunkSchema.safeParse(
          req.body
        );
        const actualText = safeParsedDocumentToChunk.data?.documentText;
        if (!actualText) {
          throw new ApiException(400, "Document text is required");
        }
        const chunks = await Chunker.Chunk(actualText);

        res.status(200).json({ documentChunks: chunks } as IChunkedDocument);
      }
    );
  }
  public InvokeRoutes(): void {
    this.ChunkDocument();
  }
}
