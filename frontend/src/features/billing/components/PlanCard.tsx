import { CheckCircle2, Loader2 } from "lucide-react";
import type { Plan } from "../../conversions/types/subscriptionTypes";

type PlanCardProps = {
  plan: Plan;
  isCurrent: boolean;
  isChanging: boolean;
  onChangePlan: (planSlug: Plan["slug"]) => void;
};

export function PlanCard({ plan, isCurrent, isChanging, onChangePlan }: PlanCardProps) {
  return (
    <article
      className={`rounded-lg border p-6 shadow-sm ${
        isCurrent ? "border-emerald-300 bg-emerald-50" : "border-slate-200 bg-white"
      }`}
    >
      <div className="flex items-start justify-between gap-4">
        <div>
          <p className="text-sm font-semibold uppercase tracking-wide text-emerald-700">{plan.slug}</p>
          <h2 className="mt-2 text-2xl font-semibold text-slate-950">{plan.name}</h2>
        </div>
        {isCurrent ? <CheckCircle2 aria-hidden="true" className="h-6 w-6 text-emerald-700" /> : null}
      </div>

      <p className="mt-5 text-3xl font-semibold text-slate-950">{formatPrice(plan.priceCents)}</p>
      <p className="mt-1 text-sm text-slate-600">Simulated MVP plan</p>

      <dl className="mt-6 space-y-3 text-sm text-slate-700">
        <Row label="Conversions/month" value={String(plan.monthlyConversionLimit)} />
        <Row label="Max file size" value={`${plan.maxFileSizeMb} MB`} />
        <Row label="File retention" value={`${plan.retentionHours}h`} />
      </dl>

      {isCurrent ? (
        <div className="mt-6 flex h-11 items-center justify-center rounded-md border border-emerald-300 bg-white text-sm font-semibold text-emerald-800">
          Current plan
        </div>
      ) : (
        <button
          className="mt-6 inline-flex h-11 w-full items-center justify-center gap-2 rounded-md bg-emerald-600 px-5 text-sm font-semibold text-white transition hover:bg-emerald-700 disabled:cursor-not-allowed disabled:bg-slate-300"
          disabled={isChanging}
          onClick={() => onChangePlan(plan.slug)}
          type="button"
        >
          {isChanging ? <Loader2 aria-hidden="true" className="h-4 w-4 animate-spin" /> : null}
          {plan.priceCents === 0 ? "Switch to Free" : "Change plan"}
        </button>
      )}
    </article>
  );
}

function Row({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex justify-between gap-4">
      <dt>{label}</dt>
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
