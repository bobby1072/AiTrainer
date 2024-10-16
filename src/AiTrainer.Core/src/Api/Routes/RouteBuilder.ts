import { Application } from "express";
import ChunkingRoute from "./ChunkingRoute";

export default abstract class ApplicationBuilder {
  public static AddRoutes(app: Application): void {
    new ChunkingRoute(app).InvokeRoutes();
  }
}
