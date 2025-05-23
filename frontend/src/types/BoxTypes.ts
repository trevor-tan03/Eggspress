export interface IFile {
  name: string;
  size: number;
}

export interface BoxDTO {
  code: string;
  expiresAt: string;
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
