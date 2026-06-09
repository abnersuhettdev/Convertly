import { Loader2 } from "lucide-react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { PageShell } from "../../../components/ui/PageShell";
import { FileUploadBox } from "../components/FileUploadBox";
import { useCreateConversion } from "../hooks/useCreateConversion";
import { useSubscription } from "../hooks/useSubscription";
import { getApiErrorMessage } from "../services/conversionService";

export function NewConversionPage() {
  const navigate = useNavigate();
  const [file, setFile] = useState<File | null>(null);
  const [validationError, setValidationError] = useState<string | null>(null);
  const subscriptionQuery = useSubscription();
  const createConversion = useCreateConversion();
  const maxFileSizeMb = subscriptionQuery.data?.maxFileSizeMb ?? 0;

  async function handleSubmit() {
    setValidationError(null);

    const error = validateFile(file, maxFileSizeMb);
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

  const apiError = createConversion.error ? getApiErrorMessage(createConversion.error) : null;

  return (
    <PageShell
      description="Upload a DOCX file and Convertly will create a PDF conversion job."
      title="New conversion"
    >
      <div className="space-y-5">
        {subscriptionQuery.isLoading ? (
          <p className="text-sm text-slate-600">Loading your plan limits...</p>
        ) : subscriptionQuery.isError ? (
          <p className="rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
            {getApiErrorMessage(subscriptionQuery.error)}
          </p>
        ) : (
          <p className="rounded-md border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-800">
            Your {subscriptionQuery.data?.plan.name} plan accepts DOCX files up to {maxFileSizeMb} MB. Target format:
            PDF.
          </p>
        )}

        <FileUploadBox file={file} onFileChange={setFile} />

        {validationError || apiError ? (
          <p className="rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
            {validationError ?? apiError}
          </p>
        ) : null}

        <button
          className="inline-flex h-11 items-center justify-center gap-2 rounded-md bg-emerald-600 px-5 text-sm font-semibold text-white transition hover:bg-emerald-700 disabled:cursor-not-allowed disabled:bg-slate-300"
          disabled={subscriptionQuery.isLoading || createConversion.isPending}
          onClick={handleSubmit}
          type="button"
        >
          {createConversion.isPending ? <Loader2 aria-hidden="true" className="h-4 w-4 animate-spin" /> : null}
          Create conversion
        </button>
      </div>
    </PageShell>
  );
}

function validateFile(file: File | null, maxFileSizeMb: number) {
  if (!file) {
    return "Choose a DOCX file before creating a conversion.";
  }

  if (!file.name.toLowerCase().endsWith(".docx")) {
    return "Only .docx files are supported.";
  }

  if (maxFileSizeMb > 0 && file.size > maxFileSizeMb * 1024 * 1024) {
    return `This file is larger than your ${maxFileSizeMb} MB plan limit.`;
  }

  return null;
}
