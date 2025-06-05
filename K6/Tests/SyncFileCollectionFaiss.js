import { thresholdSettings, webApiEndpoint } from "../Helpers/Details.js";
import getAccessToken from "../Helpers/GetAccessToken.js";
import http from "k6/http";
import { check } from "k6";

export const options = {
  vus: 2,
  iterations: 500,
  thresholds: thresholdSettings,
};
const url = `${webApiEndpoint}/Api/Faiss/Sync`;

export default function SyncFileCollectionFaiss() {
  const testNum = __ITER;
  const accessToken = getAccessToken(
    `k6user${testNum}`,
    `k6password${testNum}`
  );

  const idInput = {
    id: null,
  };

  const res = http.post(url, JSON.stringify(idInput), {
    headers: {
      Authorization: `Bearer ${accessToken}`,
      "Content-type": "application/json",
    },
  });
  check(res, {
    "File collection faiss sync succeeded": (r) => r.status === 200,
  });
}
