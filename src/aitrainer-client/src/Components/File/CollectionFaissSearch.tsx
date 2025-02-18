import { Button, Grid2, TextField } from "@mui/material";
import { useSignalRFileCollectionFaissSimilaritySearchMutation } from "../../Hooks/useSignalRFileCollectionFaissSimilaritySearchMutation";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { ErrorComponent } from "../Common/ErrorComponent";

const searchSchema = z.object({
  question: z.string().min(1).max(500),
  documentsToReturn: z.number().min(1).max(100),
});

type SearchSchemaType = z.infer<typeof searchSchema>;

export const CollectionFaissSearch: React.FC<{
  collectionId?: string | null;
}> = ({ collectionId }) => {
  const {
    isLoading: similaritySearchLoading,
    mutate: similaritySearch,
    data: similaritySearchData,
    error: similaritySearchError,
  } = useSignalRFileCollectionFaissSimilaritySearchMutation();
  const {
    register: formRegister,
    handleSubmit: formSubmit,
    formState: { errors: formErrors },
  } = useForm<SearchSchemaType>({
    resolver: zodResolver(searchSchema),
    defaultValues: {
      documentsToReturn: 1,
    },
  });
  return (
    <form
      id="SimSearchForm"
      onSubmit={formSubmit((vals) => {
        similaritySearch({ ...vals, collectionId });
      })}
    >
      <Grid2
        container
        alignItems="center"
        direction="row"
        spacing={1}
        padding={1}
        textAlign="center"
        width="100%"
      >
        <Grid2 width="90%">
          <TextField
            {...formRegister("question")}
            label="Faiss search collection"
            placeholder="Search..."
            fullWidth
            error={!!formErrors.question}
            helperText={
              formErrors.question?.message
                ? formErrors.question?.message
                : undefined
            }
          />
        </Grid2>
        <Grid2 width={"10%"}>
          <Button
            form="SimSearchForm"
            type="submit"
            disabled={similaritySearchLoading}
            color="primary"
            fullWidth
          >
            Search
          </Button>
        </Grid2>
        {similaritySearchData && <Grid2 width={"100%"}></Grid2>}
        {similaritySearchError && (
          <Grid2 width={"100%"}>
            <ErrorComponent errorMessage="Error occurred during search" />
          </Grid2>
        )}
      </Grid2>
    </form>
  );
};
