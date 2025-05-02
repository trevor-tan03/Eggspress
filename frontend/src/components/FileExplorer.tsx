export default function FileExplorer() {
  return (
    <div className="w-full h-full border-2 border-black rounded-md overflow-hidden">
      <div className="bg-blue-600 text-white p-2">
        <h2 className="font-bold text-xl mb-2">File Explorer</h2>
        <div className="h-full bg-white">
          <div className="bg-white min-h-60">
            <input
              type="file"
              multiple
              className="border border-black text-black"
            />
          </div>
        </div>
      </div>
    </div>
  );
}
