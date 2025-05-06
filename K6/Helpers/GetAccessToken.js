import { oidcDetails, tokenEndpoint } from "./Details";
import http from "k6/http";

const getAccessToken = () => {
  const reqPayload = new URLSearchParams({
    ...oidcDetails,
    grant_type: "password",
  });

  const response = http.post(tokenEndpoint, reqPayload.toString(), {
    headers: {
      "Content-Type": "application/x-www-form-urlencoded",
    },
  });

  return response.access_token;
};

export default getAccessToken;
