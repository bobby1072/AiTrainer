import { useForm } from "react-hook-form";
import { z } from "zod";
import { useSaveFileDocumentMutation } from "../../Hooks/useSaveFileDocumentMutation";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  Grid2,
} from "@mui/material";
import { useSnackbar } from "notistack";
import { useEffect } from "react";
import { StyledDialogTitle } from "../Common/StyledDialogTitle";
import { ErrorComponent } from "../Common/ErrorComponent";

const checkFileSize = (file: File): boolean => {
  // Convert bytes to megabytes
  const fileSizeInMegabytes = file.size / (1024 * 1024);
  return fileSizeInMegabytes <= 3;
};

const uploadFileDocSchema = z.object({
  collectionId: z.string().uuid().optional().nullable(),
  file: z.any().refine((file) => {
    return (
      file &&
      file instanceof File &&
      (file.type === "application/pdf" || file.type === "text/plain") &&
      checkFileSize(file)
    );
  }),
});

type UploadFileDocSchemaType = z.infer<typeof uploadFileDocSchema>;

const mapDefaultValues = (collectionId?: string | null) => {
  return {
    collectionId,
  };
};

export const AddFileDocumentModal: React.FC<{
  collectionId?: string | null;
  closeModal: () => void;
}> = ({ closeModal, collectionId }) => {
  const {
    handleSubmit,
    setValue,
    watch,
    formState: { errors: formErrors },
  } = useForm<UploadFileDocSchemaType>({
    defaultValues: mapDefaultValues(collectionId),
    resolver: zodResolver(uploadFileDocSchema),
  });
  const { mutate, reset, isLoading, error, data } =
    useSaveFileDocumentMutation();
  const { enqueueSnackbar } = useSnackbar();
  useEffect(() => {
    if (data) {
      closeModal();
      enqueueSnackbar("Document added successfully", { variant: "success" });
    }
  }, [data, closeModal, enqueueSnackbar]);

  const { file } = watch();
  return (
    <Dialog open onClose={closeModal}>
      <form
        id="uploadFileDocumentForm"
        onSubmit={handleSubmit((formVals) => {
          reset();
          var formData = new FormData();
          formData.append("file", formVals.file);
          if (formVals.collectionId) {
            formData.append("collectionId", formVals.collectionId);
          }
          mutate({ saveInput: formData });
        })}
      >
        <StyledDialogTitle title="Add file document" />
        <DialogContent dividers>
          <Grid2
            container
            justifyContent="center"
            alignItems="center"
            spacing={4}
            padding={1}
            width="100%"
          >
            <Grid2 width="90%">
              <input
                type="file"
                accept=".pdf,.txt"
                onChange={(e) => {
                  const foundFile = e.target.files?.item(0);

                  if (foundFile) {
                    setValue("file", foundFile);
                  }
                }}
              />
            </Grid2>
            {error && (
              <Grid2 width={"100%"}>
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
                  isLoading ||
                  !!formErrors.file ||
                  !!formErrors.collectionId ||
                  !file
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