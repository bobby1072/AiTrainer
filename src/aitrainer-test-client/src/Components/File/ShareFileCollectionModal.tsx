import { z } from "zod";
import { useShareFileCollectionWithMembersMutation } from "../../Hooks/useShareFileCollectionWithMembersMutation";
import { useSnackbar } from "notistack";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  FormControlLabel,
  Grid2,
  Switch,
  TextField,
} from "@mui/material";
import { StyledDialogTitle } from "../Common/StyledDialogTitle";

const sharedFileCollectionSingleMemberSaveInputSchema = z.object({
  email: z.string().email(),
  canViewDocuments: z.boolean(),
  canDownloadDocuments: z.boolean(),
  canCreateDocuments: z.boolean(),
  canRemoveDocuments: z.boolean(),
  canSimilaritySearch: z.boolean(),
});

export type SharedFileCollectionSingleMemberSaveInput = z.infer<
  typeof sharedFileCollectionSingleMemberSaveInputSchema
>;

const mapDefaultValues =
  (): Partial<SharedFileCollectionSingleMemberSaveInput> => {
    return {
      canViewDocuments: false,
      canDownloadDocuments: false,
      canCreateDocuments: false,
      canRemoveDocuments: false,
      canSimilaritySearch: false,
    };
  };

export const ShareFileCollectionModal: React.FC<{
  closeModal: () => void;
  collectionId: string;
}> = ({ closeModal, collectionId }) => {
  const {
    handleSubmit,
    register,
    watch,
    formState: { errors: formErrors },
  } = useForm<SharedFileCollectionSingleMemberSaveInput>({
    defaultValues: mapDefaultValues(),
    resolver: zodResolver(sharedFileCollectionSingleMemberSaveInputSchema),
  });

  const { mutate, reset, isLoading, error, data } =
    useShareFileCollectionWithMembersMutation();
  const { enqueueSnackbar } = useSnackbar();
  const {
    canViewDocuments,
    canCreateDocuments,
    canDownloadDocuments,
    canRemoveDocuments,
    canSimilaritySearch,
  } = watch();
  useEffect(() => {
    if (error) {
      enqueueSnackbar("Failed to share file collection", { variant: "error" });
    }
  }, [enqueueSnackbar, error]);
  useEffect(() => {
    if (data) {
      closeModal();
      enqueueSnackbar("Shared file collection successfully", {
        variant: "success",
      });
    }
  }, [data, closeModal, enqueueSnackbar]);

  return (
    <Dialog open onClose={closeModal}>
      <form
        id="shareFileCollectionToMembersForm"
        onSubmit={handleSubmit((formVals) => {
          reset();
          mutate({
            fileColInput: {
              collectionId,
              membersToShareTo: [formVals],
            },
          });
        })}
      >
        <StyledDialogTitle title="Share file members" />
        <DialogContent dividers>
          <Grid2
            container
            justifyContent="center"
            alignItems="center"
            spacing={1}
            width="100%"
          >
            <Grid2 width={"100%"}>
              <TextField
                {...register("email", { required: true })}
                disabled={isLoading}
                label="Email..."
                fullWidth
                error={!!formErrors.email}
                helperText={
                  formErrors.email ? formErrors.email.message : undefined
                }
              />
            </Grid2>
            <Grid2>
              <FormControlLabel
                label="Can view documents"
                control={
                  <Switch
                    {...register("canViewDocuments")}
                    checked={canViewDocuments}
                    defaultChecked={canViewDocuments}
                  />
                }
              />
            </Grid2>
            <Grid2>
              <FormControlLabel
                label="Can remove documents"
                control={
                  <Switch
                    {...register("canRemoveDocuments")}
                    checked={canRemoveDocuments}
                    defaultChecked={canRemoveDocuments}
                  />
                }
              />
            </Grid2>
            <Grid2>
              <FormControlLabel
                label="Can download documents"
                control={
                  <Switch
                    {...register("canDownloadDocuments")}
                    checked={canDownloadDocuments}
                    defaultChecked={canDownloadDocuments}
                  />
                }
              />
            </Grid2>
            <Grid2>
              <FormControlLabel
                label="Can create documents"
                control={
                  <Switch
                    {...register("canCreateDocuments")}
                    checked={canCreateDocuments}
                    defaultChecked={canCreateDocuments}
                  />
                }
              />
            </Grid2>
            <Grid2>
              <FormControlLabel
                label="Can similarity search"
                control={
                  <Switch
                    {...register("canSimilaritySearch")}
                    checked={canSimilaritySearch}
                    defaultChecked={canSimilaritySearch}
                  />
                }
              />
            </Grid2>
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
                disabled={isLoading}
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
