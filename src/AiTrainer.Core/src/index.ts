import express, { Application } from "express";
import ApplicationBuilder from "./Api/ApplicationBuilder";

abstract class Program {
  private static _app: Application = express();
  public static async Main(): Promise<void> {
    ApplicationBuilder.AddSystemMiddlewares(Program._app);
    ApplicationBuilder.AddDomainMiddlewares(Program._app);
    ApplicationBuilder.AddRoutes(Program._app);
    ApplicationBuilder.Listen(Program._app);
  }
}
Program.Main();
