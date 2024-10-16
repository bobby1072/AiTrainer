import { Application } from "express";
import ChunkingRoute from "./ChunkingRoute";
import compression from "compression";
import bodyParser from "body-parser";

export default abstract class ApplicationBuilder {
  public static AddMiddlewares(app: Application): void {
    app.use(compression());
    app.use(bodyParser.json());
    app.use(bodyParser.urlencoded({ extended: true }));
  }
  public static AddRoutes(app: Application): void {
    new ChunkingRoute(app).InvokeRoutes();
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
