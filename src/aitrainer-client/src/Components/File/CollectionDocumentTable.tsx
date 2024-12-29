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
import { FileCollectionTableTab } from "./FileCollectionTableTab";

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
              <Grid2
                width={"100%"}
                sx={{ display: "flex", justifyContent: "flex-end" }}
              >
                <Tooltip title="Add new folder">
                  <IconButton
                    color="inherit"
                    size="large"
                    onClick={() => setAddModalOpen(true)}
                  >
                    <AddIcon />
                  </IconButton>
                </Tooltip>
              </Grid2>
              <Grid2 width="2%"></Grid2>
              <Grid2
                width="54%"
                sx={{ display: "flex", justifyContent: "flex-start" }}
              >
                <Typography variant="subtitle2" fontSize={16}>
                  Name
                </Typography>
              </Grid2>
              <Grid2 width="16%">
                <Typography variant="subtitle2" fontSize={16}>
                  Date created
                </Typography>
              </Grid2>
              <Grid2 width="3%" />
              <Grid2 width="12%">
                <Typography variant="subtitle2" fontSize={16}>
                  Date modified
                </Typography>
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
              ((flatCollection.fileCollections?.length ?? 0) < 1 &&
                (flatCollection.fileDocuments?.length ?? 0) < 1) ? (
                <Grid2 width={"100%"} textAlign={"center"}>
                  <Typography variant="subtitle2" fontSize={30}>
                    No documents or folders found...
                  </Typography>
                </Grid2>
              ) : (
                flatCollection.fileCollections.map((x) => (
                  <Grid2 width={"100%"}>
                    <FileCollectionTableTab fileCollection={x} />
                  </Grid2>
                ))
              )}
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
