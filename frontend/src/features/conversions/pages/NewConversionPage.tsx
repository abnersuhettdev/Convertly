import { CheckCircle2, Loader2, ShieldCheck } from "lucide-react";
import { useState } from "react";
import type { TFunction } from "i18next";
import { useTranslation } from "react-i18next";
import { Link, useNavigate } from "react-router-dom";
import { PageLoadingState } from "../../../components/ui/PageLoadingState";
import { PageShell } from "../../../components/ui/PageShell";
import { FileUploadBox } from "../components/FileUploadBox";
import { useCreateConversion } from "../hooks/useCreateConversion";
import { useSubscription } from "../hooks/useSubscription";
import { getApiErrorMessage } from "../services/conversionService";

export function NewConversionPage() {
  const navigate = useNavigate();
  const { t } = useTranslation();
  const [file, setFile] = useState<File | null>(null);
  const [contentResponsibilityAccepted, setContentResponsibilityAccepted] = useState(false);
  const [validationError, setValidationError] = useState<string | null>(null);
  const subscriptionQuery = useSubscription();
  const createConversion = useCreateConversion();
  const maxFileSizeMb = subscriptionQuery.data?.maxFileSizeMb ?? 0;

  async function handleSubmit() {
    setValidationError(null);

    const error = validateFile(file, maxFileSizeMb, t);
    if (error) {
      setValidationError(error);
      return;
    }

    try {
      const result = await createConversion.mutateAsync(file!);
      navigate(`/conversions/${result.conversionId}`);
    } catch {
      // The friendly API message is rendered from mutation state below.
    }
  }

  const apiError = createConversion.error ? getApiErrorMessage(createConversion.error, t) : null;

  return (
    <PageShell
      description={t("upload.description")}
      title={t("upload.title")}
    >
      {subscriptionQuery.isLoading ? (
        <PageLoadingState label={t("upload.loadingPlan")} />
      ) : (
      <div className="grid gap-6 lg:grid-cols-[0.9fr_1.1fr]">
        <aside className="space-y-4">
          <div className="rounded-3xl border border-slate-200/80 bg-white p-5 shadow-lg shadow-slate-900/5">
            <div className="flex h-10 w-10 items-center justify-center rounded-2xl bg-emerald-50 text-emerald-700">
              <ShieldCheck aria-hidden="true" className="h-5 w-5" />
            </div>
            <h2 className="mt-4 text-lg font-semibold text-slate-950">{t("upload.beforeTitle")}</h2>
            <p className="mt-2 text-sm leading-6 text-slate-600">{t("upload.beforeText")}</p>
          </div>

          <div className="rounded-3xl border border-slate-200/80 bg-white p-5 text-sm shadow-lg shadow-slate-900/5">
            <p className="font-semibold text-slate-950">{t("upload.acceptedFormatTitle")}</p>
            <div className="mt-3 flex items-center gap-2 text-slate-600">
              <CheckCircle2 aria-hidden="true" className="h-4 w-4 text-emerald-700" />
              {t("upload.acceptedFormat")}
            </div>
          </div>
        </aside>

        <section className="rounded-3xl border border-slate-200/80 bg-white p-5 shadow-xl shadow-slate-900/5">
          <div className="space-y-5">
        {subscriptionQuery.isError ? (
          <p className="rounded-2xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700" role="alert">
            {getApiErrorMessage(subscriptionQuery.error, t)}
          </p>
        ) : (
          <p className="rounded-2xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-800">
            {t("upload.planNotice", { planName: subscriptionQuery.data?.plan.name, maxFileSizeMb })}
          </p>
        )}

        <FileUploadBox file={file} onFileChange={setFile} />

        <div className="rounded-2xl border border-amber-200 bg-amber-50 px-4 py-4 text-sm leading-6 text-amber-950">
          <p>{t("upload.contentResponsibility.notice")}</p>
          <p className="mt-2 text-amber-900">
            <Link
              className="font-semibold underline decoration-amber-500 underline-offset-4 transition hover:text-amber-800 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
              to="/terms"
            >
              {t("legal.terms.linkLabel")}
            </Link>
            <span aria-hidden="true"> · </span>
            <Link
              className="font-semibold underline decoration-amber-500 underline-offset-4 transition hover:text-amber-800 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
              to="/copyright"
            >
              {t("legal.copyright.linkLabel")}
            </Link>
          </p>
        </div>

        <div className="rounded-2xl border border-slate-200 bg-white px-4 py-4 shadow-sm shadow-slate-900/5">
          <label className="flex items-start gap-3 text-sm font-medium leading-6 text-slate-800" htmlFor="content-responsibility">
            <input
              aria-describedby="content-responsibility-help"
              checked={contentResponsibilityAccepted}
              className="mt-1 h-4 w-4 rounded border-slate-300 text-emerald-600 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
              id="content-responsibility"
              onChange={(event) => setContentResponsibilityAccepted(event.target.checked)}
              type="checkbox"
            />
            <span>{t("upload.contentResponsibility.checkbox")}</span>
          </label>
          <p className="mt-2 pl-7 text-xs leading-5 text-slate-500" id="content-responsibility-help">
            {t("upload.contentResponsibility.help")}
          </p>
        </div>

        {validationError || apiError ? (
          <p className="rounded-2xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700" role="alert">
            {validationError ?? apiError}
          </p>
        ) : null}

        <button
          aria-describedby={!contentResponsibilityAccepted ? "create-conversion-disabled-reason" : undefined}
          className="inline-flex h-11 w-full items-center justify-center gap-2 rounded-full bg-gradient-to-r from-emerald-600 to-slate-950 px-5 text-sm font-semibold text-white shadow-lg shadow-emerald-900/15 transition hover:from-emerald-700 hover:to-slate-900 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600 disabled:cursor-not-allowed disabled:from-slate-300 disabled:to-slate-300 sm:w-auto"
          disabled={subscriptionQuery.isLoading || createConversion.isPending || !contentResponsibilityAccepted}
          onClick={handleSubmit}
          type="button"
        >
          {createConversion.isPending ? <Loader2 aria-hidden="true" className="h-4 w-4 animate-spin" /> : null}
          {t("common.createConversion")}
        </button>
        {!contentResponsibilityAccepted ? (
          <p className="text-sm text-slate-600" id="create-conversion-disabled-reason">
            {t("upload.contentResponsibility.disabledReason")}
          </p>
        ) : null}
          </div>
        </section>
      </div>
      )}
    </PageShell>
  );
}

function validateFile(file: File | null, maxFileSizeMb: number, t: TFunction) {
  if (!file) {
    return t("upload.errors.chooseFile");
  }

  if (!file.name.toLowerCase().endsWith(".docx")) {
    return t("upload.errors.unsupported");
  }

  if (maxFileSizeMb > 0 && file.size > maxFileSizeMb * 1024 * 1024) {
    return t("upload.errors.tooLarge", { maxFileSizeMb });
  }

  return null;
}
