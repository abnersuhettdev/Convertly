import { AlertCircle, ArrowLeft, CheckCircle2 } from "lucide-react";
import { useState } from "react";
import { Link } from "react-router-dom";
import { PageShell } from "../../../components/ui/PageShell";
import { getApiErrorMessage } from "../../conversions/services/conversionService";
import { useSubscription } from "../../conversions/hooks/useSubscription";
import type { Plan } from "../../conversions/types/subscriptionTypes";
import { PlanCard } from "../components/PlanCard";
import { useChangePlan } from "../hooks/useChangePlan";
import { usePlans } from "../hooks/usePlans";

export function BillingPage() {
  const plansQuery = usePlans();
  const subscriptionQuery = useSubscription();
  const changePlan = useChangePlan();
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [changingPlanSlug, setChangingPlanSlug] = useState<Plan["slug"] | null>(null);

  async function handleChangePlan(planSlug: Plan["slug"]) {
    setSuccessMessage(null);
    setChangingPlanSlug(planSlug);

    try {
      const subscription = await changePlan.mutateAsync(planSlug);
      setSuccessMessage(`Your plan was changed to ${subscription.plan.name}.`);
    } finally {
      setChangingPlanSlug(null);
    }
  }

  const isLoading = plansQuery.isLoading || subscriptionQuery.isLoading;
  const error = plansQuery.error ?? subscriptionQuery.error;
  const currentPlanSlug = subscriptionQuery.data?.plan.slug;
  const plans = sortPlans(plansQuery.data ?? []);

  return (
    <PageShell
      description="Compare plans and switch instantly. Billing is simulated for this portfolio MVP."
      title="Billing"
    >
      <div className="space-y-6">
        <div className="rounded-lg border border-amber-200 bg-amber-50 p-4 text-sm leading-6 text-amber-900">
          Payments are simulated in this portfolio MVP. Plan changes update your account immediately without real
          billing.
        </div>

        <div className="flex flex-col justify-between gap-4 rounded-lg border border-slate-200 bg-slate-50 p-5 sm:flex-row sm:items-center">
          <div>
            <p className="text-sm font-semibold text-slate-700">Current usage</p>
            <p className="mt-1 text-sm text-slate-600">
              {subscriptionQuery.data
                ? `${subscriptionQuery.data.conversionsUsed} used, ${subscriptionQuery.data.conversionsRemaining} remaining`
                : "Loading usage..."}
            </p>
          </div>
          <Link
            className="inline-flex h-10 items-center justify-center gap-2 rounded-md border border-slate-300 bg-white px-4 text-sm font-semibold text-slate-800 transition hover:border-slate-400 hover:bg-slate-50"
            to="/dashboard"
          >
            <ArrowLeft aria-hidden="true" className="h-4 w-4" />
            Back to dashboard
          </Link>
        </div>

        {successMessage ? (
          <div className="flex items-start gap-3 rounded-lg border border-emerald-200 bg-emerald-50 p-4 text-sm text-emerald-800">
            <CheckCircle2 aria-hidden="true" className="mt-0.5 h-5 w-5 shrink-0" />
            {successMessage}
          </div>
        ) : null}

        {changePlan.isError ? (
          <div className="flex items-start gap-3 rounded-lg border border-red-200 bg-red-50 p-4 text-sm text-red-700">
            <AlertCircle aria-hidden="true" className="mt-0.5 h-5 w-5 shrink-0" />
            {getApiErrorMessage(changePlan.error)}
          </div>
        ) : null}

        {isLoading ? (
          <div className="rounded-lg border border-slate-200 bg-white p-6 text-sm text-slate-600 shadow-sm">
            Loading plans...
          </div>
        ) : error ? (
          <div className="flex items-start gap-3 rounded-lg border border-red-200 bg-red-50 p-5 text-sm text-red-700">
            <AlertCircle aria-hidden="true" className="mt-0.5 h-5 w-5 shrink-0" />
            {getApiErrorMessage(error)}
          </div>
        ) : (
          <div className="grid gap-5 lg:grid-cols-3">
            {plans.map((plan) => (
              <PlanCard
                isChanging={changingPlanSlug === plan.slug}
                isCurrent={plan.slug === currentPlanSlug}
                key={plan.id}
                onChangePlan={handleChangePlan}
                plan={plan}
              />
            ))}
          </div>
        )}
      </div>
    </PageShell>
  );
}

function sortPlans(plans: Plan[]) {
  const order: Record<Plan["slug"], number> = {
    free: 1,
    pro: 2,
    business: 3,
  };

  return [...plans].sort((first, second) => order[first.slug] - order[second.slug]);
}
