import { useState } from "react";
import { useGetSignalRHubContext } from "../Components/Contexts/AiTrainerSignalRContext";
import { useMutation } from "react-query";
import {
  AiTrainerWebOutcome,
  AiTrainerWebOutcomeBase,
} from "../Models/AiTrainerWebOutcome";
export const useSignalRFileCollectionFaissSyncMutation = (
  collectionId?: string | null
) => {
  const { hubConnection } = useGetSignalRHubContext();
  const [customMutationState, setCustomMutationState] = useState<{
    successMessage?: string | null;
    error?: Error | null;
    isLoading: boolean;
  }>({
    isLoading: false,
  });
  hubConnection.on("SyncFaissStoreError", (data: AiTrainerWebOutcomeBase) => {
    if (data.exceptionMessage) {
      const newError = new Error(data.exceptionMessage);
      setCustomMutationState({ isLoading: false, error: newError });
    } else {
      setCustomMutationState({ isLoading: false });
    }
  });
  hubConnection.on(
    "SyncFaissStoreSuccess",
    (data: AiTrainerWebOutcome<string>) => {
      if (data.data) {
        setCustomMutationState({
          successMessage: data.data,
          isLoading: false,
        });
      } else {
        setCustomMutationState({ isLoading: false });
      }
    }
  );
  const { mutate } = useMutation(
    async () => {
      setCustomMutationState({ isLoading: true });
      await hubConnection.send("SyncFaissStore", {
        collectionId: collectionId,
      });
    },
    {
      onError: (ex: Error) =>
        setCustomMutationState((acc) => ({ ...acc, error: ex })),
    }
  );

  return {
    ...customMutationState,
    data: customMutationState.successMessage,
    mutate,
  };
};
