export interface IFile {
  name: string;
  size: number;
}

export interface BoxDTO {
  code: string;
  expiresAt: string;
  files: IFile[];
}
