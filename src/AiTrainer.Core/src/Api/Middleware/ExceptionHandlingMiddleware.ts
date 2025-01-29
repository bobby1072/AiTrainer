import { NextFunction } from "express";
import ExceptionConstants from "../../Exceptions/ExceptionConstants";
import { Request, Response } from "express";
import { UnsuccessfulRouteResponse } from "../ResponseModels/RouteResponse";
import ApiException from "../../Exceptions/ApiException";

export default abstract class ExceptionHandlingMiddleware {
  public static InvokeMiddleware(
    defaultErrorMessage: string = ExceptionConstants.InternalServerError
  ) {
    return (_: Request, res: Response, next: NextFunction) => {
      try {
        next();
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
    };
  }
}
