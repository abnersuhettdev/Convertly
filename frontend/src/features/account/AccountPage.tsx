import { PageShell } from "../../components/ui/PageShell";

export function AccountPage() {
  return (
    <PageShell description="Profile and session controls will be connected after auth is implemented." title="Account">
      <div className="grid gap-4 sm:max-w-lg">
        <input className="h-11 rounded-md border border-slate-300 px-3 text-sm" disabled placeholder="Name" />
        <input className="h-11 rounded-md border border-slate-300 px-3 text-sm" disabled placeholder="Email" />
      </div>
    </PageShell>
  );
}
