import { Grid2 } from "@mui/material";
import { PageBase } from "../Common/PageBase";
import { useGetTopLayerOfFile } from "../../Hooks/useGetTopLayerOfFile";

export const TestHome: React.FC = () => {
  const { data, error, isLoading } = useGetTopLayerOfFile();
  return (
    <PageBase>
      <Grid2
        container
        height={"100vh"}
        justifyContent="center"
        alignItems="center"
        direction="column"
        spacing={4}
        textAlign="center"
        width="100%"
      >
        <Grid2 width={"50%"}>
          <h1>Test Home</h1>
        </Grid2>
      </Grid2>
    </PageBase>
  );
};
