import { AlertTriangle, CheckCircle2, KeyRound, Link as LinkIcon, Loader2, ShieldCheck, UserRound, type LucideIcon } from "lucide-react";
import type { TFunction } from "i18next";
import type { FormEvent } from "react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Link, useNavigate } from "react-router-dom";
import { PageLoadingState } from "../../components/ui/PageLoadingState";
import { PageShell } from "../../components/ui/PageShell";
import { useAuth } from "../auth/hooks/useAuth";
import { useSubscription } from "../conversions/hooks/useSubscription";
import { changePassword, deleteAccount, getAccountErrorMessage } from "./services/accountService";

type PasswordForm = {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
};

type PasswordErrors = Partial<Record<keyof PasswordForm, string>>;

export function AccountPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { logout, user } = useAuth();
  const subscriptionQuery = useSubscription();
  const [passwordForm, setPasswordForm] = useState<PasswordForm>({
    currentPassword: "",
    newPassword: "",
    confirmPassword: "",
  });
  const [passwordErrors, setPasswordErrors] = useState<PasswordErrors>({});
  const [passwordMessage, setPasswordMessage] = useState<string | null>(null);
  const [passwordError, setPasswordError] = useState<string | null>(null);
  const [isChangingPassword, setIsChangingPassword] = useState(false);
  const [deletePassword, setDeletePassword] = useState("");
  const [deleteConfirmed, setDeleteConfirmed] = useState(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const [isDeletingAccount, setIsDeletingAccount] = useState(false);

  function updatePasswordField(field: keyof PasswordForm, value: string) {
    setPasswordForm((current) => ({ ...current, [field]: value }));
    setPasswordErrors((current) => ({ ...current, [field]: undefined }));
    setPasswordMessage(null);
    setPasswordError(null);
  }

  async function handlePasswordSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setPasswordMessage(null);
    setPasswordError(null);

    const nextErrors = validatePasswordForm(passwordForm, t);
    setPasswordErrors(nextErrors);
    if (Object.keys(nextErrors).length > 0) {
      return;
    }

    setIsChangingPassword(true);
    try {
      await changePassword({
        currentPassword: passwordForm.currentPassword,
        newPassword: passwordForm.newPassword,
      });
      setPasswordForm({ currentPassword: "", newPassword: "", confirmPassword: "" });
      setPasswordMessage(t("account.messages.passwordChanged"));
    } catch (error) {
      setPasswordError(getAccountErrorMessage(error, t));
    } finally {
      setIsChangingPassword(false);
    }
  }

  async function handleDeleteSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setDeleteError(null);

    if (!deleteConfirmed) {
      setDeleteError(t("account.validation.deleteConfirmationRequired"));
      return;
    }

    if (!deletePassword.trim()) {
      setDeleteError(t("account.validation.currentPasswordRequired"));
      return;
    }

    setIsDeletingAccount(true);
    try {
      await deleteAccount({ currentPassword: deletePassword });
      logout();
      navigate("/login");
    } catch (error) {
      setDeleteError(getAccountErrorMessage(error, t));
    } finally {
      setIsDeletingAccount(false);
    }
  }

  const subscription = subscriptionQuery.data;

  return (
    <PageShell description={t("account.description")} title={t("account.title")}>
      {subscriptionQuery.isLoading ? (
        <PageLoadingState label={t("account.plan.loading")} />
      ) : (
      <div className="grid gap-6 lg:grid-cols-[1fr_1fr]">
        <section className="rounded-3xl border border-slate-200/80 bg-white p-6 shadow-xl shadow-slate-900/5">
          <SectionHeader
            description={t("account.profile.description")}
            icon={UserRound}
            title={t("account.profile.title")}
          />
          <dl className="mt-6 grid gap-4">
            <InfoRow label={t("account.name")} value={user?.name ?? t("account.unavailable")} />
            <InfoRow label={t("account.email")} value={user?.email ?? t("account.unavailable")} />
            <InfoRow label={t("account.profile.status")} value={t("account.profile.active")} />
          </dl>
        </section>

        <section className="rounded-3xl border border-slate-200/80 bg-white p-6 shadow-xl shadow-slate-900/5">
          <SectionHeader
            description={t("account.plan.description")}
            icon={ShieldCheck}
            title={t("account.plan.title")}
          />
          {subscriptionQuery.isError ? (
            <div className="mt-6 rounded-2xl border border-red-200 bg-red-50 p-4 text-sm text-red-700" role="alert">
              {t("account.plan.error")}
            </div>
          ) : subscription ? (
            <dl className="mt-6 grid gap-4">
              <InfoRow label={t("common.currentPlan")} value={subscription.plan.name} />
              <InfoRow label={t("dashboard.summary.usedMetric")} value={`${subscription.conversionsUsed} / ${subscription.monthlyLimit}`} />
              <InfoRow label={t("dashboard.summary.retention")} value={`${subscription.retentionHours}h`} />
            </dl>
          ) : null}
        </section>

        <section className="rounded-3xl border border-slate-200/80 bg-white p-6 shadow-xl shadow-slate-900/5 lg:col-span-2">
          <SectionHeader
            description={t("account.security.description")}
            icon={KeyRound}
            title={t("account.security.title")}
          />
          <form className="mt-6 grid gap-5 sm:max-w-2xl" onSubmit={handlePasswordSubmit}>
            <PasswordField
              autoComplete="current-password"
              error={passwordErrors.currentPassword}
              id="current-password"
              label={t("account.security.currentPassword")}
              onChange={(value) => updatePasswordField("currentPassword", value)}
              value={passwordForm.currentPassword}
            />
            <PasswordField
              autoComplete="new-password"
              error={passwordErrors.newPassword}
              id="new-password"
              label={t("account.security.newPassword")}
              onChange={(value) => updatePasswordField("newPassword", value)}
              value={passwordForm.newPassword}
            />
            <PasswordField
              autoComplete="new-password"
              error={passwordErrors.confirmPassword}
              id="confirm-password"
              label={t("account.security.confirmPassword")}
              onChange={(value) => updatePasswordField("confirmPassword", value)}
              value={passwordForm.confirmPassword}
            />

            {passwordError ? (
              <div className="rounded-2xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700" role="alert">
                {passwordError}
              </div>
            ) : null}

            {passwordMessage ? (
              <div className="flex items-start gap-3 rounded-2xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-800" role="status">
                <CheckCircle2 aria-hidden="true" className="mt-0.5 h-5 w-5 shrink-0" />
                {passwordMessage}
              </div>
            ) : null}

            <button
              className="inline-flex h-11 w-full items-center justify-center gap-2 rounded-full bg-gradient-to-r from-emerald-600 to-slate-950 px-5 text-sm font-semibold text-white shadow-lg shadow-emerald-900/15 transition hover:from-emerald-700 hover:to-slate-900 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600 disabled:cursor-not-allowed disabled:from-slate-300 disabled:to-slate-300 sm:w-auto"
              disabled={isChangingPassword}
              type="submit"
            >
              {isChangingPassword ? <Loader2 aria-hidden="true" className="h-4 w-4 animate-spin" /> : null}
              {t("account.security.submit")}
            </button>
          </form>
        </section>

        <section className="rounded-3xl border border-slate-200/80 bg-white p-6 shadow-xl shadow-slate-900/5">
          <SectionHeader
            description={t("account.links.description")}
            icon={LinkIcon}
            title={t("account.links.title")}
          />
          <div className="mt-6 flex flex-wrap gap-3">
            <Link
              className="inline-flex h-10 items-center rounded-full border border-slate-300 bg-white px-4 text-sm font-semibold text-slate-800 shadow-sm transition hover:border-slate-400 hover:bg-slate-50 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
              to="/terms"
            >
              {t("legal.terms.linkLabel")}
            </Link>
            <Link
              className="inline-flex h-10 items-center rounded-full border border-slate-300 bg-white px-4 text-sm font-semibold text-slate-800 shadow-sm transition hover:border-slate-400 hover:bg-slate-50 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
              to="/copyright"
            >
              {t("legal.copyright.linkLabel")}
            </Link>
          </div>
        </section>

        <section className="rounded-3xl border border-red-200 bg-red-50/70 p-6 shadow-xl shadow-red-900/5">
          <SectionHeader
            description={t("account.danger.description")}
            icon={AlertTriangle}
            title={t("account.danger.title")}
          />
          <form className="mt-6 grid gap-4" onSubmit={handleDeleteSubmit}>
            <PasswordField
              autoComplete="current-password"
              error={undefined}
              id="delete-current-password"
              label={t("account.danger.currentPassword")}
              onChange={(value) => {
                setDeletePassword(value);
                setDeleteError(null);
              }}
              value={deletePassword}
            />
            <label className="flex items-start gap-3 rounded-2xl border border-red-200 bg-white/80 p-4 text-sm font-medium leading-6 text-slate-800" htmlFor="delete-confirmation">
              <input
                checked={deleteConfirmed}
                className="mt-1 h-4 w-4 rounded border-slate-300 text-red-600 focus-visible:outline focus-visible:outline-2 focus-visible:outline-red-600"
                id="delete-confirmation"
                onChange={(event) => {
                  setDeleteConfirmed(event.target.checked);
                  setDeleteError(null);
                }}
                type="checkbox"
              />
              <span>{t("account.danger.confirmation")}</span>
            </label>

            {deleteError ? (
              <div className="rounded-2xl border border-red-300 bg-white px-4 py-3 text-sm text-red-700" role="alert">
                {deleteError}
              </div>
            ) : null}

            <button
              className="inline-flex h-11 w-full items-center justify-center gap-2 rounded-full bg-red-700 px-5 text-sm font-semibold text-white shadow-lg shadow-red-900/15 transition hover:bg-red-800 focus-visible:outline focus-visible:outline-2 focus-visible:outline-red-700 disabled:cursor-not-allowed disabled:bg-slate-300 sm:w-auto"
              disabled={isDeletingAccount || !deleteConfirmed}
              type="submit"
            >
              {isDeletingAccount ? <Loader2 aria-hidden="true" className="h-4 w-4 animate-spin" /> : null}
              {t("account.danger.submit")}
            </button>
          </form>
        </section>
      </div>
      )}
    </PageShell>
  );
}

type SectionHeaderProps = {
  description: string;
  icon: LucideIcon;
  title: string;
};

function SectionHeader({ description, icon: Icon, title }: SectionHeaderProps) {
  return (
    <div>
      <div className="flex h-11 w-11 items-center justify-center rounded-2xl bg-emerald-50 text-emerald-700">
        <Icon aria-hidden="true" className="h-5 w-5" />
      </div>
      <h2 className="mt-4 text-xl font-semibold text-slate-950">{title}</h2>
      <p className="mt-2 text-sm leading-6 text-slate-600">{description}</p>
    </div>
  );
}

function InfoRow({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-2xl border border-slate-200 bg-slate-50 p-4">
      <dt className="text-sm font-semibold text-slate-700">{label}</dt>
      <dd className="mt-1 break-words text-sm text-slate-600">{value}</dd>
    </div>
  );
}

type PasswordFieldProps = {
  autoComplete: string;
  error: string | undefined;
  id: string;
  label: string;
  onChange: (value: string) => void;
  value: string;
};

function PasswordField({ autoComplete, error, id, label, onChange, value }: PasswordFieldProps) {
  const errorId = `${id}-error`;

  return (
    <div>
      <label className="text-sm font-semibold text-slate-800" htmlFor={id}>
        {label}
      </label>
      <input
        aria-describedby={error ? errorId : undefined}
        aria-invalid={error ? "true" : "false"}
        autoComplete={autoComplete}
        className="mt-2 h-12 w-full rounded-xl border border-slate-300 bg-white px-3 text-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-100"
        id={id}
        onChange={(event) => onChange(event.target.value)}
        type="password"
        value={value}
      />
      {error ? (
        <p className="mt-2 text-sm text-red-600" id={errorId}>
          {error}
        </p>
      ) : null}
    </div>
  );
}

function validatePasswordForm(passwordForm: PasswordForm, t: TFunction) {
  const errors: PasswordErrors = {};

  if (!passwordForm.currentPassword.trim()) {
    errors.currentPassword = t("account.validation.currentPasswordRequired");
  }

  if (!passwordForm.newPassword) {
    errors.newPassword = t("account.validation.newPasswordRequired");
  } else if (passwordForm.newPassword.length < 8) {
    errors.newPassword = t("account.validation.newPasswordMin");
  } else if (passwordForm.currentPassword && passwordForm.newPassword === passwordForm.currentPassword) {
    errors.newPassword = t("account.validation.newPasswordDifferent");
  }

  if (!passwordForm.confirmPassword) {
    errors.confirmPassword = t("account.validation.confirmPasswordRequired");
  } else if (passwordForm.confirmPassword !== passwordForm.newPassword) {
    errors.confirmPassword = t("account.validation.confirmPasswordMatch");
  }

  return errors;
}
