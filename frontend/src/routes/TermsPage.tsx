import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";

const sections = [
  "whatConvertlyDoes",
  "userResponsibility",
  "allowedUse",
  "privateFiles",
  "retention",
  "serviceLimits",
  "futureContact",
];

export function TermsPage() {
  const { t } = useTranslation();

  return (
    <div className="bg-slate-50">
      <section className="mx-auto w-full max-w-4xl px-4 py-12 sm:px-6 lg:px-8">
        <div className="rounded-3xl border border-slate-200/80 bg-white/90 p-6 shadow-xl shadow-slate-900/5 backdrop-blur sm:p-8">
          <p className="text-sm font-semibold uppercase tracking-wide text-emerald-700">
            {t("legal.terms.eyebrow")}
          </p>
          <h1 className="mt-3 text-4xl font-semibold text-slate-950">
            {t("legal.terms.title")}
          </h1>
          <p className="mt-4 text-base leading-7 text-slate-600">
            {t("legal.terms.intro")}
          </p>
        </div>

        <div className="mt-8 space-y-4">
          {sections.map((section) => (
            <article className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm shadow-slate-900/5" key={section}>
              <h2 className="text-lg font-semibold text-slate-950">
                {t(`legal.terms.sections.${section}.title`)}
              </h2>
              <p className="mt-2 text-sm leading-6 text-slate-600">
                {t(`legal.terms.sections.${section}.text`)}
              </p>
            </article>
          ))}
        </div>

        <div className="mt-8 rounded-2xl border border-emerald-200 bg-emerald-50 p-5 text-sm leading-6 text-emerald-950">
          {t("legal.terms.relatedText")}{" "}
          <Link
            className="font-semibold underline decoration-emerald-500 underline-offset-4 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
            to="/copyright"
          >
            {t("legal.copyright.linkLabel")}
          </Link>
        </div>
      </section>
    </div>
  );
}
