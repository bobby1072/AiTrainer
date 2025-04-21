import InMemoryAiTrainerFaissStore from "./InMemoryAiTrainerFaissStore";
import { faker } from "@faker-js/faker";
import { describe, expect, test } from "@jest/globals";
import { FaissStore } from "@langchain/community/vectorstores/faiss";
import { FaissEmbeddings } from "./FaissEmbeddings";

describe(InMemoryAiTrainerFaissStore.name, () => {
  test(`${InMemoryAiTrainerFaissStore.name}.CreateFaissStore can create new stores`, () => {
    //Act
    const storeResult = InMemoryAiTrainerFaissStore.CreateFaissStore();

    //Assert
    expect(storeResult).toBeTruthy();
    expect(storeResult).toBeInstanceOf(FaissStore);
    expect(storeResult).toBeInstanceOf(InMemoryAiTrainerFaissStore);
    expect(storeResult.embeddings).toBe(FaissEmbeddings);
  });

  // it(`${InMemoryAiTrainerFaissStore.name}.LoadDocumentsIntoStore can load new documents into empty store`, async () => {
  //   //Arrange
  //   const newStore = InMemoryAiTrainerFaissStore.CreateFaissStore();
  //   const documentsToLoad = Array.from({ length: 6 }, faker.lorem.paragraph);

  //   //Act
  //   await newStore.LoadDocumentsIntoStore(
  //     documentsToLoad.map((x, i) => ({
  //       metadata: {},
  //       pageContent: x,
  //       id: i.toString(),
  //     }))
  //   );

  //   //Assert
  //   const finalDocStore = newStore.getDocstore();
  //   for (let i = 0; i < documentsToLoad.length; i++) {
  //     expect(
  //       finalDocStore.search(documentsToLoad[i])?.pageContent
  //     ).toBeTruthy();
  //   }
  // });
});
