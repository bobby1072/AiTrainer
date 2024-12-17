import React, { useCallback, useContext, useEffect, useState } from "react";
import { User, UserManager } from "oidc-client-ts";
import { useNavigate } from "react-router-dom";
import { Loading } from "../Common/Loading";

type UserState = { targetUrl: string | undefined } | undefined;

interface IAuthenticationContext {
  user: User | null;
  isLoggedIn: boolean;
  signIn: (targetUrl?: string) => void;
  signOut: () => void;
}

export const AuthenticationContext = React.createContext<
  IAuthenticationContext | undefined
>(undefined);

interface Props {
  children?: React.ReactNode;
  clientRootHost: string;
  settings: {
    authority: string;
    client_id: string;
    scope: string;
  };
}

const isCodeCallback = () =>
  new URLSearchParams(window.location.search).has("code");

export const AuthenticationContextProvider: React.FC<Props> = ({
  children,
  clientRootHost,
  settings,
}) => {
  const navigate = useNavigate();
  const [initializing, setInitializing] = useState(true);
  const [silentRenewFailed, setSilentRenewFailed] = useState<boolean>(false);
  const [userData, setUserData] = useState<User | null>(null);
  const [userManager] = useState<UserManager>(
    new UserManager({
      ...settings,
      response_type: "code",
      redirect_uri: `${clientRootHost}/oidc-signin`,
      automaticSilentRenew: true,
      silent_redirect_uri: `${clientRootHost}/oidc-silent-renew`,
      post_logout_redirect_uri: `${clientRootHost}/login`,
    })
  );

  userManager.clearStaleState();

  useEffect(() => {
    const onAccessTokenExpired = () => {
      setUserData(null);
      userManager.signoutRedirect();
    };

    const onSilentRenewFailed = () => setSilentRenewFailed(true);

    userManager.events.addSilentRenewError(onSilentRenewFailed);
    userManager.events.addAccessTokenExpired(onAccessTokenExpired);

    return () => {
      userManager.events.removeSilentRenewError(onSilentRenewFailed);
      userManager.events.removeAccessTokenExpired(onAccessTokenExpired);
    };
  }, [userManager]);

  const onSignIn = useCallback(
    (user: User | null) => {
      setUserData(user);

      if (user) {
        const state = user.state as UserState;
        if (
          state?.targetUrl !== undefined &&
          window.location.pathname !== state?.targetUrl
        ) {
          navigate(state.targetUrl, { replace: true });
        } else if (isCodeCallback()) {
          // remove code query param
          navigate(window.location.pathname, { replace: true });
        }
      }
    },
    [navigate]
  );

  useEffect(() => {
    const signInSilent = async () => {
      try {
        if (!silentRenewFailed) {
          const user = await userManager.signinSilent({
            silentRequestTimeoutInSeconds: 4,
          });
          onSignIn(user);
        }
      } catch (error) {
        setSilentRenewFailed(true);
      }
    };

    const getSession = async () => {
      try {
        const user = await userManager.getUser();
        onSignIn(user);

        if (user === null) await signInSilent();
      } finally {
        setInitializing(false);
      }
    };

    if (isCodeCallback()) {
      userManager
        .signinCallback()
        .then((user) => {
          onSignIn(user as User);
          setInitializing(false);
        })
        .catch((error) => {
          // Sometimes this returns 'invalid_grant' when the user is already logged in
        });
    } else {
      getSession();
    }
  }, [onSignIn, silentRenewFailed, userManager]);

  const signIn = useCallback(
    (targetUrl?: string) => {
      userManager.signinRedirect({ state: { targetUrl } });
    },
    [userManager]
  );

  if (initializing) return <Loading fullScreen={true} />;

  return (
    <AuthenticationContext.Provider
      value={{
        user: userData,
        isLoggedIn: !!userData,
        signIn,
        signOut: () => {
          userManager?.signoutRedirect();
        },
      }}
    >
      {children}
    </AuthenticationContext.Provider>
  );
};

export const useAuthentication = () => {
  const context = useContext(AuthenticationContext);
  if (!context)
    throw new Error(
      `${AuthenticationContext.displayName} has not been registered`
    );

  return context;
};
