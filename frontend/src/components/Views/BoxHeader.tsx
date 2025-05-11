import BoxTimer from "../BoxTimer";

interface Props {
  code: string;
  expiresAt: string;
  setExpired: React.Dispatch<React.SetStateAction<boolean>>;
}

export default function BoxHeader({ code, expiresAt, setExpired }: Props) {
  async function DeleteBox(code: string) {
    const res = await fetch(
      `${import.meta.env.VITE_BACKEND_API}/api/box/${code}/delete`,
      {
        method: "DELETE",
      }
    );

    if (res.ok) setExpired(true);
  }

  return (
    <div className="pt-12">
      <h1 className="text-4xl font-extrabold text-white text-center mb-6">
        Box: {code}
      </h1>
      <div className="flex mt-3 items-center justify-center">
        <BoxTimer expiresAt={expiresAt} setExpired={setExpired} />
        <button
          className="p-3 border-2 bg-yellow-400 border-yellow-400 rounded-md h-[52px] hover:text-white hover:bg-red-400 transition-colors duration-200 cursor-pointer w-14 grid place-items-center rounded-l-none"
          data-tooltip-id="my-tooltip"
          data-tooltip-content="Detonate Box"
          onClick={() => DeleteBox(code)}
        >
          <img src="/explosive-dark.png" alt="explode" width="20" height="20" />
        </button>
      </div>
    </div>
  );
}
