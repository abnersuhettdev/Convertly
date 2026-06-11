import { FileUp } from "lucide-react";
import { useMemo, useState } from "react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { PageLoadingState } from "../../../components/ui/PageLoadingState";
import { PageShell } from "../../../components/ui/PageShell";
import { ConversionTable } from "../components/ConversionTable";
import { useConversions } from "../hooks/useConversions";
import { getApiErrorMessage } from "../services/conversionService";
import type { ConversionStatus } from "../types/conversionTypes";

const statuses: Array<ConversionStatus | ""> = ["", "Pending", "Processing", "Completed", "Failed", "Expired"];

export function ConversionsPage() {
  const [page, setPage] = useState(1);
  const [status, setStatus] = useState<ConversionStatus | "">("");
  const { t } = useTranslation();
  const query = useMemo(() => ({ page, pageSize: 10, status }), [page, status]);
  const conversionsQuery = useConversions(query);
  const data = conversionsQuery.data;

  function handleStatusChange(nextStatus: ConversionStatus | "") {
    setStatus(nextStatus);
    setPage(1);
  }

  return (
    <PageShell description={t("conversions.description")} title={t("conversions.title")}>
      <section className="rounded-3xl border border-slate-200/80 bg-white p-5 shadow-xl shadow-slate-900/5">
        <div className="mb-5 flex flex-col justify-between gap-3 sm:flex-row sm:items-center">
          <label className="text-sm font-semibold text-slate-700">
            {t("conversions.statusFilter")}
            <select
              aria-label={t("conversions.statusFilter")}
              className="mt-2 h-10 rounded-full border border-slate-300 bg-white px-3 text-sm font-medium text-slate-800 outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-100 sm:ml-3 sm:mt-0"
              onChange={(event) => handleStatusChange(event.target.value as ConversionStatus | "")}
              value={status}
            >
              {statuses.map((item) => (
                <option key={item || "All"} value={item}>
                  {item ? t(`conversions.status.${item}`) : t("conversions.allStatuses")}
                </option>
              ))}
            </select>
          </label>
          <Link
            className="inline-flex h-11 items-center justify-center gap-2 rounded-full bg-gradient-to-r from-emerald-600 to-slate-950 px-5 text-sm font-semibold text-white shadow-lg shadow-emerald-900/15 transition hover:from-emerald-700 hover:to-slate-900 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
            to="/conversions/new"
          >
            <FileUp aria-hidden="true" className="h-4 w-4" />
            {t("common.newConversion")}
          </Link>
        </div>

        {conversionsQuery.isLoading ? (
          <PageLoadingState label={t("conversions.loading")} />
        ) : conversionsQuery.isError ? (
          <p className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700" role="alert">
            {getApiErrorMessage(conversionsQuery.error, t)}
          </p>
        ) : (
          <>
            <ConversionTable conversions={data?.items ?? []} />
            <div className="mt-5 flex flex-col justify-between gap-3 text-sm text-slate-600 sm:flex-row sm:items-center">
              <span>
                {t("common.pageOfItems", {
                  page: data?.page ?? page,
                  totalItems: data?.totalItems ?? 0,
                  totalPages: data?.totalPages ?? 0,
                })}
              </span>
              <div className="flex gap-2">
                <button
                  className="h-10 rounded-full border border-slate-300 bg-white px-4 font-semibold text-slate-800 shadow-sm transition hover:border-slate-400 hover:bg-slate-50 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600 disabled:cursor-not-allowed disabled:opacity-50"
                  disabled={page <= 1}
                  onClick={() => setPage((current) => Math.max(1, current - 1))}
                  type="button"
                >
                  {t("common.previous")}
                </button>
                <button
                  className="h-10 rounded-full border border-slate-300 bg-white px-4 font-semibold text-slate-800 shadow-sm transition hover:border-slate-400 hover:bg-slate-50 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600 disabled:cursor-not-allowed disabled:opacity-50"
                  disabled={!data || page >= data.totalPages}
                  onClick={() => setPage((current) => current + 1)}
                  type="button"
                >
                  {t("common.next")}
                </button>
              </div>
            </div>
          </>
        )}
      </section>
    </PageShell>
  );
}
