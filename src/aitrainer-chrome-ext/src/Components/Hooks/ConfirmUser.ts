import { useMutation, UseMutationOptions } from "react-query";
import AiTrainerWebClient from "../../Utils/AiTrainerWebClient";
import { SaveUserInput } from "../../Models/SaveUserInput";
import { User } from "../../Models/User";

export const useConfirmUser = (
  opts?: UseMutationOptions<User, Error, SaveUserInput, unknown>
) => {
  const mutationResults = useMutation<User, Error, SaveUserInput>((input) =>
    AiTrainerWebClient.ConfirmUser(input)
  );

  return { ...mutationResults, confirmUser: mutationResults.mutate };
};
