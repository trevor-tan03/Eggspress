import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import Box from "../components/Box";
import Modal from "../components/Modal";

export const Route = createFileRoute("/")({
  component: RouteComponent,
});

function RouteComponent() {
  const [showOverlay, setShowOverlay] = useState(false);
  const handleClose = () => setShowOverlay(false);
  const handleOpen = () => setShowOverlay(true);

  return (
    <div className="pt-20 relative h-dvh">
      <div className="grid place-items-center gap-2">
        <h1 className="font-extrabold text-5xl text-white">Eggspress 🐣</h1>
        <p className="text-gray-300 mb-3">
          Temporarily store files for up to 10 minutes
        </p>
        <a
          className="border-2 border-black py-2 px-6 rounded-md w-40 bg-yellow-400 cursor-pointer border-b-4 hover:bg-yellow-200 transition-colors duration-200 text-center"
          href="/create"
        >
          Create
        </a>

        <button
          className="border-2 border-black py-2 px-6 rounded-md w-40 bg-yellow-400 cursor-pointer border-b-4 hover:bg-yellow-200 transition-colors duration-200 text-center"
          onClick={handleOpen}
        >
          Open
        </button>
      </div>

      <Modal open={showOverlay} onClose={handleClose}>
        <Box>
          <form>
            <h1 className="text-center text-3xl font-bold">
              Enter Box Details
            </h1>
            <div className="grid">
              <label htmlFor="code">Code</label>
              <input
                id="code"
                className="border-2 border-black rounded-md p-2"
                type="text"
              />
            </div>

            <div className="grid">
              <label htmlFor="password">Password</label>
              <input
                id="password"
                className="border-2 border-black rounded-md p-2 tracking-widest"
                type="password"
              />
            </div>

            <button
              type="submit"
              className="p-2 border-2 border-b-4 border-black rounded-md mt-3 w-full bg-yellow-400 hover:bg-yellow-200 transition-colors duration-200 cursor-pointer"
            >
              Submit
            </button>
          </form>
        </Box>
      </Modal>
    </div>
  );
}
