import type { ReactNode } from "react";
import { useTranslation } from "react-i18next";

type PageShellProps = {
  title: string;
  description: string;
  children?: ReactNode;
};

export function PageShell({ title, description, children }: PageShellProps) {
  const { t } = useTranslation();

  return (
    <section className="mx-auto flex w-full max-w-6xl flex-1 flex-col px-4 py-10 sm:px-6 lg:px-8">
      <div className="overflow-hidden rounded-3xl border border-slate-200/80 bg-white/90 p-6 shadow-xl shadow-slate-900/5 backdrop-blur sm:p-8">
        <div className="max-w-3xl">
          <p className="text-xs font-semibold uppercase tracking-[0.16em] text-emerald-700">{t("common.brandEyebrow")}</p>
          <h1 className="mt-3 text-3xl font-semibold leading-tight text-slate-950 sm:text-4xl">{title}</h1>
          <p className="mt-4 text-base leading-7 text-slate-600">{description}</p>
        </div>
      </div>
      <div className="mt-8">{children}</div>
    </section>
  );
}
