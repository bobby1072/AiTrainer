import { Application } from "express";
import { ISuccessfulRouteResponse } from "../ResponseModels/IRouteResponse";

export default abstract class HealthRouter {
  private static HealthCheck(app: Application) {
    return app.get(
      `/api/${HealthRouter.name.toLowerCase()}/`,
      async (req, res) => {
        res.status(200).json({
          data: { message: "Healthy" },
          isSuccess: true,
        } as ISuccessfulRouteResponse<{ message: string }>);
      }
    );
  }

  public static InvokeRoutes(app: Application): void {
    this.HealthCheck(app);
  }
}
