import { Route, RouteProps } from "react-router-dom";
import React from "react";
import { UnauthorizedPage } from "./pages/UnauthorizedPage";
import { LoadingPage } from "./pages/LoadingPage";

export type AuthorizedRouteProps = RouteProps & {
  authorized: boolean;
  loaded: boolean;
};

export function AuthorizedRoute(props: AuthorizedRouteProps) {
  return (
    <div>
      { props.loaded ? props.authorized ? (
        <Route {...props} />
      ) : <Route path={props.path} component={UnauthorizedPage} />
      : <Route path={props.path} component={LoadingPage}/> }
    </div>
  )
}