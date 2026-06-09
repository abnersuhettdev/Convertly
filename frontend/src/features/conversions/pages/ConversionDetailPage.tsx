import { Download, Loader2, RefreshCw } from "lucide-react";
import { useState } from "react";
import { useParams } from "react-router-dom";
import { PageShell } from "../../../components/ui/PageShell";
import { formatDateTime } from "../../../lib/format";
import { ConversionStatusBadge } from "../components/ConversionStatusBadge";
import { useConversionDetail } from "../hooks/useConversionDetail";
import { downloadConversion, getApiErrorMessage } from "../services/conversionService";

export function ConversionDetailPage() {
  const { id } = useParams();
  const conversionQuery = useConversionDetail(id);
  const [downloadError, setDownloadError] = useState<string | null>(null);
  const [isDownloading, setIsDownloading] = useState(false);
  const conversion = conversionQuery.data;

  async function handleDownload() {
    if (!id) {
      return;
    }

    setDownloadError(null);
    setIsDownloading(true);

    try {
      const result = await downloadConversion(id);
      const url = URL.createObjectURL(result.blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = result.fileName;
      document.body.appendChild(link);
      link.click();
      link.remove();
      URL.revokeObjectURL(url);
    } catch (error) {
      setDownloadError(getApiErrorMessage(error));
    } finally {
      setIsDownloading(false);
    }
  }

  return (
    <PageShell description="Track conversion status and download the PDF when it is ready." title="Conversion detail">
      {conversionQuery.isLoading ? (
        <p className="text-sm text-slate-600">Loading conversion...</p>
      ) : conversionQuery.isError ? (
        <p className="rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {getApiErrorMessage(conversionQuery.error)}
        </p>
      ) : conversion ? (
        <div className="space-y-6">
          <div className="flex flex-col justify-between gap-4 sm:flex-row sm:items-start">
            <div>
              <h2 className="text-xl font-semibold text-slate-950">{conversion.sourceFileName}</h2>
              <p className="mt-2 text-sm uppercase text-slate-500">
                {conversion.sourceFormat} to {conversion.targetFormat}
              </p>
            </div>
            <ConversionStatusBadge status={conversion.status} />
          </div>

          {conversion.status === "Pending" || conversion.status === "Processing" ? (
            <div className="inline-flex items-center gap-2 rounded-md border border-sky-200 bg-sky-50 px-4 py-3 text-sm text-sky-800">
              <RefreshCw aria-hidden="true" className="h-4 w-4 animate-spin" />
              Conversion is running. This page refreshes automatically.
            </div>
          ) : null}

          {conversion.status === "Failed" ? (
            <p className="rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
              {conversion.errorMessage ?? "Conversion failed. Try again with another DOCX file."}
            </p>
          ) : null}

          <dl className="grid gap-4 text-sm sm:grid-cols-2">
            <Info label="Created" value={formatDateTime(conversion.createdAt)} />
            <Info label="Started" value={formatDateTime(conversion.startedAt)} />
            <Info label="Completed" value={formatDateTime(conversion.completedAt)} />
            <Info label="Expires" value={formatDateTime(conversion.expiresAt)} />
          </dl>

          {downloadError ? (
            <p className="rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{downloadError}</p>
          ) : null}

          <button
            className="inline-flex h-11 items-center justify-center gap-2 rounded-md bg-emerald-600 px-5 text-sm font-semibold text-white transition hover:bg-emerald-700 disabled:cursor-not-allowed disabled:bg-slate-300"
            disabled={!conversion.downloadAvailable || isDownloading}
            onClick={handleDownload}
            type="button"
          >
            {isDownloading ? <Loader2 aria-hidden="true" className="h-4 w-4 animate-spin" /> : <Download aria-hidden="true" className="h-4 w-4" />}
            Download PDF
          </button>
        </div>
      ) : null}
    </PageShell>
  );
}

function Info({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-lg border border-slate-200 bg-slate-50 p-4">
      <dt className="font-semibold text-slate-700">{label}</dt>
      <dd className="mt-1 text-slate-600">{value}</dd>
    </div>
  );
}
