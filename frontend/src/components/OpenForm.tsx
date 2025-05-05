import { useState } from "react";

function openBox(e: React.FormEvent<HTMLFormElement>, code: string) {
  e.preventDefault();
  window.location.href = `/box/${code}`;
}

export default function OpenForm() {
  const [code, setCode] = useState("");

  return (
    <form onSubmit={(e) => openBox(e, code)}>
      <h1 className="text-center text-3xl font-bold">Enter Box Details</h1>

      <div className="grid mt-3">
        <label htmlFor="code" className="text-gray-800">
          Code
        </label>
        <input
          id="code"
          className="border-2 border-black rounded-md p-2"
          type="text"
          value={code}
          onChange={(e) => setCode(e.target.value)}
        />
      </div>

      <div className="grid">
        <label htmlFor="password" className="text-gray-800">
          Password
        </label>
        <input
          disabled // Disabled for now
          id="password"
          className="border-2 border-black rounded-md p-2 tracking-widest disabled:bg-gray-200 disabled:cursor-not-allowed"
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
  );
}
