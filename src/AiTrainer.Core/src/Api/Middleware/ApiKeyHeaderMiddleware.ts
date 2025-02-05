import ExceptionConstants from "../../Exceptions/ExceptionConstants";
import AppSettingsProvider from "../../Utils/AppSettingsProvider";
import { Request, Response, NextFunction } from "express";
export default abstract class ApiKeyHeaderMiddleware {
  public static InvokeMiddleware() {
    return (req: Request, res: Response, next: NextFunction) => {
      const foundHeader = req.headers["api-key"];
      if (!foundHeader) {
        res.status(200).json({
          isSuccess: false,
          exceptionMessage: ExceptionConstants.NoApiKeyHeader,
        });
        return;
      } else if (foundHeader !== AppSettingsProvider.AllAppSettings.ApiKey) {
        res.status(200).json({
          isSuccess: false,
          exceptionMessage: ExceptionConstants.InvalidApiKey,
        });
        return;
      }
      next();
    };
  }
}
