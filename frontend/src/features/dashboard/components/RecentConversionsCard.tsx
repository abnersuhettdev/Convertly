import { Link } from "react-router-dom";
import { ConversionTable } from "../../conversions/components/ConversionTable";
import type { ConversionListItem } from "../../conversions/types/conversionTypes";

type RecentConversionsCardProps = {
  conversions: ConversionListItem[];
};

export function RecentConversionsCard({ conversions }: RecentConversionsCardProps) {
  return (
    <section className="rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
      <div className="flex flex-col justify-between gap-3 sm:flex-row sm:items-center">
        <div>
          <h2 className="text-xl font-semibold text-slate-950">Recent conversions</h2>
          <p className="mt-1 text-sm text-slate-600">The latest jobs created in your workspace.</p>
        </div>
        <Link className="text-sm font-semibold text-emerald-700 hover:text-emerald-800" to="/conversions">
          View all
        </Link>
      </div>
      <div className="mt-5">
        <ConversionTable conversions={conversions} />
      </div>
    </section>
  );
}
