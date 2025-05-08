import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { Tooltip } from "react-tooltip";
import FileUploadArea from "../../components/FileUploadArea";
import BoxHeader from "../../components/Views/BoxHeader";
import BoxNotExist from "../../components/Views/BoxNotExist";
import { BoxDTO } from "../../types/BoxTypes";

export const Route = createFileRoute("/box/$code")({
  component: RouteComponent,
});

function RouteComponent() {
  const [boxDetails, setBoxDetails] = useState<BoxDTO | null>(null);
  const { code } = Route.useParams();

  async function getBoxDetails(code: string) {
    try {
      const res = await fetch(
        `${import.meta.env.VITE_BACKEND_API}/api/box/${code}`
      );

      if (!res.ok) throw new Error(`${await res.text()}`);

      const data = (await res.json()) as BoxDTO;
      setBoxDetails(data);
    } catch (err) {
      console.error(
        "An error occurred while fetching box details: " +
          (err as Error).message
      );
    }
  }

  useEffect(() => {
    getBoxDetails(code);
  }, [code]);

  if (!boxDetails) return <BoxNotExist />;

  return (
    <div>
      <Tooltip id="my-tooltip" />
      <BoxHeader code={code} expiresAt={boxDetails.expiresAt} />
      <FileUploadArea code={code} originalFiles={boxDetails.files} />
    </div>
  );
}
