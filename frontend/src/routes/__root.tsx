import { Outlet, createRootRoute } from "@tanstack/react-router";
import * as React from "react";
import PageNotFound from "../components/Views/PageNotFound";

export const Route = createRootRoute({
  component: RootComponent,
  notFoundComponent: () => {
    return <PageNotFound />;
  },
});

function RootComponent() {
  return (
    <React.Fragment>
      <Outlet />
    </React.Fragment>
  );
}
