import Chunker from "../../DocumentChunker/Chunker";
import ApiException from "../../Exceptions/ApiException";
import ExceptionConstants from "../../Exceptions/ExceptionConstants";
import { DocumentToChunkInputSchema } from "../RequestModels/DocumentToChunkInput";
import { ChunkedDocument } from "../ResponseModels/ChunkedDocument";
import { SuccessfulRouteResponse } from "../ResponseModels/RouteResponse";
import { Application, Request, Response } from "express";

export default abstract class ChunkingRouter {
  private static ChunkDocument(app: Application) {
    return app.post(
      `/api/${ChunkingRouter.name.toLowerCase()}/chunkdocument`,
      async (req: Request, res: Response) => {
        const safeParsedDocumentToChunk = DocumentToChunkInputSchema.safeParse(
          req.body
        );
        const actualText = safeParsedDocumentToChunk.data?.documentText;
        if (!actualText) {
          throw new ApiException(ExceptionConstants.DocumentTextIsRequired);
        }
        const chunks = await Chunker.Chunk(actualText);

        res.status(200).json({
          data: { documentChunks: chunks },
          isSuccess: true,
        } as SuccessfulRouteResponse<ChunkedDocument>);
      }
    );
  }

  public static InvokeRoutes(app: Application): void {
    this.ChunkDocument(app);
  }
}
