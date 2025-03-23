import ExceptionConstants from "../../Exceptions/ExceptionConstants";
import { Response } from "express";
import { UnsuccessfulRouteResponse } from "../ResponseModels/RouteResponse";
import ApiException from "../../Exceptions/ApiException";

export default abstract class AiTrainerExpressExceptionHandling {
  public static HandleAsync(
    apiAction: () => Promise<void>,
    res: Response,
    defaultErrorMessage: string = ExceptionConstants.InternalServerError
  ) {
    try {
      apiAction();
    } catch (e: any) {
      if (e instanceof ApiException) {
        res.status(200).json({
          exceptionMessage: e.message,
          isSuccess: false,
        } as UnsuccessfulRouteResponse);
        return;
      }

      res.status(200).json({
        exceptionMessage: defaultErrorMessage,
        isSuccess: false,
      } as UnsuccessfulRouteResponse);
    }
  }
}
