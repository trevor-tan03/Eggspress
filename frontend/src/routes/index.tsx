import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import Box from "../components/Box";
import CreateForm from "../components/CreateForm";
import Modal from "../components/Modal";
import OpenForm from "../components/OpenForm";

export const Route = createFileRoute("/")({
  component: RouteComponent,
});

function RouteComponent() {
  const [menu, setMenu] = useState<"create" | "open" | undefined>(undefined);
  const [showOverlay, setShowOverlay] = useState(false);
  const handleClose = () => {
    setShowOverlay(false);
    setMenu(undefined);
  };
  const handleOpen = () => setShowOverlay(true);

  return (
    <div className="pt-20 relative h-dvh">
      <div className="grid place-items-center gap-2">
        <h1 className="font-extrabold text-5xl text-white">Eggspress 🐣</h1>
        <p className="text-gray-300 mb-3">
          Temporarily store files for up to 10 minutes
        </p>
        <button
          className="border-2 border-black py-2 px-6 rounded-md w-40 bg-yellow-400 cursor-pointer border-b-4 hover:bg-yellow-200 transition-colors duration-200 text-center"
          onClick={() => {
            handleOpen();
            setMenu("create");
          }}
        >
          Create
        </button>

        <button
          className="border-2 border-black py-2 px-6 rounded-md w-40 bg-yellow-400 cursor-pointer border-b-4 hover:bg-yellow-200 transition-colors duration-200 text-center"
          onClick={() => {
            handleOpen();
            setMenu("open");
          }}
        >
          Open
        </button>
      </div>

      <Modal open={showOverlay} onClose={handleClose}>
        <Box>{menu === "create" ? <CreateForm /> : <OpenForm />}</Box>
      </Modal>
    </div>
  );
}
