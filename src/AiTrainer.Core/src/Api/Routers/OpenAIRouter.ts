import { Application, Request, Response } from "express";
import AiTrainerExpressExceptionHandling from "../Helpers/AiTrainerExpressExceptionHandling";
import { FormattedChatQueryOptsSchema } from "../../Models/FormattedQueryOpts";
import ApiException from "../../Exceptions/ApiException";
import ChatQueryService from "../../OpenAI/ChatQueryService";
import { SuccessfulRouteResponse } from "../ResponseModels/RouteResponse";
import { MessageContent } from "@langchain/core/messages";

export default abstract class OpenAIRouter {
  private static FormattedChatQuery(app: Application) {
    return app.post(
      `/api/${OpenAIRouter.name.toLowerCase()}/formattedchatquery`,
      async (req: Request, resp: Response) => {
        return await AiTrainerExpressExceptionHandling.HandleAsync(async () => {
          const parsedInput = FormattedChatQueryOptsSchema.safeParse({
            ...req.body,
          });

          if (!parsedInput.success) {
            throw new ApiException("Invalid input");
          }

          const result = await ChatQueryService.FormattedQuery(
            parsedInput.data
          );

          resp.status(200).json({
            isSuccess: true,
            data: {
              content: result.content,
            },
          } as SuccessfulRouteResponse<{
            content: MessageContent;
          }>);
        }, resp);
      }
    );
  }

  public static InvokeRoutes(app: Application): void {
    OpenAIRouter.FormattedChatQuery(app);
  }
}
