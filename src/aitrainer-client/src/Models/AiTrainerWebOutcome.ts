export type AiTrainerWebOutcome<T> = {
  isSuccess: boolean;
  data?: T | null;
  exceptionMessage?: string | null;
};
