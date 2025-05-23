export async function setAuth(code: string, formData: FormData) {
  const authEndpoint = `${import.meta.env.VITE_BACKEND_API}/api/box/${code}/auth`;
  const authRes = await fetch(authEndpoint, {
    method: "POST",
    credentials: "include",
    body: JSON.stringify({
      password: formData.get("password") ?? "",
    }),
    headers: {
      "Content-Type": "application/json",
    },
  });

  return authRes;
}
