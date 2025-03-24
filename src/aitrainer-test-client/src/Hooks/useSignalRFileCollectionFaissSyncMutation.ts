import { useEffect, useState } from "react";
import { useGetSignalRHubContext } from "../Components/Contexts/AiTrainerSignalRContext";
import { useMutation, useQueryClient } from "react-query";
import {
  AiTrainerWebOutcome,
  AiTrainerWebOutcomeBase,
} from "../Models/AiTrainerWebOutcome";
import { FlatFileDocumentPartialCollection } from "../Models/FlatFileDocumentPartialCollection";
import { QueryKeys } from "../Constants";
export const useSignalRFileCollectionFaissSyncMutation = (
  fileCol: FlatFileDocumentPartialCollection
) => {
  const { fileDocuments } = fileCol;
  const collectionId = fileCol?.self?.id;
  const { hubConnection } = useGetSignalRHubContext();
  const queryClient = useQueryClient();
  const [customMutationState, setCustomMutationState] = useState<{
    data?: string | null;
    error?: Error | null;
    isLoading: boolean;
  }>({
    isLoading: false,
  });
  hubConnection.on("SyncFaissStoreError", (data: AiTrainerWebOutcomeBase) => {
    queryClient.refetchQueries(QueryKeys.GetTopLayerOfFile);
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
      queryClient.refetchQueries(QueryKeys.GetTopLayerOfFile);
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
        collectionId,
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
    dispose: () => {
      hubConnection.off("SyncFaissStoreError");
      hubConnection.off("SyncFaissStoreSuccess");
    },
  };
};
