import { AlertCircle, ArrowLeft, CheckCircle2 } from "lucide-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { PageLoadingState } from "../../../components/ui/PageLoadingState";
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
  const { t } = useTranslation();
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [changingPlanSlug, setChangingPlanSlug] = useState<Plan["slug"] | null>(null);

  async function handleChangePlan(planSlug: Plan["slug"]) {
    setSuccessMessage(null);
    setChangingPlanSlug(planSlug);

    try {
      const subscription = await changePlan.mutateAsync(planSlug);
      setSuccessMessage(t("billing.changeSuccess", { planName: subscription.plan.name }));
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
      description={t("billing.description")}
      title={t("billing.title")}
    >
      <div className="space-y-6">
        {isLoading ? (
          <PageLoadingState label={t("billing.loadingPlans")} />
        ) : (
          <>
        <div className="rounded-3xl border border-amber-200 bg-amber-50 p-4 text-sm leading-6 text-amber-900 shadow-sm shadow-amber-900/5">
          {t("billing.simulatedNotice")}
        </div>

        <div className="flex flex-col justify-between gap-4 rounded-3xl border border-slate-200/80 bg-white p-5 shadow-xl shadow-slate-900/5 sm:flex-row sm:items-center">
          <div>
            <p className="text-sm font-semibold text-slate-700">{t("billing.currentUsage")}</p>
            <p className="mt-1 text-sm text-slate-600">
              {subscriptionQuery.data
                ? t("billing.usage", {
                    remaining: subscriptionQuery.data.conversionsRemaining,
                    used: subscriptionQuery.data.conversionsUsed,
                  })
                : t("billing.loadingUsage")}
            </p>
          </div>
          <Link
            className="inline-flex h-10 items-center justify-center gap-2 rounded-full border border-slate-300 bg-white px-4 text-sm font-semibold text-slate-800 shadow-sm transition hover:border-slate-400 hover:bg-slate-50 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
            to="/dashboard"
          >
            <ArrowLeft aria-hidden="true" className="h-4 w-4" />
            {t("common.backToDashboard")}
          </Link>
        </div>

        {successMessage ? (
          <div className="flex items-start gap-3 rounded-2xl border border-emerald-200 bg-emerald-50 p-4 text-sm text-emerald-800 shadow-sm shadow-emerald-900/5" role="status">
            <CheckCircle2 aria-hidden="true" className="mt-0.5 h-5 w-5 shrink-0" />
            {successMessage}
          </div>
        ) : null}

        {changePlan.isError ? (
          <div className="flex items-start gap-3 rounded-2xl border border-red-200 bg-red-50 p-4 text-sm text-red-700 shadow-sm shadow-red-900/5" role="alert">
            <AlertCircle aria-hidden="true" className="mt-0.5 h-5 w-5 shrink-0" />
            {getApiErrorMessage(changePlan.error, t)}
          </div>
        ) : null}

        {error ? (
          <div className="flex items-start gap-3 rounded-2xl border border-red-200 bg-red-50 p-5 text-sm text-red-700 shadow-sm shadow-red-900/5" role="alert">
            <AlertCircle aria-hidden="true" className="mt-0.5 h-5 w-5 shrink-0" />
            {getApiErrorMessage(error, t)}
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
          </>
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
