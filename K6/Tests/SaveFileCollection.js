import { webApiEndpoint } from "../Helpers/Details.js";
import getAccessToken from "../Helpers/GetAccessToken.js";
import http from "k6/http";
import { check } from "k6";

export const options = {
  vus: 1,
  iterations: 30,
};
const url = `${webApiEndpoint}/Api/FileCollection/Save`;

export default function () {
  const testNum = __ITER + 1;
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
