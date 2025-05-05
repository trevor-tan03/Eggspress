import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { Tooltip } from "react-tooltip";
import type { IFile } from "../../components/FileUpload";
import FileUpload from "../../components/FileUpload";
import BoxHeader from "../../components/Views/BoxHeader";
import BoxNotExist from "../../components/Views/BoxNotExist";

export const Route = createFileRoute("/box/$code")({
  component: RouteComponent,
});

function RouteComponent() {
  const [exists, setExists] = useState(false);
  const [uploadedFiles, setUploadedFiles] = useState<IFile[]>([]);
  const { code } = Route.useParams();

  async function checkBoxExists(code: string) {
    const res = await fetch(
      `${import.meta.env.VITE_BACKEND_API}/api/box/${code}`
    );

    setExists(res.ok);

    if (res.ok) getBoxFiles();
  }

  async function getBoxFiles() {
    const res = await fetch(
      `${import.meta.env.VITE_BACKEND_API}/api/box/${code}/files`
    );

    setUploadedFiles(await res.json());
  }

  useEffect(() => {
    checkBoxExists(code);
  }, [code]);

  if (!exists) return <BoxNotExist />;

  return (
    <div>
      <Tooltip id="my-tooltip" />
      <BoxHeader code={code} />
      <FileUpload
        code={code}
        uploadedFiles={uploadedFiles}
        setUploadedFiles={setUploadedFiles}
      />
    </div>
  );
}
