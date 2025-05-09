import { useEffect, useState } from "react";

interface Props {
  expiresAt: string;
  setExpired: React.Dispatch<React.SetStateAction<boolean>>;
}

export default function BoxTimer({ expiresAt, setExpired }: Props) {
  const [timeRemaining, setTimeRemaining] = useState(0);

  function formatTime(ms: number): string {
    const totalSeconds = Math.floor(ms / 1000);
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;

    return `${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}`;
  }

  useEffect(() => {
    function calculateTimeRemaining() {
      const remainingTime = new Date(expiresAt + "Z").getTime() - Date.now();
      return remainingTime > 0 ? remainingTime : 0;
    }

    setTimeRemaining(calculateTimeRemaining());

    const countdownInterval = setInterval(() => {
      const remainingTime = calculateTimeRemaining();

      if (remainingTime <= 0) {
        setExpired(true);
        clearInterval(countdownInterval);
      }

      setTimeRemaining(remainingTime);
    }, 1000);

    return () => clearInterval(countdownInterval);
  }, [expiresAt, setExpired]);

  return (
    <div className="p-3 border-2 border-yellow-400 rounded-md w-60 text-center text-yellow-400 font-bold border-r-0 rounded-r-none">
      Eggsplodes In: <span>{formatTime(timeRemaining)}</span>
    </div>
  );
}
