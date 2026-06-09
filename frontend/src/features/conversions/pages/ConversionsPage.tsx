import { useMemo, useState } from "react";
import { PageShell } from "../../../components/ui/PageShell";
import { ConversionTable } from "../components/ConversionTable";
import { useConversions } from "../hooks/useConversions";
import { getApiErrorMessage } from "../services/conversionService";
import type { ConversionStatus } from "../types/conversionTypes";

const statuses: Array<ConversionStatus | ""> = ["", "Pending", "Processing", "Completed", "Failed", "Expired"];

export function ConversionsPage() {
  const [page, setPage] = useState(1);
  const [status, setStatus] = useState<ConversionStatus | "">("");
  const query = useMemo(() => ({ page, pageSize: 10, status }), [page, status]);
  const conversionsQuery = useConversions(query);
  const data = conversionsQuery.data;

  function handleStatusChange(nextStatus: ConversionStatus | "") {
    setStatus(nextStatus);
    setPage(1);
  }

  return (
    <PageShell description="Browse your DOCX to PDF conversion history." title="Conversions">
      <div className="mb-5 flex flex-col justify-between gap-3 sm:flex-row sm:items-center">
        <label className="text-sm font-semibold text-slate-700">
          Status
          <select
            className="mt-2 h-10 rounded-md border border-slate-300 bg-white px-3 text-sm font-medium text-slate-800 outline-none focus:border-emerald-600 focus:ring-2 focus:ring-emerald-100 sm:ml-3 sm:mt-0"
            onChange={(event) => handleStatusChange(event.target.value as ConversionStatus | "")}
            value={status}
          >
            {statuses.map((item) => (
              <option key={item || "All"} value={item}>
                {item || "All statuses"}
              </option>
            ))}
          </select>
        </label>
      </div>

      {conversionsQuery.isLoading ? (
        <p className="text-sm text-slate-600">Loading conversions...</p>
      ) : conversionsQuery.isError ? (
        <p className="rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {getApiErrorMessage(conversionsQuery.error)}
        </p>
      ) : (
        <>
          <ConversionTable conversions={data?.items ?? []} />
          <div className="mt-5 flex flex-col justify-between gap-3 text-sm text-slate-600 sm:flex-row sm:items-center">
            <span>
              Page {data?.page ?? page} of {data?.totalPages ?? 0} · {data?.totalItems ?? 0} items
            </span>
            <div className="flex gap-2">
              <button
                className="h-10 rounded-md border border-slate-300 bg-white px-4 font-semibold text-slate-800 disabled:cursor-not-allowed disabled:opacity-50"
                disabled={page <= 1}
                onClick={() => setPage((current) => Math.max(1, current - 1))}
                type="button"
              >
                Previous
              </button>
              <button
                className="h-10 rounded-md border border-slate-300 bg-white px-4 font-semibold text-slate-800 disabled:cursor-not-allowed disabled:opacity-50"
                disabled={!data || page >= data.totalPages}
                onClick={() => setPage((current) => current + 1)}
                type="button"
              >
                Next
              </button>
            </div>
          </div>
        </>
      )}
    </PageShell>
  );
}
