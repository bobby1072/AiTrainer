export default class ApiException extends Error {
  public override message: string;
  public innerException?: Error;
  constructor(message: string, innerException?: Error) {
    super(message);
    this.message = message;
    this.innerException = innerException;
  }
}
