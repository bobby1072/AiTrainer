import { Application, Request, Response } from "express";
import { CreateStoreInputSchema } from "../RequestModels/CreateStoreInput";
import { SuccessfulRouteResponse } from "../ResponseModels/RouteResponse";
import { DocStore } from "../../Models/DocStore";
import multer from "multer";
import { UpdateStoreInputSchema } from "../RequestModels/UpdateStoreInput";
import ApiException from "../../Exceptions/ApiException";
import AiTrainerFaissStoreApiService from "../../Faiss/AiTrainerFaissStoreApiService";
import { SimilaritySearchInputSchema } from "../RequestModels/SimilaritySearchInput";
import { DocumentInterface } from "@langchain/core/documents";

export default abstract class FaissRouter {
  private static readonly upload = multer({ storage: multer.memoryStorage() });
  private static UpdateStore(app: Application) {
    return app.post(
      `/api/${FaissRouter.name.toLowerCase()}/updatestore`,
      FaissRouter.upload.single("file"),
      async (req: Request, resp: Response) => {
        const metadata = JSON.parse(req.body.metadata);
        const fileBuffer = req.file?.buffer;
        const safeInput = UpdateStoreInputSchema.safeParse({
          fileInput: fileBuffer,
          jsonDocStore: metadata.docStore,
          newDocuments: metadata.newDocuments,
        });

        if (!safeInput.success) {
          throw new ApiException("Invalid input");
        }
        const result = await AiTrainerFaissStoreApiService.UpdateStore(
          safeInput.data
        );

        resp.status(200).json({
          isSuccess: true,
          data: result,
        } as SuccessfulRouteResponse<{
          jsonDocStore: DocStore;
          indexFile: string;
        }>);
      }
    );
  }
  private static SimilaritySearch(app: Application) {
    return app.post(
      `/api/${FaissRouter.name.toLowerCase()}/similaritysearch`,
      async (req: Request, resp: Response) => {
        const metadata = JSON.parse(req.body.metadata);
        const fileBuffer = req.file?.buffer;
        const safeInput = SimilaritySearchInputSchema.safeParse({
          fileInput: fileBuffer,
          jsonDocStore: metadata.docStore,
          question: metadata.question,
          documentsToReturn: metadata.documentsToReturn,
        });

        if (!safeInput.success) {
          throw new ApiException("Invalid input");
        }
        const result = await AiTrainerFaissStoreApiService.SimSearch(
          safeInput.data
        );

        resp.status(200).json({
          isSuccess: true,
          data: result,
        } as SuccessfulRouteResponse<DocumentInterface<Record<string, any>>[]>);
      }
    );
  }
  private static CreateNewStore(app: Application) {
    return app.post(
      `/api/${FaissRouter.name.toLowerCase()}/createstore`,
      async (req: Request, resp: Response) => {
        const parsedBody = CreateStoreInputSchema.safeParse(req.body);
        const documents = parsedBody.data?.documents;

        if (!documents || !parsedBody.success) {
          throw new ApiException("Documents are required");
        }

        const result = await AiTrainerFaissStoreApiService.CreateStore(
          documents
        );

        resp.status(200).json({
          isSuccess: true,
          data: result,
        } as SuccessfulRouteResponse<{
          jsonDocStore: DocStore;
          indexFile: string;
        }>);
      }
    );
  }

  public static InvokeRoutes(app: Application): void {
    FaissRouter.CreateNewStore(app);
    FaissRouter.UpdateStore(app);
  }
}
