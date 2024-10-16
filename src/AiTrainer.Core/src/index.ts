import express, { Application } from "express";
import ApplicationBuilder from "./Api/Routes/ApplicationBuilder";
// console.log("hellllloooo");
// const faissStoreFactory = new FaissStoreFactory();
// const testStoreService = new TestStoreService(faissStoreFactory);

// const similaritySearchResults =
//   await testStoreService.Store!.similaritySearch("biology", 2);

// console.table(similaritySearchResults);

abstract class Program {
  private static _app: Application = express();
  public static async Main(): Promise<void> {
    ApplicationBuilder.AddMiddlewares(Program._app);

    ApplicationBuilder.AddRoutes(Program._app);

    ApplicationBuilder.Listen(Program._app);
  }
}
Program.Main();
