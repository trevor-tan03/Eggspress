import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/box/$code")({
  component: RouteComponent,
});

function RouteComponent() {
  const { code } = Route.useParams();
  return <div>Hello "/box/{code}"!</div>;
}
