import { Box, ButtonBase, Grid2, Tooltip, Typography } from "@mui/material";
import { FileCollection } from "../../Models/FileCollection";
const fileCol = require("./fileCol.png");

export const FileCollectionTableTab: React.FC<{
  fileCollection: FileCollection;
}> = ({ fileCollection }) => {
  const dateCreated = new Date(fileCollection.dateCreated);
  const dateModified = new Date(fileCollection.dateModified);
  return (
    <Grid2
      container
      direction={"row"}
      justifyContent="center"
      alignItems="center"
      spacing={4}
      padding={1}
      width="100%"
    >
      <Grid2 width={"2.5%"}>
        <Box
          component="img"
          sx={{
            width: "100%",
          }}
          src={fileCol}
          alt={`fileColImage: ${fileCollection.id}`}
        />
      </Grid2>
      <Grid2
        width={"55%"}
        sx={{ display: "flex", justifyContent: "flex-start" }}
      >
        <ButtonBase
          href={`/collection/home/${fileCollection.id}`}
          sx={{
            "&:hover": {
              textDecoration: "underline",
            },
          }}
        >
          <Typography variant="subtitle2" fontSize={22}>
            {fileCollection.collectionName}
          </Typography>
        </ButtonBase>
      </Grid2>
      <Grid2 width={"15%"}>
        <Tooltip title={`${dateCreated.toISOString()}`}>
          <Typography variant="subtitle2" fontSize={18}>
            {dateCreated.toDateString()}
          </Typography>
        </Tooltip>
      </Grid2>
      <Grid2 width={"15%"}>
        <Tooltip title={`${dateCreated.toISOString()}`}>
          <Typography variant="subtitle2" fontSize={18}>
            {dateModified.toDateString()}
          </Typography>
        </Tooltip>
      </Grid2>
    </Grid2>
  );
};
