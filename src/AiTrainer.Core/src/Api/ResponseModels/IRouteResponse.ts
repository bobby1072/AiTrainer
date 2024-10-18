export interface IUnsuccessfulRouteResponse {
  isSuccess: boolean;
  exceptionMessage: string;
}

export interface ISuccessfulRouteResponse<T> {
  isSuccess: boolean;
  data: T;
}
