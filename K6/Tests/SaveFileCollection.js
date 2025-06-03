import { ThresholdsSettings, webApiEndpoint } from "../Helpers/Details.js";
import getAccessToken from "../Helpers/GetAccessToken.js";
import http from "k6/http";
import { check } from "k6";

export const options = {
  vus: 2,
  iterations: 50,
  thresholds: ThresholdsSettings,
};
const url = `${webApiEndpoint}/Api/FileCollection/Save`;

export default function SaveFileCollection() {
  const testNum = __ITER;
  const accessToken = getAccessToken(
    `k6user${testNum}`,
    `k6password${testNum}`
  );

  const saveInput = {
    collectionName: "TestCol",
    collectionDescription: "Test description",
    autoFaissSync: false,
    dateCreated: null,
    dateModified: null,
    id: null,
    parentId: null,
  };

  const res = http.post(url, JSON.stringify(saveInput), {
    headers: {
      Authorization: `Bearer ${accessToken}`,
      "Content-type": "application/json",
    },
  });
  check(res, {
    "save file collection succeeded": (r) => r.status === 200,
  });
}
