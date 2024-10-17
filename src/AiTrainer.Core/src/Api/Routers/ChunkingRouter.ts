import Chunker from "../../DocumentChunker/Chunker";
import ApiException from "../../Exceptions/ApiException";
import ExceptionConstants from "../../Exceptions/ExceptionConstants";
import { DocumentToChunkSchema } from "../RequestModels/IDocumentToChunk";
import { IChunkedDocument } from "../ResponseModels/IChunkedDocument";
import { ISuccessfulRouteResponse } from "../ResponseModels/IRouteResponse";
import { Application, Request, Response } from "express";

export default abstract class ChunkingRouter {
  private static ChunkDocument(app: Application) {
    return app.post(
      `/api/${ChunkingRouter.name}`,
      async (req: Request, res: Response) => {
        const safeParsedDocumentToChunk = DocumentToChunkSchema.safeParse(
          req.body
        );
        const actualText = safeParsedDocumentToChunk.data?.documentText;
        if (!actualText) {
          throw new ApiException(ExceptionConstants.ChunkerError);
        }
        const chunks = await Chunker.Chunk(actualText);

        res.status(200).json({
          data: { documentChunks: chunks },
          isSuccess: true,
        } as ISuccessfulRouteResponse<IChunkedDocument>);
      }
    );
  }

  public static InvokeRoutes(app: Application): void {
    this.ChunkDocument(app);
  }
}
