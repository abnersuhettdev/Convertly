import { FileText, UploadCloud } from "lucide-react";
import type { ChangeEvent, DragEvent } from "react";
import { useState } from "react";
import { useTranslation } from "react-i18next";

type FileUploadBoxProps = {
  file: File | null;
  onFileChange: (file: File | null) => void;
};

export function FileUploadBox({ file, onFileChange }: FileUploadBoxProps) {
  const { t } = useTranslation();
  const [isDragging, setIsDragging] = useState(false);
  const helperId = "file-upload-helper";

  function handleChange(event: ChangeEvent<HTMLInputElement>) {
    onFileChange(event.target.files?.[0] ?? null);
  }

  function handleDrag(event: DragEvent<HTMLLabelElement>) {
    event.preventDefault();
    event.stopPropagation();
  }

  function handleDragEnter(event: DragEvent<HTMLLabelElement>) {
    handleDrag(event);
    setIsDragging(true);
  }

  function handleDragLeave(event: DragEvent<HTMLLabelElement>) {
    handleDrag(event);
    if (event.currentTarget.contains(event.relatedTarget as Node | null)) {
      return;
    }

    setIsDragging(false);
  }

  function handleDrop(event: DragEvent<HTMLLabelElement>) {
    handleDrag(event);
    setIsDragging(false);
    onFileChange(event.dataTransfer.files?.[0] ?? null);
  }

  return (
    <label
      className={`group flex min-h-64 cursor-pointer flex-col items-center justify-center rounded-3xl border border-dashed p-8 text-center shadow-inner shadow-slate-900/5 transition focus-within:border-emerald-500 focus-within:outline focus-within:outline-2 focus-within:outline-emerald-600 ${
        isDragging
          ? "border-emerald-500 bg-emerald-50 ring-2 ring-emerald-200"
          : "border-slate-300 bg-[radial-gradient(circle_at_top,rgba(16,185,129,0.10),transparent_36%),linear-gradient(180deg,#ffffff,#f8fafc)] hover:border-emerald-400 hover:bg-emerald-50/50"
      }`}
      onDragEnter={handleDragEnter}
      onDragLeave={handleDragLeave}
      onDragOver={handleDrag}
      onDrop={handleDrop}
    >
      <input
        accept=".docx"
        aria-describedby={helperId}
        aria-label={t("upload.chooseFile")}
        className="sr-only"
        onChange={handleChange}
        type="file"
      />
      {file ? (
        <>
          <span className="flex h-14 w-14 items-center justify-center rounded-2xl bg-emerald-100 text-emerald-700 shadow-sm">
            <FileText aria-hidden="true" className="h-7 w-7" />
          </span>
          <p className="mt-5 max-w-full break-all text-sm font-semibold text-slate-950">{file.name}</p>
          <p className="mt-2 text-sm text-slate-600">
            {t("upload.selectedSize", { size: (file.size / 1024 / 1024).toFixed(2) })}
          </p>
          <p className="mt-4 rounded-full bg-white px-3 py-1 text-xs font-semibold text-emerald-700 shadow-sm">
            {t("upload.ready")}
          </p>
        </>
      ) : (
        <>
          <span className="flex h-14 w-14 items-center justify-center rounded-2xl bg-white text-emerald-700 shadow-lg shadow-emerald-900/10 transition group-hover:scale-105">
            <UploadCloud aria-hidden="true" className="h-7 w-7" />
          </span>
          <p className="mt-5 text-base font-semibold text-slate-950">{t("upload.chooseFile")}</p>
          <p className="mt-2 max-w-md text-sm leading-6 text-slate-600" id={helperId}>{t("upload.dragDropHelper")}</p>
          <p className="mt-4 rounded-full border border-slate-200 bg-white px-3 py-1 text-xs font-semibold text-slate-600">
            {t("upload.selectFromDevice")}
          </p>
        </>
      )}
    </label>
  );
}
