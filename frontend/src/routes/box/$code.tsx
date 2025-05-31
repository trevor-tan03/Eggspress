import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { Tooltip } from "react-tooltip";
import FileUploadArea from "../../components/FileUploadArea";
import Loading from "../../components/Loading";
import BoxEggsploded from "../../components/Views/BoxEggsploded";
import BoxHeader from "../../components/Views/BoxHeader";
import BoxNotExist from "../../components/Views/BoxNotExist";
import { BoxDTO } from "../../types/BoxTypes";

export const Route = createFileRoute("/box/$code")({
  component: RouteComponent,
});

function RouteComponent() {
  const [loading, setLoading] = useState(true);
  const [expired, setExpired] = useState(false);
  const [boxDetails, setBoxDetails] = useState<BoxDTO | null>(null);
  const { code } = Route.useParams();

  async function getBoxDetails(code: string) {
    setLoading(true);
    try {
      const res = await fetch(
        `${import.meta.env.VITE_BACKEND_API}/api/box/${code}`,
        {
          credentials: "include",
        }
      );

      if (!res.ok) throw new Error(`${await res.text()}`);

      const data = (await res.json()) as BoxDTO;
      setBoxDetails(data);
    } catch (err) {
      console.error(
        "An error occurred while fetching box details: " +
          (err as Error).message
      );
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    getBoxDetails(code);
  }, [code]);

  if (loading) return <Loading />;
  else if (!boxDetails) return <BoxNotExist />;
  else if (expired) return <BoxEggsploded />;

  return (
    <div className="p-3">
      <Tooltip id="my-tooltip" />
      <BoxHeader
        code={code}
        expiresAt={boxDetails.expiresAt}
        setExpired={setExpired}
      />
      <FileUploadArea code={code} originalFiles={boxDetails.files} />
    </div>
  );
}
