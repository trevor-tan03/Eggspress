import type { IFile } from "../types/BoxTypes";

interface Props {
  code: string;
  file: IFile;
}

function formatFileSize(bytes: number) {
  if (bytes === 0) return "0 B";

  const units = ["B", "KB", "MB", "GB", "TB", "PB"];
  const i = Math.floor(Math.log(bytes) / Math.log(1024));
  const size = bytes / Math.pow(1024, i);

  return `${Math.round(size)} ${units[i]}`;
}

export default function UploadedFile({ code, file }: Props) {
  return (
    <div className="grid grid-cols-4 text-sm hover:bg-blue-100">
      <a
        className="col-span-3 underline text-blue-900"
        href={`${import.meta.env.VITE_BACKEND_API}/api/download/${code}/${file.name}`}
        download={file.name}
      >
        {file.name}
      </a>
      <span className="col-span-1 text-right text-gray-500">
        {formatFileSize(file.size)}
      </span>
    </div>
  );
}
