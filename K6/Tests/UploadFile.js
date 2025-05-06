import { webApiEndpoint } from "../Helpers/Details.js";
import getAccessToken from "../Helpers/GetAccessToken.js";
import http from "k6/http";
import { check } from "k6";

export const options = {
  vus: 1,
  iterations: 10,
};
const url = `${webApiEndpoint}/Api/FileDocument/Upload`;
const fileBin = open("./../Data/MockCompanyHandbook.pdf", "b");

export default function () {
  const testNum = __ITER;
  const accessToken = getAccessToken(
    `k6user${testNum}`,
    `k6password${testNum}`
  );

  // Read the file from the local filesystem

  const formData = {
    collectionId: null,
    fileDescription: "This is a test file.",
    file: http.file(fileBin, "testfile.pdf", "application/pdf"),
  };

  const res = http.post(url, formData, {
    headers: {
      Authorization: `Bearer ${accessToken}`,
    },
  });

  check(res, {
    "upload succeeded": (r) => r.status === 200,
  });
}
