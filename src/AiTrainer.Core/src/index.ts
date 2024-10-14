import FaissStoreFactory from "./TestFaissUse/FaissStoreFactory";
import TestStoreService from "./TestFaissUse/TestStoreBuilder";

abstract class Program {
  public static async Main(): Promise<void> {
    console.log("hellllloooo");
    const faissStoreFactory = new FaissStoreFactory();
    const testStoreService = new TestStoreService(faissStoreFactory);

    await testStoreService.BuildStoreAndLoadDocuments();

    const similaritySearchResults =
      await testStoreService.Store!.similaritySearch("biology", 2);

    const secondSimilaritySearchResults =
      await testStoreService.Store!.similaritySearch("biology");

    console.table(similaritySearchResults);
  }
}
Program.Main();
