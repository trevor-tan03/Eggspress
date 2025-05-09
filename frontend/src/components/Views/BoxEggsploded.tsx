export default function BoxExpired() {
  return (
    <div className="p-3 pt-20">
      <div className="grid place-items-center">
        <h1 className="text-4xl font-extrabold text-white">
          Box has eggsploded
        </h1>
        <p className="text-gray-300">Your files will be deleted shortly</p>
        <a
          className="p-2 border-2 border-b-4 border-black rounded-md mt-3 w-full max-w-lg bg-yellow-400 hover:bg-yellow-200 transition-colors duration-200 cursor-pointer text-center"
          href="/"
        >
          Return To Menu
        </a>
      </div>
    </div>
  );
}
