import type { ConversionStatus } from "../types/conversionTypes";
import { useTranslation } from "react-i18next";

const styles: Record<ConversionStatus, string> = {
  Pending: "bg-amber-50 text-amber-800 border-amber-200",
  Processing: "bg-sky-50 text-sky-800 border-sky-200",
  Completed: "bg-emerald-50 text-emerald-800 border-emerald-200",
  Failed: "bg-red-50 text-red-800 border-red-200",
  Expired: "bg-slate-100 text-slate-700 border-slate-200",
};

type ConversionStatusBadgeProps = {
  status: ConversionStatus;
};

export function ConversionStatusBadge({ status }: ConversionStatusBadgeProps) {
  const { t } = useTranslation();

  return (
    <span className={`inline-flex items-center rounded-full border px-2.5 py-1 text-xs font-semibold ${styles[status]}`}>
      {t(`conversions.status.${status}`)}
    </span>
  );
}
