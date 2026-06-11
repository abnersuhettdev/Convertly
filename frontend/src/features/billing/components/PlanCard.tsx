import { CheckCircle2, Loader2 } from "lucide-react";
import { useTranslation } from "react-i18next";
import type { Plan } from "../../conversions/types/subscriptionTypes";

type PlanCardProps = {
  plan: Plan;
  isCurrent: boolean;
  isChanging: boolean;
  onChangePlan: (planSlug: Plan["slug"]) => void;
};

export function PlanCard({ plan, isCurrent, isChanging, onChangePlan }: PlanCardProps) {
  const { t } = useTranslation();

  return (
    <article
      className={`relative overflow-hidden rounded-3xl border p-6 shadow-xl transition ${
        isCurrent ? "border-emerald-300 bg-emerald-50 shadow-emerald-900/10" : "border-slate-200/80 bg-white shadow-slate-900/5"
      }`}
    >
      {isCurrent ? (
        <div aria-hidden="true" className="pointer-events-none absolute inset-x-0 top-0 h-1 bg-gradient-to-r from-emerald-500 to-slate-950" />
      ) : null}
      <div className="flex items-start justify-between gap-4">
        <div>
          <p className="text-xs font-semibold uppercase tracking-[0.14em] text-emerald-700">{plan.slug}</p>
          <h2 className="mt-2 text-2xl font-semibold text-slate-950">{plan.name}</h2>
        </div>
        {isCurrent ? <CheckCircle2 aria-hidden="true" className="h-6 w-6 text-emerald-700" /> : null}
      </div>

      <p className="mt-5 text-3xl font-semibold text-slate-950">{formatPrice(plan.priceCents)}</p>
      <p className="mt-1 text-sm text-slate-600">{t("billing.simulatedPlan")}</p>

      <dl className="mt-6 space-y-3 rounded-2xl border border-slate-200 bg-white/80 p-4 text-sm text-slate-700">
        <Row label={t("billing.conversionsMonth")} value={String(plan.monthlyConversionLimit)} />
        <Row label={t("billing.maxFileSize")} value={`${plan.maxFileSizeMb} MB`} />
        <Row label={t("billing.fileRetention")} value={`${plan.retentionHours}h`} />
      </dl>

      {isCurrent ? (
        <div className="mt-6 flex h-11 items-center justify-center rounded-full border border-emerald-300 bg-white text-sm font-semibold text-emerald-800 shadow-sm">
          {t("common.currentPlan")}
        </div>
      ) : (
        <button
          className="mt-6 inline-flex h-11 w-full items-center justify-center gap-2 rounded-full bg-gradient-to-r from-emerald-600 to-slate-950 px-5 text-sm font-semibold text-white shadow-lg shadow-emerald-900/15 transition hover:from-emerald-700 hover:to-slate-900 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600 disabled:cursor-not-allowed disabled:from-slate-300 disabled:to-slate-300"
          disabled={isChanging}
          onClick={() => onChangePlan(plan.slug)}
          type="button"
        >
          {isChanging ? <Loader2 aria-hidden="true" className="h-4 w-4 animate-spin" /> : null}
          {plan.priceCents === 0 ? t("billing.switchToFree") : t("billing.changePlan")}
        </button>
      )}
    </article>
  );
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex justify-between gap-4">
      <dt className="text-slate-600">{label}</dt>
      <dd className="font-semibold text-slate-950">{value}</dd>
    </div>
  );
}

function formatPrice(priceCents: number) {
  if (priceCents === 0) {
    return "$0";
  }

  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
  }).format(priceCents / 100);
}
