export default function Notepad() {
  return (
    <div className="w-full h-full border-2 border-black rounded-md overflow-hidden">
      <div className="bg-blue-600 text-white p-2">
        <h2 className="font-bold text-xl mb-2">Notepad</h2>
        <div className="h-full bg-white">
          <textarea className="text-gray-800 w-full" />
          <div className="bg-gray-300 text-gray-800 p-2 text-sm"></div>
        </div>
      </div>
    </div>
  );
}
