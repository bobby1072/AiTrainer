import { defaultOidcDetails, tokenEndpoint } from "./Details";
import http from "k6/http";

const getAccessToken = (username, password) => {
  const reqPayload = new URLSearchParams({
    ...defaultOidcDetails,
    ...(username && { username }),
    ...(password && { password }),
    grant_type: "password",
  });

  const response = http.post(tokenEndpoint, reqPayload.toString(), {
    headers: {
      "Content-Type": "application/x-www-form-urlencoded",
    },
  });

  console.log(`Access token: ${response.access_token}`);

  return response.access_token;
};

export default getAccessToken;
