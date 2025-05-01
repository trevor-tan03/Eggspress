interface Props {
  open: boolean;
  onClose: () => void;
  ariaLabelledBy?: string;
  ariaDescribedBy?: string;
  children: React.ReactNode;
}

export default function Modal({
  open,
  onClose,
  ariaLabelledBy,
  ariaDescribedBy,
  children,
}: Props) {
  return (
    <>
      {open && (
        <div
          className="absolute top-0 w-dvw h-dvh z-50 grid place-items-center"
          aria-labelledby={ariaLabelledBy}
          aria-describedby={ariaDescribedBy}
        >
          {children}
          <div
            className="absolute top-0 w-dvw h-dvh bg-[rgba(0,0,0,0.5)]"
            onClick={onClose}
          ></div>
        </div>
      )}
    </>
  );
}
