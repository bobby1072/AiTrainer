import { Grid2 } from "@mui/material";
import { FileCollection } from "../../Models/FileCollection";

export const FileCollectionTableTab: React.FC<{
  fileCollection: FileCollection;
}> = ({ fileCollection }) => {
  return (
    <Grid2
      container
      direction={"row"}
      justifyContent="center"
      alignItems="center"
      spacing={4}
      padding={1}
      width="100%"
    ></Grid2>
  );
};
