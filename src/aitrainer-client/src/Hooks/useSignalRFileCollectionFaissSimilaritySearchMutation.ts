import { useState } from "react";
import { useGetSignalRHubContext } from "../Components/Contexts/AiTrainerSignalRContext";
import {
  AiTrainerWebOutcome,
  AiTrainerWebOutcomeBase,
} from "../Models/AiTrainerWebOutcome";
import { SimilaritySearchResponse } from "../Models/SimilaritySearchResponse";
import { useMutation } from "react-query";

export const useSignalRFileCollectionFaissSimilaritySearchMutation = (
  question: string,
  documentsToReturn: number,
  collectionId?: string | null
) => {
  const { hubConnection } = useGetSignalRHubContext();
  const [customMutationState, setCustomMutationState] = useState<{
    data?: SimilaritySearchResponse | null;
    error?: Error | null;
    isLoading: boolean;
  }>({
    isLoading: false,
  });
  hubConnection.on(
    "SimilaritySearchFaissError",
    (data: AiTrainerWebOutcomeBase) => {
      if (data.exceptionMessage) {
        const newError = new Error(data.exceptionMessage);
        setCustomMutationState({ isLoading: false, error: newError });
      } else {
        setCustomMutationState({ isLoading: false });
      }
    }
  );
  hubConnection.on(
    "SimilaritySearchFaissSuccess",
    (data: AiTrainerWebOutcome<SimilaritySearchResponse>) => {
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

  const { mutate } = useMutation(
    async () => {
      setCustomMutationState({ isLoading: true });
      await hubConnection.send("SimilaritySearchFaissStore", {
        collectionId,
        question,
        documentsToReturn,
      });
    },
    {
      onError: (ex: Error) =>
        setCustomMutationState((acc) => ({ ...acc, error: ex })),
    }
  );

  return {
    ...customMutationState,
    mutate,
  };
};
