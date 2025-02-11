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
  const [successMessage, setSuccessMessage] = useState<string>();
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [error, setError] = useState<Error>();
  hubConnection.on("SyncFaissStoreError", (data: AiTrainerWebOutcomeBase) => {
    setIsLoading(false);
    if (data.exceptionMessage) {
      setError(new Error(data.exceptionMessage));
    }
  });
  hubConnection.on(
    "SyncFaissStoreSuccess",
    (data: AiTrainerWebOutcome<string>) => {
      setIsLoading(false);
      if (data.data) {
        setSuccessMessage(data.data);
      }
    }
  );
  const { mutate } = useMutation(
    async () => {
      setSuccessMessage(undefined);
      setError(undefined);
      setIsLoading(true);
      await hubConnection.send("SyncFaissStore", {
        collectionId: collectionId,
      });
    },
    {
      onError: (ex: Error) => setError(ex),
    }
  );

  return { data: successMessage, isLoading, error, mutate };
};
