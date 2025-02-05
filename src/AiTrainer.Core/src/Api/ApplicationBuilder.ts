import { Application } from "express";
import ChunkingRouter from "./Routers/ChunkingRouter";
import compression from "compression";
import bodyParser from "body-parser";
import ExceptionHandlingMiddleware from "./Middleware/ExceptionHandlingMiddleware";
import ApiKeyHeaderMiddleware from "./Middleware/ApiKeyHeaderMiddleware";
import AppSettingsProvider from "../Utils/AppSettingsProvider";
import { AppSettingsKeys } from "../Utils/AppSettingsKeys";
import HealthRouter from "./Routers/HealthRouter";
import FaissRouter from "./Routers/FaissRouter";

export default abstract class ApplicationBuilder {
  public static AddSystemMiddlewares(app: Application): void {
    app.use(compression());
    app.use(bodyParser.json());
    app.use(bodyParser.urlencoded({ extended: true }));
  }

  public static AddDomainMiddleware(app: Application): void {
    app.use(ExceptionHandlingMiddleware.InvokeMiddleware());
    app.use(ApiKeyHeaderMiddleware.InvokeMiddleware());
  }
  public static AddRoutes(app: Application): void {
    ChunkingRouter.InvokeRoutes(app);
    HealthRouter.InvokeRoutes(app);
    FaissRouter.InvokeRoutes(app);
  }
  public static Listen(app: Application): void {
    const port = Number(AppSettingsProvider.AllAppSettings.ExpressPort) || 5000;
    app.listen(port, () => {
      console.log("\n\nServer running on port: ", port);
    });
  }
}
