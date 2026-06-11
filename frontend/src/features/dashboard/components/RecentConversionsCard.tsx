import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { ConversionTable } from "../../conversions/components/ConversionTable";
import type { ConversionListItem } from "../../conversions/types/conversionTypes";

type RecentConversionsCardProps = {
  conversions: ConversionListItem[];
};

export function RecentConversionsCard({ conversions }: RecentConversionsCardProps) {
  const { t } = useTranslation();

  return (
    <section className="rounded-3xl border border-slate-200/80 bg-white p-6 shadow-xl shadow-slate-900/5">
      <div className="flex flex-col justify-between gap-3 sm:flex-row sm:items-center">
        <div>
          <h2 className="text-xl font-semibold text-slate-950">{t("dashboard.recent.title")}</h2>
          <p className="mt-1 text-sm text-slate-600">{t("dashboard.recent.description")}</p>
        </div>
        <Link
          className="inline-flex h-10 items-center justify-center rounded-full border border-slate-300 bg-white px-4 text-sm font-semibold text-slate-800 shadow-sm transition hover:border-slate-400 hover:bg-slate-50 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
          to="/conversions"
        >
          {t("common.viewAll")}
        </Link>
      </div>
      <div className="mt-5">
        <ConversionTable conversions={conversions} />
      </div>
    </section>
  );
}
