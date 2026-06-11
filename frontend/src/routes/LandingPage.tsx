import {
  ArrowRight,
  CheckCircle2,
  ClipboardCheck,
  DownloadCloud,
  FileArchive,
  FileCheck2,
  FileText,
  Gauge,
  Languages,
  LockKeyhole,
  ShieldCheck,
  Sparkles,
  Timer,
  UploadCloud,
} from "lucide-react";
import { useTranslation } from "react-i18next";
import { Link } from "react-router-dom";
import { ButtonLink } from "../components/ui/ButtonLink";

const heroBadges = [
  { icon: LockKeyhole, labelKey: "landing.heroBadges.private" },
  { icon: Timer, labelKey: "landing.heroBadges.retention" },
  { icon: Languages, labelKey: "landing.heroBadges.languages" },
];

const benefits = [
  { icon: LockKeyhole, labelKey: "landing.benefits.privateFiles.label", textKey: "landing.benefits.privateFiles.text" },
  { icon: Timer, labelKey: "landing.benefits.retention.label", textKey: "landing.benefits.retention.text" },
  { icon: Sparkles, labelKey: "landing.benefits.accessibility.label", textKey: "landing.benefits.accessibility.text" },
  { icon: FileArchive, labelKey: "landing.benefits.status.label", textKey: "landing.benefits.status.text" },
  { icon: Languages, labelKey: "landing.benefits.languages.label", textKey: "landing.benefits.languages.text" },
  { icon: ShieldCheck, labelKey: "landing.benefits.validation.label", textKey: "landing.benefits.validation.text" },
];

const steps = [
  {
    icon: UploadCloud,
    titleKey: "landing.steps.upload.title",
    textKey: "landing.steps.upload.text",
  },
  {
    icon: ClipboardCheck,
    titleKey: "landing.steps.responsibility.title",
    textKey: "landing.steps.responsibility.text",
  },
  {
    icon: Gauge,
    titleKey: "landing.steps.status.title",
    textKey: "landing.steps.status.text",
  },
  {
    icon: DownloadCloud,
    titleKey: "landing.steps.download.title",
    textKey: "landing.steps.download.text",
  },
];

const trustItems = [
  { icon: LockKeyhole, titleKey: "landing.trust.private.title", textKey: "landing.trust.private.text" },
  { icon: DownloadCloud, titleKey: "landing.trust.downloads.title", textKey: "landing.trust.downloads.text" },
  { icon: Timer, titleKey: "landing.trust.retention.title", textKey: "landing.trust.retention.text" },
  { icon: Languages, titleKey: "landing.trust.languages.title", textKey: "landing.trust.languages.text" },
  { icon: Sparkles, titleKey: "landing.trust.accessibility.title", textKey: "landing.trust.accessibility.text" },
  { icon: ClipboardCheck, titleKey: "landing.trust.responsible.title", textKey: "landing.trust.responsible.text" },
  { icon: ShieldCheck, titleKey: "landing.trust.validation.title", textKey: "landing.trust.validation.text" },
];

const plans = [
  { name: "Free", conversions: "5/month", size: "10 MB", retention: "24h" },
  { name: "Pro", conversions: "100/month", size: "50 MB", retention: "168h" },
  { name: "Business", conversions: "500/month", size: "200 MB", retention: "720h" },
];

export function LandingPage() {
  const { t } = useTranslation();

  return (
    <div className="overflow-x-hidden bg-slate-50">
      <section className="relative isolate overflow-hidden border-b border-slate-200 bg-white">
        <div aria-hidden="true" className="absolute inset-x-0 top-0 -z-20 h-full bg-[radial-gradient(circle_at_20%_20%,rgba(16,185,129,0.16),transparent_30%),linear-gradient(180deg,#f8fafc_0%,#ffffff_52%,#f8fafc_100%)]" />
        <div aria-hidden="true" className="absolute right-0 top-16 -z-10 h-72 w-72 rounded-full bg-emerald-100/60 blur-3xl" />
        <div aria-hidden="true" className="absolute bottom-0 left-0 -z-10 h-72 w-72 rounded-full bg-sky-100/70 blur-3xl" />

        <div className="mx-auto grid min-h-[calc(100vh-4rem)] w-full max-w-7xl items-center gap-12 px-4 py-16 sm:px-6 lg:grid-cols-[0.95fr_1.05fr] lg:px-8">
          <div className="max-w-3xl">
            <div className="inline-flex items-center gap-2 rounded-full border border-emerald-200 bg-white/90 px-3 py-1 text-sm font-medium text-emerald-800 shadow-sm">
              <FileText aria-hidden="true" className="h-4 w-4" />
              {t("landing.badge")}
            </div>
            <h1 className="mt-6 max-w-4xl text-4xl font-semibold leading-tight text-slate-950 sm:text-6xl lg:text-7xl">
              {t("landing.heroTitle")}
            </h1>
            <p className="mt-6 max-w-2xl text-lg leading-8 text-slate-600 sm:text-xl">
              {t("landing.heroSubtitle")}
            </p>

            <div className="mt-8 flex flex-col gap-3 sm:flex-row">
              <ButtonLink to="/register">{t("common.startNow")}</ButtonLink>
              <ButtonLink to="/#how-it-works" variant="secondary">
                {t("landing.secondaryCta")}
              </ButtonLink>
            </div>

            <div className="mt-7 flex flex-wrap gap-2">
              {heroBadges.map((badge) => (
                <span
                  className="inline-flex items-center gap-2 rounded-full border border-slate-200 bg-white px-3 py-2 text-sm font-medium text-slate-700 shadow-sm"
                  key={badge.labelKey}
                >
                  <badge.icon aria-hidden="true" className="h-4 w-4 text-emerald-700" />
                  {t(badge.labelKey)}
                </span>
              ))}
            </div>

            <p className="mt-6 text-sm leading-6 text-slate-500">{t("landing.institutional")}</p>
          </div>

          <ProductMockup />
        </div>
      </section>

      <section className="mx-auto w-full max-w-7xl px-4 py-12 sm:px-6 lg:px-8" aria-labelledby="landing-benefits-title">
        <div className="flex flex-col justify-between gap-5 sm:flex-row sm:items-end">
          <div>
            <p className="text-sm font-semibold uppercase tracking-wide text-emerald-700">{t("landing.benefitsEyebrow")}</p>
            <h2 className="mt-2 text-3xl font-semibold text-slate-950" id="landing-benefits-title">
              {t("landing.benefitsTitle")}
            </h2>
          </div>
          <p className="max-w-2xl text-sm leading-6 text-slate-600">{t("landing.benefitsDescription")}</p>
        </div>

        <div className="mt-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {benefits.map((benefit) => (
            <article
              className="group rounded-2xl border border-slate-200 bg-white p-5 text-sm shadow-sm transition hover:-translate-y-0.5 hover:border-emerald-200 hover:shadow-lg hover:shadow-slate-200/70"
              key={benefit.labelKey}
            >
              <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-emerald-50 text-emerald-700 transition group-hover:bg-emerald-600 group-hover:text-white">
                <benefit.icon aria-hidden="true" className="h-5 w-5" />
              </div>
              <h3 className="mt-5 text-base font-semibold text-slate-950">{t(benefit.labelKey)}</h3>
              <p className="mt-2 leading-6 text-slate-600">{t(benefit.textKey)}</p>
            </article>
          ))}
        </div>
      </section>

      <section className="border-y border-slate-200 bg-white" id="how-it-works" aria-labelledby="how-it-works-title">
        <div className="mx-auto w-full max-w-7xl px-4 py-14 sm:px-6 lg:px-8">
          <div className="max-w-3xl">
            <p className="text-sm font-semibold uppercase tracking-wide text-emerald-700">{t("landing.steps.eyebrow")}</p>
            <h2 className="mt-2 text-3xl font-semibold text-slate-950" id="how-it-works-title">{t("landing.steps.title")}</h2>
            <p className="mt-3 text-sm leading-6 text-slate-600">{t("landing.steps.description")}</p>
          </div>

          <div className="mt-9 grid gap-4 lg:grid-cols-4">
            {steps.map((step, index) => (
              <article className="relative rounded-2xl border border-slate-200 bg-slate-50 p-5 shadow-sm" key={step.titleKey}>
                <div className="flex items-center justify-between gap-4">
                  <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-slate-950 text-white shadow-sm">
                    <step.icon aria-hidden="true" className="h-5 w-5" />
                  </div>
                  <span className="text-sm font-semibold text-slate-400">0{index + 1}</span>
                </div>
                <h3 className="mt-5 text-lg font-semibold text-slate-950">{t(step.titleKey)}</h3>
                <p className="mt-2 text-sm leading-6 text-slate-600">{t(step.textKey)}</p>
              </article>
            ))}
          </div>
        </div>
      </section>

      <section className="mx-auto w-full max-w-7xl px-4 py-14 sm:px-6 lg:px-8" aria-labelledby="trust-title">
        <div className="rounded-3xl border border-slate-200 bg-slate-950 p-6 text-white shadow-xl shadow-slate-200/70 sm:p-8 lg:p-10">
          <div className="grid gap-8 lg:grid-cols-[0.85fr_1.15fr] lg:items-start">
            <div>
              <p className="text-sm font-semibold uppercase tracking-wide text-emerald-300">{t("landing.trust.eyebrow")}</p>
              <h2 className="mt-2 text-3xl font-semibold" id="trust-title">{t("landing.trust.title")}</h2>
              <p className="mt-4 text-sm leading-6 text-slate-300">{t("landing.trust.description")}</p>
            </div>
            <div className="grid gap-3 sm:grid-cols-2">
              {trustItems.map((item) => (
                <article className="rounded-2xl border border-white/10 bg-white/[0.06] p-4" key={item.titleKey}>
                  <item.icon aria-hidden="true" className="h-5 w-5 text-emerald-300" />
                  <h3 className="mt-3 text-sm font-semibold text-white">{t(item.titleKey)}</h3>
                  <p className="mt-1 text-sm leading-6 text-slate-300">{t(item.textKey)}</p>
                </article>
              ))}
            </div>
          </div>
        </div>
      </section>

      <section className="border-y border-slate-200 bg-white">
        <div className="mx-auto w-full max-w-7xl px-4 py-14 sm:px-6 lg:px-8">
          <div className="flex flex-col justify-between gap-5 sm:flex-row sm:items-end">
            <div>
              <p className="text-sm font-semibold uppercase tracking-wide text-emerald-700">{t("landing.plans.eyebrow")}</p>
              <h2 className="mt-2 text-3xl font-semibold text-slate-950">{t("landing.plans.title")}</h2>
            </div>
            <p className="max-w-xl text-sm leading-6 text-slate-600">{t("landing.plans.description")}</p>
          </div>
          <div className="mt-8 grid gap-5 lg:grid-cols-3">
            {plans.map((plan) => (
              <article className="rounded-2xl border border-slate-200 bg-slate-50 p-6 shadow-sm" key={plan.name}>
                <div className="flex items-center justify-between gap-4">
                  <h3 className="text-xl font-semibold text-slate-950">{plan.name}</h3>
                  <CheckCircle2 aria-hidden="true" className="h-5 w-5 text-emerald-600" />
                </div>
                <dl className="mt-6 space-y-3 text-sm text-slate-600">
                  <div className="flex justify-between gap-4">
                    <dt>{t("landing.plans.conversions")}</dt>
                    <dd className="font-semibold text-slate-950">{plan.conversions}</dd>
                  </div>
                  <div className="flex justify-between gap-4">
                    <dt>{t("landing.plans.maxFileSize")}</dt>
                    <dd className="font-semibold text-slate-950">{plan.size}</dd>
                  </div>
                  <div className="flex justify-between gap-4">
                    <dt>{t("landing.plans.retention")}</dt>
                    <dd className="font-semibold text-slate-950">{plan.retention}</dd>
                  </div>
                </dl>
              </article>
            ))}
          </div>
        </div>
      </section>

      <section className="mx-auto w-full max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
        <div className="relative overflow-hidden rounded-3xl border border-slate-200 bg-white p-6 shadow-xl shadow-slate-200/70 sm:p-8 lg:p-10">
          <div aria-hidden="true" className="absolute right-0 top-0 h-40 w-40 rounded-full bg-emerald-100 blur-3xl" />
          <div className="relative flex flex-col justify-between gap-7 lg:flex-row lg:items-center">
            <div className="max-w-2xl">
              <p className="text-sm font-semibold uppercase tracking-wide text-emerald-700">{t("landing.finalCta.eyebrow")}</p>
              <h2 className="mt-2 text-3xl font-semibold text-slate-950">{t("landing.finalCta.title")}</h2>
              <p className="mt-3 text-sm leading-6 text-slate-600">{t("landing.finalCta.text")}</p>
              <div className="mt-4 flex flex-wrap gap-x-4 gap-y-2 text-sm text-slate-600">
                <Link
                  className="rounded-sm font-semibold underline-offset-4 transition hover:text-slate-950 hover:underline focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
                  to="/terms"
                >
                  {t("legal.terms.linkLabel")}
                </Link>
                <Link
                  className="rounded-sm font-semibold underline-offset-4 transition hover:text-slate-950 hover:underline focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
                  to="/copyright"
                >
                  {t("legal.copyright.linkLabel")}
                </Link>
              </div>
            </div>
            <ButtonLink to="/register">{t("common.startNow")}</ButtonLink>
          </div>
        </div>
      </section>
    </div>
  );
}

function ProductMockup() {
  const { t } = useTranslation();

  return (
    <div aria-hidden="true" className="relative mx-auto w-full max-w-xl lg:mx-0">
      <div className="absolute -inset-4 rounded-[2rem] bg-emerald-100/60 blur-2xl" />
      <div className="relative rounded-[2rem] border border-slate-200 bg-white p-3 shadow-2xl shadow-slate-300/60">
        <div className="rounded-[1.5rem] border border-slate-200 bg-slate-950 p-4 text-white">
          <div className="flex items-center justify-between gap-4">
            <div>
              <p className="text-xs font-semibold uppercase tracking-wide text-emerald-300">{t("landing.mockup.workspace")}</p>
              <p className="mt-1 text-lg font-semibold">{t("landing.mockup.title")}</p>
            </div>
            <div className="rounded-full border border-white/10 bg-white/10 px-3 py-1 text-xs font-semibold text-emerald-200">
              {t("landing.mockup.status")}
            </div>
          </div>

          <div className="mt-5 grid gap-3 sm:grid-cols-[1fr_auto_1fr] sm:items-center">
            <div className="rounded-2xl border border-white/10 bg-white/[0.06] p-4">
              <FileText aria-hidden="true" className="h-6 w-6 text-sky-300" />
              <p className="mt-3 text-sm font-semibold">{t("landing.mockup.source")}</p>
              <p className="mt-1 text-xs text-slate-300">{t("landing.mockup.sourceMeta")}</p>
            </div>
            <div className="hidden h-10 w-10 items-center justify-center rounded-full border border-white/10 bg-white/[0.08] text-emerald-300 sm:flex">
              <ArrowRight aria-hidden="true" className="h-5 w-5" />
            </div>
            <div className="rounded-2xl border border-emerald-300/30 bg-emerald-400/10 p-4">
              <FileCheck2 aria-hidden="true" className="h-6 w-6 text-emerald-300" />
              <p className="mt-3 text-sm font-semibold">{t("landing.mockup.target")}</p>
              <p className="mt-1 text-xs text-emerald-100">{t("landing.mockup.targetMeta")}</p>
            </div>
          </div>

          <div className="mt-4 rounded-2xl border border-white/10 bg-white/[0.06] p-4">
            <div className="flex items-center justify-between gap-4 text-xs text-slate-300">
              <span>{t("landing.mockup.progressLabel")}</span>
              <span>{t("landing.mockup.progressValue")}</span>
            </div>
            <div className="mt-3 h-2 rounded-full bg-white/10">
              <div className="h-2 w-4/5 rounded-full bg-emerald-400" />
            </div>
          </div>

          <div className="mt-4 grid gap-3 sm:grid-cols-3">
            <Metric label={t("landing.mockup.metricPlan")} value="Free" />
            <Metric label={t("landing.mockup.metricLimit")} value="10 MB" />
            <Metric label={t("landing.mockup.metricRetention")} value="24h" />
          </div>
        </div>
      </div>
    </div>
  );
}

function Metric({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-2xl border border-white/10 bg-white/[0.06] p-3">
      <p className="text-xs text-slate-300">{label}</p>
      <p className="mt-1 text-sm font-semibold text-white">{value}</p>
    </div>
  );
}
