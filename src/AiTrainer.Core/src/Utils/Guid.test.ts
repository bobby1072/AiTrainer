import { describe, expect, it } from "@jest/globals";
import Guid from "./Guid";
import { faker } from "@faker-js/faker";

describe(`${Guid.name}`, () => {
  it.each([
    [faker.number.int().toString(), false],
    [faker.string.ulid(), false],
    [faker.lorem.sentence(), false],
    ["-eb16-465a-9bb2-b5d2f02bfe6d", false],
    ["02edb29b-eb16-465a--b5d2f02bfe6d", false],
    ["02edb29b-eb16-465a-9bb2-", false],
    ["02edb29b--465a-9bb2-b5d2f02bfe6d", false],
    ["02edb29b-eb16-465a-9bb2-b5d2f02bfe6d", true],
    ["ef2b2ff8-3618-4595-bbab-21fc1e38d719", true],
    ["57f4643b-a8fc-49d0-871b-126eaa647666", true],
    ["24e287d3-05d4-4a23-8f88-e3705eecb595", true],
    ["78d541d2-139e-4235-9d84-b96766558e4e", true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
    [faker.string.uuid(), true],
  ])(
    `${Guid.IsValidUUID} should return results correctly`,
    (guidToParse: string, isValidGuid: boolean) => {
      //Act
      const result = Guid.IsValidUUID(guidToParse);

      //Assert
      expect(result).toBe(isValidGuid);
    }
  );
});
