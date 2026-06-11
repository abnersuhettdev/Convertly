import { Download, Loader2, RefreshCw } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useParams } from "react-router-dom";
import { PageLoadingState } from "../../../components/ui/PageLoadingState";
import { PageShell } from "../../../components/ui/PageShell";
import { formatDateTime } from "../../../lib/format";
import { ConversionStatusBadge } from "../components/ConversionStatusBadge";
import { useConversionDetail } from "../hooks/useConversionDetail";
import { downloadConversion, getApiErrorMessage } from "../services/conversionService";

export function ConversionDetailPage() {
  const { id } = useParams();
  const { t } = useTranslation();
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
      setDownloadError(getApiErrorMessage(error, t));
    } finally {
      setIsDownloading(false);
    }
  }

  return (
    <PageShell description={t("conversions.detail.description")} title={t("conversions.detail.title")}>
      {conversionQuery.isLoading ? (
        <PageLoadingState label={t("conversions.detail.loading")} />
      ) : conversionQuery.isError ? (
        <p className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700 shadow-sm" role="alert">
          {getApiErrorMessage(conversionQuery.error, t)}
        </p>
      ) : conversion ? (
        <div className="space-y-6 rounded-3xl border border-slate-200/80 bg-white p-6 shadow-xl shadow-slate-900/5">
          <div className="flex flex-col justify-between gap-4 sm:flex-row sm:items-start">
            <div>
              <h2 className="text-xl font-semibold text-slate-950">{conversion.sourceFileName}</h2>
              <p className="mt-2 text-sm uppercase text-slate-500">
                {t("common.formatPair", { source: conversion.sourceFormat, target: conversion.targetFormat })}
              </p>
            </div>
            <ConversionStatusBadge status={conversion.status} />
          </div>

          {conversion.status === "Pending" || conversion.status === "Processing" ? (
            <div className="inline-flex items-center gap-2 rounded-2xl border border-sky-200 bg-sky-50 px-4 py-3 text-sm text-sky-800" role="status">
              <RefreshCw aria-hidden="true" className="h-4 w-4 animate-spin" />
              {t("conversions.detail.running")}
            </div>
          ) : null}

          {conversion.status === "Failed" ? (
            <p className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700" role="alert">
              {conversion.errorMessage ?? t("conversions.detail.failedFallback")}
            </p>
          ) : null}

          <dl className="grid gap-4 text-sm sm:grid-cols-2">
            <Info label={t("conversions.detail.created")} value={formatDateTime(conversion.createdAt)} />
            <Info label={t("conversions.detail.started")} value={formatDateTime(conversion.startedAt)} />
            <Info label={t("conversions.detail.completed")} value={formatDateTime(conversion.completedAt)} />
            <Info label={t("conversions.detail.expires")} value={formatDateTime(conversion.expiresAt)} />
          </dl>

          {downloadError ? (
            <p className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700" role="alert">{downloadError}</p>
          ) : null}

          <button
            className="inline-flex h-11 items-center justify-center gap-2 rounded-full bg-gradient-to-r from-emerald-600 to-slate-950 px-5 text-sm font-semibold text-white shadow-lg shadow-emerald-900/15 transition hover:from-emerald-700 hover:to-slate-900 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600 disabled:cursor-not-allowed disabled:from-slate-300 disabled:to-slate-300"
            disabled={!conversion.downloadAvailable || isDownloading}
            onClick={handleDownload}
            type="button"
          >
            {isDownloading ? <Loader2 aria-hidden="true" className="h-4 w-4 animate-spin" /> : <Download aria-hidden="true" className="h-4 w-4" />}
            {t("common.downloadPdf")}
          </button>
        </div>
      ) : null}
    </PageShell>
  );
}

function Info({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-2xl border border-slate-200 bg-slate-50 p-4">
      <dt className="font-semibold text-slate-700">{label}</dt>
      <dd className="mt-1 text-slate-600">{value}</dd>
    </div>
  );
}
