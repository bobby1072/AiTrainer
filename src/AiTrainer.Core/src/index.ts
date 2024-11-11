import express, { Application } from "express";
import ApplicationBuilder from "./Api/ApplicationBuilder";
// console.log("hellllloooo");
// const faissStoreFactory = new FaissStoreFactory();
// const testStoreService = new TestStoreService(faissStoreFactory);

// await testStoreService.BuildStoreAndLoadDocuments();

// const similaritySearchResults =
//   await testStoreService.Store!.similaritySearch("biology", 2);

// await testStoreService.SaveStore();
// console.table(similaritySearchResults);

abstract class Program {
  private static _app: Application = express();
  public static async Main(): Promise<void> {
    ApplicationBuilder.AddSystemMiddlewares(Program._app);
    ApplicationBuilder.AddDomainMiddleware(Program._app);
    ApplicationBuilder.AddRoutes(Program._app);
    ApplicationBuilder.Listen(Program._app);
  }
}
Program.Main();
