import { zodResolver } from "@hookform/resolvers/zod";
import { Languages, Loader2, ShieldCheck, Sparkles } from "lucide-react";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { getLoginSchema, type LoginFormValues } from "../schemas/authSchemas";
import { getAuthErrorMessage } from "../services/authService";

export function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const { t } = useTranslation();
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const {
    formState: { errors, isSubmitting },
    handleSubmit,
    register,
  } = useForm<LoginFormValues>({
    resolver: zodResolver(getLoginSchema(t)),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  async function onSubmit(values: LoginFormValues) {
    setErrorMessage(null);

    try {
      await login(values);
      navigate("/dashboard");
    } catch (error) {
      setErrorMessage(getAuthErrorMessage(error, t));
    }
  }

  return (
    <section className="mx-auto grid w-full max-w-6xl flex-1 items-center gap-8 px-4 py-10 sm:px-6 lg:grid-cols-[0.95fr_1.05fr] lg:px-8">
      <aside className="overflow-hidden rounded-3xl border border-slate-200/80 bg-white/90 p-6 shadow-xl shadow-slate-900/5 backdrop-blur sm:p-8">
        <p className="inline-flex rounded-full border border-emerald-200 bg-emerald-50 px-3 py-1 text-xs font-semibold uppercase tracking-[0.14em] text-emerald-800">
          {t("common.brandEyebrow")}
        </p>
        <h1 className="mt-5 text-3xl font-semibold leading-tight text-slate-950 sm:text-4xl">
          {t("auth.login.title")}
        </h1>
        <p className="mt-4 text-base leading-7 text-slate-600">{t("auth.login.description")}</p>

        <div className="mt-8 grid gap-3">
          <TrustItem icon={ShieldCheck} title={t("landing.trust.private.title")} />
          <TrustItem icon={Sparkles} title={t("landing.trust.accessibility.title")} />
          <TrustItem icon={Languages} title={t("landing.trust.languages.title")} />
        </div>
      </aside>

      <div className="rounded-3xl border border-slate-200/80 bg-white p-6 shadow-2xl shadow-slate-900/10 sm:p-8">
        <form className="grid gap-5" onSubmit={handleSubmit(onSubmit)}>
          <div>
            <label className="text-sm font-semibold text-slate-800" htmlFor="email">
              {t("auth.fields.email")}
            </label>
            <input
              {...register("email")}
              aria-describedby={errors.email ? "login-email-error" : undefined}
              aria-invalid={errors.email ? "true" : "false"}
              autoComplete="email"
              className="mt-2 h-12 w-full rounded-xl border border-slate-300 bg-white px-3 text-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-100"
              id="email"
              placeholder={t("auth.placeholders.email")}
              type="email"
            />
            {errors.email ? <p className="mt-2 text-sm text-red-600" id="login-email-error">{errors.email.message}</p> : null}
          </div>

          <div>
            <label className="text-sm font-semibold text-slate-800" htmlFor="password">
              {t("auth.fields.password")}
            </label>
            <input
              {...register("password")}
              aria-describedby={errors.password ? "login-password-error" : undefined}
              aria-invalid={errors.password ? "true" : "false"}
              autoComplete="current-password"
              className="mt-2 h-12 w-full rounded-xl border border-slate-300 bg-white px-3 text-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-100"
              id="password"
              placeholder={t("auth.placeholders.password")}
              type="password"
            />
            {errors.password ? <p className="mt-2 text-sm text-red-600" id="login-password-error">{errors.password.message}</p> : null}
          </div>

          {errorMessage ? (
            <div className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700" role="alert">{errorMessage}</div>
          ) : null}

          <button
            className="inline-flex h-12 items-center justify-center gap-2 rounded-full bg-gradient-to-r from-emerald-600 to-slate-950 px-5 text-sm font-semibold text-white shadow-lg shadow-emerald-900/15 transition hover:from-emerald-700 hover:to-slate-900 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600 disabled:cursor-not-allowed disabled:from-slate-300 disabled:to-slate-300"
            disabled={isSubmitting}
            type="submit"
          >
            {isSubmitting ? <Loader2 aria-hidden="true" className="h-4 w-4 animate-spin" /> : null}
            {t("auth.login.submit")}
          </button>

          <p className="text-sm text-slate-600">
            {t("auth.login.newToConvertly")}{" "}
            <Link className="font-semibold text-emerald-700 hover:text-emerald-800 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600" to="/register">
              {t("auth.login.createAccount")}
            </Link>
          </p>
        </form>
      </div>
    </section>
  );
}

type TrustItemProps = {
  icon: typeof ShieldCheck;
  title: string;
};

function TrustItem({ icon: Icon, title }: TrustItemProps) {
  return (
    <div className="flex items-center gap-3 rounded-2xl border border-slate-200 bg-white/80 p-3 text-sm font-semibold text-slate-800 shadow-sm shadow-slate-900/5">
      <span className="flex h-9 w-9 items-center justify-center rounded-xl bg-emerald-50 text-emerald-700">
        <Icon aria-hidden="true" className="h-4 w-4" />
      </span>
      {title}
    </div>
  );
}
