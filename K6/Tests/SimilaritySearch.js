import { ThresholdsSettings, webApiEndpoint } from "../Helpers/Details.js";
import getAccessToken from "../Helpers/GetAccessToken.js";
import http from "k6/http";
import { check } from "k6";

export const options = {
  vus: 2,
  iterations: 500,
  thresholds: ThresholdsSettings,
};
const url = `${webApiEndpoint}/Api/Faiss/SimilaritySearch`;

export default function FaissSimilaritySearch() {
  const testNum = __ITER;
  const accessToken = getAccessToken(
    `k6user${testNum}`,
    `k6password${testNum}`
  );

  const idInput = {
    collectionId: null,
    documentsToReturn: 1,
    question: "Tell me suttin bro",
  };

  const res = http.post(url, JSON.stringify(idInput), {
    headers: {
      Authorization: `Bearer ${accessToken}`,
      "Content-type": "application/json",
    },
  });
  check(res, {
    "File collection faiss similarity search succeeded": (r) =>
      r.status === 200,
  });
}
