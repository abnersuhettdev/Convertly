import { AlertCircle } from "lucide-react";
import { useTranslation } from "react-i18next";
import { PageLoadingState } from "../../../components/ui/PageLoadingState";
import { RecentConversionsCard } from "../components/RecentConversionsCard";
import { UsageSummaryCard } from "../components/UsageSummaryCard";
import { useConversions } from "../../conversions/hooks/useConversions";
import { useSubscription } from "../../conversions/hooks/useSubscription";
import { getApiErrorMessage } from "../../conversions/services/conversionService";

export function DashboardPage() {
  const subscriptionQuery = useSubscription();
  const conversionsQuery = useConversions({ page: 1, pageSize: 5 });
  const { t } = useTranslation();

  return (
    <section className="mx-auto flex w-full max-w-6xl flex-1 flex-col px-4 py-10 sm:px-6 lg:px-8">
      <div className="flex flex-col justify-between gap-4 overflow-hidden rounded-3xl border border-slate-200/80 bg-white/90 p-6 shadow-xl shadow-slate-900/5 backdrop-blur sm:flex-row sm:items-end sm:p-8">
        <div>
          <p className="text-xs font-semibold uppercase tracking-[0.16em] text-emerald-700">{t("common.brandEyebrow")}</p>
          <h1 className="mt-3 text-3xl font-semibold text-slate-950 sm:text-4xl">{t("dashboard.title")}</h1>
          <p className="mt-4 max-w-2xl text-base leading-7 text-slate-600">{t("dashboard.description")}</p>
        </div>
      </div>

      <div className="mt-8 space-y-6">
        {subscriptionQuery.isLoading || conversionsQuery.isLoading ? (
          <PageLoadingState />
        ) : subscriptionQuery.isError ? (
          <ErrorBlock message={getApiErrorMessage(subscriptionQuery.error, t)} />
        ) : subscriptionQuery.data ? (
          <UsageSummaryCard subscription={subscriptionQuery.data} />
        ) : null}

        {subscriptionQuery.isLoading || conversionsQuery.isLoading ? null : conversionsQuery.isError ? (
          <ErrorBlock message={getApiErrorMessage(conversionsQuery.error, t)} />
        ) : (
          <RecentConversionsCard conversions={conversionsQuery.data?.items ?? []} />
        )}
      </div>
    </section>
  );
}

function ErrorBlock({ message }: { message: string }) {
  return (
    <div className="flex items-start gap-3 rounded-2xl border border-red-200 bg-red-50 p-5 text-sm text-red-700 shadow-sm shadow-red-900/5" role="alert">
      <AlertCircle aria-hidden="true" className="mt-0.5 h-5 w-5 shrink-0" />
      {message}
    </div>
  );
}
