export const userDetails = {
  username: "k6user1",
  password: "k6password1",
};

export const defaultOidcDetails = {
  ...userDetails,
  client_id: "aitrainer-k6-local",
};

export const tokenEndpoint = __ENV.IDENTITYSERVERTOKENENDPOINT
  ? __ENV.IDENTITYSERVERTOKENENDPOINT
  : "https://localhost:44363/connect/token";

export const webApiEndpoint = __ENV.WEBAPIENDPOINT
  ? __ENV.WEBAPIENDPOINT
  : "http://localhost:5222";

export const thresholdSettings = {
  http_req_failed: [{ threshold: "rate<0.2", abortOnFail: true }],
  http_req_duration: ["p(99)<300000"],
};
