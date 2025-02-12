import { useEffect, useState } from "react";
import { useGetSignalRHubContext } from "../Components/Contexts/AiTrainerSignalRContext";
import { useMutation } from "react-query";
import {
  AiTrainerWebOutcome,
  AiTrainerWebOutcomeBase,
} from "../Models/AiTrainerWebOutcome";
import { FlatFileDocumentPartialCollection } from "../Models/FlatFileDocumentPartialCollection";
export const useSignalRFileCollectionFaissSyncMutation = (
  fileCol: FlatFileDocumentPartialCollection
) => {
  const {
    fileDocuments,
    self: { id: collectionId },
  } = fileCol;
  const { hubConnection } = useGetSignalRHubContext();
  const [customMutationState, setCustomMutationState] = useState<{
    data?: string | null;
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
          data: data.data,
          isLoading: false,
        });
      } else {
        setCustomMutationState({ isLoading: false });
      }
    }
  );
  const { mutate, reset: mutationReset } = useMutation(
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

  useEffect(() => {
    setCustomMutationState({ isLoading: false });
    mutationReset();
  }, [fileDocuments, setCustomMutationState, mutationReset]);
  return {
    ...customMutationState,
    mutate,
  };
};
