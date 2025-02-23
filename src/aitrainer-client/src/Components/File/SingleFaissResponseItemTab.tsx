import { Grid2, Paper, Typography } from "@mui/material";
import { SimilaritySearchResponseItem } from "../../Models/SimilaritySearchResponse";

export const SingleFaissResponseItemTab: React.FC<{
  responseItem: SimilaritySearchResponseItem;
}> = ({ responseItem }) => {
  return (
    <Paper
      elevation={1}
      sx={{
        border: "1px solid #ccc",
        borderRadius: "8px",
        overflow: "hidden",
      }}
    >
      <Grid2 container direction="column" spacing={2} padding={1} width="100%">
        <Grid2
          width={"100%"}
          sx={{
            textAlign: "left",
          }}
        >
          <Typography variant="subtitle2">
            {responseItem.pageContent}
          </Typography>
        </Grid2>
      </Grid2>
    </Paper>
  );
};
