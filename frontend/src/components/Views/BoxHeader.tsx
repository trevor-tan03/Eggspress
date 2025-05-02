interface Props {
  code: string;
  selected: "explorer" | "notepad";
  setSelected: React.Dispatch<React.SetStateAction<"explorer" | "notepad">>;
}

export default function BoxHeader({ code, selected, setSelected }: Props) {
  return (
    <div className="pt-12">
      <h1 className="text-4xl font-extrabold text-white text-center mb-6">
        Box: {code}
      </h1>
      <div className="flex mt-3 items-center justify-center">
        <div className="p-3 border-2 border-red-300 rounded-md w-60 text-center text-red-300 font-bold border-r-0 rounded-r-none">
          Eggsplodes In: <span>9m 59s</span>
        </div>
        <button
          className="p-3 border-2 bg-red-300 border-red-300 rounded-md h-[52px] hover:text-white hover:bg-red-500 transition-colors duration-200 cursor-pointer w-14 grid place-items-center rounded-l-none"
          data-tooltip-id="my-tooltip"
          data-tooltip-content="Detonate Box"
        >
          <img
            src="../../../public/explosive-dark.png"
            alt="explode"
            width="20"
            height="20"
          />
        </button>
      </div>
      <nav className="p-3 flex gap-3 items-center justify-center mt-6">
        <a
          className="bg-gray-200 p-3 rounded-md border hover:bg-yellow-200 cursor-pointer flex-1 text-center"
          style={{
            background:
              selected === "explorer" ? "oklch(85.2% 0.199 91.936)" : "",
          }}
          onClick={() => {
            history.replaceState(null, "", "#explorer");
            setSelected("explorer");
          }}
        >
          File Explorer
        </a>
        <a
          className="bg-gray-200 p-3 rounded-md border hover:bg-yellow-200 cursor-pointer flex-1 text-center"
          style={{
            background:
              selected === "notepad" ? "oklch(85.2% 0.199 91.936)" : "",
          }}
          onClick={() => {
            history.replaceState(null, "", "#notepad");
            setSelected("notepad");
          }}
        >
          Notepad
        </a>
      </nav>
    </div>
  );
}
