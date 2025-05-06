export const userDetails = {
  username: "k6user",
  password: "k6password",
};

export const oidcDetails = {
  ...userDetails,
  client_id: "aitrainer-k6-local",
};

export const tokenEndpoint = "https://localhost:44363/connect/token";
