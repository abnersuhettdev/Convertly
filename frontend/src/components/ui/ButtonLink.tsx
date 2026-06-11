import { ArrowRight } from "lucide-react";
import type { ReactNode } from "react";
import { Link } from "react-router-dom";

type ButtonLinkProps = {
  to: string;
  children: ReactNode;
  variant?: "primary" | "secondary";
};

export function ButtonLink({ to, children, variant = "primary" }: ButtonLinkProps) {
  const className =
    variant === "primary"
      ? "inline-flex min-h-11 items-center justify-center gap-2 rounded-full bg-gradient-to-r from-emerald-600 to-slate-950 px-5 py-2.5 text-sm font-semibold text-white shadow-lg shadow-emerald-900/15 transition hover:from-emerald-700 hover:to-slate-900 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
      : "inline-flex min-h-11 items-center justify-center gap-2 rounded-full border border-slate-300 bg-white/90 px-5 py-2.5 text-sm font-semibold text-slate-800 shadow-sm shadow-slate-900/5 transition hover:border-slate-400 hover:bg-white focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600";

  return (
    <Link className={className} to={to}>
      {children}
      <ArrowRight aria-hidden="true" className="h-4 w-4" />
    </Link>
  );
}
