import { FileText, Menu } from "lucide-react";
import { useTranslation } from "react-i18next";
import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../../features/auth/hooks/useAuth";
import { ButtonLink } from "../ui/ButtonLink";
import { LanguageSwitcher } from "./LanguageSwitcher";

const navItems = [
  { labelKey: "nav.dashboard", to: "/dashboard" },
  { labelKey: "nav.conversions", to: "/conversions" },
  { labelKey: "nav.billing", to: "/billing" },
  { labelKey: "nav.account", to: "/account" },
];

export function Navbar() {
  const { isAuthenticated, logout, user } = useAuth();
  const navigate = useNavigate();
  const { t } = useTranslation();

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <header className="sticky top-0 z-20 border-b border-slate-200/70 bg-white/80 shadow-sm shadow-slate-900/5 backdrop-blur-xl">
      <nav className="mx-auto flex h-16 w-full max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
        <NavLink
          className="flex items-center gap-2 rounded-full pr-2 text-base font-semibold text-slate-950 transition hover:text-emerald-800 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
          to="/"
        >
          <span className="flex h-9 w-9 items-center justify-center rounded-2xl bg-gradient-to-br from-emerald-500 to-slate-950 text-white shadow-lg shadow-emerald-900/20">
            <FileText aria-hidden="true" className="h-5 w-5" />
          </span>
          {t("common.appName")}
        </NavLink>

        <div className="hidden items-center gap-7 md:flex">
          {isAuthenticated
            ? (
              <div className="flex items-center gap-1 rounded-full border border-slate-200/80 bg-white/80 p-1 shadow-sm shadow-slate-900/5">
                {navItems.map((item) => (
                <NavLink
                  className={({ isActive }) =>
                    `rounded-full px-3 py-1.5 text-sm font-medium transition focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600 ${
                      isActive ? "bg-slate-950 text-white shadow-sm" : "text-slate-600 hover:bg-slate-100 hover:text-slate-950"
                    }`
                  }
                  key={item.to}
                  to={item.to}
                >
                  {t(item.labelKey)}
                </NavLink>
                ))}
              </div>
            )
            : null}
        </div>

        <div className="hidden items-center gap-3 sm:flex">
          <LanguageSwitcher />
          {isAuthenticated ? (
            <>
              <span className="max-w-36 truncate text-sm font-medium text-slate-600">{user?.name}</span>
              <button
                className="min-h-10 rounded-full border border-slate-300 bg-white px-4 text-sm font-semibold text-slate-800 shadow-sm transition hover:border-slate-400 hover:bg-slate-50 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
                onClick={handleLogout}
                type="button"
              >
                {t("common.logout")}
              </button>
            </>
          ) : (
            <>
              <NavLink
                className="rounded-full px-3 py-2 text-sm font-semibold text-slate-700 transition hover:bg-slate-100 hover:text-slate-950 focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600"
                to="/login"
              >
                {t("common.login")}
              </NavLink>
              <ButtonLink to="/register">{t("common.register")}</ButtonLink>
            </>
          )}
        </div>

        <div className="flex items-center gap-2 sm:hidden">
          <LanguageSwitcher />
          <NavLink
            aria-label={t("nav.openDashboard")}
            className="flex h-10 w-10 items-center justify-center rounded-full border border-slate-200 bg-white text-slate-700 shadow-sm focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600 md:hidden"
            to={isAuthenticated ? "/dashboard" : "/login"}
          >
            <Menu aria-hidden="true" className="h-5 w-5" />
          </NavLink>
        </div>
      </nav>
    </header>
  );
}
