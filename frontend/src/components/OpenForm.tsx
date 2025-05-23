import { useState } from "react";
import { setAuth } from "../api/box";

async function handleSubmit(formData: FormData) {
  const code = formData.get("code");

  if (!code) {
    console.error("No code provided");
    return;
  }

  const authResStatus = await setAuth(code.toString(), formData);

  switch (authResStatus.status) {
    case 200:
      window.location.href = `/box/${code}`;
      break;
    case 401:
      console.error("Invalid password");
      break;
    case 404:
      console.error("Box does not exist.");
      break;
    default:
      console.error("An error occurred while authenticating.");
  }
}

export default function OpenForm() {
  const [code, setCode] = useState("");

  return (
    <form
      onSubmit={async (e) => {
        e.preventDefault();
        const form = new FormData(e.currentTarget);
        await handleSubmit(form);
      }}
    >
      <h1 className="text-center text-3xl font-bold">Enter Box Details</h1>

      <div className="grid mt-3">
        <label htmlFor="code" className="text-gray-800">
          Code
        </label>
        <input
          id="code"
          name="code"
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
          id="password"
          name="password"
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
