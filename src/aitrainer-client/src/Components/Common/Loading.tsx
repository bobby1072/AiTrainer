import { Grid2, Typography } from "@mui/material";
import LinearProgress from "@mui/material/LinearProgress";
import { PageBase } from "./PageBase";
export const Loading: React.FC<{ fullScreen?: boolean }> = ({
  fullScreen = false,
}) => {
  return fullScreen ? (
    <PageBase>
      <Grid2
        height={"100vh"}
        container
        justifyContent="center"
        alignItems="center"
        direction="column"
        spacing={4}
        textAlign="center"
        width="100%"
      >
        <Grid2 width="100%">
          <Typography variant="h1" fontSize={50}>
            Loading...
          </Typography>
        </Grid2>
        <Grid2 width="70%">
          <LinearProgress />
        </Grid2>
      </Grid2>
    </PageBase>
  ) : (
    <Grid2
      container
      justifyContent="center"
      alignItems="center"
      direction="column"
      width="100%"
      textAlign="center"
      spacing={2}
    >
      <Grid2 width="100%">
        <LinearProgress />
      </Grid2>
      <Grid2 width="100%">
        <Typography variant="h1" fontSize={30}>
          Loading...
        </Typography>
      </Grid2>
    </Grid2>
  );
};
