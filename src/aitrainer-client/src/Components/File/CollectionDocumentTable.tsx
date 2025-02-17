import { Grid2, IconButton, Tooltip, Typography } from "@mui/material";
import { FlatFileDocumentPartialCollection } from "../../Models/FlatFileDocumentPartialCollection";
import AddIcon from "@mui/icons-material/Add";
import FileUploadIcon from "@mui/icons-material/FileUpload";
import { useEffect, useState } from "react";
import { SaveFileCollectionModal } from "./SaveFileCollectionModal";
import { AddFileDocumentModal } from "./AddFileDocumentModal";
import { ActualFileCollectionTable } from "./ActualFileCollectionTable";
import { useFileCollectionLevelContext } from "../Contexts/FileCollectionLevelContext";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { useNavigate } from "react-router-dom";
import SyncIcon from "@mui/icons-material/Sync";
import { useSignalRFileCollectionFaissSyncMutation } from "../../Hooks/useSignalRFileCollectionFaissSyncMutation";
import { useSnackbar } from "notistack";

export const CollectionDocumentTable: React.FC<{
  flatCollection: FlatFileDocumentPartialCollection;
  singleLevel?: boolean;
}> = ({ flatCollection, singleLevel = false }) => {
  const [addFileCollectionModalOpen, setAddFileCollectionModalOpen] =
    useState<boolean>(false);

  const [addFileDocumentModalOpen, setAddFileDocumentModalOpen] =
    useState<boolean>(false);
  const { enqueueSnackbar } = useSnackbar();
  const navigate = useNavigate();
  const {
    data: syncData,
    error: syncError,
    isLoading: syncLoading,
    mutate: sync,
  } = useSignalRFileCollectionFaissSyncMutation(flatCollection);
  const { fileColId, setFileColId } = useFileCollectionLevelContext();
  useEffect(() => {
    if (syncData) {
      enqueueSnackbar("Successfully faiss synced file collection", {
        variant: "success",
      });
    }
  }, [enqueueSnackbar, syncData]);
  useEffect(() => {
    if (syncError) {
      enqueueSnackbar("Failed to faiss sync file collection", {
        variant: "error",
      });
    }
  }, [enqueueSnackbar, syncError]);
  useEffect(() => {
    setFileColId(flatCollection?.self?.parentId ?? "");
  }, [flatCollection, setFileColId]);

  const syncDisabled =
    syncLoading ||
    !!syncData ||
    flatCollection?.fileDocuments?.length === 0 ||
    flatCollection?.fileDocuments?.every((x) => x.faissSynced);

  return (
    <>
      <Grid2
        container
        height={"90vh"}
        overflow={"auto"}
        alignItems="center"
        direction="column"
        spacing={1}
        padding={1}
        textAlign="center"
        width="100%"
      >
        <Grid2 width={"100%"}>
          <Grid2 container alignItems="center" direction="row" width="100%">
            <Grid2 width={"50%"}>
              <Grid2
                container
                justifyContent="flex-start"
                alignItems="center"
                direction="row"
                width="100%"
              >
                {flatCollection.self && !singleLevel && (
                  <Grid2>
                    <Tooltip title="Go back">
                      <IconButton
                        color="inherit"
                        size="large"
                        onClick={(e) => {
                          e.preventDefault();
                          navigate(
                            fileColId
                              ? `/collection/home/${fileColId}`
                              : "/collection/home"
                          );
                        }}
                        href={
                          fileColId
                            ? `/collection/home/${fileColId}`
                            : "/collection/home"
                        }
                      >
                        <ArrowBackIcon />
                      </IconButton>
                    </Tooltip>
                  </Grid2>
                )}
              </Grid2>
            </Grid2>
            <Grid2 width={"50%"}>
              <Grid2
                container
                justifyContent="flex-end"
                alignItems="center"
                direction="row"
                width="100%"
                spacing={0.1}
              >
                <Grid2>
                  <Tooltip title="Sync this collection so similarity search includes currently unsynced documents">
                    <IconButton
                      disabled={syncDisabled}
                      color="inherit"
                      size="large"
                      onClick={() => sync()}
                    >
                      <SyncIcon />
                    </IconButton>
                  </Tooltip>
                </Grid2>
                <Grid2>
                  <Tooltip title="Upload new document">
                    <IconButton
                      color="inherit"
                      size="large"
                      onClick={() => setAddFileDocumentModalOpen(true)}
                    >
                      <FileUploadIcon />
                    </IconButton>
                  </Tooltip>
                </Grid2>
                <Grid2>
                  <Tooltip title="Add new folder">
                    <IconButton
                      color="inherit"
                      size="large"
                      onClick={() => setAddFileCollectionModalOpen(true)}
                    >
                      <AddIcon />
                    </IconButton>
                  </Tooltip>
                </Grid2>
              </Grid2>
            </Grid2>
          </Grid2>
        </Grid2>
        <Grid2 width={"100%"}>
          <Grid2
            container
            justifyContent="center"
            alignItems="center"
            direction="column"
            spacing={2}
            padding={0.5}
            width="100%"
          >
            {!flatCollection ||
            ((flatCollection.fileCollections?.length ?? 0) < 1 &&
              (flatCollection.fileDocuments?.length ?? 0) < 1) ? (
              <Grid2 width={"100%"} textAlign={"center"}>
                <Typography variant="subtitle2" fontSize={30}>
                  No documents or folders found...
                </Typography>
              </Grid2>
            ) : (
              <Grid2 width={"100%"}>
                <ActualFileCollectionTable
                  singleLevel={singleLevel}
                  flatCollection={flatCollection}
                />
              </Grid2>
            )}
          </Grid2>
        </Grid2>
      </Grid2>
      {addFileCollectionModalOpen && (
        <SaveFileCollectionModal
          closeModal={() => setAddFileCollectionModalOpen(false)}
          fileCollInput={{
            parentId: flatCollection?.self?.id,
          }}
        />
      )}
      {addFileDocumentModalOpen && (
        <AddFileDocumentModal
          closeModal={() => setAddFileDocumentModalOpen(false)}
          collectionId={flatCollection?.self?.id}
        />
      )}
    </>
  );
};
