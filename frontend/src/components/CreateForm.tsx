export default function CreateForm() {
  async function handleSubmit(formData: FormData) {
    try {
      const createEndpoint = `${import.meta.env.VITE_BACKEND_API}/api/box/create`;
      console.log(import.meta.env.BACKEND_API);

      const res = await fetch(createEndpoint, {
        method: "POST",
        body: JSON.stringify({
          password: formData.get("password"),
        }),
        headers: {
          "Content-Type": "application/json",
        },
      });

      const code = await res.text();
      location.href = `/box/${code}`;
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
          disabled // Disabled for now
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
