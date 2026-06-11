import { BarChart3, FileUp, Gauge, Timer } from "lucide-react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import type { Subscription } from "../../conversions/types/subscriptionTypes";

type UsageSummaryCardProps = {
  subscription: Subscription;
};

export function UsageSummaryCard({ subscription }: UsageSummaryCardProps) {
  const { t } = useTranslation();
  const usedPercent =
    subscription.monthlyLimit > 0
      ? Math.min(100, Math.round((subscription.conversionsUsed / subscription.monthlyLimit) * 100))
      : 0;

  return (
    <div className="grid gap-5 lg:grid-cols-[1.2fr_0.8fr]">
      <section className="rounded-3xl border border-slate-200/80 bg-white p-6 shadow-xl shadow-slate-900/5">
        <div className="flex flex-col justify-between gap-5 sm:flex-row sm:items-start">
          <div>
            <p className="text-xs font-semibold uppercase tracking-[0.14em] text-emerald-700">
              {t("dashboard.summary.currentPlan")}
            </p>
            <h2 className="mt-2 text-3xl font-semibold text-slate-950">{subscription.plan.name}</h2>
            <p className="mt-2 text-sm text-slate-600">
              {t("dashboard.summary.remaining", { count: subscription.conversionsRemaining })}
            </p>
          </div>
          <Link
            className="inline-flex h-11 items-center justify-center gap-2 rounded-full bg-gradient-to-r from-emerald-600 to-slate-950 px-5 text-sm font-semibold text-white shadow-lg shadow-emerald-900/15 transition hover:from-emerald-700 hover:to-slate-900 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
            to="/conversions/new"
          >
            <FileUp aria-hidden="true" className="h-4 w-4" />
            {t("common.newConversion")}
          </Link>
        </div>
        <div className="mt-6 h-3 overflow-hidden rounded-full bg-slate-100 shadow-inner">
          <div className="h-full rounded-full bg-gradient-to-r from-emerald-500 to-slate-950" style={{ width: `${usedPercent}%` }} />
        </div>
        <div className="mt-3 flex justify-between text-sm text-slate-600">
          <span>{t("dashboard.summary.used", { count: subscription.conversionsUsed })}</span>
          <span>{t("dashboard.summary.total", { count: subscription.monthlyLimit })}</span>
        </div>
      </section>

      <section className="grid gap-4 sm:grid-cols-3 lg:grid-cols-1">
        <Metric icon={BarChart3} label={t("dashboard.summary.usedMetric")} value={`${subscription.conversionsUsed} / ${subscription.monthlyLimit}`} />
        <Metric icon={Gauge} label={t("dashboard.summary.maxFileSize")} value={`${subscription.maxFileSizeMb} MB`} />
        <Metric icon={Timer} label={t("dashboard.summary.retention")} value={`${subscription.retentionHours}h`} />
      </section>
    </div>
  );
}

type MetricProps = {
  icon: typeof BarChart3;
  label: string;
  value: string;
};

function Metric({ icon: Icon, label, value }: MetricProps) {
  return (
    <div className="rounded-3xl border border-slate-200/80 bg-white p-5 shadow-lg shadow-slate-900/5">
      <div className="flex h-10 w-10 items-center justify-center rounded-2xl bg-emerald-50 text-emerald-700">
        <Icon aria-hidden="true" className="h-5 w-5" />
      </div>
      <p className="mt-4 text-sm font-medium text-slate-600">{label}</p>
      <p className="mt-1 text-2xl font-semibold text-slate-950">{value}</p>
    </div>
  );
}
