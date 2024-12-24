import {
  Divider,
  Grid2,
  IconButton,
  Paper,
  Tooltip,
  Typography,
} from "@mui/material";
import { FlatFileDocumentPartialCollection } from "../../Models/FlatFileDocumentPartialCollection";
import AddIcon from "@mui/icons-material/Add";
import { useState } from "react";
import { AddFileCollectionModal } from "./AddFileCollectionModal";

export const CollectionDocumentTable: React.FC<{
  flatCollection?: FlatFileDocumentPartialCollection | null;
}> = ({ flatCollection }) => {
  const [addModalOpen, setAddModalOpen] = useState<boolean>(false);
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
          spacing={4}
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
              {flatCollection?.self?.collectionName && (
                <Grid2
                  width={"90%"}
                  sx={{ display: "flex", justifyContent: "flex-start" }}
                >
                  <Typography gutterBottom variant="subtitle2" fontSize={20}>
                    {`Parent Folder: ${flatCollection?.self?.collectionName}`}
                  </Typography>
                </Grid2>
              )}
              <Grid2
                width={flatCollection?.self?.collectionName ? "10%" : "100%"}
                sx={{ display: "flex", justifyContent: "flex-end" }}
              >
                <Tooltip title="Add new folder">
                  <IconButton
                    color="inherit"
                    size="small"
                    onClick={() => setAddModalOpen(true)}
                  >
                    <AddIcon />
                  </IconButton>
                </Tooltip>
              </Grid2>
            </Grid2>
          </Grid2>
          <Grid2 width={"100%"}>
            <Divider />
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
              ((flatCollection.fileCollections?.length ?? 0) === 0 &&
                (flatCollection.fileDocuments?.length ?? 0) === 0) ? (
                <Grid2 width={"100%"} textAlign={"center"}>
                  <Typography variant="subtitle2" fontSize={30}>
                    No documents or folders found...
                  </Typography>
                </Grid2>
              ) : null}
            </Grid2>
          </Grid2>
        </Grid2>
      </Paper>
      {addModalOpen && (
        <AddFileCollectionModal closeModal={() => setAddModalOpen(false)} />
      )}
    </>
  );
};
