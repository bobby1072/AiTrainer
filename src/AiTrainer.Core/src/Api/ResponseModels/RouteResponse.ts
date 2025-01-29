export type UnsuccessfulRouteResponse = {
  isSuccess: boolean;
  exceptionMessage: string;
};

export type SuccessfulRouteResponse<T> = {
  isSuccess: boolean;
  data: T;
};
