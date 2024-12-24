import { Divider, Grid2, IconButton, Paper, Typography } from "@mui/material";
import { FlatFileDocumentPartialCollection } from "../../Models/FlatFileDocumentPartialCollection";
import AddIcon from "@mui/icons-material/Add";

export const CollectionDocumentTable: React.FC<{
  flatCollection?: FlatFileDocumentPartialCollection | null;
}> = ({ flatCollection }) => {
  return (
    <Paper elevation={1}>
      <Grid2
        container
        height={"70vh"}
        overflow={"auto"}
        justifyContent="center"
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
            spacing={2}
            padding={0.5}
            width="100%"
          >
            <Grid2
              width={"10%"}
              sx={{ display: "flex", justifyContent: "flex-end" }}
            >
              <IconButton color="inherit" size="medium">
                <AddIcon />
              </IconButton>
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
            (flatCollection.fileCollections.length === 0 &&
              flatCollection.fileDocuments.length === 0) ? (
              <Grid2 width={"100%"} textAlign={"center"}>
                <Typography variant="subtitle2" fontSize={20}>
                  No documents or folders found...
                </Typography>
              </Grid2>
            ) : null}
          </Grid2>
        </Grid2>
      </Grid2>
    </Paper>
  );
};
