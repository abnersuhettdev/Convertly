import { AlertCircle } from "lucide-react";
import { RecentConversionsCard } from "../components/RecentConversionsCard";
import { UsageSummaryCard } from "../components/UsageSummaryCard";
import { useConversions } from "../../conversions/hooks/useConversions";
import { useSubscription } from "../../conversions/hooks/useSubscription";
import { getApiErrorMessage } from "../../conversions/services/conversionService";

export function DashboardPage() {
  const subscriptionQuery = useSubscription();
  const conversionsQuery = useConversions({ page: 1, pageSize: 5 });

  return (
    <section className="mx-auto flex w-full max-w-6xl flex-1 flex-col px-4 py-10 sm:px-6 lg:px-8">
      <div className="flex flex-col justify-between gap-4 sm:flex-row sm:items-end">
        <div>
          <p className="text-sm font-semibold uppercase tracking-wide text-emerald-700">Convertly</p>
          <h1 className="mt-3 text-3xl font-semibold text-slate-950 sm:text-4xl">Dashboard</h1>
          <p className="mt-4 max-w-2xl text-base leading-7 text-slate-600">
            Track your plan, monthly usage and recent DOCX to PDF jobs.
          </p>
        </div>
      </div>

      <div className="mt-8 space-y-6">
        {subscriptionQuery.isLoading ? (
          <LoadingBlock label="Loading subscription..." />
        ) : subscriptionQuery.isError ? (
          <ErrorBlock message={getApiErrorMessage(subscriptionQuery.error)} />
        ) : subscriptionQuery.data ? (
          <UsageSummaryCard subscription={subscriptionQuery.data} />
        ) : null}

        {conversionsQuery.isLoading ? (
          <LoadingBlock label="Loading recent conversions..." />
        ) : conversionsQuery.isError ? (
          <ErrorBlock message={getApiErrorMessage(conversionsQuery.error)} />
        ) : (
          <RecentConversionsCard conversions={conversionsQuery.data?.items ?? []} />
        )}
      </div>
    </section>
  );
}

function LoadingBlock({ label }: { label: string }) {
  return <div className="rounded-lg border border-slate-200 bg-white p-6 text-sm text-slate-600 shadow-sm">{label}</div>;
}

function ErrorBlock({ message }: { message: string }) {
  return (
    <div className="flex items-start gap-3 rounded-lg border border-red-200 bg-red-50 p-5 text-sm text-red-700">
      <AlertCircle aria-hidden="true" className="mt-0.5 h-5 w-5 shrink-0" />
      {message}
    </div>
  );
}
