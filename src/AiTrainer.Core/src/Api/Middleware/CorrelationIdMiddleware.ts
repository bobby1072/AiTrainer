import { NextFunction, Request, response, Response } from "express";
import Guid from "../../Utils/Guid";

export default abstract class CorrelationIdMiddleware {
  public static InvokeMiddleware() {
    return (req: Request, res: Response, next: NextFunction) => {
      const correlationIdFromRequest = Guid.TryParse(
        req.headers["x-correlation-id-x"] as string
      );

      const correlationId = correlationIdFromRequest
        ? correlationIdFromRequest
        : Guid.NewGuid();

      if (!correlationIdFromRequest) {
        req.headers["x-correlation-id-x"] = correlationId.ToString();
      }
      response.setHeader("x-correlation-id-x", correlationId.ToString());

      next();
    };
  }
}
