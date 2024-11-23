enum ErrorMessages {
  InternalServerError = "Internal Server Error",
}
enum QueryKeys {
  GetClientConfiguration = "get-client-configuration",
}
export default abstract class Constants {
  public static readonly ErrorMessages: typeof ErrorMessages = ErrorMessages;
  public static readonly QueryKeys: typeof QueryKeys = QueryKeys;
}
