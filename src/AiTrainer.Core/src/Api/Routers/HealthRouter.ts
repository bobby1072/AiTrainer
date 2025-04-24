import { Application, Request, Response } from "express";
import { SuccessfulRouteResponse } from "../ResponseModels/RouteResponse";
import AiTrainerExpressExceptionHandling from "../Helpers/AiTrainerExpressExceptionHandling";

export default abstract class HealthRouter {
  private static HealthCheck(app: Application) {
    return app.get(
      `/api/${HealthRouter.name.toLowerCase()}/`,
      async (req: Request, res: Response) => {
        return await AiTrainerExpressExceptionHandling.HandleAsync(async () => {
          res.status(200).json({
            data: { message: "Healthy" },
            isSuccess: true,
          } as SuccessfulRouteResponse<{ message: string }>);
        }, res);
      }
    );
  }

  public static InvokeRoutes(app: Application): void {
    HealthRouter.HealthCheck(app);
  }
}
