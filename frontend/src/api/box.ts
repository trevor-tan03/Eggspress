import type { BoxDTO, FileChunk, FileChunkMetadata } from "../types/BoxTypes";

export async function fetchBoxDetails(code: string): Promise<BoxDTO[]> {
  const res = await fetch(
    `${import.meta.env.VITE_BACKEND_API}/api/box/${code}`,
    {
      credentials: "include",
    }
  );

  switch (res.status) {
    case 200:
      return res.json();
    case 401:
      console.error(`Unauthorized.`);
      break;
    case 404:
      console.error(`Box does not exist.`);
      break;
    default:
      console.error(`Unexpected error occurred while retrieving box details.`);
  }

  return [];
}

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

export async function uploadChunk(
  boxCode: string,
  chunk: FileChunk,
  metadata: FileChunkMetadata
) {
  const formData = new FormData();
  formData.append("file", chunk.blob);
  formData.append("chunkNumber", chunk.number.toString());
  formData.append("totalChunks", metadata.totalChunks.toString());
  formData.append("fileId", metadata.fileId);
  formData.append("fileName", metadata.fileName);

  const uploadEndpoint = `${import.meta.env.VITE_BACKEND_API}/api/box/${boxCode}/upload/chunk`;
  const res = await fetch(uploadEndpoint, {
    method: "POST",
    body: formData,
    credentials: "include",
  });

  if (!res.ok) throw new Error(`Chunk ${chunk.number} failed.`);
  return res.json();
}
