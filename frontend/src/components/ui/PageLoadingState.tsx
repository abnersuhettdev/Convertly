import { Loader2 } from "lucide-react";
import { useTranslation } from "react-i18next";

type PageLoadingStateProps = {
  label?: string;
};

export function PageLoadingState({ label }: PageLoadingStateProps) {
  const { t } = useTranslation();

  return (
    <div
      className="flex min-h-64 flex-col items-center justify-center rounded-3xl border border-slate-200/80 bg-white p-8 text-center shadow-xl shadow-slate-900/5"
      role="status"
    >
      <Loader2 aria-hidden="true" className="h-8 w-8 animate-spin text-emerald-700" />
      <p className="mt-4 text-sm font-medium text-slate-600">{label ?? t("common.loadingData")}</p>
    </div>
  );
}
