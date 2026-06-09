import { CheckCircle2, FileArchive, FileText, ShieldCheck, Timer, UploadCloud } from "lucide-react";
import heroImage from "../assets/hero.png";
import { ButtonLink } from "../components/ui/ButtonLink";

const steps = [
  {
    icon: UploadCloud,
    title: "Upload DOCX",
    text: "Send a Word document to the backend API.",
  },
  {
    icon: Timer,
    title: "Background job",
    text: "Hangfire processes the conversion outside the request.",
  },
  {
    icon: FileArchive,
    title: "PDF ready",
    text: "The converted file is stored securely for download.",
  },
];

const plans = [
  { name: "Free", conversions: "5/month", size: "10 MB", retention: "24h" },
  { name: "Pro", conversions: "100/month", size: "50 MB", retention: "168h" },
  { name: "Business", conversions: "500/month", size: "200 MB", retention: "720h" },
];

export function LandingPage() {
  return (
    <div className="bg-slate-50">
      <section className="relative isolate overflow-hidden border-b border-slate-200 bg-white">
        <img
          alt=""
          className="pointer-events-none absolute inset-y-10 right-0 -z-10 hidden h-[520px] w-[520px] object-contain opacity-20 lg:block"
          src={heroImage}
        />
        <div className="mx-auto flex min-h-[calc(100vh-4rem)] w-full max-w-7xl flex-col justify-center px-4 py-16 sm:px-6 lg:px-8">
          <div className="max-w-3xl">
            <div className="inline-flex items-center gap-2 rounded-md border border-emerald-200 bg-emerald-50 px-3 py-1 text-sm font-medium text-emerald-800">
              <FileText aria-hidden="true" className="h-4 w-4" />
              MVP supports DOCX to PDF
            </div>
            <h1 className="mt-6 text-5xl font-semibold leading-tight text-slate-950 sm:text-6xl lg:text-7xl">
              Convertly
            </h1>
            <p className="mt-6 max-w-2xl text-lg leading-8 text-slate-600 sm:text-xl">
              A clean SaaS foundation for secure document conversion, with API-first processing, private storage and
              usage-aware plans.
            </p>
            <div className="mt-8 flex flex-col gap-3 sm:flex-row">
              <ButtonLink to="/login">Login</ButtonLink>
              <ButtonLink to="/register" variant="secondary">
                Register
              </ButtonLink>
            </div>
          </div>
        </div>
      </section>

      <section className="mx-auto grid w-full max-w-7xl gap-6 px-4 py-14 sm:px-6 lg:grid-cols-3 lg:px-8">
        {steps.map((step) => (
          <article className="rounded-lg border border-slate-200 bg-white p-6 shadow-sm" key={step.title}>
            <div className="flex h-11 w-11 items-center justify-center rounded-md bg-slate-950 text-white">
              <step.icon aria-hidden="true" className="h-5 w-5" />
            </div>
            <h2 className="mt-5 text-lg font-semibold text-slate-950">{step.title}</h2>
            <p className="mt-2 text-sm leading-6 text-slate-600">{step.text}</p>
          </article>
        ))}
      </section>

      <section className="border-y border-slate-200 bg-white">
        <div className="mx-auto w-full max-w-7xl px-4 py-14 sm:px-6 lg:px-8">
          <div className="flex flex-col justify-between gap-5 sm:flex-row sm:items-end">
            <div>
              <p className="text-sm font-semibold uppercase tracking-wide text-emerald-700">Plans</p>
              <h2 className="mt-2 text-3xl font-semibold text-slate-950">Simple limits for the MVP</h2>
            </div>
            <p className="max-w-xl text-sm leading-6 text-slate-600">
              Plan cards are mocked in this phase. Live plan data and billing interactions come later.
            </p>
          </div>
          <div className="mt-8 grid gap-5 lg:grid-cols-3">
            {plans.map((plan) => (
              <article className="rounded-lg border border-slate-200 bg-slate-50 p-6" key={plan.name}>
                <div className="flex items-center justify-between gap-4">
                  <h3 className="text-xl font-semibold text-slate-950">{plan.name}</h3>
                  <CheckCircle2 aria-hidden="true" className="h-5 w-5 text-emerald-600" />
                </div>
                <dl className="mt-6 space-y-3 text-sm text-slate-600">
                  <div className="flex justify-between gap-4">
                    <dt>Conversions</dt>
                    <dd className="font-semibold text-slate-950">{plan.conversions}</dd>
                  </div>
                  <div className="flex justify-between gap-4">
                    <dt>Max file size</dt>
                    <dd className="font-semibold text-slate-950">{plan.size}</dd>
                  </div>
                  <div className="flex justify-between gap-4">
                    <dt>Retention</dt>
                    <dd className="font-semibold text-slate-950">{plan.retention}</dd>
                  </div>
                </dl>
              </article>
            ))}
          </div>
        </div>
      </section>

      <section className="mx-auto flex w-full max-w-7xl flex-col gap-5 px-4 py-12 sm:px-6 lg:px-8">
        <div className="rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
          <div className="flex items-start gap-4">
            <ShieldCheck aria-hidden="true" className="mt-1 h-6 w-6 shrink-0 text-emerald-700" />
            <div>
              <h2 className="text-lg font-semibold text-slate-950">Backend-controlled storage</h2>
              <p className="mt-2 text-sm leading-6 text-slate-600">
                The frontend talks only to the Convertly API. Private Supabase buckets and service role credentials stay
                server-side.
              </p>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}
