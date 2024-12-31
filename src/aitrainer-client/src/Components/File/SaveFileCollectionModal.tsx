import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  Grid2,
  TextField,
} from "@mui/material";
import { useSaveFileCollectionMutation } from "../../Hooks/useSaveFileCollectionMutation";
import { StyledDialogTitle } from "../Common/StyledDialogTitle";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import { ErrorComponent } from "../Common/ErrorComponent";
import { useSnackbar } from "notistack";
import { FileCollectionSaveInput } from "../../Models/FileCollectionSaveInput";

const fileCollectionFormSchema = z.object({
  collectionName: z.string().max(100).nonempty("Collection name is required"),
  collectionDescription: z.string().max(500).optional().nullable(),
});

type FileCollectionFormSchemaType = z.infer<typeof fileCollectionFormSchema>;

const mapToDefaultValues = (
  fileCollInput: Partial<FileCollectionSaveInput>
): Partial<FileCollectionFormSchemaType> => {
  return {
    collectionName: fileCollInput.collectionName,
    collectionDescription: fileCollInput.collectionDescription,
  };
};

export const SaveFileCollectionModal: React.FC<{
  closeModal: () => void;
  fileCollInput: Partial<FileCollectionSaveInput>;
}> = ({ closeModal, fileCollInput }) => {
  const {
    handleSubmit,
    register,
    reset: formReset,
    watch,
    formState: { errors: formErrors, isDirty },
  } = useForm<FileCollectionFormSchemaType>({
    resolver: zodResolver(fileCollectionFormSchema),
    defaultValues: mapToDefaultValues(fileCollInput),
  });
  const { mutate, error, data, isLoading, reset } =
    useSaveFileCollectionMutation();
  const { enqueueSnackbar } = useSnackbar();
  useEffect(() => {
    if (data) {
      closeModal();
      enqueueSnackbar("Collection added successfully", { variant: "success" });
    }
  }, [data, closeModal, enqueueSnackbar]);

  const { collectionName } = watch();
  return (
    <Dialog open onClose={() => !isLoading && closeModal()}>
      <form
        id="addFileCollectionForm"
        onSubmit={handleSubmit((formVals) => {
          reset();
          mutate({
            fileColInput: {
              collectionName: formVals.collectionName,
              collectionDescription: formVals.collectionDescription,
              parentId: fileCollInput.parentId,
              id: fileCollInput.id,
              dateCreated: fileCollInput.dateCreated,
              dateModified: fileCollInput.dateModified,
            },
          });
          formReset();
        })}
      >
        <StyledDialogTitle title="Add file collection" />
        <DialogContent dividers>
          <Grid2
            container
            justifyContent="center"
            alignItems="center"
            spacing={1}
            width="100%"
          >
            <Grid2 width={"90%"}>
              <TextField
                {...register("collectionName", { required: true })}
                disabled={isLoading}
                label="Name..."
                fullWidth
                error={!!formErrors.collectionName}
                helperText={
                  formErrors.collectionName
                    ? formErrors.collectionName.message
                    : undefined
                }
              />
            </Grid2>
            <Grid2 width={"90%"}>
              <TextField
                {...register("collectionDescription", { required: false })}
                disabled={isLoading}
                label="Description..."
                fullWidth
                multiline
                rows={2}
                error={!!formErrors.collectionDescription}
                helperText={
                  formErrors.collectionDescription
                    ? formErrors.collectionDescription.message
                    : undefined
                }
              />
            </Grid2>
            {error && (
              <Grid2 width={"90%"}>
                <ErrorComponent errorMessage={error.message} fontSize={17} />
              </Grid2>
            )}
          </Grid2>
        </DialogContent>
        <DialogActions>
          <Grid2
            container
            justifyContent="center"
            alignItems="center"
            direction={"row"}
            width="100%"
          >
            <Grid2
              width={"50%"}
              sx={{ display: "flex", justifyContent: "flex-start" }}
            >
              <Button
                variant="outlined"
                color="primary"
                onClick={closeModal}
                disabled={isLoading}
              >
                Cancel
              </Button>
            </Grid2>
            <Grid2
              width={"50%"}
              sx={{ display: "flex", justifyContent: "flex-end" }}
            >
              <Button
                variant="contained"
                color="primary"
                type="submit"
                disabled={
                  !collectionName ||
                  !isDirty ||
                  isLoading ||
                  Object.values(formErrors).some((x) => !!x)
                }
              >
                Save
              </Button>
            </Grid2>
          </Grid2>
        </DialogActions>
      </form>
    </Dialog>
  );
};
