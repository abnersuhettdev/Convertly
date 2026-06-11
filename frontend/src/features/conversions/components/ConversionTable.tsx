import { Download, Eye, FileSearch } from "lucide-react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { formatDateTime } from "../../../lib/format";
import { ConversionStatusBadge } from "./ConversionStatusBadge";
import type { ConversionListItem } from "../types/conversionTypes";

type ConversionTableProps = {
  conversions: ConversionListItem[];
};

export function ConversionTable({ conversions }: ConversionTableProps) {
  const { t } = useTranslation();

  if (conversions.length === 0) {
    return (
      <div className="rounded-3xl border border-dashed border-slate-300 bg-[radial-gradient(circle_at_top,rgba(16,185,129,0.10),transparent_36%),linear-gradient(180deg,#ffffff,#f8fafc)] p-8 text-center text-sm text-slate-600 shadow-inner shadow-slate-900/5">
        <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-2xl bg-white text-emerald-700 shadow-lg shadow-emerald-900/10">
          <FileSearch aria-hidden="true" className="h-6 w-6" />
        </div>
        <p className="mt-4 font-semibold text-slate-950">{t("conversions.emptyTitle")}</p>
        <p className="mt-1">{t("conversions.emptyText")}</p>
      </div>
    );
  }

  return (
    <div className="overflow-hidden rounded-3xl border border-slate-200/80 bg-white shadow-sm shadow-slate-900/5">
      <table className="hidden w-full table-fixed border-collapse md:table">
        <thead className="bg-slate-50/80 text-left text-xs font-semibold uppercase tracking-[0.12em] text-slate-500">
          <tr>
            <th className="w-2/5 px-4 py-3" scope="col">{t("conversions.table.file")}</th>
            <th className="w-1/5 px-4 py-3" scope="col">{t("conversions.table.status")}</th>
            <th className="w-1/5 px-4 py-3" scope="col">{t("conversions.table.created")}</th>
            <th className="w-1/5 px-4 py-3" scope="col">{t("conversions.table.actions")}</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-200">
          {conversions.map((conversion) => (
            <tr className="text-sm transition hover:bg-emerald-50/40" key={conversion.id}>
              <th className="px-4 py-4 text-left font-normal" scope="row">
                <p className="font-semibold text-slate-950">{conversion.sourceFileName}</p>
                <p className="mt-1 text-xs uppercase text-slate-500">
                  {t("common.formatPair", { source: conversion.sourceFormat, target: conversion.targetFormat })}
                </p>
              </th>
              <td className="px-4 py-4"><ConversionStatusBadge status={conversion.status} /></td>
              <td className="px-4 py-4 text-slate-600">{formatDateTime(conversion.createdAt)}</td>
              <td className="px-4 py-4">
                <ConversionActions conversion={conversion} />
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <div className="divide-y divide-slate-200 md:hidden">
        {conversions.map((conversion) => (
          <div
            className="grid gap-3 px-4 py-4 text-sm transition hover:bg-emerald-50/40"
            key={conversion.id}
          >
            <div>
              <p className="font-semibold text-slate-950">{conversion.sourceFileName}</p>
              <p className="mt-1 text-xs uppercase text-slate-500">
                {t("common.formatPair", { source: conversion.sourceFormat, target: conversion.targetFormat })}
              </p>
            </div>
            <div>
              <ConversionStatusBadge status={conversion.status} />
            </div>
            <div className="text-slate-600">{formatDateTime(conversion.createdAt)}</div>
            <ConversionActions conversion={conversion} />
          </div>
        ))}
      </div>
    </div>
  );
}

function ConversionActions({ conversion }: { conversion: ConversionListItem }) {
  const { t } = useTranslation();

  return (
    <div className="flex flex-wrap gap-2">
      <Link
        className="inline-flex h-9 items-center gap-2 rounded-full border border-slate-300 bg-white px-3 text-sm font-semibold text-slate-800 shadow-sm transition hover:border-slate-400 hover:bg-slate-50 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
        to={`/conversions/${conversion.id}`}
      >
        <Eye aria-hidden="true" className="h-4 w-4" />
        {t("common.view")}
      </Link>
      {conversion.downloadAvailable ? (
        <Link
          className="inline-flex h-9 items-center gap-2 rounded-full bg-emerald-600 px-3 text-sm font-semibold text-white shadow-sm transition hover:bg-emerald-700 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
          to={`/conversions/${conversion.id}`}
        >
          <Download aria-hidden="true" className="h-4 w-4" />
          {t("common.download")}
        </Link>
      ) : null}
    </div>
  );
}
