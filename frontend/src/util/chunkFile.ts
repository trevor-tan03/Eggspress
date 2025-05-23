import type { FileChunk } from "../types/BoxTypes";

export function splitIntoChunks(file: File, chunkSize: number) {
  const totalChunks = Math.ceil(file.size / chunkSize);
  const chunks: FileChunk[] = [];

  for (let i = 0; i < totalChunks; i++) {
    const start = i * chunkSize;
    const end = Math.min(start + chunkSize, file.size);
    const blob = file.slice(start, end);
    chunks.push({
      number: i,
      blob,
    });
  }

  return chunks;
}
