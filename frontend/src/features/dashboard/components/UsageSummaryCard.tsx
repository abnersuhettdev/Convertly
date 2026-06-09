import { BarChart3, FileUp, Gauge, Timer } from "lucide-react";
import { Link } from "react-router-dom";
import type { Subscription } from "../../conversions/types/subscriptionTypes";

type UsageSummaryCardProps = {
  subscription: Subscription;
};

export function UsageSummaryCard({ subscription }: UsageSummaryCardProps) {
  const usedPercent =
    subscription.monthlyLimit > 0
      ? Math.min(100, Math.round((subscription.conversionsUsed / subscription.monthlyLimit) * 100))
      : 0;

  return (
    <div className="grid gap-5 lg:grid-cols-[1.2fr_0.8fr]">
      <section className="rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
        <div className="flex flex-col justify-between gap-5 sm:flex-row sm:items-start">
          <div>
            <p className="text-sm font-semibold uppercase tracking-wide text-emerald-700">Current plan</p>
            <h2 className="mt-2 text-3xl font-semibold text-slate-950">{subscription.plan.name}</h2>
            <p className="mt-2 text-sm text-slate-600">
              {subscription.conversionsRemaining} conversions remaining this month.
            </p>
          </div>
          <Link
            className="inline-flex h-11 items-center justify-center gap-2 rounded-md bg-emerald-600 px-5 text-sm font-semibold text-white transition hover:bg-emerald-700"
            to="/conversions/new"
          >
            <FileUp aria-hidden="true" className="h-4 w-4" />
            New conversion
          </Link>
        </div>
        <div className="mt-6 h-3 overflow-hidden rounded-full bg-slate-100">
          <div className="h-full rounded-full bg-emerald-600" style={{ width: `${usedPercent}%` }} />
        </div>
        <div className="mt-3 flex justify-between text-sm text-slate-600">
          <span>{subscription.conversionsUsed} used</span>
          <span>{subscription.monthlyLimit} total</span>
        </div>
      </section>

      <section className="grid gap-4 sm:grid-cols-3 lg:grid-cols-1">
        <Metric icon={BarChart3} label="Used" value={`${subscription.conversionsUsed} / ${subscription.monthlyLimit}`} />
        <Metric icon={Gauge} label="Max file size" value={`${subscription.maxFileSizeMb} MB`} />
        <Metric icon={Timer} label="Retention" value={`${subscription.retentionHours}h`} />
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
    <div className="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
      <Icon aria-hidden="true" className="h-5 w-5 text-emerald-700" />
      <p className="mt-4 text-sm text-slate-600">{label}</p>
      <p className="mt-1 text-2xl font-semibold text-slate-950">{value}</p>
    </div>
  );
}
