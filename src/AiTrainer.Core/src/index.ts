import express, { Application } from "express";
import ApplicationBuilder from "./Api/ApplicationBuilder";

abstract class Program {
  private static _app: Application = express();
  public static async Main(): Promise<void> {
    // const store = FaissStoreFactory.CreateFaissStore();

    // await FaissStoreServiceProvider.LoadDocumentsIntoStore(
    //   store,
    //   TestDocuments.Documents
    // );

    // await FaissStoreServiceProvider.SaveStoreToFile(store);
    ApplicationBuilder.AddSystemMiddlewares(Program._app);
    ApplicationBuilder.AddDomainMiddleware(Program._app);
    ApplicationBuilder.AddRoutes(Program._app);
    ApplicationBuilder.Listen(Program._app);
  }
}
Program.Main();
