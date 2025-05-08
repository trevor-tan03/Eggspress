import { useCallback, useState } from "react";
import { useDropzone } from "react-dropzone";
import { IFile } from "../types/BoxTypes";

interface Props {
  code: string;
  originalFiles: IFile[];
}

export default function FileUpload({ code, originalFiles }: Props) {
  const [uploadedFiles, setUploadedFiles] = useState(originalFiles);

  async function handleUpload(code: string, files: File[]) {
    try {
      const formData = new FormData();

      for (const file of files) formData.append("files", file); // "files" is the field name expected by your backend

      const res = await fetch(
        `${import.meta.env.VITE_BACKEND_API}/api/box/${code}/upload`,
        {
          method: "POST",
          body: formData,
        }
      );

      if (!res.ok) {
        throw new Error("An error occurred while uploading files.");
      }

      return true;
    } catch (err) {
      console.error((err as Error).message);
      return false;
    }
  }

  function ToFileDTO(files: File[]) {
    return files.map((f) => {
      const file: IFile = {
        name: f.name,
        size: f.size,
      };
      return file;
    });
  }

  const onDrop = useCallback(
    async (acceptedFiles: File[]) => {
      const uploadSuccess = await handleUpload(code, acceptedFiles);

      if (uploadSuccess) {
        const dto = ToFileDTO(acceptedFiles);
        setUploadedFiles((i) => i.concat(dto));
      } else {
        console.error(`Failed to upload files: ${uploadSuccess}`);
      }
    },
    [code]
  );
  const { getRootProps, getInputProps, isDragActive } = useDropzone({ onDrop });

  return (
    <div
      {...getRootProps()}
      className="p-3 rounded-md border-2 border-blue-400 max-w-lg mx-auto mt-6 bg-blue-100 flex flex-col items-center"
    >
      <button className="bg-blue-500 rounded-full w-10 h-10 text-white text-2xl cursor-pointer hover:bg-blue-600">
        <div className="pt-0.5 m-0 h-full w-full">+</div>
      </button>
      <p className="text-center mt-3 text-sm text-gray-700">
        Drag and Drop file(s) here
      </p>
      <input {...getInputProps()} />
      {uploadedFiles.length > 0 && (
        <div
          className="border p-3 border-blue-400 transition-colors duration-200 rounded-md w-full mt-3 "
          style={{
            backgroundColor: isDragActive ? "blue" : "white",
          }}
        >
          <ul>
            {uploadedFiles.map((item, i) => (
              <li key={i}>
                {item.name} - {item.size}
              </li>
            ))}
          </ul>
        </div>
      )}
    </div>
  );
}
