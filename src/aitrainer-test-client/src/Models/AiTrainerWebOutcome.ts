export type AiTrainerWebOutcomeBase = {
  isSuccess: boolean;
  exceptionMessage?: string | null;
};

export type AiTrainerWebOutcome<T> = {
  data?: T | null;
} & AiTrainerWebOutcomeBase;
