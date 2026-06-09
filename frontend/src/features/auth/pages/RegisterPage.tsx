import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2 } from "lucide-react";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router-dom";
import { PageShell } from "../../../components/ui/PageShell";
import { useAuth } from "../hooks/useAuth";
import { registerSchema, type RegisterFormValues } from "../schemas/authSchemas";
import { getAuthErrorMessage } from "../services/authService";

export function RegisterPage() {
  const { register: registerUser } = useAuth();
  const navigate = useNavigate();
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const {
    formState: { errors, isSubmitting },
    handleSubmit,
    register,
  } = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      name: "",
      email: "",
      password: "",
    },
  });

  async function onSubmit(values: RegisterFormValues) {
    setErrorMessage(null);

    try {
      await registerUser(values);
      navigate("/dashboard");
    } catch (error) {
      setErrorMessage(getAuthErrorMessage(error));
    }
  }

  return (
    <PageShell description="Create a Convertly account and start with the Free plan." title="Create account">
      <form className="grid gap-5 sm:max-w-md" onSubmit={handleSubmit(onSubmit)}>
        <div>
          <label className="text-sm font-semibold text-slate-800" htmlFor="name">
            Name
          </label>
          <input
            {...register("name")}
            autoComplete="name"
            className="mt-2 h-11 w-full rounded-md border border-slate-300 px-3 text-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-100"
            id="name"
            placeholder="Abner Suhett"
            type="text"
          />
          {errors.name ? <p className="mt-2 text-sm text-red-600">{errors.name.message}</p> : null}
        </div>

        <div>
          <label className="text-sm font-semibold text-slate-800" htmlFor="email">
            Email
          </label>
          <input
            {...register("email")}
            autoComplete="email"
            className="mt-2 h-11 w-full rounded-md border border-slate-300 px-3 text-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-100"
            id="email"
            placeholder="abner@email.com"
            type="email"
          />
          {errors.email ? <p className="mt-2 text-sm text-red-600">{errors.email.message}</p> : null}
        </div>

        <div>
          <label className="text-sm font-semibold text-slate-800" htmlFor="password">
            Password
          </label>
          <input
            {...register("password")}
            autoComplete="new-password"
            className="mt-2 h-11 w-full rounded-md border border-slate-300 px-3 text-sm outline-none transition focus:border-emerald-600 focus:ring-2 focus:ring-emerald-100"
            id="password"
            placeholder="At least 8 characters"
            type="password"
          />
          {errors.password ? <p className="mt-2 text-sm text-red-600">{errors.password.message}</p> : null}
        </div>

        {errorMessage ? (
          <div className="rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{errorMessage}</div>
        ) : null}

        <button
          className="inline-flex h-11 items-center justify-center gap-2 rounded-md bg-emerald-600 px-5 text-sm font-semibold text-white transition hover:bg-emerald-700 disabled:cursor-not-allowed disabled:bg-slate-300"
          disabled={isSubmitting}
          type="submit"
        >
          {isSubmitting ? <Loader2 aria-hidden="true" className="h-4 w-4 animate-spin" /> : null}
          Register
        </button>

        <p className="text-sm text-slate-600">
          Already have an account?{" "}
          <Link className="font-semibold text-emerald-700 hover:text-emerald-800" to="/login">
            Login
          </Link>
        </p>
      </form>
    </PageShell>
  );
}
