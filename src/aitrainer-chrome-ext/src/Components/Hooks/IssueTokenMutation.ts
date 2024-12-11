import { useMutation } from "react-query";
import { SolicitedDeviceToken } from "../../Models/SolicitedDeviceToken";
import AiTrainerWebClient from "../../Utils/AiTrainerWebClient";
import { useChromeStorageLocal } from "use-chrome-storage";
import { useEffect } from "react";

export const useGetDeviceToken = () => {
  const [
    localValue,
    setValue,
    isPersistent,
    localStorageError,
    isInitialStateResolved,
  ] = useChromeStorageLocal<string>("deviceToken");

  const {
    data: mutationData,
    error: mutationError,
    isLoading: mutationLoading,
    mutate,
  } = useMutation<SolicitedDeviceToken, Error>(() =>
    AiTrainerWebClient.IssueDeviceToken()
  );

  useEffect(() => {
    if (isInitialStateResolved && !localValue) {
      mutate();
    }
  }, [localValue, mutate, isInitialStateResolved]);

  useEffect(() => {
    if (mutationData) {
      setValue(JSON.stringify(mutationData));
    }
  }, [mutationData, setValue]);

  return {
    data: localValue ? JSON.parse(localValue) : null,
    errorMessage: mutationError?.message || localStorageError,
    isLoading: mutationLoading,
    setValue: (val: {}) => setValue(JSON.stringify(val)),
  };
};
