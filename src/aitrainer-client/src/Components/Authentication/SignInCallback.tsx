import React, { Fragment, useEffect } from "react";
import { Card, CardContent, Grid, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useAuthentication } from "../Contexts/AuthenticationContext";
import { Loading } from "../Common/Loading";

export const SignInCallback: React.FC = () => {
  const { user } = useAuthentication();
  const push = useNavigate();

  useEffect(() => {
    if (user) {
      push("/Home");
    }
  }, [user, push]);

  return (
    <Grid container justifyContent="center" alignContent="center">
      <Grid item xs={12} sm={9} md={8} lg={8}>
        <Card>
          {user ? (
            <Loading fullScreen />
          ) : (
            <CardContent>
              <Fragment>
                <Typography
                  component="h1"
                  variant="h4"
                  color="primary"
                  gutterBottom
                >
                  Access Denied
                </Typography>
                <Typography>Please contact an administrator.</Typography>
              </Fragment>
            </CardContent>
          )}
        </Card>
      </Grid>
    </Grid>
  );
};
