import { Application } from "express";
import ChunkingRouter from "./Routers/ChunkingRouter";
import compression from "compression";
import bodyParser from "body-parser";
import ApiKeyHeaderMiddleware from "./Middleware/ApiKeyHeaderMiddleware";
import { ApplicationSettings } from "../Utils/AppSettingsProvider";
import HealthRouter from "./Routers/HealthRouter";
import FaissRouter from "./Routers/FaissRouter";
import OpenAIRouter from "./Routers/OpenAIRouter";

export default abstract class ApplicationBuilder {
  public static AddSystemMiddlewares(app: Application): void {
    app.use(compression());
    app.use(bodyParser.json());
    app.use(bodyParser.urlencoded({ extended: true }));
  }

  public static AddDomainMiddlewares(app: Application): void {
    app.use(ApiKeyHeaderMiddleware.InvokeMiddleware());
  }
  public static AddRoutes(app: Application): void {
    ChunkingRouter.InvokeRoutes(app);
    HealthRouter.InvokeRoutes(app);
    FaissRouter.InvokeRoutes(app);
    OpenAIRouter.InvokeRoutes(app);
  }
  public static Listen(app: Application): void {
    const port = Number(ApplicationSettings.AllAppSettings.ExpressPort) || 5000;
    app.listen(port, () => {
      console.log("\n\nServer running on port: ", port);
    });
  }
}
