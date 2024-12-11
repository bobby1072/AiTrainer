import { Alert, Grid2, Typography } from "@mui/material";
import { PageBase } from "./PageBase";
import ErrorIcon from "@mui/icons-material/Error";

export const ErrorComponent: React.FC<{
  fullScreen?: boolean;
  errorMessage?: string;
  fontSize?: number;
}> = ({ fullScreen = false, errorMessage, fontSize = 30 }) => {
  return fullScreen ? (
    <PageBase>
      <Grid2
        container
        justifyContent="center"
        alignItems="center"
        direction="column"
        spacing={4}
        textAlign="center"
        width="100%"
      >
        <Grid2 width={"70%"}>
          <Alert
            severity="error"
            icon={<ErrorIcon color="inherit" fontSize="large" />}
          >
            <Typography fontSize={fontSize}>
              {errorMessage ? errorMessage : "An error has occurred"}
            </Typography>
          </Alert>
        </Grid2>
      </Grid2>
    </PageBase>
  ) : (
    <Grid2
      container
      justifyContent="center"
      alignItems="center"
      direction="column"
      spacing={4}
      textAlign="center"
      width="100%"
    >
      <Grid2 width={"70%"}>
        <Alert
          severity="error"
          icon={<ErrorIcon color="inherit" fontSize="inherit" />}
        >
          <Typography fontSize={fontSize}>{errorMessage}</Typography>
        </Alert>
      </Grid2>
    </Grid2>
  );
};
