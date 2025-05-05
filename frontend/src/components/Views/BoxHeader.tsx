interface Props {
  code: string;
}

async function DeleteBox(code: string) {
  const res = await fetch(
    `${import.meta.env.VITE_BACKEND_API}/api/box/${code}/delete`,
    {
      method: "DELETE",
    }
  );

  if (res.ok) window.location.reload();
}

export default function BoxHeader({ code }: Props) {
  return (
    <div className="pt-12">
      <h1 className="text-4xl font-extrabold text-white text-center mb-6">
        Box: {code}
      </h1>
      <div className="flex mt-3 items-center justify-center">
        <div className="p-3 border-2 border-yellow-400 rounded-md w-60 text-center text-yellow-400 font-bold border-r-0 rounded-r-none">
          Eggsplodes In: <span>9m 59s</span>
        </div>
        <button
          className="p-3 border-2 bg-yellow-400 border-yellow-400 rounded-md h-[52px] hover:text-white hover:bg-red-400 transition-colors duration-200 cursor-pointer w-14 grid place-items-center rounded-l-none"
          data-tooltip-id="my-tooltip"
          data-tooltip-content="Detonate Box"
          onClick={() => DeleteBox(code)}
        >
          <img
            src="../../../public/explosive-dark.png"
            alt="explode"
            width="20"
            height="20"
          />
        </button>
      </div>
    </div>
  );
}
