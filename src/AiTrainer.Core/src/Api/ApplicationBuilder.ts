import { Application } from "express";
import ChunkingRouter from "./Routers/ChunkingRouter";
import compression from "compression";
import bodyParser from "body-parser";
import ExceptionHandlingMiddleware from "./Middleware/ExceptionHandlingMiddleware";
import ApiKeyHeaderMiddleware from "./Middleware/ApiKeyHeaderMiddleware";

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
  }
  public static Listen(
    app: Application,
    port?: number,
    hostname?: string
  ): void {
    port = port ?? 5000;
    app.listen(port, hostname ?? "0.0.0.0", () => {
      console.log(`\n\nServer running on port: ${port}\n\n`);
    });
  }
}
