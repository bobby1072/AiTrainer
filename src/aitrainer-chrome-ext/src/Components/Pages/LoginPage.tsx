import { Grid2, Typography } from "@mui/material";
import { useGetClientConfigurationQuery } from "../../Hooks/useGetClientConfigurationQuery";
import { ErrorComponent } from "../Common/ErrorComponent";
import { Loading } from "../Common/Loading";
import { PageBase } from "../Common/PageBase";
export const LoginPage: React.FC = () => {
  const { isLoading, error, data } = useGetClientConfigurationQuery();

  if (isLoading) return <Loading fullScreen />;
  else if (error)
    return <ErrorComponent fullScreen errorMessage={error.message} />;
  else if (!data) return <ErrorComponent fullScreen />;
  return (
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
        {Object.entries(data).map(([key, val]) => (
          <Grid2 width={"100%"}>
            <Typography key={key}>
              {key}: {val}
            </Typography>
          </Grid2>
        ))}
      </Grid2>
    </PageBase>
  );
};
