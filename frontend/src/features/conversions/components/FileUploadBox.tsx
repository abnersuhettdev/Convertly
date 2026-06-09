import { FileText, UploadCloud } from "lucide-react";
import type { ChangeEvent } from "react";

type FileUploadBoxProps = {
  file: File | null;
  onFileChange: (file: File | null) => void;
};

export function FileUploadBox({ file, onFileChange }: FileUploadBoxProps) {
  function handleChange(event: ChangeEvent<HTMLInputElement>) {
    onFileChange(event.target.files?.[0] ?? null);
  }

  return (
    <label className="flex min-h-52 cursor-pointer flex-col items-center justify-center rounded-lg border border-dashed border-slate-300 bg-slate-50 p-8 text-center transition hover:border-emerald-400 hover:bg-emerald-50/40">
      <input accept=".docx" className="sr-only" onChange={handleChange} type="file" />
      {file ? (
        <>
          <FileText aria-hidden="true" className="h-9 w-9 text-emerald-700" />
          <p className="mt-4 max-w-full break-all text-sm font-semibold text-slate-900">{file.name}</p>
          <p className="mt-2 text-sm text-slate-600">{(file.size / 1024 / 1024).toFixed(2)} MB selected</p>
        </>
      ) : (
        <>
          <UploadCloud aria-hidden="true" className="h-9 w-9 text-emerald-700" />
          <p className="mt-4 text-sm font-semibold text-slate-900">Choose a DOCX file</p>
          <p className="mt-2 max-w-md text-sm leading-6 text-slate-600">
            Convertly currently accepts Word documents and converts them to PDF.
          </p>
        </>
      )}
    </label>
  );
}
