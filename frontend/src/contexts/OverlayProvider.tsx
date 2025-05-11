import { createContext, useContext, useState } from "react";

interface IOverlayContext {
  show: boolean;
  showOverlay: () => void;
  hideOverlay: () => void;
}

const OverlayContext = createContext<IOverlayContext | undefined>(undefined);

export default function OverlayProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const [show, setShow] = useState(false);

  const showOverlay = () => setShow(true);
  const hideOverlay = () => setShow(false);

  return (
    <OverlayContext.Provider value={{ show, showOverlay, hideOverlay }}>
      {children}
    </OverlayContext.Provider>
  );
}

export function useOverlay(): IOverlayContext {
  const context = useContext(OverlayContext);

  if (!context)
    throw new Error("useoverlay must be used within an OverlayProvider");

  return context;
}
