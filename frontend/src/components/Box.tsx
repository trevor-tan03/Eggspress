export default function Box({ children }: { children: React.ReactNode }) {
  return (
    <div className="sm:w-5/6 md:w-2xl min-h-10 bg-white border-2 border-black z-50 py-6 px-12 grid gap-2 rounded-md">
      {children}
    </div>
  );
}
