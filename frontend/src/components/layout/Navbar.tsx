import { FileText, Menu } from "lucide-react";
import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../../features/auth/hooks/useAuth";
import { ButtonLink } from "../ui/ButtonLink";

const navItems = [
  { label: "Dashboard", to: "/dashboard" },
  { label: "Conversions", to: "/conversions" },
  { label: "Billing", to: "/billing" },
  { label: "Account", to: "/account" },
];

export function Navbar() {
  const { isAuthenticated, logout, user } = useAuth();
  const navigate = useNavigate();

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <header className="sticky top-0 z-20 border-b border-slate-200 bg-white/95 backdrop-blur">
      <nav className="mx-auto flex h-16 w-full max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
        <NavLink className="flex items-center gap-2 text-base font-semibold text-slate-950" to="/">
          <span className="flex h-9 w-9 items-center justify-center rounded-md bg-slate-950 text-white">
            <FileText aria-hidden="true" className="h-5 w-5" />
          </span>
          Convertly
        </NavLink>

        <div className="hidden items-center gap-7 md:flex">
          {isAuthenticated
            ? navItems.map((item) => (
                <NavLink
                  className={({ isActive }) =>
                    `text-sm font-medium transition ${
                      isActive ? "text-emerald-700" : "text-slate-600 hover:text-slate-950"
                    }`
                  }
                  key={item.to}
                  to={item.to}
                >
                  {item.label}
                </NavLink>
              ))
            : null}
        </div>

        <div className="hidden items-center gap-3 sm:flex">
          {isAuthenticated ? (
            <>
              <span className="max-w-36 truncate text-sm font-medium text-slate-600">{user?.name}</span>
              <button
                className="min-h-10 rounded-md border border-slate-300 bg-white px-4 text-sm font-semibold text-slate-800 transition hover:border-slate-400 hover:bg-slate-50"
                onClick={handleLogout}
                type="button"
              >
                Logout
              </button>
            </>
          ) : (
            <>
              <NavLink className="text-sm font-semibold text-slate-700 hover:text-slate-950" to="/login">
                Login
              </NavLink>
              <ButtonLink to="/register">Register</ButtonLink>
            </>
          )}
        </div>

        <NavLink
          aria-label="Open dashboard"
          className="flex h-10 w-10 items-center justify-center rounded-md border border-slate-200 text-slate-700 md:hidden"
          to={isAuthenticated ? "/dashboard" : "/login"}
        >
          <Menu aria-hidden="true" className="h-5 w-5" />
        </NavLink>
      </nav>
    </header>
  );
}
