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
    setCustomMutationState((acc) => ({ ...acc, isLoading: false }));
    if (data.exceptionMessage) {
      const errro = new Error(data.exceptionMessage);
      setCustomMutationState((acc) => ({ ...acc, error: errro }));
    }
  });
  hubConnection.on(
    "SyncFaissStoreSuccess",
    (data: AiTrainerWebOutcome<string>) => {
      setCustomMutationState((acc) => ({ ...acc, isLoading: false }));
      if (data.data) {
        setCustomMutationState((acc) => ({
          ...acc,
          successMessage: data.data,
        }));
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
