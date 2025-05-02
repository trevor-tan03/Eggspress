import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { Tooltip } from "react-tooltip";
import FileExplorer from "../../components/FileExplorer";
import Notepad from "../../components/Notepad";
import BoxHeader from "../../components/Views/BoxHeader";
import BoxNotExist from "../../components/Views/BoxNotExist";

export const Route = createFileRoute("/box/$code")({
  component: RouteComponent,
});

function getView() {
  const hash = window.location.hash.substring(1);
  return hash;
}

function RouteComponent() {
  const [selected, setSelected] = useState<"explorer" | "notepad">(
    getView() === "notepad" ? "notepad" : "explorer"
  );
  const [exists, setExists] = useState(false);
  const { code } = Route.useParams();

  useEffect(() => {
    function handleHashChange() {
      setSelected(getView() === "notepad" ? "notepad" : "explorer");
    }

    window.addEventListener("hashchange", handleHashChange);
    handleHashChange();

    return () => {
      window.removeEventListener("hashchange", handleHashChange);
    };
  }, []);

  async function checkBoxExists(code: string) {
    const res = await fetch(`http://localhost:5120/api/box/${code}`);
    const boxExists = await res.text();
    setExists(boxExists === "true");
  }

  useEffect(() => {
    checkBoxExists(code);
  }, [code]);

  if (!exists) return <BoxNotExist />;

  return (
    <div>
      <Tooltip id="my-tooltip" />
      <BoxHeader code={code} selected={selected} setSelected={setSelected} />
      <div className="p-3">
        {selected === "notepad" ? <Notepad /> : <FileExplorer />}
      </div>
    </div>
  );
}
