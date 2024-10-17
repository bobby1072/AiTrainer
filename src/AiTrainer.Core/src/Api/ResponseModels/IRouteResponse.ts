export interface IUnsuccessfulRouteResponse {
  isSuccess: boolean;
  exceptionMessage: string;
  innerExceptionMessage: string;
}

export interface ISuccessfulRouteResponse<T> {
  isSuccess: boolean;
  data: T;
}
