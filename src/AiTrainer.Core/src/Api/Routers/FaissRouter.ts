import { Application, Request, Response } from "express";
import { CreateStoreInputSchema } from "../RequestModels/CreateStoreInput";
import AiTrainerFaissStore from "../../Faiss/AiTrainerFaissStore";
import { SuccessfulRouteResponse } from "../ResponseModels/RouteResponse";
import { DocStore } from "../../Models/DocStore";
import { IndexFlatL2 } from "faiss-node";

export default abstract class FaissRouter {
  private static CreateNewStore(app: Application) {
    return app.post(
      `/api/${FaissRouter.name.toLowerCase()}/createstore`,
      async (req: Request, resp: Response) => {
        const parsedBody = CreateStoreInputSchema.safeParse(req.body);
        const documents = parsedBody.data?.documents;

        if (!documents || !parsedBody.success) {
          throw new Error("Documents are required");
        }

        const faissStore = AiTrainerFaissStore.CreateFaissStore();

        await faissStore.LoadDocumentsIntoStore(
          documents.map((x) => ({
            pageContent: x,
            metadata: {},
          }))
        );

        resp.status(200).json({
          isSuccess: true,
          data: faissStore.GetSaveItemsFromStore(),
        } as SuccessfulRouteResponse<{
          jsonDocStore: DocStore;
          indexFile: IndexFlatL2;
        }>);
      }
    );
  }

  public static InvokeRoutes(app: Application): void {
    FaissRouter.CreateNewStore(app);
  }
}
