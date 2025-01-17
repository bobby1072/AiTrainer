import { Grid2, IconButton, Paper, Tooltip, Typography } from "@mui/material";
import { FlatFileDocumentPartialCollection } from "../../Models/FlatFileDocumentPartialCollection";
import AddIcon from "@mui/icons-material/Add";
import FileUploadIcon from "@mui/icons-material/FileUpload";
import { useEffect, useState } from "react";
import { SaveFileCollectionModal } from "./SaveFileCollectionModal";
import { AddFileDocumentModal } from "./AddFileDocumentModal";
import { NewFileTable } from "./NewFileTable";
import { useFileCollectionLevelContext } from "../Contexts/FileCollectionLevelContext";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { useNavigate } from "react-router-dom";

export const CollectionDocumentTable: React.FC<{
  flatCollection: FlatFileDocumentPartialCollection;
}> = ({ flatCollection }) => {
  const [addFileCollectionModalOpen, setAddFileCollectionModalOpen] =
    useState<boolean>(false);

  const [addFileDocumentModalOpen, setAddFileDocumentModalOpen] =
    useState<boolean>(false);

  const navigate = useNavigate();
  const { fileColId, setFileColId } = useFileCollectionLevelContext();

  useEffect(() => {
    setFileColId(flatCollection?.self?.parentId ?? "");
  }, [flatCollection, setFileColId]);

  return (
    <>
      <Paper
        elevation={1}
        sx={{
          border: "1px solid #ccc",
          borderRadius: "8px",
          overflow: "hidden",
        }}
      >
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
            <Grid2
              container
              justifyContent="center"
              alignItems="center"
              direction="row"
              width="100%"
            >
              {flatCollection.self && (
                <Grid2 width={"3%"}>
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
              <Grid2
                width={"93%"}
                sx={{ display: "flex", justifyContent: "flex-end" }}
              >
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
              <Grid2
                width={"3%"}
                sx={{ display: "flex", justifyContent: "flex-end" }}
              >
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
                  <NewFileTable flatCollection={flatCollection} />
                </Grid2>
              )}
            </Grid2>
          </Grid2>
        </Grid2>
      </Paper>
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
