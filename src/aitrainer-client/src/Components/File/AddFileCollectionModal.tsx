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
import { useEffect, useState } from "react";
import { ErrorComponent } from "../Common/ErrorComponent";
import { useSnackbar } from "notistack";

const fileCollectionFormSchema = z.object({
  collectionName: z.string().max(100).nonempty("Collection name is required"),
});

type FileCollectionFormSchemaType = z.infer<typeof fileCollectionFormSchema>;

export const AddFileCollectionModal: React.FC<{
  closeModal: () => void;
}> = ({ closeModal }) => {
  const {
    handleSubmit,
    register,
    reset: formReset,
    formState: { errors: formErrors, isDirty },
  } = useForm<FileCollectionFormSchemaType>({
    resolver: zodResolver(fileCollectionFormSchema),
  });
  const { mutate, error, data, isLoading, reset } =
    useSaveFileCollectionMutation();
  const { enqueueSnackbar } = useSnackbar();
  const [isCollectionNamePresent, setIsCollectionNamePresent] =
    useState<boolean>(false);
  useEffect(() => {
    if (data) {
      closeModal();
      enqueueSnackbar("Collection added successfully", { variant: "success" });
    }
  }, [data, closeModal, enqueueSnackbar]);

  return (
    <Dialog open onClose={closeModal}>
      <form
        id="addFileCollectionForm"
        onSubmit={handleSubmit((formVals) => {
          reset();
          mutate({
            fileColInput: {
              collectionName: formVals.collectionName,
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
            spacing={4}
            padding={1}
            width="100%"
          >
            <Grid2 width={"90%"}>
              <TextField
                {...register("collectionName", { required: true })}
                disabled={isLoading}
                onChange={(e) => {
                  reset();
                  if (e.target.value?.length > 0) {
                    setIsCollectionNamePresent(true);
                  }
                }}
                label="Collection name"
                fullWidth
                multiline
                rows={2}
                error={!!formErrors.collectionName}
                helperText={
                  formErrors.collectionName
                    ? formErrors.collectionName.message
                    : undefined
                }
              />
            </Grid2>
            {error && (
              <Grid2 width={"90%"}>
                <ErrorComponent errorMessage={error.message} />
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
                  !isCollectionNamePresent ||
                  !isDirty ||
                  isLoading ||
                  !!formErrors.collectionName
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
