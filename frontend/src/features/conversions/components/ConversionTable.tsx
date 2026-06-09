import { Download, Eye } from "lucide-react";
import { Link } from "react-router-dom";
import { formatDateTime } from "../../../lib/format";
import { ConversionStatusBadge } from "./ConversionStatusBadge";
import type { ConversionListItem } from "../types/conversionTypes";

type ConversionTableProps = {
  conversions: ConversionListItem[];
};

export function ConversionTable({ conversions }: ConversionTableProps) {
  if (conversions.length === 0) {
    return (
      <div className="rounded-lg border border-dashed border-slate-300 bg-slate-50 p-8 text-center text-sm text-slate-600">
        No conversions found.
      </div>
    );
  }

  return (
    <div className="overflow-hidden rounded-lg border border-slate-200">
      <div className="hidden grid-cols-[2fr_1fr_1fr_1fr] bg-slate-100 px-4 py-3 text-sm font-semibold text-slate-700 md:grid">
        <span>File</span>
        <span>Status</span>
        <span>Created</span>
        <span>Actions</span>
      </div>
      <div className="divide-y divide-slate-200">
        {conversions.map((conversion) => (
          <div
            className="grid gap-3 px-4 py-4 text-sm md:grid-cols-[2fr_1fr_1fr_1fr] md:items-center"
            key={conversion.id}
          >
            <div>
              <p className="font-semibold text-slate-950">{conversion.sourceFileName}</p>
              <p className="mt-1 text-xs uppercase text-slate-500">
                {conversion.sourceFormat} to {conversion.targetFormat}
              </p>
            </div>
            <div>
              <ConversionStatusBadge status={conversion.status} />
            </div>
            <div className="text-slate-600">{formatDateTime(conversion.createdAt)}</div>
            <div className="flex flex-wrap gap-2">
              <Link
                className="inline-flex h-9 items-center gap-2 rounded-md border border-slate-300 bg-white px-3 text-sm font-semibold text-slate-800 transition hover:border-slate-400 hover:bg-slate-50"
                to={`/conversions/${conversion.id}`}
              >
                <Eye aria-hidden="true" className="h-4 w-4" />
                View
              </Link>
              {conversion.downloadAvailable ? (
                <Link
                  className="inline-flex h-9 items-center gap-2 rounded-md bg-emerald-600 px-3 text-sm font-semibold text-white transition hover:bg-emerald-700"
                  to={`/conversions/${conversion.id}`}
                >
                  <Download aria-hidden="true" className="h-4 w-4" />
                  Download
                </Link>
              ) : null}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
