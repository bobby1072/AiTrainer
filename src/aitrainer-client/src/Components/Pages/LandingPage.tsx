import { Button, Grid2, Paper } from "@mui/material";
import { PageBase } from "../Common/PageBase";
import { useAuthentication } from "../Contexts/AuthenticationContext";
import { Navigate, useLocation } from "react-router-dom";
export const LandingPage: React.FC = () => {
  const { user, signIn } = useAuthentication();
  const location = useLocation();
  const targetUrl =
    (location.state as { targetUrl?: string } | undefined)?.targetUrl || "";

  if (user) return <Navigate to={targetUrl} />;
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
        height={"100vh"}
      >
        <Grid2 width={"15%"}>
          <Paper>
            <Button
              variant="contained"
              color="primary"
              fullWidth
              onClick={() => signIn(targetUrl)}
              sx={{ fontWeight: 700 }}
            >
              Login
            </Button>
          </Paper>
        </Grid2>
      </Grid2>
    </PageBase>
  );
};
