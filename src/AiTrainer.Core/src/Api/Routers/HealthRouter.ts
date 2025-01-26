import { Application } from "express";
import { SuccessfulRouteResponse } from "../ResponseModels/RouteResponse";

export default abstract class HealthRouter {
  private static HealthCheck(app: Application) {
    return app.get(
      `/api/${HealthRouter.name.toLowerCase()}/`,
      async (req, res) => {
        res.status(200).json({
          data: { message: "Healthy" },
          isSuccess: true,
        } as SuccessfulRouteResponse<{ message: string }>);
      }
    );
  }

  public static InvokeRoutes(app: Application): void {
    this.HealthCheck(app);
  }
}
