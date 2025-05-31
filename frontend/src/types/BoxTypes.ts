export interface IFile {
  name: string;
  size: number;
}

export interface IFileUploading {
  name: string;
  progress: number; // decimal (0-1)
  state: "uploading" | "paused" | "cancelled" | "waiting";
}

export interface BoxDTO {
  code: string;
  expiresAt: string;
  boxSize: number;
  files: IFile[];
}

export interface FileChunk {
  number: number;
  blob: Blob;
}

export interface FileChunkMetadata {
  totalChunks: number;
  fileId: string;
  fileName: string;
}
