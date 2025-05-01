export default function OpenForm() {
  return (
    <form>
      <h1 className="text-center text-3xl font-bold">Enter Box Details</h1>

      <div className="grid mt-3">
        <label htmlFor="code">Code</label>
        <input
          id="code"
          className="border-2 border-black rounded-md p-2"
          type="text"
        />
      </div>

      <div className="grid">
        <label htmlFor="password">Password</label>
        <input
          id="password"
          className="border-2 border-black rounded-md p-2 tracking-widest"
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
