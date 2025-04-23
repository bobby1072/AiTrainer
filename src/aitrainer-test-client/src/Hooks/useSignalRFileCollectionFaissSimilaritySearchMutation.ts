import { useState } from "react";
import { useGetSignalRHubContext } from "../Components/Contexts/AiTrainerSignalRContext";
import {
  AiTrainerWebOutcome,
  AiTrainerWebOutcomeBase,
} from "../Models/AiTrainerWebOutcome";
import { SingleDocumentChunk } from "../Models/SingleDocumentChunk";
import { useMutation } from "react-query";

export const useSignalRFileCollectionFaissSimilaritySearchMutation = () => {
  const { hubConnection } = useGetSignalRHubContext();
  const [customMutationState, setCustomMutationState] = useState<{
    data?: SingleDocumentChunk[] | null;
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
    (data: AiTrainerWebOutcome<SingleDocumentChunk[]>) => {
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

  const { mutate } = useMutation<
    void,
    Error,
    {
      question: string;
      documentsToReturn: number;
      collectionId?: string | null;
    }
  >(
    async ({ documentsToReturn, question, collectionId }) => {
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
    dispose: () => {
      hubConnection.off("SimilaritySearchFaissSuccess");
      hubConnection.off("SimilaritySearchFaissError");
    },
  };
};
