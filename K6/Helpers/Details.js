export const userDetails = {
  username: "k6user1",
  password: "k6password1",
};

export const defaultOidcDetails = {
  ...userDetails,
  client_id: "aitrainer-k6-local",
};

export const tokenEndpoint = "https://localhost:44363/connect/token";

export const webApiEndpoint = "http://localhost:5222";
