import { Application, Request, Response } from "express";
import { CreateStoreInputSchema } from "../RequestModels/CreateStoreInput";
import AiTrainerFaissStore from "../../Faiss/AiTrainerFaissStore";
import { SuccessfulRouteResponse } from "../ResponseModels/RouteResponse";
import { DocStore } from "../../Models/DocStore";
import multer from "multer";
import { UpdateStoreInputSchema } from "../RequestModels/UpdateStoreInput";
import ApiException from "../../Exceptions/ApiException";

export default abstract class FaissRouter {
  private static readonly upload = multer({ storage: multer.memoryStorage() });
  private static UpdateStore(app: Application) {
    return app.post(
      `/api/${FaissRouter.name.toLowerCase()}/updatestore`,
      FaissRouter.upload.single("file"),
      async (req: Request, resp: Response) => {
        const metadata = JSON.parse(req.body.metadata); // Parse metadata JSON
        const fileBuffer = req.file?.buffer;
        const safeInput = UpdateStoreInputSchema.safeParse({
          fileInput: fileBuffer,
          jsonDocStore: metadata.docStore,
          newDocuments: metadata.newDocuments,
        });

        if (!safeInput.success) {
          throw new ApiException("Invalid input");
        }

        const faissStoreFilePath = await AiTrainerFaissStore.SaveRawStoreToFile(
          safeInput.data.jsonDocStore,
          safeInput.data.fileInput
        );

        const faissStore =
          await AiTrainerFaissStore.LoadFaissStoreFromFileAndRemoveFile(
            faissStoreFilePath
          );

        await faissStore.LoadDocumentsIntoStore(
          safeInput.data.newDocuments.documents.map((x) => ({
            pageContent: x,
            metadata: {},
          }))
        );

        const storeItems = faissStore.GetSaveItemsFromStore();

        resp.status(200).json({
          isSuccess: true,
          data: {
            jsonDocStore: storeItems.jsonDocStore,
            indexFile: storeItems.indexFile.toString("base64"),
          },
        } as SuccessfulRouteResponse<{
          jsonDocStore: DocStore;
          indexFile: string;
        }>);
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

        const faissStore = AiTrainerFaissStore.CreateFaissStore();

        await faissStore.LoadDocumentsIntoStore(
          documents.map((x) => ({
            pageContent: x,
            metadata: {},
          }))
        );

        const storeItems = faissStore.GetSaveItemsFromStore();

        resp.status(200).json({
          isSuccess: true,
          data: {
            jsonDocStore: storeItems.jsonDocStore,
            indexFile: storeItems.indexFile.toString("base64"),
          },
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
