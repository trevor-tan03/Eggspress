import { setAuth } from "../api/box";

export default function CreateForm() {
  async function handleSubmit(formData: FormData) {
    try {
      const createEndpoint = `${import.meta.env.VITE_BACKEND_API}/api/box/create`;
      const createRes = await fetch(createEndpoint, {
        method: "POST",
        credentials: "include",
        body: JSON.stringify({
          password: formData.get("password"),
        }),
        headers: {
          "Content-Type": "application/json",
        },
      });

      const code = await createRes.text();
      const authResStatus = await setAuth(code, formData);

      if (authResStatus === 200) location.href = `/box/${code}`;
      else console.error("Authentication failed.");
    } catch (err) {
      console.error((err as Error).message);
    }
  }

  return (
    <form action={handleSubmit}>
      <h1 className="text-center text-3xl font-bold">Create Box</h1>

      <div className="grid mt-3">
        <label htmlFor="password" className="text-gray-800">
          Password
        </label>
        <input
          id="password"
          name="password"
          className="border-2 border-black rounded-md p-2 tracking-widest disabled:bg-gray-200 disabled:cursor-not-allowed"
          maxLength={20}
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
