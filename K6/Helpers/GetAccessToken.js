import { defaultOidcDetails, tokenEndpoint } from "./Details.js";
import http from "k6/http";

function encodeForm(data) {
  return Object.entries(data)
    .map(([k, v]) => `${encodeURIComponent(k)}=${encodeURIComponent(v)}`)
    .join("&");
}

const getAccessToken = (username, password) => {
  const reqPayload = encodeForm({
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

  const token = JSON.parse(response.body).access_token;

  //   console.log(`Access token: ${token}`);

  return token;
};

export default getAccessToken;
