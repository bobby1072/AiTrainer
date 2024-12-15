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

  const parsedLocalValue = localValue
    ? (JSON.parse(localValue) as SolicitedDeviceToken)
    : undefined;

  useEffect(() => {
    if (isInitialStateResolved && !parsedLocalValue) {
      mutate();
    }
  }, [parsedLocalValue, mutate, isInitialStateResolved]);

  useEffect(() => {
    if (mutationData) {
      setValue(JSON.stringify(mutationData));
    }
  }, [mutationData, setValue]);

  return {
    data: parsedLocalValue,
    errorMessage: mutationError?.message || localStorageError,
    isLoading: mutationLoading,
    setValue: (val: SolicitedDeviceToken) => setValue(JSON.stringify(val)),
  };
};
