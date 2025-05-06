import { webApiEndpoint } from "../Helpers/Details";
import getAccessToken from "../Helpers/GetAccessToken";

export const options = {
  vus: 1,
  iterations: 10,
};
const url = `http://${webApiEndpoint}/Api/FileDocument/Upload`;

export default function () {
  const testNum = __ITER;
  const accessToken = getAccessToken(
    `k6user${testNum}`,
    `k6password${testNum}`
  );

  // Read the file from the local filesystem
  const fileBin = readFileSync("./../Data/MockCompanyHandbook.pdf", "binary");

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
