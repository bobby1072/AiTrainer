import Chunker from "../../DocumentChunker/Chunker";
import ApiException from "../../Exceptions/ApiException";
import ExceptionConstants from "../../Exceptions/ExceptionConstants";
import AiTrainerExpressExceptionHandling from "../Helpers/AiTrainerExpressExceptionHandling";
import { DocumentToChunkInputSchema } from "../RequestModels/DocumentToChunkInput";
import { ChunkedDocument } from "../ResponseModels/ChunkedDocument";
import { SuccessfulRouteResponse } from "../ResponseModels/RouteResponse";
import { Application, Request, Response } from "express";

export default abstract class ChunkingRouter {
  private static ChunkDocument(app: Application) {
    return app.post(
      `/api/${ChunkingRouter.name.toLowerCase()}/chunkdocument`,
      async (req: Request, res: Response) => {
        return await AiTrainerExpressExceptionHandling.HandleAsync(async () => {
          const safeParsedDocumentToChunk =
            DocumentToChunkInputSchema.safeParse(req.body);
          if (!safeParsedDocumentToChunk.success) {
            throw new ApiException(ExceptionConstants.DocumentTextIsRequired);
          }
          const actualText = safeParsedDocumentToChunk.data;
          const chunkingInput = {
            documentsToChunk: actualText.documentsToChunk.map((x) => ({
              ...x,
              fileDocumentId: x.fileDocumentId.ToString(),
            })),
          };

          const chunks =
            safeParsedDocumentToChunk.data.chunkingType === "recursive"
              ? await Chunker.RecursiveChunk(chunkingInput)
              : await Chunker.SemanticChunk(chunkingInput);

          res.status(200).json({
            data: { documentChunks: chunks },
            isSuccess: true,
          } as SuccessfulRouteResponse<ChunkedDocument>);
        }, res);
      }
    );
  }

  public static InvokeRoutes(app: Application): void {
    ChunkingRouter.ChunkDocument(app);
  }
}
